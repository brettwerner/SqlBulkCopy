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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SqlBulkCopy;
using SqlBulkCopy.Internal.BulkCopy;
using SqlBulkCopy.Internal.Odbc;

namespace SqlBulkCopyTests.Internal.Drivers
{
    [TestClass]
    public abstract class DriverTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BatchThrowsExceptionWhenHandleIsNull()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            sut.Batch(null);
        }

        protected abstract object GetSubjectUnderTest();

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BindStandardColumnThrowsExceptionWhenHandleIsNull()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = new Mock<IStandardColumn<byte>>();

            sut.Bind(null, column.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BindStandardColumnThrowsExceptionWhenColumnIsNull()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            using (var environmentHandle = sut.CreateEnvironment(OdbcConnectionPooling.None))
            {
                using (var connectionHandle = (OdbcConnectionHandle)environmentHandle.CreateConnection())
                {
                    sut.Bind(connectionHandle, (IStandardColumn<byte>)null);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BindMaxColumnThrowsExceptionWhenHandleIsNull()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = new Mock<IMaxColumn<byte>>();

            sut.Bind(null, column.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BindMaxColumnThrowsExceptionWhenColumnIsNull()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            using (var environmentHandle = sut.CreateEnvironment(OdbcConnectionPooling.None))
            {
                using (var connectionHandle = (OdbcConnectionHandle)environmentHandle.CreateConnection())
                {
                    sut.Bind(connectionHandle, (IMaxColumn<byte>)null);
                }
            }
        }

        [TestMethod]
        public void CreateBigIntReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateBigInt(0);

            Assert.AreEqual(8, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.BigInt, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(8, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateBinaryReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            const int anyLengthWillDo = 100;

            var column1 = sut.CreateBinary(0, anyLengthWillDo);
            var column2 = sut.CreateBinary(1, anyLengthWillDo, true);

            Assert.AreEqual(anyLengthWillDo, column1.ByteLength);
            Assert.AreEqual(BulkCopyDataType.Binary, column1.DataType);
            Assert.AreEqual(0, column1.Index);
            Assert.IsTrue(column1.IsBound);
            Assert.IsFalse(column1.IsNullable);
            Assert.IsFalse(column1.IsVariableLength);
            Assert.AreEqual(anyLengthWillDo, column1.Length);
            Assert.IsFalse(column1.RequiresConversion);

            Assert.AreEqual(anyLengthWillDo, column2.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableBinary, column2.DataType);
            Assert.AreEqual(1, column2.Index);
            Assert.IsTrue(column2.IsBound);
            Assert.IsTrue(column2.IsNullable);
            Assert.IsFalse(column2.IsVariableLength);
            Assert.AreEqual(anyLengthWillDo, column2.Length);
            Assert.IsFalse(column2.RequiresConversion);
        }

        [TestMethod]
        public void CreateBitReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateBit(0);

            Assert.AreEqual(1, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.Bit, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(1, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateCharReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            const int anyLengthWillDo = 100;

            var column1 = sut.CreateChar(0, anyLengthWillDo);
            var column2 = sut.CreateChar(1, anyLengthWillDo, true);

            Assert.AreEqual(anyLengthWillDo, column1.ByteLength);
            Assert.AreEqual(BulkCopyDataType.Char, column1.DataType);
            Assert.AreEqual(0, column1.Index);
            Assert.IsTrue(column1.IsBound);
            Assert.IsFalse(column1.IsNullable);
            Assert.IsFalse(column1.IsVariableLength);
            Assert.AreEqual(anyLengthWillDo, column1.Length);
            Assert.IsFalse(column1.RequiresConversion);

            Assert.AreEqual(anyLengthWillDo, column2.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableChar, column2.DataType);
            Assert.AreEqual(1, column2.Index);
            Assert.IsTrue(column2.IsBound);
            Assert.IsTrue(column2.IsNullable);
            Assert.IsFalse(column2.IsVariableLength);
            Assert.AreEqual(anyLengthWillDo, column2.Length);
            Assert.IsFalse(column2.RequiresConversion);
        }

        [TestMethod]
        public void CreateDateTimeReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateDateTime(0);

            Assert.AreEqual(8, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.DateTime, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(8, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateDecimalReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateDecimal(0);

            Assert.AreEqual(19, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.Decimal, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(19, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateFloatReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateFloat(0);

            Assert.AreEqual(8, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.Float, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(8, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateIntReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateInt(0);

            Assert.AreEqual(4, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.Int, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(4, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateMoneyReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateMoney(0);

            Assert.AreEqual(8, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.Money, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(8, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNCharReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            const int anyLengthWillDo = 100;

            var column1 = sut.CreateNChar(0, anyLengthWillDo);
            var column2 = sut.CreateNChar(1, anyLengthWillDo, true);

            Assert.AreEqual(anyLengthWillDo * 2, column1.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NChar, column1.DataType);
            Assert.AreEqual(0, column1.Index);
            Assert.IsTrue(column1.IsBound);
            Assert.IsFalse(column1.IsNullable);
            Assert.IsFalse(column1.IsVariableLength);
            Assert.AreEqual(anyLengthWillDo, column1.Length);
            Assert.IsFalse(column1.RequiresConversion);

            Assert.AreEqual(anyLengthWillDo * 2, column2.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableNChar, column2.DataType);
            Assert.AreEqual(1, column2.Index);
            Assert.IsTrue(column2.IsBound);
            Assert.IsTrue(column2.IsNullable);
            Assert.IsFalse(column2.IsVariableLength);
            Assert.AreEqual(anyLengthWillDo, column2.Length);
            Assert.IsFalse(column2.RequiresConversion);
        }

        [TestMethod]
        public void CreateNVarCharReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            const int anyLengthWillDo = 100;

            var column1 = sut.CreateNVarChar(0, anyLengthWillDo);
            var column2 = sut.CreateNVarChar(1, anyLengthWillDo, true);

            Assert.AreEqual(anyLengthWillDo * 2, column1.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NVarChar, column1.DataType);
            Assert.AreEqual(0, column1.Index);
            Assert.IsTrue(column1.IsBound);
            Assert.IsFalse(column1.IsNullable);
            Assert.IsTrue(column1.IsVariableLength);
            Assert.AreEqual(anyLengthWillDo, column1.Length);
            Assert.IsFalse(column1.RequiresConversion);

            Assert.AreEqual(anyLengthWillDo * 2, column2.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableNVarChar, column2.DataType);
            Assert.AreEqual(1, column2.Index);
            Assert.IsTrue(column2.IsBound);
            Assert.IsTrue(column2.IsNullable);
            Assert.IsTrue(column2.IsVariableLength);
            Assert.AreEqual(anyLengthWillDo, column2.Length);
            Assert.IsFalse(column2.RequiresConversion);
        }

        [TestMethod]
        public void CreateNVarCharMaxReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column1 = sut.CreateNVarCharMax(0);
            var column2 = sut.CreateNVarCharMax(1, true);

            Assert.AreEqual(-1, column1.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NVarCharMax, column1.DataType);
            Assert.AreEqual(0, column1.Index);
            Assert.IsTrue(column1.IsBound);
            Assert.IsFalse(column1.IsNullable);
            Assert.IsTrue(column1.IsVariableLength);
            Assert.AreEqual(-1, column1.Length);

            Assert.AreEqual(-1, column2.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableNVarCharMax, column2.DataType);
            Assert.AreEqual(1, column2.Index);
            Assert.IsTrue(column2.IsBound);
            Assert.IsTrue(column2.IsNullable);
            Assert.IsTrue(column2.IsVariableLength);
            Assert.AreEqual(-1, column2.Length);
        }

        [TestMethod]
        public void CreateRealReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateReal(0);

            Assert.AreEqual(4, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.Real, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(4, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateSmallDateTimeReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateSmallDateTime(0);

            Assert.AreEqual(4, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.SmallDateTime, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(4, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateSmallIntReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateSmallInt(0);

            Assert.AreEqual(2, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.SmallInt, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(2, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateSmallMoneyReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateSmallMoney(0);

            Assert.AreEqual(4, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.SmallMoney, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(4, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateTinyIntReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateTinyInt(0);

            Assert.AreEqual(1, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.TinyInt, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(1, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateUniqueIdentifierReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateUniqueIdentifier(0);

            Assert.AreEqual(16, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.UniqueIdentifier, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(16, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNullableBigIntReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateNullableBigInt(0);

            Assert.AreEqual(8, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableBigInt, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsTrue(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(8, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNullableBitReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateNullableBit(0);

            Assert.AreEqual(1, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableBit, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsTrue(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(1, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNullableDateTimeReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateNullableDateTime(0);

            Assert.AreEqual(8, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableDateTime, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsTrue(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(8, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNullableDecimalReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateNullableDecimal(0);

            Assert.AreEqual(19, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableDecimal, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsTrue(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(19, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNullableFloatReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateNullableFloat(0);

            Assert.AreEqual(8, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableFloat, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsTrue(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(8, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNullableIntReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateNullableInt(0);

            Assert.AreEqual(4, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableInt, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsTrue(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(4, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNullableMoneyReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateNullableMoney(0);

            Assert.AreEqual(8, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableMoney, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsTrue(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(8, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNullableRealReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateNullableReal(0);

            Assert.AreEqual(4, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableReal, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsTrue(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(4, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNullableSmallDateTimeReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateNullableSmallDateTime(0);

            Assert.AreEqual(4, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableSmallDateTime, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsTrue(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(4, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNullableSmallIntReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateNullableSmallInt(0);

            Assert.AreEqual(2, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableSmallInt, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsTrue(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(2, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNullableSmallMoneyReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateNullableSmallMoney(0);

            Assert.AreEqual(4, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableSmallMoney, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsTrue(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(4, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNullableTinyIntReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateNullableTinyInt(0);

            Assert.AreEqual(1, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableTinyInt, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsTrue(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(1, column.Length);
            Assert.IsFalse(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateVarBinaryReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            const int anyLengthWillDo = 100;

            var column1 = sut.CreateVarBinary(0, anyLengthWillDo);
            var column2 = sut.CreateVarBinary(1, anyLengthWillDo, true);

            Assert.AreEqual(anyLengthWillDo, column1.ByteLength);
            Assert.AreEqual(BulkCopyDataType.VarBinary, column1.DataType);
            Assert.AreEqual(0, column1.Index);
            Assert.IsTrue(column1.IsBound);
            Assert.IsFalse(column1.IsNullable);
            Assert.IsTrue(column1.IsVariableLength);
            Assert.AreEqual(anyLengthWillDo, column1.Length);
            Assert.IsFalse(column1.RequiresConversion);

            Assert.AreEqual(anyLengthWillDo, column2.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableVarBinary, column2.DataType);
            Assert.AreEqual(1, column2.Index);
            Assert.IsTrue(column2.IsBound);
            Assert.IsTrue(column2.IsNullable);
            Assert.IsTrue(column2.IsVariableLength);
            Assert.AreEqual(anyLengthWillDo, column2.Length);
            Assert.IsFalse(column2.RequiresConversion);
        }

        [TestMethod]
        public void CreateVarBinaryMaxReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column1 = sut.CreateVarBinaryMax(0);
            var column2 = sut.CreateVarBinaryMax(1, true);

            Assert.AreEqual(-1, column1.ByteLength);
            Assert.AreEqual(BulkCopyDataType.VarBinaryMax, column1.DataType);
            Assert.AreEqual(0, column1.Index);
            Assert.IsTrue(column1.IsBound);
            Assert.IsFalse(column1.IsNullable);
            Assert.IsTrue(column1.IsVariableLength);
            Assert.AreEqual(-1, column1.Length);

            Assert.AreEqual(-1, column2.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableVarBinaryMax, column2.DataType);
            Assert.AreEqual(1, column2.Index);
            Assert.IsTrue(column2.IsBound);
            Assert.IsTrue(column2.IsNullable);
            Assert.IsTrue(column2.IsVariableLength);
            Assert.AreEqual(-1, column2.Length);
        }

        [TestMethod]
        public void CreateVarCharReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            const int anyLengthWillDo = 100;

            var column1 = sut.CreateVarChar(0, anyLengthWillDo);
            var column2 = sut.CreateVarChar(1, anyLengthWillDo, true);

            Assert.AreEqual(anyLengthWillDo, column1.ByteLength);
            Assert.AreEqual(BulkCopyDataType.VarChar, column1.DataType);
            Assert.AreEqual(0, column1.Index);
            Assert.IsTrue(column1.IsBound);
            Assert.IsFalse(column1.IsNullable);
            Assert.IsTrue(column1.IsVariableLength);
            Assert.AreEqual(anyLengthWillDo, column1.Length);
            Assert.IsFalse(column1.RequiresConversion);

            Assert.AreEqual(anyLengthWillDo, column2.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableVarChar, column2.DataType);
            Assert.AreEqual(1, column2.Index);
            Assert.IsTrue(column2.IsBound);
            Assert.IsTrue(column2.IsNullable);
            Assert.IsTrue(column2.IsVariableLength);
            Assert.AreEqual(anyLengthWillDo, column2.Length);
            Assert.IsFalse(column2.RequiresConversion);
        }

        [TestMethod]
        public void CreateVarCharMaxReturnsColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column1 = sut.CreateVarCharMax(0);
            var column2 = sut.CreateVarCharMax(1, true);

            Assert.AreEqual(-1, column1.ByteLength);
            Assert.AreEqual(BulkCopyDataType.VarCharMax, column1.DataType);
            Assert.AreEqual(0, column1.Index);
            Assert.IsTrue(column1.IsBound);
            Assert.IsFalse(column1.IsNullable);
            Assert.IsTrue(column1.IsVariableLength);
            Assert.AreEqual(-1, column1.Length);

            Assert.AreEqual(-1, column2.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableVarCharMax, column2.DataType);
            Assert.AreEqual(1, column2.Index);
            Assert.IsTrue(column2.IsBound);
            Assert.IsTrue(column2.IsNullable);
            Assert.IsTrue(column2.IsVariableLength);
            Assert.AreEqual(-1, column2.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DoneThrowsExceptionWhenHandleIsNull()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            sut.Done(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InitializeThrowsExceptionWhenHandleIsNull()
        {
            const string anyTableNameWillDo = "x";

            var sut = (IOdbcDriver)GetSubjectUnderTest();

            sut.Initialize(null, anyTableNameWillDo);
        }

        [TestMethod]
        public void InitializeThrowsExceptionWhenTableNameIsNullOrEmpty()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            using (var environmentHandle = sut.CreateEnvironment(OdbcConnectionPooling.None))
            {
                using (var connectionHandle = (OdbcConnectionHandle)environmentHandle.CreateConnection())
                {
                    var handle = connectionHandle;

                    TestHelper.AssertThrows<ArgumentNullException>(() => sut.Initialize(handle, null));
                    TestHelper.AssertThrows<ArgumentNullException>(() => sut.Initialize(handle, string.Empty));
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendMoreTextThrowsExceptionWhenHandleIsNull()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = new Mock<IMaxDataType>();

            sut.SendMoreText(null, column.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendMoreTextThrowsExceptionWhenColumnIsNull()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            using (var environmentHandle = sut.CreateEnvironment(OdbcConnectionPooling.None))
            {
                using (var connectionHandle = (OdbcConnectionHandle)environmentHandle.CreateConnection())
                {
                    sut.SendMoreText(connectionHandle, null);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendRowThrowsExceptionWhenHandleIsNull()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            sut.SendRow(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetColumnLengthThrowsExceptionWhenHandleIsNull()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = new Mock<IMaxDataType>();

            sut.SetColumnLength(null, column.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetColumnLengthThrowsExceptionWhenColumnIsNull()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            using (var environmentHandle = sut.CreateEnvironment(OdbcConnectionPooling.None))
            {
                using (var connectionHandle = (OdbcConnectionHandle)environmentHandle.CreateConnection())
                {
                    sut.SetColumnLength(connectionHandle, null);
                }
            }
        }
    }
}
