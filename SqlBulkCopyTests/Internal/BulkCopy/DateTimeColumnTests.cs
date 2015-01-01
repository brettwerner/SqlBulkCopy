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
using SqlBulkCopy.Internal.BulkCopy;

namespace SqlBulkCopyTests.Internal.BulkCopy
{
    [TestClass]
    public class DateTimeColumnTests
    {
        [TestMethod]
        public void SetValueThrowsExceptionForValueLessThanMinimum()
        {
            var minDateTime = new DateTime(1753, 1, 1);

            var sut = DateTimeColumn.CreateNonNullableInstance(0);

            sut.SetValue(minDateTime); // this should succeed

            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() => sut.SetValue(minDateTime.AddMilliseconds(-1)));
        }

        [TestMethod]
        public void SetValueThrowsExceptionForValueGreaterThanMaximum()
        {
            var maxDateTime = new DateTime(9999, 12, 31, 23, 59, 59, 997);

            var sut = DateTimeColumn.CreateNonNullableInstance(0);

            sut.SetValue(maxDateTime); // this should succeed

            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() => sut.SetValue(maxDateTime.AddMilliseconds(1)));
        }
    }
}
