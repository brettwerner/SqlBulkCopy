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
using SqlBulkCopy.Internal.BulkCopy;

namespace SqlBulkCopyTests.Internal.BulkCopy
{
    [TestClass]
    public class DecimalFunctionsTests
    {
        [TestMethod]
        public void AdjustScaleReturnsExpectedValue()
        {
            Assert.AreEqual(1.0000M, 1M);

            CollectionAssert.AreNotEqual(decimal.GetBits(1.0000M), decimal.GetBits(1M));

            CollectionAssert.AreEqual(decimal.GetBits(1.0000M), DecimalFunctions.AdjustScale(1M, 4));
            CollectionAssert.AreEqual(decimal.GetBits(1.2000M), DecimalFunctions.AdjustScale(1.2M, 4));
            CollectionAssert.AreEqual(decimal.GetBits(1.2300M), DecimalFunctions.AdjustScale(1.23M, 4));
            CollectionAssert.AreEqual(decimal.GetBits(1.2340M), DecimalFunctions.AdjustScale(1.234M, 4));
            CollectionAssert.AreEqual(decimal.GetBits(1.2345M), DecimalFunctions.AdjustScale(1.2345M, 4));
            CollectionAssert.AreEqual(decimal.GetBits(1.2346M), DecimalFunctions.AdjustScale(1.23456M, 4));
        }
    }
}
