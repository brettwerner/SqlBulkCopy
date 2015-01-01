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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlBulkCopy;
using SqlBulkCopy.Internal.Drivers;
using SqlBulkCopy.Internal.Odbc;

namespace SqlBulkCopyTests.Internal.Drivers
{
    [TestClass]
    public sealed class SqlServerDriverTests : DriverTests
    {
        protected override object GetSubjectUnderTest()
        {
            return SqlServerDriver.SingleInstance;
        }

        [TestMethod]
        public void CreateDateInstantiatesColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateDate(0);

            Assert.AreEqual(10, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.Date, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(10, column.Length);
            Assert.IsTrue(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateDateTime2InstantiatesColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateDateTime2(0);

            Assert.AreEqual(27, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.DateTime2, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(27, column.Length);
            Assert.IsTrue(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateDateTimeOffsetInstantiatesColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateDateTimeOffset(0);

            Assert.AreEqual(34, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.DateTimeOffset, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(34, column.Length);
            Assert.IsTrue(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateTimeInstantiatesColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateTime(0);

            Assert.AreEqual(16, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.Time, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsFalse(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(16, column.Length);
            Assert.IsTrue(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNullableDateInstantiatesColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateNullableDate(0);

            Assert.AreEqual(10, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableDate, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsTrue(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(10, column.Length);
            Assert.IsTrue(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNullableDateTime2InstantiatesColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateNullableDateTime2(0);

            Assert.AreEqual(27, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableDateTime2, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsTrue(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(27, column.Length);
            Assert.IsTrue(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNullableDateTimeOffsetInstantiatesColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateNullableDateTimeOffset(0);

            Assert.AreEqual(34, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableDateTimeOffset, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsTrue(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(34, column.Length);
            Assert.IsTrue(column.RequiresConversion);
        }

        [TestMethod]
        public void CreateNullableTimeInstantiatesColumnCorrectly()
        {
            var sut = (IOdbcDriver)GetSubjectUnderTest();

            var column = sut.CreateNullableTime(0);

            Assert.AreEqual(16, column.ByteLength);
            Assert.AreEqual(BulkCopyDataType.NullableTime, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsTrue(column.IsBound);
            Assert.IsTrue(column.IsNullable);
            Assert.IsFalse(column.IsVariableLength);
            Assert.AreEqual(16, column.Length);
            Assert.IsTrue(column.RequiresConversion);
        }
    }
}
