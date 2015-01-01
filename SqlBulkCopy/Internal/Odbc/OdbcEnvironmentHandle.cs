/*
    Copyright © 2014 Brett Werner

    This file is part of SQL Bulk Copy.

    SQL Bulk Copy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    SQL Bulk Copy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with SQL Bulk Copy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using SqlBulkCopy.Internal.Common;

namespace SqlBulkCopy.Internal.Odbc
{
    internal sealed class OdbcEnvironmentHandle : OdbcHandle, IOdbcEnvironment
    {
        private readonly IOdbcDriver _driver;

        internal OdbcEnvironmentHandle(OdbcVersion version, OdbcConnectionPooling connectionPooling, IOdbcDriver driver)
        {
            if (driver == null) throw new ArgumentNullException("driver");

            _driver = driver;

            handle = OdbcMethods.AllocateEnvironmentHandle();

            try
            {
                OdbcMethods.SetIntEnvironmentAttribute(this, OdbcEnvironmentAttribute.Version, (int)version);
                OdbcMethods.SetIntEnvironmentAttribute(this, OdbcEnvironmentAttribute.ConnectionPooling, (int)connectionPooling);
            }
            catch (Exception)
            {
                OdbcMethods.ReleaseHandle(OdbcHandleType.Environment, handle);
                throw;
            }
        }

        #region properties

        public OdbcConnectionPooling ConnectionPooling
        {
            get { return (OdbcConnectionPooling)OdbcMethods.GetIntEnvironmentAttribute(this, OdbcEnvironmentAttribute.ConnectionPooling); }
        }

        public override OdbcHandleType HandleType
        {
            get { return OdbcHandleType.Environment; }
        }

        public override bool IsInvalid
        {
            get { return (handle == Constants.IntPtrZero); }
        }

        #endregion

        #region methods

        protected override bool ReleaseHandle()
        {
            var releaseHandle = handle;

            if (releaseHandle == Constants.IntPtrZero) return true;

            handle = Constants.IntPtrZero;

            OdbcMethods.ReleaseHandle(OdbcHandleType.Environment, releaseHandle);

            return true;
        }

        #endregion

        #region IOdbcConnection

        IOdbcConnection IOdbcEnvironment.CreateConnection()
        {
            return new OdbcConnectionHandle(this, _driver);
        }

        #endregion
    }
}