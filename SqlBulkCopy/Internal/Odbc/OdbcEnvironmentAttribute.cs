﻿/*
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

namespace SqlBulkCopy.Internal.Odbc
{
    internal enum OdbcEnvironmentAttribute  // Defined in sqlext.h
    {
        Version = 200,            // SQL_ATTR_ODBC_VERSION: indicates whether ODBC 2.x functionality or ODBC 3.x functionality is required.
        ConnectionPooling = 201,  // SQL_ATTR_CONNECTION_POOLING: enables or disables connection pooling.
    }
}
