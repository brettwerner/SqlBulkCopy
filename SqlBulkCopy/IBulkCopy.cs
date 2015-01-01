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

namespace SqlBulkCopy
{
    /// <summary>
    /// Defines the functionality provided by <see cref="BulkCopy"/>.
    /// <para>It may be used to substitute the <see cref="BulkCopy"/> class for unit testing.</para>
    /// </summary>
    public interface IBulkCopy : IDisposable
    {
        #region initialization

        /// <summary>
        /// Connects to the database using an ODBC connection string.
        /// </summary>
        /// <param name="connectionString">The ODBC connection string.</param>
        void Connect(string connectionString);

        /// <summary>
        /// This method executes a SQL statement. Can be used for pre- and post- bulk copy execution.
        /// </summary>
        /// <param name="commandText">The SQL statement to execute.</param>
        void ExecuteDirect(string commandText);

        /// <summary>
        /// Initializes the bulk copy operation.
        /// </summary>
        /// <param name="tableName">The name of the destination table.</param>
        void Initialize(string tableName);

        #endregion

        #region connection properties

        /// <summary>
        /// The number of seconds to wait for any request on the connection to complete before returning to the application. When zero (the default), there is no timeout.
        /// </summary>
        int ConnectionTimeout { get; set; }

        /// <summary>
        /// Returns the Driver connection string option for the current instance. For example: Driver={SQL Server Native Client 11.0}.
        /// </summary>
        string DriverOptionString { get; }

        /// <summary>
        /// The number of seconds to wait for a login request to complete before returning to the application. The default is driver dependent. When zero, there is no timeout and a connection attempt will wait indefinitely.
        /// </summary>
        int LoginTimeout { get; set; }

        /// <summary>
        /// The network packet size in bytes. Note: many data sources either do not support this option or are read-only.
        /// </summary>
        int PacketSize { get; set; }

        #endregion

        #region column binding

        /// <summary>
        /// Enumerates the columns used by the instance. Columns are added using one of the Bindxxx methods, or the <see cref="SkipColumn"/> method,
        /// and cannot be changed.
        /// </summary>
        IEnumerable<IBulkCopyColumn> Columns { get; }

        /// <summary>
        /// Binds a non-nullable bigint column. The data type used by the application is <see cref="BulkCopyDataType.BigInt"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableBigInt"/>
        ITypedBulkCopyBoundColumn<long> BindBigInt();

        /// <summary>
        /// Binds a binary column. The data type used by the application is <see cref="BulkCopyDataType.Binary"/> or <see cref="BulkCopyDataType.NullableBinary"/>.
        /// </summary>
        /// <param name="fieldLength">The field length of the column as defined in SQL Server.</param>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        ITypedBulkCopyBoundColumn<byte[]> BindBinary(short fieldLength, bool nullable = false);

        /// <summary>
        /// Binds a non-nullable bit column. The data type used by the application is <see cref="BulkCopyDataType.Bit"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableBit"/>
        ITypedBulkCopyBoundColumn<bool> BindBit();

        /// <summary>
        /// Binds a char column. The data type used by the application is <see cref="BulkCopyDataType.Char"/> or <see cref="BulkCopyDataType.NullableChar"/>.
        /// </summary>
        /// <param name="fieldLength">The field length of the column as defined in SQL Server.</param>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        ITypedBulkCopyBoundColumn<string> BindChar(short fieldLength, bool nullable = false);

        /// <summary>
        /// Binds a non-nullable date column. The data type used by the application is <see cref="BulkCopyDataType.Date"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableDate"/>
        ITypedBulkCopyBoundColumn<DateTime> BindDate();

        /// <summary>
        /// Binds a non-nullable datetime column. The data type used by the application is <see cref="BulkCopyDataType.DateTime"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableDateTime"/>
        ITypedBulkCopyBoundColumn<DateTime> BindDateTime();

        /// <summary>
        /// Binds a non-nullable datetime2 column. The data type used by the application is <see cref="BulkCopyDataType.DateTime2"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableDateTime2"/>
        ITypedBulkCopyBoundColumn<DateTime> BindDateTime2();

        /// <summary>
        /// Binds a non-nullable datetimeoffset column. The data type used by the application is <see cref="BulkCopyDataType.DateTimeOffset"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableDateTimeOffset"/>
        ITypedBulkCopyBoundColumn<DateTimeOffset> BindDateTimeOffset();

        /// <summary>
        /// Binds a non-nullable decimal column. The data type used by the application is <see cref="BulkCopyDataType.Decimal"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableDecimal"/>
        ITypedBulkCopyBoundColumn<decimal> BindDecimal();

        /// <summary>
        /// Binds a non-nullable float column. The data type used by the application is <see cref="BulkCopyDataType.Float"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableFloat"/>
        ITypedBulkCopyBoundColumn<Double> BindFloat();

        /// <summary>
        /// Binds a non-nullable int column. The data type used by the application is <see cref="BulkCopyDataType.Int"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableInt"/>
        ITypedBulkCopyBoundColumn<int> BindInt();

        /// <summary>
        /// Binds a non-nullable money column. The data type used by the application is <see cref="BulkCopyDataType.Money"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableMoney"/>
        /// <seealso cref="BindSmallMoney"/>
        /// <seealso cref="BindNullableSmallMoney"/>
        ITypedBulkCopyBoundColumn<decimal> BindMoney();

        /// <summary>
        /// Binds a nchar column. The data type used by the application is <see cref="BulkCopyDataType.NChar"/> or <see cref="BulkCopyDataType.NullableNChar"/>.
        /// </summary>
        /// <param name="fieldLength">The field length of the column as defined in SQL Server.</param>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        ITypedBulkCopyBoundColumn<string> BindNChar(short fieldLength, bool nullable = false);

        /// <summary>
        /// Binds a nvarchar column. The data type used by the application is <see cref="BulkCopyDataType.NVarChar"/> or <see cref="BulkCopyDataType.NullableNVarChar"/>.
        /// </summary>
        /// <param name="fieldLength">The field length of the column as defined in SQL Server.</param>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        ITypedBulkCopyBoundColumn<string> BindNVarChar(short fieldLength, bool nullable = false);

        /// <summary>
        /// Binds a nvarchar(max) column. The data type used by the application is <see cref="BulkCopyDataType.NVarCharMax"/> or <see cref="BulkCopyDataType.NullableNVarCharMax"/>.
        /// <para>Use for the SQL Server <c>ntext</c> data type also.</para>
        /// </summary>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        ITypedBulkCopyBoundColumn<string> BindNVarCharMax(bool nullable = false);

        /// <summary>
        /// Binds a non-nullable real column. The data type used by the application is <see cref="BulkCopyDataType.Real"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableReal"/>
        ITypedBulkCopyBoundColumn<float> BindReal();

        /// <summary>
        /// Binds a non-nullable smalldatetime column. The data type used by the application is <see cref="BulkCopyDataType.SmallDateTime"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableSmallDateTime"/>
        ITypedBulkCopyBoundColumn<DateTime> BindSmallDateTime();

        /// <summary>
        /// Binds a non-nullable smallint column. The data type used by the application is <see cref="BulkCopyDataType.SmallInt"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableSmallInt"/>
        ITypedBulkCopyBoundColumn<short> BindSmallInt();

        /// <summary>
        /// Binds a non-nullable smallmoney column. The data type used by the application is <see cref="BulkCopyDataType.SmallMoney"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableSmallMoney"/>
        /// <seealso cref="BindMoney"/>
        /// <seealso cref="BindNullableMoney"/>
        ITypedBulkCopyBoundColumn<decimal> BindSmallMoney();

        /// <summary>
        /// Binds a non-nullable time column. The data type used by the application is <see cref="BulkCopyDataType.Time"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableTime"/>
        ITypedBulkCopyBoundColumn<TimeSpan> BindTime();

        /// <summary>
        /// Binds a non-nullable tinyint column. The data type used by the application is <see cref="BulkCopyDataType.TinyInt"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableTinyInt"/>
        ITypedBulkCopyBoundColumn<byte> BindTinyInt();

        /// <summary>
        /// Binds a non-nullable uniqueidentifier column. The data type used by the application is <see cref="BulkCopyDataType.UniqueIdentifier"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindNullableUniqueIdentifier"/>
        ITypedBulkCopyBoundColumn<Guid> BindUniqueIdentifier();

        /// <summary>
        /// Binds a nullable bigint column. The data type used by the application is <see cref="BulkCopyDataType.NullableBigInt"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindBigInt"/>
        ITypedBulkCopyBoundColumn<long?> BindNullableBigInt();

        /// <summary>
        /// Binds a nullable bit column. The data type used by the application is <see cref="BulkCopyDataType.NullableBit"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindBit"/>
        ITypedBulkCopyBoundColumn<bool?> BindNullableBit();

        /// <summary>
        /// Binds a nullable date column. The data type used by the application is <see cref="BulkCopyDataType.NullableDate"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindDate"/>
        ITypedBulkCopyBoundColumn<DateTime?> BindNullableDate();

        /// <summary>
        /// Binds a nullable datetime column. The data type used by the application is <see cref="BulkCopyDataType.NullableDateTime"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindDateTime"/>
        ITypedBulkCopyBoundColumn<DateTime?> BindNullableDateTime();

        /// <summary>
        /// Binds a nullable datetime2 column. The data type used by the application is <see cref="BulkCopyDataType.NullableDateTime2"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindDateTime2"/>
        ITypedBulkCopyBoundColumn<DateTime?> BindNullableDateTime2();

        /// <summary>
        /// Binds a nullable datetimeoffset column. The data type used by the application is <see cref="BulkCopyDataType.NullableDateTimeOffset"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindDateTimeOffset"/>
        ITypedBulkCopyBoundColumn<DateTimeOffset?> BindNullableDateTimeOffset();

        /// <summary>
        /// Binds a nullable decimal column. The data type used by the application is <see cref="BulkCopyDataType.NullableDecimal"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindDecimal"/>
        ITypedBulkCopyBoundColumn<decimal?> BindNullableDecimal();

        /// <summary>
        /// Binds a nullable float column. The data type used by the application is <see cref="BulkCopyDataType.NullableFloat"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindFloat"/>
        ITypedBulkCopyBoundColumn<Double?> BindNullableFloat();

        /// <summary>
        /// Binds a nullable int column. The data type used by the application is <see cref="BulkCopyDataType.NullableInt"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindInt"/>
        ITypedBulkCopyBoundColumn<int?> BindNullableInt();

        /// <summary>
        /// Binds a nullable money column. The data type used by the application is <see cref="BulkCopyDataType.NullableMoney"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindMoney"/>
        /// <seealso cref="BindSmallMoney"/>
        /// <seealso cref="BindNullableSmallMoney"/>
        ITypedBulkCopyBoundColumn<decimal?> BindNullableMoney();

        /// <summary>
        /// Binds a nullable real column. The data type used by the application is <see cref="BulkCopyDataType.NullableReal"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindReal"/>
        ITypedBulkCopyBoundColumn<float?> BindNullableReal();

        /// <summary>
        /// Binds a nullable smalldatetime column. The data type used by the application is <see cref="BulkCopyDataType.NullableSmallDateTime"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindSmallDateTime"/>
        ITypedBulkCopyBoundColumn<DateTime?> BindNullableSmallDateTime();

        /// <summary>
        /// Binds a nullable smallint column. The data type used by the application is <see cref="BulkCopyDataType.NullableSmallInt"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindSmallInt"/>
        ITypedBulkCopyBoundColumn<short?> BindNullableSmallInt();

        /// <summary>
        /// Binds a nullable smallmoney column. The data type used by the application is <see cref="BulkCopyDataType.NullableSmallMoney"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindSmallMoney"/>
        /// <seealso cref="BindMoney"/>
        /// <seealso cref="BindNullableMoney"/>
        ITypedBulkCopyBoundColumn<decimal?> BindNullableSmallMoney();

        /// <summary>
        /// Binds a nullable time column. The data type used by the application is <see cref="BulkCopyDataType.NullableTime"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindTime"/>
        ITypedBulkCopyBoundColumn<TimeSpan?> BindNullableTime();

        /// <summary>
        /// Binds a nullable tinyint column. The data type used by the application is <see cref="BulkCopyDataType.NullableTinyInt"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindTinyInt"/>
        ITypedBulkCopyBoundColumn<byte?> BindNullableTinyInt();

        /// <summary>
        /// Binds a nullable uniqueidentifier column. The data type used by the application is <see cref="BulkCopyDataType.NullableUniqueIdentifier"/>.
        /// </summary>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        /// <seealso cref="BindUniqueIdentifier"/>
        ITypedBulkCopyBoundColumn<Guid?> BindNullableUniqueIdentifier();

        /// <summary>
        /// Binds a varbinary column. The data type used by the application is <see cref="BulkCopyDataType.VarBinary"/> or <see cref="BulkCopyDataType.NullableVarBinary"/>.
        /// </summary>
        /// <param name="fieldLength">The field length of the column as defined in SQL Server.</param>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        ITypedBulkCopyBoundColumn<byte[]> BindVarBinary(short fieldLength, bool nullable = false);

        /// <summary>
        /// Binds a varbinary(max) column. The data type used by the application is <see cref="BulkCopyDataType.VarBinaryMax"/> or <see cref="BulkCopyDataType.NullableVarBinaryMax"/>.
        /// <para>Use for the SQL Server <c>image</c> data type also.</para>
        /// </summary>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        ITypedBulkCopyBoundColumn<byte[]> BindVarBinaryMax(bool nullable = false);

        /// <summary>
        /// Binds a varchar column. The data type used by the application is <see cref="BulkCopyDataType.VarChar"/> or <see cref="BulkCopyDataType.NullableVarChar"/>.
        /// </summary>
        /// <param name="fieldLength">The field length of the column as defined in SQL Server.</param>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        ITypedBulkCopyBoundColumn<string> BindVarChar(short fieldLength, bool nullable = false);

        /// <summary>
        /// Binds a varchar(max) column. The data type used by the application is <see cref="BulkCopyDataType.VarCharMax"/> or <see cref="BulkCopyDataType.NullableVarCharMax"/>.
        /// <para>Use for the SQL Server <c>text</c> data type also.</para>
        /// </summary>
        /// <param name="nullable">Indicates whether the column supports nulls or not.</param>
        /// <returns>A strongly typed interface that can be used to update the program variable for the column.</returns>
        ITypedBulkCopyBoundColumn<string> BindVarCharMax(bool nullable = false);

        /// <summary>
        /// Skips a column that does not require data - a column that has an identity constraint or that allows nulls (in this case, all row values will be null).
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns>The unbound column.</returns>
        IBulkCopyColumn SkipColumn(BulkCopyDataType dataType);

        #endregion

        #region data transfer

        /// <summary>
        /// Commits all rows previously sent to SQL Server by <see cref="SendRow"/>.
        /// </summary>
        /// <returns>The number of rows sent to SQL Server by <see cref="SendRow"/> since the last call to <c>Batch</c>, or -1 in case of error.</returns>
        int Batch();

        /// <summary>
        /// Ends a bulk copy transfer.
        /// </summary>
        /// <returns>The number of rows sent to SQL Server by <see cref="SendRow"/> since the last call to <see cref="Batch"/>, or -1 in case of error.</returns>
        int Done();

        /// <summary>
        /// Sends a row of data from program variables to SQL Server.
        /// </summary>
        /// <returns><c>true</c> indicates success.</returns>
        bool SendRow();

        #endregion
    }
}
