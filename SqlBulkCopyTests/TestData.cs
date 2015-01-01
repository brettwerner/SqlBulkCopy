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
using System.Collections.Generic;

namespace SqlBulkCopyTests
{
    public sealed class TestData
    {
        public bool Bit { get; set; }
        public byte TinyInt { get; set; }
        public short SmallInt { get; set; }
        public int Int { get; set; }
        public long BigInt { get; set; }
        public byte[] Binary { get; set; }
        public byte[] VarBinary { get; set; }
        public string Char { get; set; }
        public string NChar { get; set; }
        public string VarChar { get; set; }
        public string NVarChar { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime SmallDateTime { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateTime2 { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public TimeSpan Time { get; set; }
        public decimal Decimal1 { get; set; }
        public decimal Decimal2 { get; set; }
        public decimal SmallMoney { get; set; }
        public decimal Money { get; set; }
        public Double Float { get; set; }
        public Single Real { get; set; }
        public Guid UniqueIdentifier { get; set; }
        public byte[] VarBinaryMax { get; set; }
        public string VarCharMax { get; set; }
        public string NVarCharMax { get; set; }
        public bool? NullableBit { get; set; }
        public byte? NullableTinyInt { get; set; }
        public short? NullableSmallInt { get; set; }
        public int? NullableInt { get; set; }
        public long? NullableBigInt { get; set; }
        public byte[] NullableBinary { get; set; }
        public byte[] NullableVarBinary { get; set; }
        public string NullableChar { get; set; }
        public string NullableNChar { get; set; }
        public string NullableVarChar { get; set; }
        public string NullableNVarChar { get; set; }
        public DateTime? NullableDateTime { get; set; }
        public DateTime? NullableSmallDateTime { get; set; }
        public DateTime? NullableDate { get; set; }
        public DateTime? NullableDateTime2 { get; set; }
        public DateTimeOffset? NullableDateTimeOffset { get; set; }
        public TimeSpan? NullableTime { get; set; }
        public decimal? NullableDecimal1 { get; set; }
        public decimal? NullableDecimal2 { get; set; }
        public decimal? NullableSmallMoney { get; set; }
        public decimal? NullableMoney { get; set; }
        public Double? NullableFloat { get; set; }
        public Single? NullableReal { get; set; }
        public Guid? NullableUniqueIdentifier { get; set; }
        public byte[] NullableVarBinaryMax { get; set; }
        public string NullableVarCharMax { get; set; }
        public string NullableNVarCharMax { get; set; }

        public static List<TestData> GetSampleData()
        {
            var now = DateTime.Now;

            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);

            var today = now.Date;
            var minDateTime = new DateTime(1753, 1, 1, 0, 0, 0, 3);
            var maxDateTime = new DateTime(9999, 12, 31, 23, 59, 59, 997);
            var minSmallDateTime = new DateTime(1900, 1, 1, 0, 0, 0);
            var maxSmallDateTime = new DateTime(2079, 6, 6, 23, 59, 0);
            var minDate = new DateTime(1, 1, 1);
            var maxDate = new DateTime(9999, 12, 31);
            var minDateTime2 = new DateTime(1, 1, 1, 0, 0, 0, 0);
            var maxDateTime2 = new DateTime(9999, 12, 31, 23, 59, 59, 999);
            var minDateTimeOffset = new DateTimeOffset(1, 1, 1, 0, 0, 0, 0, new TimeSpan(-14, 0, 0));
            var maxDateTimeOffset = new DateTimeOffset(9999, 12, 31, 23, 59, 59, 999, new TimeSpan(14, 0, 0));
            var minTime = new TimeSpan(0, 0, 0, 0, 0);
            var maxTime = new TimeSpan(0, 23, 59, 59, 999);
            var largeByteArray = new byte[8001];

            for (var i = 0; i < 8001; i++)
            {
                largeByteArray[i] = (byte)i;
            }

            return new List<TestData>
            {
                    new TestData
                    {
                        Bit = false,
                        TinyInt = byte.MinValue,
                        SmallInt = short.MinValue,
                        Int = int.MinValue,
                        BigInt = long.MinValue,
                        Binary = new byte[] {0, 1, 2, 3},
                        VarBinary = new byte[] {0},
                        Char = "1234",
                        NChar = "அஆஇஈ",
                        VarChar = "abcdefghij",
                        NVarChar = "กขฃคฅฆ",
                        DateTime = minDateTime,
                        SmallDateTime = minSmallDateTime,
                        Date = minDate,
                        DateTime2 = minDateTime2,
                        DateTimeOffset = minDateTimeOffset,
                        Time = minTime,
                        Decimal1 = -99999.9999M,
                        Decimal2 = -123456789.123456M,
                        SmallMoney = -214748.3648M,
                        Money = -922337203685477.5808M,
                        Float = -1.79E+308D,
                        Real = -3.40E+38F,
                        UniqueIdentifier = Guid.Parse("3D543B1F-AE1B-4D47-84EA-535C5A8EB38A"),
                        VarBinaryMax = new byte[] {0},
                        VarCharMax = "abc",
                        NVarCharMax = "ずせぜそぞただちぢっつづてでとどなにぬねのはばぱひびぴ",
                        NullableBit = false,
                        NullableTinyInt = byte.MinValue,
                        NullableSmallInt = short.MinValue,
                        NullableInt = int.MinValue,
                        NullableBigInt = long.MinValue,
                        NullableBinary = new byte[] {0, 1, 2, 3},
                        NullableVarBinary = new byte[] {1},
                        NullableChar = "5678",
                        NullableNChar = "ઍએઐઑ",
                        NullableVarChar = "ABCDEFGHI",
                        NullableNVarChar = "ഇഈഉഊഋഌ",
                        NullableDateTime = minDateTime,
                        NullableSmallDateTime = minSmallDateTime,
                        NullableDate = minDate,
                        NullableDateTime2 = minDateTime2,
                        NullableDateTimeOffset = minDateTimeOffset,
                        NullableTime = minTime,
                        NullableDecimal1 = -99999.9999M,
                        NullableDecimal2 = -123456789.123456M,
                        NullableSmallMoney = -214748.3648M,
                        NullableMoney = -922337203685477.5808M,
                        NullableFloat = -1.79E+308D,
                        NullableReal = 1.18E-38F,
                        NullableUniqueIdentifier = Guid.Parse("57927300-1FA1-425C-B9AB-4E7C2CFD4455"),
                        NullableVarBinaryMax = new byte[] {0, 255},
                        NullableVarCharMax = "xyz",
                        NullableNVarCharMax = "가각갂갃간갅갆갇갈갉갊갋갌갍갎갏감갑값갓갔강갖갗갘같갚갛개객갞갟갠갡갢갣"
                    },
                    new TestData
                    {
                        Bit = true,
                        TinyInt = byte.MaxValue / 2,
                        SmallInt = short.MaxValue / 2,
                        Int = int.MaxValue / 2,
                        BigInt = long.MaxValue / 2,
                        Binary = null,
                        VarBinary = null,
                        Char = null,
                        NChar = null,
                        VarChar = null,
                        NVarChar = null,
                        DateTime = today,
                        SmallDateTime = today,
                        Date = today,
                        DateTime2 = now,
                        DateTimeOffset = now,
                        Time = now.TimeOfDay,
                        Decimal1 = 12345.1234M,
                        Decimal2 = 0M,
                        SmallMoney = 0M,
                        Money = 0M,
                        Float = 0D,
                        Real = 0F,
                        UniqueIdentifier = Guid.Parse("1F3928D0-104A-436E-8536-89BC052DFDAA"),
                        VarBinaryMax = null,
                        VarCharMax = null,
                        NVarCharMax = null,
                        NullableBit = null,
                        NullableTinyInt = null,
                        NullableSmallInt = null,
                        NullableInt = null,
                        NullableBigInt = null,
                        NullableBinary = null,
                        NullableVarBinary = null,
                        NullableChar = null,
                        NullableNChar = null,
                        NullableVarChar = null,
                        NullableNVarChar = null,
                        NullableDateTime = null,
                        NullableSmallDateTime = null,
                        NullableDate = null,
                        NullableDateTime2 = null,
                        NullableDateTimeOffset = null,
                        NullableTime = null,
                        NullableDecimal1 = null,
                        NullableDecimal2 = null,
                        NullableSmallMoney = null,
                        NullableMoney = null,
                        NullableFloat = null,
                        NullableReal = null,
                        NullableUniqueIdentifier = null,
                        NullableVarBinaryMax = null,
                        NullableVarCharMax = null,
                        NullableNVarCharMax = null,
                    },
                    new TestData
                    {
                        Bit = false,
                        TinyInt = byte.MaxValue,
                        SmallInt = short.MaxValue,
                        Int = int.MaxValue,
                        BigInt = long.MaxValue,
                        Binary = new byte[] {0, 156, 255},
                        VarBinary = new byte[] {17, 26, 34, 29, 218, 18},
                        Char = "5678",
                        NChar = "ઍએઐઑ",
                        VarChar = "ABCDEFGHI",
                        NVarChar = "ഇഈഉഊഋഌ",
                        DateTime = maxDateTime,
                        SmallDateTime = maxSmallDateTime,
                        Date = maxDate,
                        DateTime2 = maxDateTime2,
                        DateTimeOffset = maxDateTimeOffset,
                        Time = maxTime,
                        Decimal1 = 99999.9999M,
                        Decimal2 = 123456789.123456M,
                        SmallMoney = 214748.3647M,
                        Money = 922337203685477.5807M,
                        Float = 1.79E+308D,
                        Real = -1.18E-38F,
                        UniqueIdentifier = Guid.Parse("0ABBFE98-179C-46E9-A1E4-5BDB4D6BBFBE"),
                        VarBinaryMax = largeByteArray,
                        VarCharMax = "0123456789",
                        NVarCharMax = "ポマミムメモャヤュユョヨラリルレロヮワヰヱヲンヴヵヶ",
                        NullableBit = true,
                        NullableTinyInt = byte.MaxValue,
                        NullableSmallInt = short.MaxValue,
                        NullableInt = int.MaxValue,
                        NullableBigInt = long.MaxValue,
                        NullableBinary = new byte[] {0, 156, 255},
                        NullableVarBinary = new byte[] {17, 26, 34, 29, 218, 18},
                        NullableChar = "1234",
                        NullableNChar = "அஆஇஈ",
                        NullableVarChar = "abcdefghij",
                        NullableNVarChar = "กขฃคฅฆ",
                        NullableDateTime = maxDateTime,
                        NullableSmallDateTime = maxSmallDateTime,
                        NullableDate = maxDate,
                        NullableDateTime2 = maxDateTime2,
                        NullableDateTimeOffset = maxDateTimeOffset,
                        NullableTime = maxTime,
                        NullableDecimal1 = 99999.9999M,
                        NullableDecimal2 = 123456789.123456M,
                        NullableSmallMoney = 214748.3647M,
                        NullableMoney = 922337203685477.5807M,
                        NullableFloat = 1.79E+308D,
                        NullableReal = 3.40E+38F,
                        NullableUniqueIdentifier = Guid.Parse("80B6E839-E642-4D12-99B8-ADE8F6C9770F"),
                        NullableVarBinaryMax = largeByteArray,
                        NullableVarCharMax = "0123456789",
                        NullableNVarCharMax = "갤갥갦갧갨갩갪갫갬갭갮갯갰갱갲갳갴갵갶갷갸갹갺갻갼갽갾갿걀걁걂걃걄걅걆걇걈걉걊걋걌걍걎걏걐걑걒걓걔걕걖걗"
                    }
                };
        }

        public object[] GetValues(int identityValue)
        {
                return new object[]
                {
                    identityValue,
                    Bit,
                    TinyInt,
                    SmallInt,
                    Int,
                    BigInt,
                    Binary,
                    VarBinary,
                    Char,
                    NChar,
                    VarChar,
                    NVarChar,
                    DateTime,
                    SmallDateTime,
                    Date,
                    DateTime2,
                    DateTimeOffset,
                    Time,
                    Decimal1,
                    Decimal2,
                    SmallMoney,
                    Money,
                    Float,
                    Real,
                    UniqueIdentifier,
                    VarBinaryMax,
                    VarCharMax,
                    NVarCharMax,
                    NullableBit,
                    NullableTinyInt,
                    NullableSmallInt,
                    NullableInt,
                    NullableBigInt,
                    NullableBinary,
                    NullableVarBinary,
                    NullableChar,
                    NullableNChar,
                    NullableVarChar,
                    NullableNVarChar,
                    NullableDateTime,
                    NullableSmallDateTime,
                    NullableDate,
                    NullableDateTime2,
                    NullableDateTimeOffset,
                    NullableTime,
                    NullableDecimal1,
                    NullableDecimal2,
                    NullableSmallMoney,
                    NullableMoney,
                    NullableFloat,
                    NullableReal,
                    NullableUniqueIdentifier,
                    NullableVarBinaryMax,
                    NullableVarCharMax,
                    NullableNVarCharMax
                };
        }
    }
}
