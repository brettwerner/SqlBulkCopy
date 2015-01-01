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
using SqlBulkCopy;
using SqlBulkCopy.Internal.Common;

namespace SqlBulkCopyTests.Internal.Common
{
    [TestClass]
    public class ResourcesTests
    {
        [TestMethod]
        public void GetDataTypeDescriptionReturnsCorrectValues()
        {
            var allValues = Enum.GetValues(typeof (BulkCopyDataType));

            foreach (var value in allValues)
            {
                var nullable = (((int)value & Constants.NullableDataType) == Constants.NullableDataType);

                string expected;

                var dataType = (BulkCopyDataType)((int)value & ~Constants.NullableDataType);

                switch (dataType)
                {
                    case BulkCopyDataType.VarBinaryMax:
                        {
                            expected = "VarBinary(MAX)";
                            break;
                        }
                    case BulkCopyDataType.NVarCharMax:
                        {
                            expected = "NVarChar(MAX)";
                            break;
                        }
                    case BulkCopyDataType.VarCharMax:
                        {
                            expected = "VarChar(MAX)";
                            break;
                        }
                    default:
                        {
                            expected = dataType.ToString("G");
                            break;
                        }
                }

                expected = expected + (nullable ? " NULL" : " NOT NULL");

                var actual = Resources.GetDataTypeDescription((BulkCopyDataType)value);

                Assert.AreEqual(expected, actual, string.Format("{0:G}", value));
            }
        }
    }
}
