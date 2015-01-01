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
using SqlBulkCopy.Internal.BulkCopy;
using SqlBulkCopy.Internal.Common;
using SqlBulkCopy.Internal.Odbc;

namespace SqlBulkCopy.Internal.Drivers
{
    internal unsafe abstract class OdbcDriver : IOdbcDriver
    {
        #region Setup

        public abstract IOdbcEnvironment CreateEnvironment(OdbcConnectionPooling connectionPooling);

        public abstract string DriverOptionString { get; }

        public virtual void Initialize(OdbcConnectionHandle connectionHandle, string tableName)
        {
            if (connectionHandle == null) throw new ArgumentNullException("connectionHandle");

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }

            const int databaseIn = 1;  // DB_IN: the direction of the copy - in.

            var result = bcp_initW(connectionHandle, tableName, databaseIn);

            if (!result)
            {
                throw new Exception("Unable to initialise BCP.");
            }
        }

        #endregion

        #region Column Binding

        public virtual void Bind<T>(OdbcConnectionHandle connectionHandle, IStandardColumn<T> column)
        {
            if (connectionHandle == null) throw new ArgumentNullException("connectionHandle");
            if (column == null) throw new ArgumentNullException("column");

            const int sqlVariableLengthData = -10;  // SQL_VARLEN_DATA: the indicator will be used to determine the length of data copied.
            const int sqlVarChar = 39;              // SQLVARCHAR: the data is being sent as varchar.

            var byteLength = column.IndicatorLength > 0 ? sqlVariableLengthData : column.ByteLength;

            // NB bcp_bind column index lower bound is 1.

            var eDataType = column.RequiresConversion ? sqlVarChar : 0;
            var result = bcp_bind(connectionHandle, column.Pointer, column.IndicatorLength, byteLength, (byte*)0, 0, eDataType, column.Index + 1);

            if (result) return;

            var ex = OdbcMethods.GetException(connectionHandle, string.Format("Unable to bind {0} column ({1}).", column.DataTypeDescription, column.Index));

            throw ex;
        }

        public virtual void Bind<T>(OdbcConnectionHandle connectionHandle, IMaxColumn<T> column)
        {
            if (connectionHandle == null) throw new ArgumentNullException("connectionHandle");
            if (column == null) throw new ArgumentNullException("column");

            // NB bcp_bind column index lower bound is 1.

            var result = bcp_bind(connectionHandle, (byte*)0, 0, 0, (byte*)0, 0, 0, column.Index + 1);

            if (result) return;

            var ex = OdbcMethods.GetException(connectionHandle, string.Format("Unable to bind {0} column ({1}).", column.DataTypeDescription, column.Index));

            throw ex;
        }

        IStandardColumn<long> IOdbcDriver.CreateBigInt(short index)
        {
            return BigIntColumn.CreateNonNullableInstance(index);
        }

        IStandardColumn<byte[]> IOdbcDriver.CreateBinary(short index, short fieldLength, bool nullable)
        {
            return BinaryColumn.CreateInstance(index, nullable ? BulkCopyDataType.NullableBinary : BulkCopyDataType.Binary, fieldLength);
        }

        IStandardColumn<bool> IOdbcDriver.CreateBit(short index)
        {
            return BitColumn.CreateNonNullableInstance(index);
        }

        IStandardColumn<string> IOdbcDriver.CreateChar(short index, short fieldLength, bool nullable)
        {
            return CharColumn.CreateInstance(index, nullable ? BulkCopyDataType.NullableChar : BulkCopyDataType.Char, fieldLength);
        }

        public abstract IStandardColumn<DateTime> CreateDate(short index);

        IStandardColumn<DateTime> IOdbcDriver.CreateDateTime(short index)
        {
            return DateTimeColumn.CreateNonNullableInstance(index);
        }

        public abstract IStandardColumn<DateTime> CreateDateTime2(short index);

        public abstract IStandardColumn<DateTimeOffset> CreateDateTimeOffset(short index);

        IStandardColumn<decimal> IOdbcDriver.CreateDecimal(short index)
        {
            return DecimalColumn.CreateNonNullableInstance(index);
        }

        IStandardColumn<double> IOdbcDriver.CreateFloat(short index)
        {
            return FloatColumn.CreateNonNullableInstance(index);
        }

        IStandardColumn<int> IOdbcDriver.CreateInt(short index)
        {
            return IntColumn.CreateNonNullableInstance(index);
        }

        IStandardColumn<decimal> IOdbcDriver.CreateMoney(short index)
        {
            return MoneyColumn.CreateNonNullableMoneyInstance(index);
        }

        IStandardColumn<string> IOdbcDriver.CreateNChar(short index, short fieldLength, bool nullable)
        {
            return CharColumn.CreateInstance(index, nullable ? BulkCopyDataType.NullableNChar : BulkCopyDataType.NChar, fieldLength);
        }

        IStandardColumn<string> IOdbcDriver.CreateNVarChar(short index, short fieldLength, bool nullable)
        {
            return CharColumn.CreateInstance(index, nullable ? BulkCopyDataType.NullableNVarChar : BulkCopyDataType.NVarChar, fieldLength);
        }

        IMaxColumn<string> IOdbcDriver.CreateNVarCharMax(short index, bool nullable)
        {
            return VarCharMaxColumn.CreateInstance(index, nullable ? BulkCopyDataType.NullableNVarCharMax : BulkCopyDataType.NVarCharMax);
        }

        IStandardColumn<float> IOdbcDriver.CreateReal(short index)
        {
            return RealColumn.CreateNonNullableInstance(index);
        }

        IStandardColumn<DateTime> IOdbcDriver.CreateSmallDateTime(short index)
        {
            return SmallDateTimeColumn.CreateNonNullableInstance(index);
        }

        IStandardColumn<short> IOdbcDriver.CreateSmallInt(short index)
        {
            return SmallIntColumn.CreateNonNullableInstance(index);
        }

        IStandardColumn<decimal> IOdbcDriver.CreateSmallMoney(short index)
        {
            return MoneyColumn.CreateNonNullableSmallMoneyInstance(index);
        }

        public abstract IStandardColumn<TimeSpan> CreateTime(short index);

        IStandardColumn<byte> IOdbcDriver.CreateTinyInt(short index)
        {
            return TinyIntColumn.CreateNonNullableInstance(index);
        }

        IStandardColumn<Guid> IOdbcDriver.CreateUniqueIdentifier(short index)
        {
            return UniqueIdentifierColumn.CreateNonNullableInstance(index);
        }

        IStandardColumn<long?> IOdbcDriver.CreateNullableBigInt(short index)
        {
            return BigIntColumn.CreateNullableInstance(index);
        }

        IStandardColumn<bool?> IOdbcDriver.CreateNullableBit(short index)
        {
            return BitColumn.CreateNullableInstance(index);
        }

        public abstract IStandardColumn<DateTime?> CreateNullableDate(short index);

        IStandardColumn<DateTime?> IOdbcDriver.CreateNullableDateTime(short index)
        {
            return DateTimeColumn.CreateNullableInstance(index);
        }

        public abstract IStandardColumn<DateTime?> CreateNullableDateTime2(short index);

        public abstract IStandardColumn<DateTimeOffset?> CreateNullableDateTimeOffset(short index);

        IStandardColumn<decimal?> IOdbcDriver.CreateNullableDecimal(short index)
        {
            return DecimalColumn.CreateNullableInstance(index);
        }

        IStandardColumn<double?> IOdbcDriver.CreateNullableFloat(short index)
        {
            return FloatColumn.CreateNullableInstance(index);
        }

        IStandardColumn<int?> IOdbcDriver.CreateNullableInt(short index)
        {
            return IntColumn.CreateNullableInstance(index);
        }

        IStandardColumn<decimal?> IOdbcDriver.CreateNullableMoney(short index)
        {
            return MoneyColumn.CreateNullableMoneyInstance(index);
        }

        IStandardColumn<float?> IOdbcDriver.CreateNullableReal(short index)
        {
            return RealColumn.CreateNullableInstance(index);
        }

        IStandardColumn<DateTime?> IOdbcDriver.CreateNullableSmallDateTime(short index)
        {
            return SmallDateTimeColumn.CreateNullableInstance(index);
        }

        IStandardColumn<short?> IOdbcDriver.CreateNullableSmallInt(short index)
        {
            return SmallIntColumn.CreateNullableInstance(index);
        }

        IStandardColumn<decimal?> IOdbcDriver.CreateNullableSmallMoney(short index)
        {
            return MoneyColumn.CreateNullableSmallMoneyInstance(index);
        }

        public abstract IStandardColumn<TimeSpan?> CreateNullableTime(short index);

        IStandardColumn<byte?> IOdbcDriver.CreateNullableTinyInt(short index)
        {
            return TinyIntColumn.CreateNullableInstance(index);
        }

        IStandardColumn<Guid?> IOdbcDriver.CreateNullableUniqueIdentifier(short index)
        {
            return UniqueIdentifierColumn.CreateNullableInstance(index);
        }

        IStandardColumn<byte[]> IOdbcDriver.CreateVarBinary(short index, short fieldLength, bool nullable)
        {
            return BinaryColumn.CreateInstance(index, nullable ? BulkCopyDataType.NullableVarBinary : BulkCopyDataType.VarBinary, fieldLength);
        }

        IMaxColumn<byte[]> IOdbcDriver.CreateVarBinaryMax(short index, bool nullable)
        {
            return VarBinaryMaxColumn.CreateInstance(index, nullable ? BulkCopyDataType.NullableVarBinaryMax : BulkCopyDataType.VarBinaryMax);
        }

        IStandardColumn<string> IOdbcDriver.CreateVarChar(short index, short fieldLength, bool nullable)
        {
            return CharColumn.CreateInstance(index, nullable ? BulkCopyDataType.NullableVarChar : BulkCopyDataType.VarChar, fieldLength);
        }

        IMaxColumn<string> IOdbcDriver.CreateVarCharMax(short index, bool nullable)
        {
            return VarCharMaxColumn.CreateInstance(index, nullable ? BulkCopyDataType.NullableVarCharMax : BulkCopyDataType.VarCharMax);
        }

        #endregion

        #region Bulk Copy

        public virtual int Batch(OdbcConnectionHandle connectionHandle)
        {
            if (connectionHandle == null) throw new ArgumentNullException("connectionHandle");

            return bcp_batch(connectionHandle);
        }

        public virtual int Done(OdbcConnectionHandle connectionHandle)
        {
            if (connectionHandle == null) throw new ArgumentNullException("connectionHandle");

            return bcp_done(connectionHandle);
        }

        public virtual bool SendMoreText(OdbcConnectionHandle connectionHandle, IMaxDataType column)
        {
            if (connectionHandle == null) throw new ArgumentNullException("connectionHandle");
            if (column == null) throw new ArgumentNullException("column");

            var data = column.Data;
            int length;

            if (data == null || (length = data.Length) == 0)
            {
                var result = bcp_moretext(connectionHandle, column.IsNullable ? Constants.SqlNullData : 0, (byte*)0);

                return result;
            }

            const int chunkSize = 2048;

            fixed (byte* ptr = column.Data)
            {
                var pData = ptr;

                while (length >= 0) // here we use >= 0. In SqlServerDriver we use > . Go figure.
                {
                    var cbData = length >= chunkSize ? chunkSize : length;

                    var result = bcp_moretext(connectionHandle, cbData, pData);

                    if (!result)
                    {
                        return false;
                    }

                    if (length == 0)
                    {
                        return true;
                    }

                    unchecked
                    {
                        length -= cbData;
                        pData += cbData;
                    }
                }
            }

            return true;
        }

        public virtual bool SendRow(OdbcConnectionHandle connectionHandle)
        {
            if (connectionHandle == null) throw new ArgumentNullException("connectionHandle");

            var result = bcp_sendrow(connectionHandle);

            return result;
        }

        public virtual bool SetColumnLength(OdbcConnectionHandle connectionHandle, IMaxDataType column)
        {
            if (connectionHandle == null) throw new ArgumentNullException("connectionHandle");
            if (column == null) throw new ArgumentNullException("column");

            var data = column.Data;
            var length = data == null ? 0 : data.Length;

            var result = bcp_collen(connectionHandle, length, column.Index + 1);

            return result;
        }

        #endregion

        protected abstract int bcp_batch(OdbcConnectionHandle hdbc);
        protected abstract bool bcp_bind(OdbcConnectionHandle hdbc, byte* pData, int cbIndicator, int cbData, byte* pTerm, int cbTerm, int eDataType, int idxServerCol);
        protected abstract bool bcp_collen(OdbcConnectionHandle hdbc, int cbData, int idxServerCol);
        protected abstract int bcp_done(OdbcConnectionHandle hdbc);
        protected abstract bool bcp_initW(OdbcConnectionHandle hdbc, string szTable, int eDirection);
        protected abstract bool bcp_moretext(OdbcConnectionHandle hdbc, int cbData, byte* pData);
        protected abstract bool bcp_sendrow(OdbcConnectionHandle hdbc);
    }
}
