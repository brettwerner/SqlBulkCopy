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
    internal sealed class OdbcStatementHandle : OdbcHandle
    {
        private OdbcConnectionHandle _parentHandle;

        internal OdbcStatementHandle(OdbcConnectionHandle parentHandle)
        {
            if (parentHandle == null) throw new ArgumentNullException("parentHandle");

            handle = OdbcMethods.AllocateStatementHandle(parentHandle);

            var success = false;

            parentHandle.DangerousAddRef(ref success);

            if (!success)
            {
                throw new Exception("Unable to increment ODBC connection handle reference count.");
            }

            _parentHandle = parentHandle;
        }

        #region properties

        public override OdbcHandleType HandleType
        {
            get { return OdbcHandleType.Statement; }
        }

        public override bool IsInvalid
        {
            get { return (handle == Constants.IntPtrZero); }
        }

        #endregion

        #region methods

        protected override bool ReleaseHandle()
        {
            try
            {
                var releaseHandle = handle;

                if (releaseHandle != Constants.IntPtrZero)
                {
                    handle = Constants.IntPtrZero;

                    OdbcMethods.ReleaseHandle(OdbcHandleType.Statement, releaseHandle);
                }
            }
            finally
            {
                var parentHandle = _parentHandle;

                if (parentHandle != null)
                {
                    _parentHandle = null;
                    parentHandle.DangerousRelease();
                }
            }

            return true;
        }

        #endregion
    }
}