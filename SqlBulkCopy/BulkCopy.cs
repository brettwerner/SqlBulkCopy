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
using System.Security.Permissions;
using SqlBulkCopy.Internal.BulkCopy;
using SqlBulkCopy.Internal.Common;
using SqlBulkCopy.Internal.Drivers;
using SqlBulkCopy.Internal.Odbc;

namespace SqlBulkCopy
{
    /// <summary>
    /// This class is a .NET wrapper for the ODBC Bulk Copy API.
    /// </summary>
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public sealed class BulkCopy : IBulkCopy
    {
        private short _columnIndex;
        private readonly List<IBulkCopyColumn> _columns;
        private readonly IOdbcConnection _connection;
        private static IOdbcDriver _customDriver; // used for testing only
        private bool _disposed;
        private static OdbcDriver _driver = OdbcDriver.SqlServerNativeClient11;
        private readonly IOdbcEnvironment _environment;
        private readonly bool _environmentOwner;
        private List<IMaxDataType> _maxColumns;

        #region ctor -> Finalize

        /// <summary>
        /// Constructor for the BulkCopy class.
        /// </summary>
        /// <para>To enable connection pooling, use the <see cref="BulkCopy(OdbcEnvironment)"/> constructor.</para>
        public BulkCopy()
        {
            _environment = CurrentDriver.CreateEnvironment(OdbcConnectionPooling.None);
            _environmentOwner = true;

            _connection = _environment.CreateConnection();
            _columns = new List<IBulkCopyColumn>();
        }

        /// <summary>
        /// Constructor for the BulkCopy class.
        /// </summary>
        /// <param name="environment">A shared environment for connection pooling. Instantiated using <see cref="CreateSharedEnvironment"/>.</param>
        public BulkCopy(OdbcEnvironment environment)
        {
            if (environment == null) throw new ArgumentNullException("environment");

            _environment = environment.InternalEnvironment;
            _connection = _environment.CreateConnection();
            _columns = new List<IBulkCopyColumn>();
        }

        /// <summary>
        /// Returns a shared ODBC environment for connection pooling.
        /// </summary>
        /// <param name="connectionPooling">Used to specify the connection pooling that is required. Defaults to <see cref="OdbcConnectionPooling.PerEnvironment"/>.</param>
        public static OdbcEnvironment CreateSharedEnvironment(OdbcConnectionPooling connectionPooling = OdbcConnectionPooling.PerEnvironment)
        {
            var environment = CurrentDriver.CreateEnvironment(connectionPooling);

            return new OdbcEnvironment(environment);
        }

        internal static IOdbcDriver CurrentDriver
        {
            get
            {
                if (_customDriver != null)
                {
                    return _customDriver;
                }

                switch (_driver)
                {
                    case OdbcDriver.SqlServer:
                        return SqlServerDriver.SingleInstance;
                    case OdbcDriver.SqlServerNativeClient:
                        return SqlnclDriver.SingleInstance;
                    case OdbcDriver.SqlServerNativeClient10:
                        return Sqlncl10Driver.SingleInstance;
                    case OdbcDriver.SqlServerNativeClient11:
                        return Sqlncl11Driver.SingleInstance;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set { _customDriver = value; }
        }

        /// <summary>
        /// Indicates the ODBC driver that should be used. The default is SQL Server Native Client 11.0.
        /// </summary>
        public static OdbcDriver Driver
        {
            get { return _driver; }
            set { _driver = value; }
        }

        /// <summary>
        /// Disposes the class and releases ODBC handles.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;

            foreach (var disposable in _columns)
            {
                disposable.Dispose();
            }

            _connection.Disconnect();
            _connection.Dispose();

            if (_environmentOwner)
            {
                _environment.Dispose();
            }
        }

        #endregion

        #region initialization

        /// <summary>
        /// Connects to the database.
        /// </summary>
        /// <param name="connectionString">The ODBC connection string.</param>
        public void Connect(string connectionString)
        {
            CheckDisposed();
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("connectionString is required.");

            if (_connection.Connected)
            {
                throw new InvalidOperationException("Already connected.");
            }

            _connection.Connect(connectionString);
        }

        /// <summary>
        /// This method executes a SQL statement. Can be used for pre- and post- bulk copy execution.
        /// </summary>
        /// <param name="commandText">The SQL statement to execute.</param>
        public void ExecuteDirect(string commandText)
        {
            CheckDisposed();
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentException("commandText");
            CheckConnected();

            _connection.ExecuteDirect(commandText);
        }

        /// <summary>
        /// Initializes the bulk copy operation.
        /// </summary>
        /// <param name="tableName">The name of the destination table.</param>
        public void Initialize(string tableName)
        {
            CheckDisposed();
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentException("tableName is required.");
            CheckConnected();

            if (_connection.Initialized)
            {
                throw new InvalidOperationException("Already initialized.");
            }

            _connection.Initialize(tableName);
        }

        #endregion

        #region connection properties

        /// <summary>
        /// The number of seconds to wait for any request on the connection to complete before returning to the application. When zero (the default), there is no timeout.
        /// </summary>
        public int ConnectionTimeout
        {
            get { return _connection.GetIntAttribute(OdbcConnectionAttribute.ConnectionTimeout); }
            set { _connection.SetIntAttribute(OdbcConnectionAttribute.ConnectionTimeout, value); }
        }

        /// <summary>
        /// Returns the Driver connection string option for the current instance. For example: Driver={SQL Server Native Client 11.0}.
        /// </summary>
        public string DriverOptionString
        {
            get { return _connection.Driver.DriverOptionString; }
        }

        /// <summary>
        /// The number of seconds to wait for a login request to complete before returning to the application. The default is driver dependent. When zero, there is no timeout and a connection attempt will wait indefinitely.
        /// </summary>
        public int LoginTimeout
        {
            get { return _connection.GetIntAttribute(OdbcConnectionAttribute.LoginTimeout); }
            set { _connection.SetIntAttribute(OdbcConnectionAttribute.LoginTimeout, value); }
        }

        /// <summary>
        /// The network packet size in bytes. Note: many data sources either do not support this option or are read-only.
        /// </summary>
        public int PacketSize
        {
            get { return _connection.GetIntAttribute(OdbcConnectionAttribute.PacketSize); }
            set { _connection.SetIntAttribute(OdbcConnectionAttribute.PacketSize, value); }
        }

        private const int SqlTracingOff = 0;  // SQL_OPT_TRACE_OFF: disable tracing.
        private const int SqlTracingOn = 1;   // SQL_OPT_TRACE_ON: enable tracing.

        /// <summary>
        /// Tells the Driver Manager whether to perform tracing. This is for all connections.
        /// </summary>
        /// <seealso cref="TraceFile"/>
        public static bool Trace
        {
            get { return OdbcMethods.GetIntConnectionAttribute(OdbcConnectionAttribute.Trace) == SqlTracingOn; }
            set
            {
                OdbcMethods.SetIntConnectionAttribute(OdbcConnectionAttribute.Trace, value ? SqlTracingOn : SqlTracingOff);
            }
        }

        /// <summary>
        /// The path and name of the file used when tracing. This is for all connections.
        /// </summary>
        /// <seealso cref="Trace"/>
        public static string TraceFile
        {
            get { return OdbcMethods.GetStringConnectionAttribute(OdbcConnectionAttribute.TraceFile); }
            set { OdbcMethods.SetStringConnectionAttribute(OdbcConnectionAttribute.TraceFile, value); }
        }

        #endregion

        #region column binding

        /// <summary>
        /// Enumerates the columns used by the instance. Columns are added using one of the Bindxxx methods, or the <see cref="SkipColumn"/> method,
        /// and cannot be changed.
        /// </summary>
        public IEnumerable<IBulkCopyColumn> Columns
        {
            get { return _columns; }
        }

        /// <summary>
        /// Binds a non-nullable bigint column. The data type used by the application is <see cref="BulkCopyDataType.BigInt"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableBigInt"/>
        public ITypedBulkCopyBoundColumn<long> BindBigInt()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateBigInt(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a binary column. The data type used by the application is <see cref="BulkCopyDataType.Binary"/> or <see cref="BulkCopyDataType.NullableBinary"/>.
        /// </summary>
        /// <param name="fieldLength">The field length of the column as defined in SQL Server.</param>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        public ITypedBulkCopyBoundColumn<byte[]> BindBinary(short fieldLength, bool nullable = false)
        {
            CheckDisposed();

            if (fieldLength < 1 || fieldLength > Constants.SqlMaximumByteLength)
            {
                throw new ArgumentOutOfRangeException("fieldLength", fieldLength, "The argument must be between 1 and 8000.");
            }

            var column = _connection.Driver.CreateBinary(NextColumnIndex, fieldLength, nullable);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a non-nullable bit column. The data type used by the application is <see cref="BulkCopyDataType.Bit"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableBit"/>
        public ITypedBulkCopyBoundColumn<bool> BindBit()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateBit(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a char column. The data type used by the application is <see cref="BulkCopyDataType.Char"/> or <see cref="BulkCopyDataType.NullableChar"/>.
        /// </summary>
        /// <param name="fieldLength">The field length of the column as defined in SQL Server.</param>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        public ITypedBulkCopyBoundColumn<string> BindChar(short fieldLength, bool nullable = false)
        {
            CheckDisposed();

            if (fieldLength < 1 || fieldLength > Constants.SqlMaximumByteLength)
            {
                throw new ArgumentOutOfRangeException("fieldLength", fieldLength, "The argument must be between 1 and 8000.");
            }

            var column = _connection.Driver.CreateChar(NextColumnIndex, fieldLength, nullable);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a non-nullable date column. The data type used by the application is <see cref="BulkCopyDataType.Date"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableDate"/>
        public ITypedBulkCopyBoundColumn<DateTime> BindDate()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateDate(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a non-nullable datetime column. The data type used by the application is <see cref="BulkCopyDataType.DateTime"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableDateTime"/>
        public ITypedBulkCopyBoundColumn<DateTime> BindDateTime()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateDateTime(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a non-nullable datetime2 column. The data type used by the application is <see cref="BulkCopyDataType.DateTime2"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableDateTime2"/>
        public ITypedBulkCopyBoundColumn<DateTime> BindDateTime2()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateDateTime2(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a non-nullable datetimeoffset column. The data type used by the application is <see cref="BulkCopyDataType.DateTimeOffset"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableDateTimeOffset"/>
        public ITypedBulkCopyBoundColumn<DateTimeOffset> BindDateTimeOffset()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateDateTimeOffset(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a non-nullable decimal column. The data type used by the application is <see cref="BulkCopyDataType.Decimal"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableDecimal"/>
        public ITypedBulkCopyBoundColumn<decimal> BindDecimal()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateDecimal(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a non-nullable float column. The data type used by the application is <see cref="BulkCopyDataType.Float"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableFloat"/>
        public ITypedBulkCopyBoundColumn<Double> BindFloat()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateFloat(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a non-nullable int column. The data type used by the application is <see cref="BulkCopyDataType.Int"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableInt"/>
        public ITypedBulkCopyBoundColumn<int> BindInt()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateInt(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a non-nullable money column. The data type used by the application is <see cref="BulkCopyDataType.Money"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableMoney"/>
        /// <seealso cref="BindSmallMoney"/>
        /// <seealso cref="BindNullableSmallMoney"/>
        public ITypedBulkCopyBoundColumn<decimal> BindMoney()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateMoney(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nchar column. The data type used by the application is <see cref="BulkCopyDataType.NChar"/> or <see cref="BulkCopyDataType.NullableNChar"/>.
        /// </summary>
        /// <param name="fieldLength">The field length of the column as defined in SQL Server.</param>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        public ITypedBulkCopyBoundColumn<string> BindNChar(short fieldLength, bool nullable = false)
        {
            CheckDisposed();

            if (fieldLength < 1 || fieldLength > Constants.SqlMaximumDoubleByteCharacterSetByteLength)
            {
                throw new ArgumentOutOfRangeException("fieldLength", fieldLength, "The argument must be between 1 and 4000.");
            }

            var column = _connection.Driver.CreateNChar(NextColumnIndex, fieldLength, nullable);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nvarchar column. The data type used by the application is <see cref="BulkCopyDataType.NVarChar"/> or <see cref="BulkCopyDataType.NullableNVarChar"/>.
        /// </summary>
        /// <param name="fieldLength">The field length of the column as defined in SQL Server.</param>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        public ITypedBulkCopyBoundColumn<string> BindNVarChar(short fieldLength, bool nullable = false)
        {
            CheckDisposed();

            if (fieldLength < 1 || fieldLength > Constants.SqlMaximumDoubleByteCharacterSetByteLength)
            {
                throw new ArgumentOutOfRangeException("fieldLength", fieldLength, "The argument must be between 1 and 4000.");
            }

            var column = _connection.Driver.CreateNVarChar(NextColumnIndex, fieldLength, nullable);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nvarchar(max) column. The data type used by the application is <see cref="BulkCopyDataType.NVarCharMax"/> or <see cref="BulkCopyDataType.NullableNVarCharMax"/>.
        /// <para>Use for the SQL Server <c>ntext</c> data type also.</para>
        /// </summary>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        public ITypedBulkCopyBoundColumn<string> BindNVarCharMax(bool nullable = false)
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNVarCharMax(NextColumnIndex, nullable);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a non-nullable real column. The data type used by the application is <see cref="BulkCopyDataType.Real"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableReal"/>
        public ITypedBulkCopyBoundColumn<float> BindReal()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateReal(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a non-nullable smalldatetime column. The data type used by the application is <see cref="BulkCopyDataType.SmallDateTime"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableSmallDateTime"/>
        public ITypedBulkCopyBoundColumn<DateTime> BindSmallDateTime()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateSmallDateTime(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a non-nullable smallint column. The data type used by the application is <see cref="BulkCopyDataType.SmallInt"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableSmallInt"/>
        public ITypedBulkCopyBoundColumn<short> BindSmallInt()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateSmallInt(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a non-nullable smallmoney column. The data type used by the application is <see cref="BulkCopyDataType.SmallMoney"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableSmallMoney"/>
        /// <seealso cref="BindMoney"/>
        /// <seealso cref="BindNullableMoney"/>
        public ITypedBulkCopyBoundColumn<decimal> BindSmallMoney()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateSmallMoney(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a non-nullable time column. The data type used by the application is <see cref="BulkCopyDataType.Time"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableTime"/>
        public ITypedBulkCopyBoundColumn<TimeSpan> BindTime()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateTime(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a non-nullable tinyint column. The data type used by the application is <see cref="BulkCopyDataType.TinyInt"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableTinyInt"/>
        public ITypedBulkCopyBoundColumn<byte> BindTinyInt()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateTinyInt(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a non-nullable uniqueidentifier column. The data type used by the application is <see cref="BulkCopyDataType.UniqueIdentifier"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableUniqueIdentifier"/>
        public ITypedBulkCopyBoundColumn<Guid> BindUniqueIdentifier()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateUniqueIdentifier(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable bigint column. The data type used by the application is <see cref="BulkCopyDataType.NullableBigInt"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindBigInt"/>
        public ITypedBulkCopyBoundColumn<long?> BindNullableBigInt()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableBigInt(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable bit column. The data type used by the application is <see cref="BulkCopyDataType.NullableBit"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindBit"/>
        public ITypedBulkCopyBoundColumn<bool?> BindNullableBit()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableBit(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable date column. The data type used by the application is <see cref="BulkCopyDataType.NullableDate"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindDate"/>
        public ITypedBulkCopyBoundColumn<DateTime?> BindNullableDate()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableDate(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable datetime column. The data type used by the application is <see cref="BulkCopyDataType.NullableDateTime"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindDateTime"/>
        public ITypedBulkCopyBoundColumn<DateTime?> BindNullableDateTime()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableDateTime(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable datetime2 column. The data type used by the application is <see cref="BulkCopyDataType.NullableDateTime2"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindDateTime2"/>
        public ITypedBulkCopyBoundColumn<DateTime?> BindNullableDateTime2()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableDateTime2(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable datetimeoffset column. The data type used by the application is <see cref="BulkCopyDataType.NullableDateTimeOffset"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindDateTimeOffset"/>
        public ITypedBulkCopyBoundColumn<DateTimeOffset?> BindNullableDateTimeOffset()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableDateTimeOffset(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable decimal column. The data type used by the application is <see cref="BulkCopyDataType.NullableDecimal"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindDecimal"/>
        public ITypedBulkCopyBoundColumn<decimal?> BindNullableDecimal()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableDecimal(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable float column. The data type used by the application is <see cref="BulkCopyDataType.NullableFloat"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindFloat"/>
        public ITypedBulkCopyBoundColumn<Double?> BindNullableFloat()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableFloat(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable int column. The data type used by the application is <see cref="BulkCopyDataType.NullableInt"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindInt"/>
        public ITypedBulkCopyBoundColumn<int?> BindNullableInt()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableInt(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable money column. The data type used by the application is <see cref="BulkCopyDataType.NullableMoney"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindMoney"/>
        /// <seealso cref="BindSmallMoney"/>
        /// <seealso cref="BindNullableSmallMoney"/>
        public ITypedBulkCopyBoundColumn<decimal?> BindNullableMoney()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableMoney(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable real column. The data type used by the application is <see cref="BulkCopyDataType.NullableReal"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindReal"/>
        public ITypedBulkCopyBoundColumn<float?> BindNullableReal()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableReal(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable smalldatetime column. The data type used by the application is <see cref="BulkCopyDataType.NullableSmallDateTime"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindSmallDateTime"/>
        public ITypedBulkCopyBoundColumn<DateTime?> BindNullableSmallDateTime()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableSmallDateTime(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable smallint column. The data type used by the application is <see cref="BulkCopyDataType.NullableSmallInt"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindSmallInt"/>
        public ITypedBulkCopyBoundColumn<short?> BindNullableSmallInt()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableSmallInt(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable smallmoney column. The data type used by the application is <see cref="BulkCopyDataType.NullableSmallMoney"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindSmallMoney"/>
        /// <seealso cref="BindMoney"/>
        /// <seealso cref="BindNullableMoney"/>
        public ITypedBulkCopyBoundColumn<decimal?> BindNullableSmallMoney()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableSmallMoney(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable time column. The data type used by the application is <see cref="BulkCopyDataType.NullableTime"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindTime"/>
        public ITypedBulkCopyBoundColumn<TimeSpan?> BindNullableTime()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableTime(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable tinyint column. The data type used by the application is <see cref="BulkCopyDataType.NullableTinyInt"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindTinyInt"/>
        public ITypedBulkCopyBoundColumn<byte?> BindNullableTinyInt()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableTinyInt(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a nullable uniqueidentifier column. The data type used by the application is <see cref="BulkCopyDataType.NullableUniqueIdentifier"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindUniqueIdentifier"/>
        public ITypedBulkCopyBoundColumn<Guid?> BindNullableUniqueIdentifier()
        {
            CheckDisposed();

            var column = _connection.Driver.CreateNullableUniqueIdentifier(NextColumnIndex);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a varbinary column. The data type used by the application is <see cref="BulkCopyDataType.VarBinary"/> or <see cref="BulkCopyDataType.NullableVarBinary"/>.
        /// </summary>
        /// <param name="fieldLength">The field length of the column as defined in SQL Server.</param>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        public ITypedBulkCopyBoundColumn<byte[]> BindVarBinary(short fieldLength, bool nullable = false)
        {
            CheckDisposed();

            if (fieldLength < 1 || fieldLength > Constants.SqlMaximumByteLength)
            {
                throw new ArgumentOutOfRangeException("fieldLength", fieldLength, "The argument must be between 1 and 8000.");
            }

            var column = _connection.Driver.CreateVarBinary(NextColumnIndex, fieldLength, nullable);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a varbinary(max) column. The data type used by the application is <see cref="BulkCopyDataType.VarBinaryMax"/> or <see cref="BulkCopyDataType.NullableVarBinaryMax"/>.
        /// <para>Use for the SQL Server <c>image</c> data type also.</para>
        /// </summary>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        public ITypedBulkCopyBoundColumn<byte[]> BindVarBinaryMax(bool nullable = false)
        {
            CheckDisposed();

            var column = _connection.Driver.CreateVarBinaryMax(NextColumnIndex, nullable);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a varchar column. The data type used by the application is <see cref="BulkCopyDataType.VarChar"/> or <see cref="BulkCopyDataType.NullableVarChar"/>.
        /// </summary>
        /// <param name="fieldLength">The field length of the column as defined in SQL Server.</param>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        public ITypedBulkCopyBoundColumn<string> BindVarChar(short fieldLength, bool nullable = false)
        {
            CheckDisposed();

            if (fieldLength < 1 || fieldLength > Constants.SqlMaximumByteLength)
            {
                throw new ArgumentOutOfRangeException("fieldLength", fieldLength, "The argument must be between 1 and 8000.");
            }

            var column = _connection.Driver.CreateVarChar(NextColumnIndex, fieldLength, nullable);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Binds a varchar(max) column. The data type used by the application is <see cref="BulkCopyDataType.VarCharMax"/> or <see cref="BulkCopyDataType.NullableVarCharMax"/>.
        /// <para>Use for the SQL Server <c>text</c> data type also.</para>
        /// </summary>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        public ITypedBulkCopyBoundColumn<string> BindVarCharMax(bool nullable = false)
        {
            CheckDisposed();

            var column = _connection.Driver.CreateVarCharMax(NextColumnIndex, nullable);

            Bind(column);

            return column;
        }

        /// <summary>
        /// Skips a column that does not require data - a column that has an identity constraint or that allows nulls (in this case, all row values will be null).
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns>The unbound column.</returns>
        public IBulkCopyColumn SkipColumn(BulkCopyDataType dataType)
        {
            CheckDisposed();
            CheckConnected();
            CheckInitialized();

            var column = new UnboundColumn(NextColumnIndex, dataType);

            _columns.Add(column);

            return column;
        }

        #endregion

        #region data transfer

        /// <summary>
        /// Commits all rows previously sent to SQL Server by <see cref="SendRow"/>.
        /// </summary>
        /// <returns>The number of rows sent since the last call to <c>Batch</c>.</returns>
        public int Batch()
        {
            CheckDisposed();
            CheckConnected();
            CheckInitialized();

            return _connection.Batch();
        }

        /// <summary>
        /// Ends a bulk copy transfer.
        /// </summary>
        /// <returns>The number of rows sent to SQL Server by <see cref="SendRow"/> since the last call to <see cref="Batch"/>, or -1 in case of error.</returns>
        public int Done()
        {
            CheckDisposed();
            CheckConnected();
            CheckInitialized();

            return _connection.Done();
        }

        /// <summary>
        /// Sends a row of data from program variables to SQL Server. 
        /// </summary>
        /// <returns><c>true</c> if successful, or <c>false</c> if an error occurred.</returns>
        public bool SendRow()
        {
            CheckDisposed();
            CheckConnected();
            CheckInitialized();

            if (_maxColumns == null)
            {
                return _connection.SendRow();
            }

// ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var column in _maxColumns)
            {
                if (!_connection.SetColumnLength(column))
                {
                    return false;
                }
            }

            if (!_connection.SendRow())
            {
                return false;
            }

// ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var column in _maxColumns)
            {
                if (!_connection.SendMoreText(column))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region private procedures

        internal void Bind<T>(IStandardColumn<T> column)
        {
            if (column == null) throw new ArgumentNullException("column");
            CheckConnected();
            CheckInitialized();

            _connection.Bind(column);

            _columns.Add(column);
        }

        internal void Bind<T>(IMaxColumn<T> column)
        {
            if (column == null) throw new ArgumentNullException("column");
            CheckConnected();
            CheckInitialized();

            _connection.Bind(column);

            _columns.Add(column);

            if (_maxColumns == null)
            {
                _maxColumns = new List<IMaxDataType>();
            }

            _maxColumns.Add(column);
        }

        private void CheckConnected()
        {
            if (!_connection.Connected)
            {
                throw new InvalidOperationException("Not connected.");
            }
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(String.Format("{0} has been disposed.", GetType().Name));
            }
        }

        private void CheckInitialized()
        {
            if (!_connection.Initialized)
            {
                throw new InvalidOperationException("Not initialized for bulk copy.");
            }
        }

        private short NextColumnIndex
        {
            get { return _columnIndex++; }
        }

        #endregion
    }
}