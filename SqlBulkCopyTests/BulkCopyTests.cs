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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SqlBulkCopy;
using SqlBulkCopy.Internal.BulkCopy;
using SqlBulkCopy.Internal.Odbc;

namespace SqlBulkCopyTests
{
    [TestClass]
    public class BulkCopyTests
    {
        private Mock<IOdbcDriver> _driver;
        private Mock<IOdbcEnvironment> _environment;
        private Mock<IOdbcConnection> _connection;

        private const string AnyCommandTextWillDo = "x";
        private const string AnyConnectionStringWillDo = "y";
        private const string AnyTableNameWillDo = "z";

        [TestInitialize]
        public void TestInitialize()
        {
            // Ensure all tests in this class use the mocked IOdbcDriver.

            _driver = new Mock<IOdbcDriver>();
            _environment = new Mock<IOdbcEnvironment>();
            _connection = new Mock<IOdbcConnection>();

            _driver
                .Setup(m => m.CreateEnvironment(It.IsAny<OdbcConnectionPooling>()))
                .Returns(_environment.Object);

            _environment
                .Setup(m => m.CreateConnection())
                .Returns(_connection.Object);

            _connection
                .SetupGet(m => m.Driver)
                .Returns(_driver.Object);

            BulkCopy.CurrentDriver = _driver.Object;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Reset the static CurrentDriver property.

            BulkCopy.CurrentDriver = null;
        }

        #region setup

        [TestMethod]
        public void CreatesSharedEnvironment()
        {
            var environment = BulkCopy.CreateSharedEnvironment();

            Assert.AreSame(_environment.Object, environment.InternalEnvironment);

            _driver.Verify(m => m.CreateEnvironment(OdbcConnectionPooling.PerEnvironment), Times.Once);
        }

        [TestMethod]
        public void InitializesAndDisposesAsExpectedWhenNotUsingSharedEnvironment()
        {
            using (var sut = new BulkCopy())
            {
                Assert.IsNotNull(sut.Columns);
            }

            // construction

            _driver.Verify(m => m.CreateEnvironment(OdbcConnectionPooling.None), Times.Once);
            _environment.Verify(m => m.CreateConnection(), Times.Once);

            // destruction

            _connection.Verify(m => m.Disconnect(), Times.Once);
            _connection.Verify(m => m.Dispose(), Times.Once);
            _environment.Verify(m => m.Dispose(), Times.Once);
        }

        [TestMethod]
        public void InitializesAndDisposesAsExpectedWhenUsingSharedEnvironment()
        {
            var environment = BulkCopy.CreateSharedEnvironment();

            using (var sut = new BulkCopy(environment))
            {
                Assert.IsNotNull(sut.Columns);
            }

            // construction

            _driver.Verify(m => m.CreateEnvironment(OdbcConnectionPooling.PerEnvironment), Times.Once);
            _environment.Verify(m => m.CreateConnection(), Times.Once);

            // destruction

            _connection.Verify(m => m.Disconnect(), Times.Once);
            _connection.Verify(m => m.Dispose(), Times.Once);
            _environment.Verify(m => m.Dispose(), Times.Never);

            environment.Dispose();

            _environment.Verify(m => m.Dispose(), Times.Once);
        }

        [TestMethod]
        public void ConnectThrowsExceptionWhenTableNameIsNullOrEmpty()
        {
            using (var sut = new BulkCopy())
            {
                var bc = sut;

                TestHelper.AssertThrows<ArgumentException>(() => bc.Connect(null));
                TestHelper.AssertThrows<ArgumentException>(() => bc.Connect(string.Empty));
            }
        }

        [TestMethod]
        public void ConnectThrowsExceptionIfAlreadyConnected()
        {
            _connection
                .SetupGet(m => m.Connected)
                .Returns(() => true);

            using (var sut = new BulkCopy())
            {
                var bc = sut;

                TestHelper.AssertThrows<InvalidOperationException>(() => bc.Connect(AnyConnectionStringWillDo));
            }

            _connection.VerifyGet(m => m.Connected, Times.Once);
            _connection.Verify(m => m.Connect(AnyConnectionStringWillDo), Times.Never);
        }

        [TestMethod]
        public void ConnectBehavesAsExpected()
        {
            var connected = false;

            _connection
                .SetupGet(m => m.Connected)
                .Returns(() => connected);

            _connection
                .Setup(m => m.Connect(AnyConnectionStringWillDo))
                .Callback(() => connected = true);

            using (var sut = new BulkCopy())
            {
                sut.Connect(AnyConnectionStringWillDo);
            }

            _connection.VerifyGet(m => m.Connected, Times.Once);
            _connection.Verify(m => m.Connect(AnyConnectionStringWillDo), Times.Once);
        }

        [TestMethod]
        public void ExecuteDirectThrowsExceptionWhenCommandTextIsNullOrEmpty()
        {
            using (var sut = new BulkCopy())
            {
                var bc = sut;

                TestHelper.AssertThrows<ArgumentException>(() => bc.ExecuteDirect(null));
                TestHelper.AssertThrows<ArgumentException>(() => bc.ExecuteDirect(string.Empty));
            }
        }

        [TestMethod]
        public void ExecuteDirectThrowsExceptionIfNotConnected()
        {
            _connection
                .SetupGet(m => m.Connected)
                .Returns(() => false);

            using (var sut = new BulkCopy())
            {
                var bc = sut;

                TestHelper.AssertThrows<InvalidOperationException>(() => bc.ExecuteDirect(AnyCommandTextWillDo));
            }

            _connection.VerifyGet(m => m.Connected, Times.Once);
            _connection.Verify(m => m.ExecuteDirect(AnyCommandTextWillDo), Times.Never);
        }

        [TestMethod]
        public void ExecuteDirectBehavesAsExpected()
        {
            _connection
                .SetupGet(m => m.Connected)
                .Returns(true);

            using (var sut = new BulkCopy())
            {
                sut.ExecuteDirect(AnyCommandTextWillDo);
            }

            _connection.VerifyGet(m => m.Connected, Times.Once);
            _connection.Verify(m => m.ExecuteDirect(AnyCommandTextWillDo), Times.Once);
        }

        [TestMethod]
        public void InitializeThrowsExceptionWhenTableNameIsNullOrEmpty()
        {
            using (var sut = new BulkCopy())
            {
                var bc = sut;

                TestHelper.AssertThrows<ArgumentException>(() => bc.Initialize(null));
                TestHelper.AssertThrows<ArgumentException>(() => bc.Initialize(string.Empty));
            }
        }

        [TestMethod]
        public void InitializeThrowsExceptionIfNotConnected()
        {
            _connection
                .SetupGet(m => m.Connected)
                .Returns(() => false);

            using (var sut = new BulkCopy())
            {
                var bc = sut;

                TestHelper.AssertThrows<InvalidOperationException>(() => bc.Initialize(AnyTableNameWillDo));
            }

            _connection.VerifyGet(m => m.Connected, Times.Once);
            _connection.Verify(m => m.Initialize(AnyTableNameWillDo), Times.Never);
        }

        [TestMethod]
        public void InitializeThrowsExceptionIfAlreadyInitialized()
        {
            _connection
                .SetupGet(m => m.Connected)
                .Returns(() => true);

            _connection
                .SetupGet(m => m.Initialized)
                .Returns(() => true);

            using (var sut = new BulkCopy())
            {
                var bc = sut;

                TestHelper.AssertThrows<InvalidOperationException>(() => bc.Initialize(AnyTableNameWillDo));
            }

            _connection.VerifyGet(m => m.Connected, Times.Once);
            _connection.VerifyGet(m => m.Initialized, Times.Once);
            _connection.Verify(m => m.Initialize(AnyTableNameWillDo), Times.Never);
        }

        [TestMethod]
        public void InitializeBehavesAsExpected()
        {
            var initialized = false;

            _connection
                .SetupGet(m => m.Connected)
                .Returns(() => true);

            _connection
                .SetupGet(m => m.Initialized)
                .Returns(() => initialized);

            _connection
                .Setup(m => m.Initialize(AnyTableNameWillDo))
                .Callback(() => initialized = true);

            using (var sut = new BulkCopy())
            {
                sut.Initialize(AnyTableNameWillDo);
            }

            _connection.VerifyGet(m => m.Connected, Times.Once);
            _connection.VerifyGet(m => m.Initialized, Times.Once);
            _connection.Verify(m => m.Initialize(AnyTableNameWillDo), Times.Once);
        }

        #endregion

        private BulkCopy GetInitializedSubjectUnderTest()
        {
            var connected = false;
            var initialized = false;

            _connection
                .SetupGet(m => m.Connected)
                .Returns(() => connected);

            _connection
                .SetupGet(m => m.Initialized)
                .Returns(() => initialized);

            _connection
                .Setup(m => m.Connect(AnyConnectionStringWillDo))
                .Callback(() => connected = true);

            _connection
                .Setup(m => m.Initialize(AnyTableNameWillDo))
                .Callback(() => initialized = true);

            var sut = new BulkCopy();

            sut.Connect(AnyConnectionStringWillDo);
            sut.Initialize(AnyTableNameWillDo);

            return sut;
        }

        #region column binding

        [TestMethod]
        public void BindBigIntBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<long>>().Object;
            
            _driver
                .Setup(m => m.CreateBigInt(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindBigInt());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateBigInt(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindBinaryBehavesAsExpected()
        {
            var column1 = new Mock<IStandardColumn<byte[]>>().Object;
            var column2 = new Mock<IStandardColumn<byte[]>>().Object;

            const int anyLengthWillDo = 100;

            _driver
                .Setup(m => m.CreateBinary(0, anyLengthWillDo, false))
                .Returns(column1);

            _driver
                .Setup(m => m.CreateBinary(1, anyLengthWillDo, true))
                .Returns(column2);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column1, sut.BindBinary(anyLengthWillDo));
                Assert.AreSame(column2, sut.BindBinary(anyLengthWillDo, true));
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(4));
            _driver.Verify(m => m.CreateBinary(0, anyLengthWillDo, false), Times.Once);
            _driver.Verify(m => m.CreateBinary(1, anyLengthWillDo, true), Times.Once);
            _connection.Verify(m => m.Bind(column1), Times.Once);
            _connection.Verify(m => m.Bind(column2), Times.Once);
        }

        [TestMethod]
        public void BindBitBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<bool>>().Object;

            _driver
                .Setup(m => m.CreateBit(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindBit());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateBit(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindCharBehavesAsExpected()
        {
            var column1 = new Mock<IStandardColumn<string>>().Object;
            var column2 = new Mock<IStandardColumn<string>>().Object;

            const int anyLengthWillDo = 100;

            _driver
                .Setup(m => m.CreateChar(0, anyLengthWillDo, false))
                .Returns(column1);

            _driver
                .Setup(m => m.CreateChar(1, anyLengthWillDo, true))
                .Returns(column2);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column1, sut.BindChar(anyLengthWillDo));
                Assert.AreSame(column2, sut.BindChar(anyLengthWillDo, true));
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(4));
            _driver.Verify(m => m.CreateChar(0, anyLengthWillDo, false), Times.Once);
            _driver.Verify(m => m.CreateChar(1, anyLengthWillDo, true), Times.Once);
            _connection.Verify(m => m.Bind(column1), Times.Once);
            _connection.Verify(m => m.Bind(column2), Times.Once);
        }

        [TestMethod]
        public void BindDateBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<DateTime>>().Object;

            _driver
                .Setup(m => m.CreateDate(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindDate());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateDate(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindDateTimeBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<DateTime>>().Object;

            _driver
                .Setup(m => m.CreateDateTime(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindDateTime());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateDateTime(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindDateTime2BehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<DateTime>>().Object;

            _driver
                .Setup(m => m.CreateDateTime2(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindDateTime2());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateDateTime2(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindDateTimeOffsetOffsetBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<DateTimeOffset>>().Object;

            _driver
                .Setup(m => m.CreateDateTimeOffset(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindDateTimeOffset());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateDateTimeOffset(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindDecimalBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<decimal>>().Object;

            _driver
                .Setup(m => m.CreateDecimal(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindDecimal());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateDecimal(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindFloatBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<Double>>().Object;

            _driver
                .Setup(m => m.CreateFloat(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindFloat());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateFloat(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindIntBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<int>>().Object;

            _driver
                .Setup(m => m.CreateInt(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindInt());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateInt(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindMoneyBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<decimal>>().Object;

            _driver
                .Setup(m => m.CreateMoney(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindMoney());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateMoney(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNCharBehavesAsExpected()
        {
            var column1 = new Mock<IStandardColumn<string>>().Object;
            var column2 = new Mock<IStandardColumn<string>>().Object;

            const int anyLengthWillDo = 100;

            _driver
                .Setup(m => m.CreateNChar(0, anyLengthWillDo, false))
                .Returns(column1);

            _driver
                .Setup(m => m.CreateNChar(1, anyLengthWillDo, true))
                .Returns(column2);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column1, sut.BindNChar(anyLengthWillDo));
                Assert.AreSame(column2, sut.BindNChar(anyLengthWillDo, true));
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(4));
            _driver.Verify(m => m.CreateNChar(0, anyLengthWillDo, false), Times.Once);
            _driver.Verify(m => m.CreateNChar(1, anyLengthWillDo, true), Times.Once);
            _connection.Verify(m => m.Bind(column1), Times.Once);
            _connection.Verify(m => m.Bind(column2), Times.Once);
        }

        [TestMethod]
        public void BindNVarCharBehavesAsExpected()
        {
            var column1 = new Mock<IStandardColumn<string>>().Object;
            var column2 = new Mock<IStandardColumn<string>>().Object;

            const int anyLengthWillDo = 100;

            _driver
                .Setup(m => m.CreateNVarChar(0, anyLengthWillDo, false))
                .Returns(column1);

            _driver
                .Setup(m => m.CreateNVarChar(1, anyLengthWillDo, true))
                .Returns(column2);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column1, sut.BindNVarChar(anyLengthWillDo));
                Assert.AreSame(column2, sut.BindNVarChar(anyLengthWillDo, true));
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(4));
            _driver.Verify(m => m.CreateNVarChar(0, anyLengthWillDo, false), Times.Once);
            _driver.Verify(m => m.CreateNVarChar(1, anyLengthWillDo, true), Times.Once);
            _connection.Verify(m => m.Bind(column1), Times.Once);
            _connection.Verify(m => m.Bind(column2), Times.Once);
        }

        [TestMethod]
        public void BindNVarCharMaxBehavesAsExpected()
        {
            var column1 = new Mock<IMaxColumn<string>>().Object;
            var column2 = new Mock<IMaxColumn<string>>().Object;

            _driver
                .Setup(m => m.CreateNVarCharMax(0, false))
                .Returns(column1);

            _driver
                .Setup(m => m.CreateNVarCharMax(1, true))
                .Returns(column2);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column1, sut.BindNVarCharMax());
                Assert.AreSame(column2, sut.BindNVarCharMax(true));
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(4));
            _driver.Verify(m => m.CreateNVarCharMax(0, false), Times.Once);
            _driver.Verify(m => m.CreateNVarCharMax(1, true), Times.Once);
            _connection.Verify(m => m.Bind(column1), Times.Once);
            _connection.Verify(m => m.Bind(column2), Times.Once);
        }

        [TestMethod]
        public void BindRealBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<Single>>().Object;

            _driver
                .Setup(m => m.CreateReal(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindReal());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateReal(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindSmallDateTimeBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<DateTime>>().Object;

            _driver
                .Setup(m => m.CreateSmallDateTime(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindSmallDateTime());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateSmallDateTime(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindSmallIntBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<short>>().Object;

            _driver
                .Setup(m => m.CreateSmallInt(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindSmallInt());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateSmallInt(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindSmallMoneyBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<decimal>>().Object;

            _driver
                .Setup(m => m.CreateSmallMoney(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindSmallMoney());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateSmallMoney(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindTimeBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<TimeSpan>>().Object;

            _driver
                .Setup(m => m.CreateTime(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindTime());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateTime(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindTinyIntBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<byte>>().Object;

            _driver
                .Setup(m => m.CreateTinyInt(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindTinyInt());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateTinyInt(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindUniqueIdentifierBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<Guid>>().Object;

            _driver
                .Setup(m => m.CreateUniqueIdentifier(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindUniqueIdentifier());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateUniqueIdentifier(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableBigIntBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<long?>>().Object;

            _driver
                .Setup(m => m.CreateNullableBigInt(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableBigInt());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableBigInt(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableBitBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<bool?>>().Object;

            _driver
                .Setup(m => m.CreateNullableBit(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableBit());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableBit(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableDateBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<DateTime?>>().Object;

            _driver
                .Setup(m => m.CreateNullableDate(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableDate());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableDate(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableDateTimeBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<DateTime?>>().Object;

            _driver
                .Setup(m => m.CreateNullableDateTime(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableDateTime());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableDateTime(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableDateTime2BehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<DateTime?>>().Object;

            _driver
                .Setup(m => m.CreateNullableDateTime2(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableDateTime2());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableDateTime2(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableDateTimeOffsetOffsetBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<DateTimeOffset?>>().Object;

            _driver
                .Setup(m => m.CreateNullableDateTimeOffset(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableDateTimeOffset());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableDateTimeOffset(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableDecimalBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<decimal?>>().Object;

            _driver
                .Setup(m => m.CreateNullableDecimal(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableDecimal());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableDecimal(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableFloatBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<Double?>>().Object;

            _driver
                .Setup(m => m.CreateNullableFloat(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableFloat());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableFloat(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableIntBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<int?>>().Object;

            _driver
                .Setup(m => m.CreateNullableInt(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableInt());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableInt(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableMoneyBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<decimal?>>().Object;

            _driver
                .Setup(m => m.CreateNullableMoney(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableMoney());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableMoney(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableRealBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<Single?>>().Object;

            _driver
                .Setup(m => m.CreateNullableReal(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableReal());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableReal(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableSmallDateTimeBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<DateTime?>>().Object;

            _driver
                .Setup(m => m.CreateNullableSmallDateTime(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableSmallDateTime());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableSmallDateTime(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableSmallIntBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<short?>>().Object;

            _driver
                .Setup(m => m.CreateNullableSmallInt(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableSmallInt());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableSmallInt(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableSmallMoneyBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<decimal?>>().Object;

            _driver
                .Setup(m => m.CreateNullableSmallMoney(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableSmallMoney());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableSmallMoney(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableTimeBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<TimeSpan?>>().Object;

            _driver
                .Setup(m => m.CreateNullableTime(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableTime());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableTime(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableTinyIntBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<byte?>>().Object;

            _driver
                .Setup(m => m.CreateNullableTinyInt(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableTinyInt());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableTinyInt(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindNullableUniqueIdentifierBehavesAsExpected()
        {
            var column = new Mock<IStandardColumn<Guid?>>().Object;

            _driver
                .Setup(m => m.CreateNullableUniqueIdentifier(0))
                .Returns(column);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column, sut.BindNullableUniqueIdentifier());
                Assert.AreSame(column, sut.Columns.Single());
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _driver.Verify(m => m.CreateNullableUniqueIdentifier(0), Times.Once);
            _connection.Verify(m => m.Bind(column), Times.Once);
        }

        [TestMethod]
        public void BindVarBinaryBehavesAsExpected()
        {
            var column1 = new Mock<IStandardColumn<byte[]>>().Object;
            var column2 = new Mock<IStandardColumn<byte[]>>().Object;

            const int anyLengthWillDo = 100;

            _driver
                .Setup(m => m.CreateVarBinary(0, anyLengthWillDo, false))
                .Returns(column1);

            _driver
                .Setup(m => m.CreateVarBinary(1, anyLengthWillDo, true))
                .Returns(column2);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column1, sut.BindVarBinary(anyLengthWillDo));
                Assert.AreSame(column2, sut.BindVarBinary(anyLengthWillDo, true));
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(4));
            _driver.Verify(m => m.CreateVarBinary(0, anyLengthWillDo, false), Times.Once);
            _driver.Verify(m => m.CreateVarBinary(1, anyLengthWillDo, true), Times.Once);
            _connection.Verify(m => m.Bind(column1), Times.Once);
            _connection.Verify(m => m.Bind(column2), Times.Once);
        }

        [TestMethod]
        public void BindVarBinaryMaxBehavesAsExpected()
        {
            var column1 = new Mock<IMaxColumn<byte[]>>().Object;
            var column2 = new Mock<IMaxColumn<byte[]>>().Object;

            _driver
                .Setup(m => m.CreateVarBinaryMax(0, false))
                .Returns(column1);

            _driver
                .Setup(m => m.CreateVarBinaryMax(1, true))
                .Returns(column2);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column1, sut.BindVarBinaryMax());
                Assert.AreSame(column2, sut.BindVarBinaryMax(true));
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(4));
            _driver.Verify(m => m.CreateVarBinaryMax(0, false), Times.Once);
            _driver.Verify(m => m.CreateVarBinaryMax(1, true), Times.Once);
            _connection.Verify(m => m.Bind(column1), Times.Once);
            _connection.Verify(m => m.Bind(column2), Times.Once);
        }

        [TestMethod]
        public void BindVarCharBehavesAsExpected()
        {
            var column1 = new Mock<IStandardColumn<string>>().Object;
            var column2 = new Mock<IStandardColumn<string>>().Object;

            const int anyLengthWillDo = 100;

            _driver
                .Setup(m => m.CreateVarChar(0, anyLengthWillDo, false))
                .Returns(column1);

            _driver
                .Setup(m => m.CreateVarChar(1, anyLengthWillDo, true))
                .Returns(column2);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column1, sut.BindVarChar(anyLengthWillDo));
                Assert.AreSame(column2, sut.BindVarChar(anyLengthWillDo, true));
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(4));
            _driver.Verify(m => m.CreateVarChar(0, anyLengthWillDo, false), Times.Once);
            _driver.Verify(m => m.CreateVarChar(1, anyLengthWillDo, true), Times.Once);
            _connection.Verify(m => m.Bind(column1), Times.Once);
            _connection.Verify(m => m.Bind(column2), Times.Once);
        }

        [TestMethod]
        public void BindVarCharMaxBehavesAsExpected()
        {
            var column1 = new Mock<IMaxColumn<string>>().Object;
            var column2 = new Mock<IMaxColumn<string>>().Object;

            _driver
                .Setup(m => m.CreateVarCharMax(0, false))
                .Returns(column1);

            _driver
                .Setup(m => m.CreateVarCharMax(1, true))
                .Returns(column2);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreSame(column1, sut.BindVarCharMax());
                Assert.AreSame(column2, sut.BindVarCharMax(true));
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(4));
            _driver.Verify(m => m.CreateVarCharMax(0, false), Times.Once);
            _driver.Verify(m => m.CreateVarCharMax(1, true), Times.Once);
            _connection.Verify(m => m.Bind(column1), Times.Once);
            _connection.Verify(m => m.Bind(column2), Times.Once);
        }

        [TestMethod]
        public void SkipColumnBehavesAsExpected()
        {
            const BulkCopyDataType anyBulkCopyDataTypeWillDo = BulkCopyDataType.Bit;

            IBulkCopyColumn column;

            using (var sut = GetInitializedSubjectUnderTest())
            {
                column = sut.SkipColumn(anyBulkCopyDataTypeWillDo);
                Assert.AreSame(column, sut.Columns.Single());
            }

            Assert.AreEqual(anyBulkCopyDataTypeWillDo, column.DataType);
            Assert.AreEqual(0, column.Index);
            Assert.IsFalse(column.IsBound);

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
        }

        #endregion

        #region properties

        [TestMethod]
        public void ConnectionTimeoutPropertyGetBehavesAsExpected()
        {
            const int anyConnectionTimeoutWillDo = 999;

            _connection
                .Setup(c => c.GetIntAttribute(OdbcConnectionAttribute.ConnectionTimeout))
                .Returns(anyConnectionTimeoutWillDo);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreEqual(anyConnectionTimeoutWillDo, sut.ConnectionTimeout);
            }

            _connection
                .Verify(c => c.GetIntAttribute(OdbcConnectionAttribute.ConnectionTimeout), Times.Once);
        }

        [TestMethod]
        public void ConnectionTimeoutSetPropertyGetBehavesAsExpected()
        {
            const int anyConnectionTimeoutWillDo = 999;

            using (var sut = GetInitializedSubjectUnderTest())
            {
                sut.ConnectionTimeout = anyConnectionTimeoutWillDo;
            }

            _connection
                .Verify(c => c.SetIntAttribute(OdbcConnectionAttribute.ConnectionTimeout, anyConnectionTimeoutWillDo), Times.Once);
        }

        [TestMethod]
        public void LoginTimeoutPropertyGetBehavesAsExpected()
        {
            const int anyLoginTimeoutWillDo = 999;

            _connection
                .Setup(c => c.GetIntAttribute(OdbcConnectionAttribute.LoginTimeout))
                .Returns(anyLoginTimeoutWillDo);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreEqual(anyLoginTimeoutWillDo, sut.LoginTimeout);
            }

            _connection
                .Verify(c => c.GetIntAttribute(OdbcConnectionAttribute.LoginTimeout), Times.Once);
        }

        [TestMethod]
        public void LoginTimeoutSetPropertyGetBehavesAsExpected()
        {
            const int anyLoginTimeoutWillDo = 999;

            using (var sut = GetInitializedSubjectUnderTest())
            {
                sut.LoginTimeout = anyLoginTimeoutWillDo;
            }

            _connection
                .Verify(c => c.SetIntAttribute(OdbcConnectionAttribute.LoginTimeout, anyLoginTimeoutWillDo), Times.Once);
        }

        [TestMethod]
        public void PacketSizePropertyGetBehavesAsExpected()
        {
            const int anyPacketSizeWillDo = 999;

            _connection
                .Setup(c => c.GetIntAttribute(OdbcConnectionAttribute.PacketSize))
                .Returns(anyPacketSizeWillDo);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                Assert.AreEqual(anyPacketSizeWillDo, sut.PacketSize);
            }

            _connection
                .Verify(c => c.GetIntAttribute(OdbcConnectionAttribute.PacketSize), Times.Once);
        }

        [TestMethod]
        public void PacketSizeSetPropertyGetBehavesAsExpected()
        {
            const int anyPacketSizeWillDo = 999;

            using (var sut = GetInitializedSubjectUnderTest())
            {
                sut.PacketSize = anyPacketSizeWillDo;
            }

            _connection
                .Verify(c => c.SetIntAttribute(OdbcConnectionAttribute.PacketSize, anyPacketSizeWillDo), Times.Once);
        }

        #endregion

        #region IBulkCopy

        [TestMethod]
        public void BatchBehavesAsExpected()
        {
            const int expected = 1000;
            int actual;

            _connection
                .Setup(m => m.Batch())
                .Returns(expected);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                actual = sut.Batch();
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _connection.VerifyGet(m => m.Initialized, Times.Exactly(2));
            _connection.Verify(m => m.Batch(), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DoneBehavesAsExpected()
        {
            const int expected = 1000;
            int actual;

            _connection
                .Setup(m => m.Done())
                .Returns(expected);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                actual = sut.Done();
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _connection.VerifyGet(m => m.Initialized, Times.Exactly(2));
            _connection.Verify(m => m.Done(), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SendRowBehavesAsExpected()
        {
            const bool expected = true;
            bool actual;

            _connection
                .Setup(m => m.SendRow())
                .Returns(expected);

            using (var sut = GetInitializedSubjectUnderTest())
            {
                actual = sut.SendRow();
            }

            _connection.VerifyGet(m => m.Connected, Times.Exactly(3));
            _connection.VerifyGet(m => m.Initialized, Times.Exactly(2));
            _connection.Verify(m => m.SendRow(), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
