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

namespace SqlBulkCopy
{
    /// <summary>
    /// Indicates the type of ODBC connection pooling that is required.
    /// </summary>
    public enum OdbcConnectionPooling   // This enum matches values defined in sqlext.h
    {
        /// <summary>
        /// Connection pooling is disabled.
        /// </summary>
        None = 0,                       // SQL_CP_OFF

        /// <summary>
        /// A single connection pool is supported for each driver. Every connection in a pool is associated with one driver.
        /// </summary>
        PerDriver = 1,                  // SQL_CP_ONE_PER_DRIVER

        /// <summary>
        /// A single connection pool is supported for each environment. Every connection in a pool is associated with one environment.
        /// </summary>
        PerEnvironment = 2              // SQL_CP_ONE_PER_HENV
    }
}
