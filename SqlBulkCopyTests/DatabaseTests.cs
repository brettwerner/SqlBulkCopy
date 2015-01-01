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
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlBulkCopy;

namespace SqlBulkCopyTests
{
    [TestClass]
    public class DatabaseTests
    {
        [TestMethod]
        public void RoundtripsAsExpectedUsingSqlServerDriver()
        {
            try
            {
                VerifyRoundtripsAsExpected(OdbcDriver.SqlServer);
            }
            finally
            {
                // Ensure we restore this property to the default value for all other tests.
                BulkCopy.Driver = OdbcDriver.SqlServerNativeClient11;
            }
        }

        [TestMethod, Ignore]
        public void RoundtripsAsExpectedUsingSqlServerNativeClientDriver()
        {
            VerifyRoundtripsAsExpected(OdbcDriver.SqlServerNativeClient);
        }

        [TestMethod, Ignore]
        public void RoundtripsAsExpectedUsingSqlServerNativeClient10Driver()
        {
            VerifyRoundtripsAsExpected(OdbcDriver.SqlServerNativeClient10);
        }

        [TestMethod]
        public void RoundtripsAsExpectedUsingSqlServerNativeClient11Driver()
        {
            VerifyRoundtripsAsExpected(OdbcDriver.SqlServerNativeClient11);
        }

        private static void VerifyRoundtripsAsExpected(OdbcDriver driver)
        {
            /*
            * Reasons this test might fail:
            * 1. You need to execute SqlBulkCopy.sql (in the same directory as this test) to create the test database.
            * 2. You need to update the connection string to specify the location of your instance of the test database.
            * 3. The particular driver is not installed on your PC.
            */

            const string connectionString = "Server=WORKSTATION\\SQL2012;Database=SqlBulkCopy;Trusted_Connection=yes;";
            const string tableName = "SqlBulkCopyTest";

            List<IBulkCopyBoundColumn> columns;
            var data = TestData.GetSampleData();

            BulkCopy.Driver = driver;
            BulkCopy.Trace = true;

            try
            {
                using (var sut = new BulkCopy())
                {
                    sut.LoginTimeout = 2;

                    try
                    {
                        sut.Connect(string.Concat(sut.DriverOptionString, ";", connectionString));

                    }
                    catch (Exception ex)
                    {
                        const string message = "Please refer to this test for possible solutions." +
                            "\r\n\r\nTesting: {0}." +
                            "\r\n\r\nODBC exception text: {1}.";

                        Assert.Fail(message, sut.DriverOptionString, ex.Message);
                    }

                    sut.ConnectionTimeout = 2;

                    sut.ExecuteDirect("TRUNCATE TABLE SqlBulkCopyTest;");

                    sut.Initialize(tableName);

                    var importer = new TestDataImporter(sut);

                    columns = sut.Columns
                        .Where(c => c.Index > 0)
                        .Cast<IBulkCopyBoundColumn>()
                        .ToList();

                    LoadData(importer, data);
                }
            }
            finally
            {
                BulkCopy.Trace = false;
            }

            List<object[]> rows;

            using (var cn = new SqlConnection(connectionString))
            {
                cn.Open();

                rows = FetchData(cn, columns.Count);
            }

            VerifyFetchedData(columns, data, rows);
        }

        private static void LoadData(TestDataImporter importer, IEnumerable<TestData> data)
        {
            foreach (var row in data)
            {
                var success = importer.SendRow(row);

                Assert.IsTrue(success, "SendRow");
            }

            Assert.IsTrue(importer.Done(), "Done");
        }

        private static List<object[]> FetchData(SqlConnection cn, int expectedColumns)
        {
            var rows = new List<object[]>();

            using (var cmd = new SqlCommand("SELECT * FROM SqlBulkCopyTest ORDER BY identityColumn;", cn))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        var row = new object[expectedColumns];
                        var actualColumns = reader.GetValues(row);

                        Assert.AreEqual(expectedColumns, actualColumns);

                        rows.Add(row);
                    }
                }
            }

            return rows;
        }

        private static void VerifyFetchedData(IReadOnlyList<IBulkCopyBoundColumn> columns, IReadOnlyList<TestData> data, IReadOnlyList<object[]> rows)
        {
            for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var expectedValues = data[rowIndex].GetValues(rowIndex + 1);
                var actualValues = rows[rowIndex];

                for (var columnIndex = 1; columnIndex < columns.Count; columnIndex++)
                {
                    var column = columns[columnIndex - 1];
                    var expectedValue = expectedValues[columnIndex];
                    var actualValue = actualValues[columnIndex];

                    var areEqual = AreEqual(expectedValue, actualValue, column.Length, column.IsVariableLength);

                    if (!areEqual)
                    {
                        Assert.Fail("Values at (column {0}, row {1}) are not equal. Data type: {2}. Expected: {3}. Actual: {4}.", columnIndex, rowIndex, column.DataTypeDescription, expectedValue ?? "(null)", actualValue ?? "(null)");
                    }
                }
            }
        }

        private static bool AreEqual(object value1, object value2, int columnLength, bool columnIsVariableLength)
        {
            if (value1 is byte[] || value2 is byte[])
            {
                return VerifyByteArray(value1 as byte[], value2 as byte[], columnLength, columnIsVariableLength);
            }

            if (value1 is string || value2 is string)
            {
                return VerifyString(value1 as string, value2 as string, columnLength, columnIsVariableLength);
            }

            var comparable = value1 as IComparable;

            if (comparable != null)
            {
                return comparable.Equals(value2);
            }

            comparable = value2 as IComparable;

            if (comparable != null)
            {
                return comparable.Equals(value1);
            }

            if (IsNullOrDbNull(value1))
            {
                return IsNullOrDbNull(value2);
            }

            return value1.GetType() == value2.GetType() && value1.Equals(value2);
        }

        private static bool IsNullOrDbNull(object value)
        {
            return value == null || value == DBNull.Value;
        }

        private static bool VerifyByteArray(IList<byte> array1, IList<byte> array2, int length, bool variableLength)
        {
            var array1Length = array1 == null ? 0 : array1.Count;
            var array2Length = array2 == null ? 0 : array2.Count;

            if (variableLength)
            {
                if (array2Length != array1Length)
                {
                    return false;
                }
            }
            else
            {
                if (array2Length != length)
                {
                    return false;
                }
            }

            for (var i = 0; i < array1Length; i++)
            {
// ReSharper disable PossibleNullReferenceException
                if (array2[i] != array1[i])
// ReSharper restore PossibleNullReferenceException
                {
                    return false;
                }
            }

            if (!variableLength)
            {
                for (var i = array1Length; i < length; i++)
                {
// ReSharper disable PossibleNullReferenceException
                    if (array2[i] != 0)
// ReSharper restore PossibleNullReferenceException
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool VerifyString(string value1, string value2, int length, bool variableLength)
        {
            var string1Length = value1 == null ? 0 : value1.Length;
            var string2Length = value2 == null ? 0 : value2.Length;

            if (variableLength)
            {
                if (string2Length != string1Length)
                {
                    return false;
                }
            }
            else
            {
                if (string2Length != length)
                {
                    return false;
                }
            }

            if ((string1Length > 0) && (string.CompareOrdinal(value1, 0, value2, 0, string1Length) != 0))
            {
                return false;
            }

            if (!variableLength)
            {
                for (var i = string1Length; i < length; i++)
                {
// ReSharper disable PossibleNullReferenceException
                    if (value2[i] != ' ')
// ReSharper restore PossibleNullReferenceException
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
