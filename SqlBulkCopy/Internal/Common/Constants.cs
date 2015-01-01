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

namespace SqlBulkCopy.Internal.Common
{
    internal static class Constants
    {
        internal const int SqlNullData = -1;  // SQL_NULL_DATA: indicates that the column contains a null value.

        internal static readonly IntPtr IntPtrZero = new IntPtr(0);

        internal const int NullableDataType = 128;

        internal const int SqlMaximumByteLength = 8000;  // the maximum length in bytes for SQL Server binary and character data types
        internal const int SqlMaximumDoubleByteCharacterSetByteLength = SqlMaximumByteLength / 2;  // the maximum length for nchar and nvarchar data types
        internal const int SqlMaximumMaxByteLength = 2147483647;  // (2^31 - 1) the maximum length for MAX columns
        internal const int SqlThousandthsToBillionthsMultipler = 1000000;  // multiplier to change from thousandths to billionths of a second
    }
}
