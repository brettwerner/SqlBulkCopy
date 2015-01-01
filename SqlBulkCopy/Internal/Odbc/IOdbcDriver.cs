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
using SqlBulkCopy.Internal.BulkCopy;

namespace SqlBulkCopy.Internal.Odbc
{
    internal interface IOdbcDriver
    {
        // Setup
        IOdbcEnvironment CreateEnvironment(OdbcConnectionPooling connectionPooling);
        string DriverOptionString { get; }
        void Initialize(OdbcConnectionHandle connectionHandle, string tableName);

        // Column Binding
        void Bind<T>(OdbcConnectionHandle connectionHandle, IStandardColumn<T> column);
        void Bind<T>(OdbcConnectionHandle connectionHandle, IMaxColumn<T> column);
        IStandardColumn<long> CreateBigInt(short index);
        IStandardColumn<byte[]> CreateBinary(short index, short fieldLength, bool nullable = false);
        IStandardColumn<bool> CreateBit(short index);
        IStandardColumn<string> CreateChar(short index, short fieldLength, bool nullable = false);
        IStandardColumn<DateTime> CreateDate(short index);
        IStandardColumn<DateTime> CreateDateTime(short index);
        IStandardColumn<DateTime> CreateDateTime2(short index);
        IStandardColumn<DateTimeOffset> CreateDateTimeOffset(short index);
        IStandardColumn<decimal> CreateDecimal(short index);
        IStandardColumn<Double> CreateFloat(short index);
        IStandardColumn<int> CreateInt(short index);
        IStandardColumn<decimal> CreateMoney(short index);
        IStandardColumn<string> CreateNChar(short index, short fieldLength, bool nullable = false);
        IStandardColumn<string> CreateNVarChar(short index, short fieldLength, bool nullable = false);
        IMaxColumn<string> CreateNVarCharMax(short index, bool nullable = false);
        IStandardColumn<float> CreateReal(short index);
        IStandardColumn<DateTime> CreateSmallDateTime(short index);
        IStandardColumn<short> CreateSmallInt(short index);
        IStandardColumn<decimal> CreateSmallMoney(short index);
        IStandardColumn<TimeSpan> CreateTime(short index);
        IStandardColumn<byte> CreateTinyInt(short index);
        IStandardColumn<Guid> CreateUniqueIdentifier(short index);
        IStandardColumn<long?> CreateNullableBigInt(short index);
        IStandardColumn<bool?> CreateNullableBit(short index);
        IStandardColumn<DateTime?> CreateNullableDate(short index);
        IStandardColumn<DateTime?> CreateNullableDateTime(short index);
        IStandardColumn<DateTime?> CreateNullableDateTime2(short index);
        IStandardColumn<DateTimeOffset?> CreateNullableDateTimeOffset(short index);
        IStandardColumn<decimal?> CreateNullableDecimal(short index);
        IStandardColumn<Double?> CreateNullableFloat(short index);
        IStandardColumn<int?> CreateNullableInt(short index);
        IStandardColumn<decimal?> CreateNullableMoney(short index);
        IStandardColumn<float?> CreateNullableReal(short index);
        IStandardColumn<DateTime?> CreateNullableSmallDateTime(short index);
        IStandardColumn<short?> CreateNullableSmallInt(short index);
        IStandardColumn<decimal?> CreateNullableSmallMoney(short index);
        IStandardColumn<TimeSpan?> CreateNullableTime(short index);
        IStandardColumn<byte?> CreateNullableTinyInt(short index);
        IStandardColumn<Guid?> CreateNullableUniqueIdentifier(short index);
        IStandardColumn<byte[]> CreateVarBinary(short index, short fieldLength, bool nullable = false);
        IMaxColumn<byte[]> CreateVarBinaryMax(short index, bool nullable = false);
        IStandardColumn<string> CreateVarChar(short index, short fieldLength, bool nullable = false);
        IMaxColumn<string> CreateVarCharMax(short index, bool nullable = false);

        // Bulk Copy
        int Batch(OdbcConnectionHandle connectionHandle);
        int Done(OdbcConnectionHandle connectionHandle);
        bool SendMoreText(OdbcConnectionHandle connectionHandle, IMaxDataType column);
        bool SendRow(OdbcConnectionHandle connectionHandle);
        bool SetColumnLength(OdbcConnectionHandle connectionHandle, IMaxDataType column);
    }
}
