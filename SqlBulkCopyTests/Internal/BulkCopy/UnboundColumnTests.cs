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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlBulkCopy;
using SqlBulkCopy.Internal.BulkCopy;

namespace SqlBulkCopyTests.Internal.BulkCopy
{
    [TestClass]
    public class UnboundColumnTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorThrowsExceptionWhenIndexLessThanZero()
        {
// ReSharper disable UnusedVariable
            var sut = new UnboundColumn(-1, BulkCopyDataType.VarBinaryMax);
// ReSharper restore UnusedVariable
        }

        [TestMethod]
        public void ReturnsExpectedPropertyValues()
        {
            var sut = new UnboundColumn(0, BulkCopyDataType.VarBinaryMax);

            Assert.AreEqual(BulkCopyDataType.VarBinaryMax, sut.DataType);
            Assert.AreEqual("VarBinary(MAX) NOT NULL", sut.DataTypeDescription);
            Assert.AreEqual(0, sut.Index);
            Assert.AreEqual(false, sut.IsBound);

            sut = new UnboundColumn(1, BulkCopyDataType.NullableVarBinaryMax);

            Assert.AreEqual(BulkCopyDataType.NullableVarBinaryMax, sut.DataType);
            Assert.AreEqual("VarBinary(MAX) NULL", sut.DataTypeDescription);
            Assert.AreEqual(1, sut.Index);
            Assert.AreEqual(false, sut.IsBound);
        }
    }
}
