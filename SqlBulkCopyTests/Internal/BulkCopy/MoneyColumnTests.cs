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
    public class MoneyColumnTests
    {
        [TestMethod]
        public void SetValueThrowsExceptionForMoneyValueLessThanMinimum()
        {
            const decimal minMoney = -922337203685477.5808M;

            var sut = MoneyColumn.CreateNonNullableMoneyInstance(0);

            sut.SetValue(minMoney); // this should succeed

            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() => sut.SetValue(minMoney - 0.00000001M));
        }

        [TestMethod]
        public void SetValueThrowsExceptionForMoneyValueGreaterThanMaximum()
        {
            const decimal maxMoney = 922337203685477.5807M;

            var sut = MoneyColumn.CreateNonNullableMoneyInstance(0);

            sut.SetValue(maxMoney); // this should succeed

            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() => sut.SetValue(maxMoney + 0.00000001M));
        }

        [TestMethod]
        public void SetValueThrowsExceptionForSmallMoneyValueLessThanMinimum()
        {
            const decimal minSmallMoney = -214748.3648M;

            var sut = MoneyColumn.CreateNonNullableSmallMoneyInstance(0);

            sut.SetValue(minSmallMoney); // this should succeed

            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() => sut.SetValue(minSmallMoney - 0.00000001M));
        }

        [TestMethod]
        public void SetValueThrowsExceptionForSmallMoneyValueGreaterThanMaximum()
        {
            const decimal maxSmallMoney = 214748.3647M;

            var sut = MoneyColumn.CreateNonNullableSmallMoneyInstance(0);

            sut.SetValue(maxSmallMoney); // this should succeed

            TestHelper.AssertThrows<ArgumentOutOfRangeException>(() => sut.SetValue(maxSmallMoney + 0.00000001M));
        }
    }
}
