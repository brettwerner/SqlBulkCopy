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
using SqlBulkCopy.Internal.Odbc;

namespace SqlBulkCopy
{
    /// <summary>
    /// Represents an ODBC environment.
    /// </summary>
    public sealed class OdbcEnvironment : IDisposable
    {
        private readonly IOdbcEnvironment _environment;

        internal OdbcEnvironment(IOdbcEnvironment environment)
        {
            if (environment == null) throw new ArgumentNullException("environment");

            _environment = environment;
        }

        internal IOdbcEnvironment InternalEnvironment
        {
            get { return _environment; }
        }

        /// <summary>
        /// Returns the connection pooling being used by this environment.
        /// </summary>
        public OdbcConnectionPooling ConnectionPooling
        {
            get { return _environment.ConnectionPooling; }
        }

        /// <summary>
        /// Disposes the class and releases ODBC handles.
        /// </summary>
        public void Dispose()
        {
            _environment.Dispose();
        }
    }
}
