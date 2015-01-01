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
using SqlBulkCopy;

namespace SqlBulkCopyTests
{
    public sealed class TestDataImporter : BulkCopyImporter<TestData>
    {
        private readonly ITypedBulkCopyBoundColumn<bool> _bit;
        private readonly ITypedBulkCopyBoundColumn<byte> _tinyInt;
        private readonly ITypedBulkCopyBoundColumn<short> _smallInt;
        private readonly ITypedBulkCopyBoundColumn<int> _int;
        private readonly ITypedBulkCopyBoundColumn<long> _bigInt;
        private readonly ITypedBulkCopyBoundColumn<byte[]> _binary;
        private readonly ITypedBulkCopyBoundColumn<byte[]> _varBinary;
        private readonly ITypedBulkCopyBoundColumn<string> _char;
        private readonly ITypedBulkCopyBoundColumn<string> _nChar;
        private readonly ITypedBulkCopyBoundColumn<string> _varChar;
        private readonly ITypedBulkCopyBoundColumn<string> _nVarChar;
        private readonly ITypedBulkCopyBoundColumn<DateTime> _dateTime;
        private readonly ITypedBulkCopyBoundColumn<DateTime> _smallDateTime;
        private readonly ITypedBulkCopyBoundColumn<DateTime> _date;
        private readonly ITypedBulkCopyBoundColumn<DateTime> _dateTime2;
        private readonly ITypedBulkCopyBoundColumn<DateTimeOffset> _dateTimeOffset;
        private readonly ITypedBulkCopyBoundColumn<TimeSpan> _time;
        private readonly ITypedBulkCopyBoundColumn<decimal> _decimal1;
        private readonly ITypedBulkCopyBoundColumn<decimal> _decimal2;
        private readonly ITypedBulkCopyBoundColumn<decimal> _smallMoney;
        private readonly ITypedBulkCopyBoundColumn<decimal> _money;
        private readonly ITypedBulkCopyBoundColumn<Double> _float;
        private readonly ITypedBulkCopyBoundColumn<Single> _real;
        private readonly ITypedBulkCopyBoundColumn<Guid> _uniqueIdentifier;
        private readonly ITypedBulkCopyBoundColumn<byte[]> _varBinaryMax;
        private readonly ITypedBulkCopyBoundColumn<string> _varCharMax;
        private readonly ITypedBulkCopyBoundColumn<string> _nVarCharMax;
        private readonly ITypedBulkCopyBoundColumn<bool?> _nullableBit;
        private readonly ITypedBulkCopyBoundColumn<byte?> _nullableTinyInt;
        private readonly ITypedBulkCopyBoundColumn<short?> _nullableSmallInt;
        private readonly ITypedBulkCopyBoundColumn<int?> _nullableInt;
        private readonly ITypedBulkCopyBoundColumn<long?> _nullableBigInt;
        private readonly ITypedBulkCopyBoundColumn<byte[]> _nullableBinary;
        private readonly ITypedBulkCopyBoundColumn<byte[]> _nullableVarBinary;
        private readonly ITypedBulkCopyBoundColumn<string> _nullableChar;
        private readonly ITypedBulkCopyBoundColumn<string> _nullableNChar;
        private readonly ITypedBulkCopyBoundColumn<string> _nullableVarChar;
        private readonly ITypedBulkCopyBoundColumn<string> _nullableNVarChar;
        private readonly ITypedBulkCopyBoundColumn<DateTime?> _nullableDateTime;
        private readonly ITypedBulkCopyBoundColumn<DateTime?> _nullableSmallDateTime;
        private readonly ITypedBulkCopyBoundColumn<DateTime?> _nullableDate;
        private readonly ITypedBulkCopyBoundColumn<DateTime?> _nullableDateTime2;
        private readonly ITypedBulkCopyBoundColumn<DateTimeOffset?> _nullableDateTimeOffset;
        private readonly ITypedBulkCopyBoundColumn<TimeSpan?> _nullableTime;
        private readonly ITypedBulkCopyBoundColumn<decimal?> _nullableDecimal1;
        private readonly ITypedBulkCopyBoundColumn<decimal?> _nullableDecimal2;
        private readonly ITypedBulkCopyBoundColumn<decimal?> _nullableSmallMoney;
        private readonly ITypedBulkCopyBoundColumn<decimal?> _nullableMoney;
        private readonly ITypedBulkCopyBoundColumn<Double?> _nullableFloat;
        private readonly ITypedBulkCopyBoundColumn<Single?> _nullableReal;
        private readonly ITypedBulkCopyBoundColumn<Guid?> _nullableUniqueIdentifier;
        private readonly ITypedBulkCopyBoundColumn<byte[]> _nullableVarBinaryMax;
        private readonly ITypedBulkCopyBoundColumn<string> _nullableVarCharMax;
        private readonly ITypedBulkCopyBoundColumn<string> _nullableNVarCharMax;

        private const int BatchSize = 100;

        public TestDataImporter(IBulkCopy bulkCopy)
            : base(bulkCopy, BatchSize)
        {
            if (bulkCopy == null) throw new ArgumentNullException("bulkCopy");

            bulkCopy.SkipColumn(BulkCopyDataType.Int);
            _bit = bulkCopy.BindBit();
            _tinyInt = bulkCopy.BindTinyInt();
            _smallInt = bulkCopy.BindSmallInt();
            _int = bulkCopy.BindInt();
            _bigInt = bulkCopy.BindBigInt();
            _binary = bulkCopy.BindBinary(4);
            _varBinary = bulkCopy.BindVarBinary(10);
            _char = bulkCopy.BindChar(4);
            _nChar = bulkCopy.BindNChar(8);
            _varChar = bulkCopy.BindVarChar(10);
            _nVarChar = bulkCopy.BindNVarChar(12);
            _dateTime = bulkCopy.BindDateTime();
            _smallDateTime = bulkCopy.BindSmallDateTime();
            _date = bulkCopy.BindDate();
            _dateTime2 = bulkCopy.BindDateTime2();
            _dateTimeOffset = bulkCopy.BindDateTimeOffset();
            _time = bulkCopy.BindTime();
            _decimal1 = bulkCopy.BindDecimal();
            _decimal2 = bulkCopy.BindDecimal();
            _smallMoney = bulkCopy.BindSmallMoney();
            _money = bulkCopy.BindMoney();
            _float = bulkCopy.BindFloat();
            _real = bulkCopy.BindReal();
            _uniqueIdentifier = bulkCopy.BindUniqueIdentifier();
            _varBinaryMax = bulkCopy.BindVarBinaryMax();
            _varCharMax = bulkCopy.BindVarCharMax();
            _nVarCharMax = bulkCopy.BindNVarCharMax();
            _nullableBit = bulkCopy.BindNullableBit();
            _nullableTinyInt = bulkCopy.BindNullableTinyInt();
            _nullableSmallInt = bulkCopy.BindNullableSmallInt();
            _nullableInt = bulkCopy.BindNullableInt();
            _nullableBigInt = bulkCopy.BindNullableBigInt();
            _nullableBinary = bulkCopy.BindBinary(4, true);
            _nullableVarBinary = bulkCopy.BindVarBinary(10, true);
            _nullableChar = bulkCopy.BindChar(4, true);
            _nullableNChar = bulkCopy.BindNChar(8, true);
            _nullableVarChar = bulkCopy.BindVarChar(10, true);
            _nullableNVarChar = bulkCopy.BindNVarChar(12, true);
            _nullableDateTime = bulkCopy.BindNullableDateTime();
            _nullableSmallDateTime = bulkCopy.BindNullableSmallDateTime();
            _nullableDate = bulkCopy.BindNullableDate();
            _nullableDateTime2 = bulkCopy.BindNullableDateTime2();
            _nullableDateTimeOffset = bulkCopy.BindNullableDateTimeOffset();
            _nullableTime = bulkCopy.BindNullableTime();
            _nullableDecimal1 = bulkCopy.BindNullableDecimal();
            _nullableDecimal2 = bulkCopy.BindNullableDecimal();
            _nullableSmallMoney = bulkCopy.BindNullableSmallMoney();
            _nullableMoney = bulkCopy.BindNullableMoney();
            _nullableFloat = bulkCopy.BindNullableFloat();
            _nullableReal = bulkCopy.BindNullableReal();
            _nullableUniqueIdentifier = bulkCopy.BindNullableUniqueIdentifier();
            _nullableVarBinaryMax = bulkCopy.BindVarBinaryMax(true);
            _nullableVarCharMax = bulkCopy.BindVarCharMax(true);
            _nullableNVarCharMax = bulkCopy.BindNVarCharMax(true);
        }

// ReSharper disable FunctionComplexityOverflow
        protected override void SetRowValues(TestData data)
// ReSharper restore FunctionComplexityOverflow
        {
            if (data == null) throw new ArgumentNullException("data");

            _bit.SetValue(data.Bit);
            _tinyInt.SetValue(data.TinyInt);
            _smallInt.SetValue(data.SmallInt);
            _int.SetValue(data.Int);
            _bigInt.SetValue(data.BigInt);
            _binary.SetValue(data.Binary);
            _varBinary.SetValue(data.VarBinary);
            _char.SetValue(data.Char);
            _nChar.SetValue(data.NChar);
            _varChar.SetValue(data.VarChar);
            _nVarChar.SetValue(data.NVarChar);
            _dateTime.SetValue(data.DateTime);
            _smallDateTime.SetValue(data.SmallDateTime);
            _date.SetValue(data.Date);
            _dateTime2.SetValue(data.DateTime2);
            _dateTimeOffset.SetValue(data.DateTimeOffset);
            _time.SetValue(data.Time);
            _decimal1.SetValue(data.Decimal1);
            _decimal2.SetValue(data.Decimal2);
            _smallMoney.SetValue(data.SmallMoney);
            _money.SetValue(data.Money);
            _float.SetValue(data.Float);
            _real.SetValue(data.Real);
            _uniqueIdentifier.SetValue(data.UniqueIdentifier);
            _varBinaryMax.SetValue(data.VarBinaryMax);
            _varCharMax.SetValue(data.VarCharMax);
            _nVarCharMax.SetValue(data.NVarCharMax);
            _nullableBit.SetValue(data.NullableBit);
            _nullableTinyInt.SetValue(data.NullableTinyInt);
            _nullableSmallInt.SetValue(data.NullableSmallInt);
            _nullableInt.SetValue(data.NullableInt);
            _nullableBigInt.SetValue(data.NullableBigInt);
            _nullableBinary.SetValue(data.NullableBinary);
            _nullableVarBinary.SetValue(data.NullableVarBinary);
            _nullableChar.SetValue(data.NullableChar);
            _nullableNChar.SetValue(data.NullableNChar);
            _nullableVarChar.SetValue(data.NullableVarChar);
            _nullableNVarChar.SetValue(data.NullableNVarChar);
            _nullableDateTime.SetValue(data.NullableDateTime);
            _nullableSmallDateTime.SetValue(data.NullableSmallDateTime);
            _nullableDate.SetValue(data.NullableDate);
            _nullableDateTime2.SetValue(data.NullableDateTime2);
            _nullableDateTimeOffset.SetValue(data.NullableDateTimeOffset);
            _nullableTime.SetValue(data.NullableTime);
            _nullableDecimal1.SetValue(data.NullableDecimal1);
            _nullableDecimal2.SetValue(data.NullableDecimal2);
            _nullableSmallMoney.SetValue(data.NullableSmallMoney);
            _nullableMoney.SetValue(data.NullableMoney);
            _nullableFloat.SetValue(data.NullableFloat);
            _nullableReal.SetValue(data.NullableReal);
            _nullableUniqueIdentifier.SetValue(data.NullableUniqueIdentifier);
            _nullableVarBinaryMax.SetValue(data.NullableVarBinaryMax);
            _nullableVarCharMax.SetValue(data.NullableVarCharMax);
            _nullableNVarCharMax.SetValue(data.NullableNVarCharMax);
        }
    }
}
