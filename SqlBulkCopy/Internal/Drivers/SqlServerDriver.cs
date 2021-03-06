﻿/*
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
using System.Runtime.InteropServices;
using SqlBulkCopy.Internal.BulkCopy;
using SqlBulkCopy.Internal.Common;
using SqlBulkCopy.Internal.Odbc;

namespace SqlBulkCopy.Internal.Drivers
{
    internal unsafe sealed class SqlServerDriver : OdbcDriver
    {
        private static class NativeMethods
        {
            #region odbcbcp.dll

            [DllImport("odbcbcp.dll", SetLastError = false)]
            internal static extern int bcp_batch(OdbcConnectionHandle hdbc);

            [DllImport("odbcbcp.dll", SetLastError = false)]
            internal static extern short bcp_bind(OdbcConnectionHandle hdbc, byte* pData, int cbIndicator, int cbData, byte* pTerm, int cbTerm, int eDataType, int idxServerCol);

            [DllImport("odbcbcp.dll", SetLastError = false)]
            internal static extern short bcp_collen(OdbcConnectionHandle hdbc, int cbData, int idxServerCol);

            [DllImport("odbcbcp.dll", SetLastError = false)]
            internal static extern int bcp_done(OdbcConnectionHandle hdbc);

            [DllImport("odbcbcp.dll", CharSet = CharSet.Unicode, SetLastError = false)]
            internal static extern short bcp_initW(OdbcConnectionHandle hdbc, [In, MarshalAs(UnmanagedType.LPWStr)] string szTable, IntPtr szDataFileMustBeZero, IntPtr szErrorFileMustBeZero, int eDirection);

            [DllImport("odbcbcp.dll", SetLastError = false)]
            internal static extern short bcp_moretext(OdbcConnectionHandle hdbc, int cbData, byte* pData);

            [DllImport("odbcbcp.dll", SetLastError = false)]
            internal static extern short bcp_sendrow(OdbcConnectionHandle hdbc);

            internal const short Succeed = 1;

            #endregion
        }

        private static readonly SqlServerDriver Instance = new SqlServerDriver();

        private SqlServerDriver()
        {
            //
        }

        public static SqlServerDriver SingleInstance
        {
            get { return Instance; }
        }

        public override IOdbcEnvironment CreateEnvironment(OdbcConnectionPooling connectionPooling)
        {
            return new OdbcEnvironmentHandle(OdbcVersion.Version38, connectionPooling, this);
        }

        public override string DriverOptionString
        {
            get { return "Driver={SQL Server}"; }
        }

        public override IStandardColumn<DateTime> CreateDate(short index)
        {
            return VarCharConversionColumn.CreateNonNullableDateInstance(index);
        }

        public override IStandardColumn<DateTime> CreateDateTime2(short index)
        {
            return VarCharConversionColumn.CreateNonNullableDateTime2Instance(index);
        }

        public override IStandardColumn<DateTimeOffset> CreateDateTimeOffset(short index)
        {
            return VarCharConversionColumn.CreateNonNullableDateTimeOffsetInstance(index);
        }

        public override IStandardColumn<TimeSpan> CreateTime(short index)
        {
            return VarCharConversionColumn.CreateNonNullableTimeInstance(index);
        }

        public override IStandardColumn<DateTime?> CreateNullableDate(short index)
        {
            return VarCharConversionColumn.CreateNullableDateInstance(index);
        }

        public override IStandardColumn<DateTime?> CreateNullableDateTime2(short index)
        {
            return VarCharConversionColumn.CreateNullableDateTime2Instance(index);
        }

        public override IStandardColumn<DateTimeOffset?> CreateNullableDateTimeOffset(short index)
        {
            return VarCharConversionColumn.CreateNullableDateTimeOffsetInstance(index);
        }

        public override IStandardColumn<TimeSpan?> CreateNullableTime(short index)
        {
            return VarCharConversionColumn.CreateNullableTimeInstance(index);
        }

        public override bool SendMoreText(OdbcConnectionHandle connectionHandle, IMaxDataType column)
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

                while (length > 0) // here we use > 0. In all other drivers we use >= . Go figure.
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

        protected override int bcp_batch(OdbcConnectionHandle hdbc)
        {
            return NativeMethods.bcp_batch(hdbc);
        }

        protected override bool bcp_bind(OdbcConnectionHandle hdbc, byte* pData, int cbIndicator, int cbData, byte* pTerm, int cbTerm, int eDataType, int idxServerCol)
        {
            return NativeMethods.bcp_bind(hdbc, pData, cbIndicator, cbData, pTerm, cbTerm, eDataType, idxServerCol) == NativeMethods.Succeed;
        }

        protected override bool bcp_collen(OdbcConnectionHandle hdbc, int cbData, int idxServerCol)
        {
            return NativeMethods.bcp_collen(hdbc, cbData, idxServerCol) == NativeMethods.Succeed;
        }

        protected override int bcp_done(OdbcConnectionHandle hdbc)
        {
            return NativeMethods.bcp_done(hdbc);
        }

        protected override bool bcp_initW(OdbcConnectionHandle hdbc, string szTable, int eDirection)
        {
            return NativeMethods.bcp_initW(hdbc, szTable, Constants.IntPtrZero, Constants.IntPtrZero, eDirection) == NativeMethods.Succeed;
        }

        protected override bool bcp_moretext(OdbcConnectionHandle hdbc, int cbData, byte* pData)
        {
            return NativeMethods.bcp_moretext(hdbc, cbData, pData) == NativeMethods.Succeed;
        }

        protected override bool bcp_sendrow(OdbcConnectionHandle hdbc)
        {
            return NativeMethods.bcp_sendrow(hdbc) == NativeMethods.Succeed;
        }
    }
}
