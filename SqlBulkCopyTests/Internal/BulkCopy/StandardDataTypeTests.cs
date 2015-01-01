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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlBulkCopy;
using SqlBulkCopy.Internal.BulkCopy;
using SqlBulkCopy.Internal.Common;

namespace SqlBulkCopyTests.Internal.BulkCopy
{
    [TestClass]
    public class StandardDataTypeTests
    {
        private sealed class StandardDataTypeImpl : StandardDataType
        {
            public StandardDataTypeImpl(short index, BulkCopyDataType dataType, int length, BindingFlags options)
                : base(index, dataType, length, options)
            {
                //
            }

            public new void CopyByte(int index, byte value)
            {
                base.CopyByte(index, value);
            }

            public new void CopyInt16(int index, short value)
            {
                base.CopyInt16(index, value);
            }

            public new void CopyInt32(int index, int value)
            {
                base.CopyInt32(index, value);
            }

            public new void CopyInt64(int index, long value)
            {
                base.CopyInt64(index, value);
            }

            public new void CopyFrom(byte[] bytes, int length)
            {
                base.CopyFrom(bytes, length);
            }

            public new void SetLength(int length)
            {
                base.SetLength(length);
            }

            public new void SetNull()
            {
                base.SetNull();
            }
        }

        private const BulkCopyDataType AnyBulkCopyDataTypeWillDo = BulkCopyDataType.Char;
        private const int AnyValidIndexWillDo = 0;
        private const int AnyValidLengthWillDo = 1;
        private const int IndicatorLength = 4;

        #region constructor tests

// ReSharper disable NotAccessedVariable
// ReSharper disable UnusedVariable
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorThrowsExceptionWhenIndexLessThanZero()
        {
            var sut = new StandardDataTypeImpl(-1, AnyBulkCopyDataTypeWillDo, 1, BindingFlags.None);
        }

        [TestMethod]
        public void ConstructorThrowsExceptionWhenLengthLessThanOne()
        {
            const int minLength = 1;

            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, minLength, BindingFlags.None);

            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() =>
            {
                sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, minLength - 1, BindingFlags.None);
            });
        }

        [TestMethod]
        public void ConstructorThrowsExceptionWhenLengthGreaterThanMaximumForNonDoubleByteCharacterSet()
        {
            const int maxLength = 8000;

            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, maxLength, BindingFlags.None);

            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() =>
            {
                sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, maxLength + 1, BindingFlags.None);
            });
        }

        [TestMethod]
        public void ConstructorThrowsExceptionWhenLengthGreaterThanMaximumForDoubleByteCharacterSet()
        {
            const int maxLength = 4000;

            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, maxLength, BindingFlags.DoubleByteCharacterSet);

            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() =>
            {
                sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, maxLength + 1, BindingFlags.DoubleByteCharacterSet);
            });
        }
// ReSharper restore NotAccessedVariable
// ReSharper restore UnusedVariable

        [TestMethod]
        public void ConstructorSetsByteLengthCorrectly()
        {
            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, AnyValidLengthWillDo, BindingFlags.None);

            Assert.AreEqual(AnyValidLengthWillDo, sut.ByteLength);
            Assert.AreEqual(AnyValidLengthWillDo, sut.Length);

            sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, AnyValidLengthWillDo, BindingFlags.DoubleByteCharacterSet);

            Assert.AreEqual(AnyValidLengthWillDo * 2, sut.ByteLength);
            Assert.AreEqual(AnyValidLengthWillDo, sut.Length);
        }

        #endregion

        #region properties

        [TestMethod]
        public void ReturnsExpectedPropertyValues()
        {
            var sut = new StandardDataTypeImpl(0, BulkCopyDataType.TinyInt, AnyValidLengthWillDo, BindingFlags.None);

            const int tinyIntDataTypeLength = 1;

            Assert.AreEqual(tinyIntDataTypeLength, ((IBulkCopyBoundColumn)sut).ByteLength);
            Assert.AreEqual(BulkCopyDataType.TinyInt, sut.DataType);
            Assert.AreEqual("TinyInt NOT NULL", sut.DataTypeDescription);
            Assert.AreEqual(0, sut.Index);
            Assert.AreEqual(0, sut.IndicatorLength);
            Assert.AreEqual(true, sut.IsBound);
            Assert.AreEqual(false, sut.IsNullable);
            Assert.AreEqual(false, sut.IsVariableLength);
            Assert.AreEqual(tinyIntDataTypeLength, ((IBulkCopyBoundColumn)sut).Length);
            Assert.AreEqual(BindingFlags.None, sut.Options);

            sut = new StandardDataTypeImpl(1, BulkCopyDataType.NullableVarBinary, AnyValidLengthWillDo, BindingFlags.Nullable | BindingFlags.VariableLengthIn | BindingFlags.VariableLengthOut);

            Assert.AreEqual(AnyValidLengthWillDo, ((IBulkCopyBoundColumn)sut).ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableVarBinary, sut.DataType);
            Assert.AreEqual("VarBinary NULL", sut.DataTypeDescription);
            Assert.AreEqual(1, sut.Index);
            Assert.AreEqual(IndicatorLength, sut.IndicatorLength);
            Assert.AreEqual(true, sut.IsBound);
            Assert.AreEqual(true, sut.IsNullable);
            Assert.AreEqual(true, sut.IsVariableLength);
            Assert.AreEqual(AnyValidLengthWillDo, ((IBulkCopyBoundColumn)sut).Length);
            Assert.AreEqual(BindingFlags.Nullable | BindingFlags.VariableLengthIn | BindingFlags.VariableLengthOut, sut.Options);
        }

        #endregion

        #region methods

        [TestMethod]
        public void CopyByteThrowsExceptionWhenIndexOutOfRange()
        {
            const int dataTypeLengthInBytes = 1;
            const byte anyValueWillDo = 1;

            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, dataTypeLengthInBytes, BindingFlags.None);

            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() => sut.CopyByte(-1, anyValueWillDo));
            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() => sut.CopyByte(dataTypeLengthInBytes, anyValueWillDo));
        }

        [TestMethod]
        public void CopyByteBehavesAsExpected()
        {
            const int dataTypeLengthInBytes = 1;
            const byte minValue = byte.MinValue;
            const byte maxValue = byte.MaxValue;

            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, dataTypeLengthInBytes, BindingFlags.None);

            Assert.AreEqual(0, sut.IndicatorLength);

            sut.CopyByte(0, minValue);

            Assert.AreEqual(minValue, sut.Data[sut.IndicatorLength]);

            sut.CopyByte(0, maxValue);

            Assert.AreEqual(maxValue, sut.Data[sut.IndicatorLength]);

            sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, dataTypeLengthInBytes, BindingFlags.Nullable);

            Assert.AreEqual(IndicatorLength, sut.IndicatorLength);

            sut.CopyByte(0, minValue);

            Assert.AreEqual(minValue, sut.Data[sut.IndicatorLength]);

            sut.CopyByte(0, maxValue);

            Assert.AreEqual(maxValue, sut.Data[sut.IndicatorLength]);
        }

        [TestMethod]
        public void CopyInt16ThrowsExceptionWhenIndexOutOfRange()
        {
            const int dataTypeLengthInBytes = 2;
            const short anyValueWillDo = 1;

            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, dataTypeLengthInBytes, BindingFlags.None);

            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() => sut.CopyInt16(-1, anyValueWillDo));
            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() => sut.CopyInt16(dataTypeLengthInBytes, anyValueWillDo));
        }

        [TestMethod]
        public void CopyInt16BehavesAsExpected()
        {
            const int dataTypeLengthInBytes = 2;
            const short minValue = short.MinValue;
            const short maxValue = short.MaxValue;

            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, dataTypeLengthInBytes, BindingFlags.None);

            Assert.AreEqual(0, sut.IndicatorLength);

            sut.CopyInt16(0, minValue);

            Assert.AreEqual(minValue, BitConverter.ToInt16(sut.Data, sut.IndicatorLength));

            sut.CopyInt16(0, maxValue);

            Assert.AreEqual(maxValue, BitConverter.ToInt16(sut.Data, sut.IndicatorLength));

            sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, dataTypeLengthInBytes, BindingFlags.Nullable);

            Assert.AreEqual(IndicatorLength, sut.IndicatorLength);

            sut.CopyInt16(0, minValue);

            Assert.AreEqual(minValue, BitConverter.ToInt16(sut.Data, sut.IndicatorLength));

            sut.CopyInt16(0, maxValue);

            Assert.AreEqual(maxValue, BitConverter.ToInt16(sut.Data, sut.IndicatorLength));
        }

        [TestMethod]
        public void CopyInt32ThrowsExceptionWhenIndexOutOfRange()
        {
            const int dataTypeLengthInBytes = 4;
            const int anyValueWillDo = 1;

            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, dataTypeLengthInBytes, BindingFlags.None);

            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() => sut.CopyInt32(-1, anyValueWillDo));
            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() => sut.CopyInt32(dataTypeLengthInBytes, anyValueWillDo));
        }

        [TestMethod]
        public void CopyInt32BehavesAsExpected()
        {
            const int dataTypeLengthInBytes = 4;
            const int minValue = int.MinValue;
            const int maxValue = int.MaxValue;

            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, dataTypeLengthInBytes, BindingFlags.None);

            Assert.AreEqual(0, sut.IndicatorLength);

            sut.CopyInt32(0, minValue);

            Assert.AreEqual(minValue, BitConverter.ToInt32(sut.Data, sut.IndicatorLength));

            sut.CopyInt32(0, maxValue);

            Assert.AreEqual(maxValue, BitConverter.ToInt32(sut.Data, sut.IndicatorLength));

            sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, dataTypeLengthInBytes, BindingFlags.Nullable);

            Assert.AreEqual(IndicatorLength, sut.IndicatorLength);

            sut.CopyInt32(0, minValue);

            Assert.AreEqual(minValue, BitConverter.ToInt32(sut.Data, sut.IndicatorLength));

            sut.CopyInt32(0, maxValue);

            Assert.AreEqual(maxValue, BitConverter.ToInt32(sut.Data, sut.IndicatorLength));
        }

        [TestMethod]
        public void CopyInt64ThrowsExceptionWhenIndexOutOfRange()
        {
            const int dataTypeLengthInBytes = 8;
            const long anyValueWillDo = 1;

            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, dataTypeLengthInBytes, BindingFlags.None);

            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() => sut.CopyInt64(-1, anyValueWillDo));
            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() => sut.CopyInt64(dataTypeLengthInBytes, anyValueWillDo));
        }

        [TestMethod]
        public void CopyInt64BehavesAsExpected()
        {
            const int dataTypeLengthInBytes = 8;
            const long minValue = long.MinValue;
            const long maxValue = long.MaxValue;

            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, dataTypeLengthInBytes, BindingFlags.None);

            Assert.AreEqual(0, sut.IndicatorLength);

            sut.CopyInt64(0, minValue);

            Assert.AreEqual(minValue, BitConverter.ToInt64(sut.Data, sut.IndicatorLength));

            sut.CopyInt64(0, maxValue);

            Assert.AreEqual(maxValue, BitConverter.ToInt64(sut.Data, sut.IndicatorLength));

            sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, dataTypeLengthInBytes, BindingFlags.Nullable);

            Assert.AreEqual(IndicatorLength, sut.IndicatorLength);

            sut.CopyInt64(0, minValue);

            Assert.AreEqual(minValue, BitConverter.ToInt64(sut.Data, sut.IndicatorLength));

            sut.CopyInt64(0, maxValue);

            Assert.AreEqual(maxValue, BitConverter.ToInt64(sut.Data, sut.IndicatorLength));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CopyFromThrowsExceptionWhenBytesIsNull()
        {
            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, AnyValidLengthWillDo, BindingFlags.None);

            sut.CopyFrom(null, AnyValidLengthWillDo);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CopyFromThrowsExceptionWhenIndexLessThanZero()
        {
            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, AnyValidLengthWillDo, BindingFlags.None);

            sut.CopyFrom(new byte[AnyValidLengthWillDo], -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CopyFromThrowsExceptionWhenIndexGreaterThanBytesLength()
        {
            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, AnyValidLengthWillDo, BindingFlags.None);

            sut.CopyFrom(new byte[AnyValidLengthWillDo], AnyValidLengthWillDo + 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CopyFromBehavesAsExpected()
        {
            const int dataTypeLengthInBytes = 4;
            var bytes = new byte[] {0, 128, 255};

            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, dataTypeLengthInBytes, BindingFlags.None);

            Assert.AreEqual(0, sut.IndicatorLength);

            sut.CopyFrom(bytes, 0);

            VerifyCopiedBytes(bytes, sut);

            sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, dataTypeLengthInBytes, BindingFlags.Nullable);

            Assert.AreEqual(IndicatorLength, sut.IndicatorLength);

            sut.CopyFrom(bytes, 0);

            Assert.AreEqual(bytes.Length, GetIndicator(sut));
            VerifyCopiedBytes(bytes, sut);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetLengthThrowsExceptionWhenIndicatorNotPresent()
        {
            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, AnyValidLengthWillDo, BindingFlags.None);

            sut.SetLength(AnyValidLengthWillDo);
        }

        [TestMethod]
        public void SetLengthThrowsExceptionWhenIndexOutOfRange()
        {
            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, AnyValidLengthWillDo, BindingFlags.Nullable);

            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() => sut.SetLength(-1));
            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() => sut.SetLength(AnyValidLengthWillDo + 1));
        }

        [TestMethod]
        public void SetLengthBehavesAsExpected()
        {
            const int maxLength = 8000;
            const int minValue = 1;
            const int maxValue = maxLength;

            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, maxLength, BindingFlags.Nullable);

            Assert.AreEqual(IndicatorLength, sut.IndicatorLength);

            sut.SetLength(minValue);

            Assert.AreEqual(minValue, GetIndicator(sut));

            sut.SetLength(maxValue);

            Assert.AreEqual(maxValue, GetIndicator(sut));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetNullThrowsExceptionWhenIndicatorNotPresent()
        {
            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, AnyValidLengthWillDo, BindingFlags.None);

            sut.SetNull();
        }

        [TestMethod]
        public void SetNullBehavesAsExpectedForNullableColumn()
        {
            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, AnyValidLengthWillDo, BindingFlags.Nullable);

            Assert.AreEqual(IndicatorLength, sut.IndicatorLength);

            sut.SetLength(AnyValidLengthWillDo);

            Assert.AreEqual(AnyValidLengthWillDo, GetIndicator(sut));

            sut.SetNull();

            Assert.AreEqual(Constants.SqlNullData, GetIndicator(sut));
        }

        [TestMethod]
        public void SetNullBehavesAsExpectedForVariableLengthColumn()
        {
            const int zeroLength = 0;

            var sut = new StandardDataTypeImpl(AnyValidIndexWillDo, AnyBulkCopyDataTypeWillDo, AnyValidLengthWillDo, BindingFlags.VariableLengthOut);

            Assert.AreEqual(IndicatorLength, sut.IndicatorLength);

            sut.SetLength(AnyValidLengthWillDo);

            Assert.AreEqual(AnyValidLengthWillDo, GetIndicator(sut));

            sut.SetNull();

            Assert.AreEqual(zeroLength, GetIndicator(sut));
        }

        private static int GetIndicator(StandardDataType sut)
        {
            return BitConverter.ToInt32(sut.Data, 0);
        }

        private static void VerifyCopiedBytes(IList<byte> bytes, StandardDataType sut)
        {
            for (var i = 0; i < bytes.Count; i++)
            {
                Assert.AreEqual(bytes[i], sut.Data[sut.IndicatorLength], "Index = {0}", i);
            }
        }

        #endregion
    }
}
