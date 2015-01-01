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

namespace SqlBulkCopy.Internal.Odbc
{
    internal enum OdbcConnectionAttribute   // Defined in sqlext.h
    {
        LoginTimeout = 103,       //  SQL_ATTR_LOGIN_TIMEOUT: the number of seconds to wait for a login request to complete before returning to the application.
        Trace = 104,              //  SQL_ATTR_TRACE: tells the Driver Manager whether to perform tracing.
        TraceFile = 105,          //  SQL_ATTR_TRACEFILE: the name of the trace file.
        PacketSize = 112,         //  SQL_ATTR_PACKET_SIZE: the network packet size in bytes.
        ConnectionTimeout = 113,  //  SQL_ATTR_CONNECTION_TIMEOUT: the number of seconds to wait for any request on the connection to complete before returning to the application.
        EnableBcp = 1219,         //  SQL_COPT_SS_BCP: enables bulk copy functions on a connection.
    }
}
