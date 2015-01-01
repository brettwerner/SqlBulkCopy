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
using SqlBulkCopy.Internal.Odbc;

namespace SqlBulkCopyTests.Internal.Odbc
{
    [TestClass]
    public class OdbcConnectionHandleTests
    {
// ReSharper disable UnusedVariable
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsExceptionWhenParentHandleIsNull()
        {
            var driver = new Mock<IOdbcDriver>();

            var sut = new OdbcConnectionHandle(null, driver.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsExceptionWhenDriverIsNull()
        {
            var driver = new Mock<IOdbcDriver>();

            using (var environmentHandle = new OdbcEnvironmentHandle(OdbcVersion.Version38, OdbcConnectionPooling.None, driver.Object))
            {
                var sut = new OdbcConnectionHandle(environmentHandle, null);
            }
        }
// ReSharper restore UnusedVariable

        [TestMethod]
        public void ConnectThrowsExceptionWhenTableNameIsNullOrEmpty()
        {
            var driver = new Mock<IOdbcDriver>();

            using (var environmentHandle = new OdbcEnvironmentHandle(OdbcVersion.Version38, OdbcConnectionPooling.None, driver.Object))
            {
                using (IOdbcConnection sut = new OdbcConnectionHandle(environmentHandle, driver.Object))
                {
                    var bc = sut;

                    TestHelper.AssertThrows<ArgumentException>(() => bc.Connect(null));
                    TestHelper.AssertThrows<ArgumentException>(() => bc.Connect(string.Empty));
                }
            }
        }

        [TestMethod]
        public void ConnectThrowsExceptionIfAlreadyConnected()
        {
            const string anyConnectionStringWillDo = "x";

            var driver = new Mock<IOdbcDriver>();

            using (var environmentHandle = new OdbcEnvironmentHandle(OdbcVersion.Version38, OdbcConnectionPooling.None, driver.Object))
            {
                using (IOdbcConnection sut = new OdbcConnectionHandle(environmentHandle, driver.Object))
                {
                    sut.SetPrivateFieldValue("_connected", true);

                    var bc = sut;

                    try
                    {
                        TestHelper.AssertThrows<InvalidOperationException>(() => bc.Connect(anyConnectionStringWillDo));
                    }
                    finally
                    {
                        sut.SetPrivateFieldValue("_connected", false);
                    }
                }
            }
        }

        [TestMethod]
        public void ExecuteDirectThrowsExceptionWhenCommandTextIsNullOrEmpty()
        {
            var driver = new Mock<IOdbcDriver>();

            using (var environmentHandle = new OdbcEnvironmentHandle(OdbcVersion.Version38, OdbcConnectionPooling.None, driver.Object))
            {
                using (IOdbcConnection sut = new OdbcConnectionHandle(environmentHandle, driver.Object))
                {
                    var bc = sut;

                    TestHelper.AssertThrows<ArgumentException>(() => bc.ExecuteDirect(null));
                    TestHelper.AssertThrows<ArgumentException>(() => bc.ExecuteDirect(string.Empty));
                }
            }
        }
    }
}
