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
using System.Runtime.InteropServices;
using SqlBulkCopy.Internal.BulkCopy;
using SqlBulkCopy.Internal.Common;
using SqlBulkCopy.Internal.Odbc;

namespace SqlBulkCopy.Internal.Drivers
{
    internal unsafe sealed class Sqlncl10Driver : OdbcDriver
    {
        private static class NativeMethods
        {
            #region sqlncli10.dll

            [DllImport("sqlncli10.dll", SetLastError = false)]
            internal static extern int bcp_batch(OdbcConnectionHandle hdbc);

            [DllImport("sqlncli10.dll", SetLastError = false)]
            internal static extern bool bcp_bind(OdbcConnectionHandle hdbc, byte* pData, int cbIndicator, int cbData, byte* pTerm, int cbTerm, int eDataType, int idxServerCol);

            [DllImport("sqlncli10.dll", SetLastError = false)]
            internal static extern bool bcp_collen(OdbcConnectionHandle hdbc, int cbData, int idxServerCol);

            [DllImport("sqlncli10.dll", SetLastError = false)]
            internal static extern int bcp_done(OdbcConnectionHandle hdbc);

            [DllImport("sqlncli10.dll", CharSet = CharSet.Unicode, SetLastError = false)]
            internal static extern bool bcp_initW(OdbcConnectionHandle hdbc, [In, MarshalAs(UnmanagedType.LPWStr)] string szTable, IntPtr szDataFileMustBeZero, IntPtr szErrorFileMustBeZero, int eDirection);

            [DllImport("sqlncli10.dll", SetLastError = false)]
            internal static extern bool bcp_moretext(OdbcConnectionHandle hdbc, int cbData, byte* pData);

            [DllImport("sqlncli10.dll", SetLastError = false)]
            internal static extern bool bcp_sendrow(OdbcConnectionHandle hdbc);

            #endregion
        }

        private static readonly Sqlncl10Driver Instance = new Sqlncl10Driver();

        private Sqlncl10Driver()
        {
            //
        }

        public static Sqlncl10Driver SingleInstance
        {
            get { return Instance; }
        }

        public override IOdbcEnvironment CreateEnvironment(OdbcConnectionPooling connectionPooling)
        {
            return new OdbcEnvironmentHandle(OdbcVersion.Version38, connectionPooling, this);
        }

        public override string DriverOptionString
        {
            get { return "Driver={SQL Server Native Client 10.0}"; }
        }

        public override IStandardColumn<DateTime> CreateDate(short index)
        {
            return DateColumn.CreateNonNullableInstance(index);
        }

        public override IStandardColumn<DateTime> CreateDateTime2(short index)
        {
            return DateTime2Column.CreateNonNullableInstance(index);
        }

        public override IStandardColumn<DateTimeOffset> CreateDateTimeOffset(short index)
        {
            return DateTimeOffsetColumn.CreateNonNullableInstance(index);
        }

        public override IStandardColumn<TimeSpan> CreateTime(short index)
        {
            return TimeColumn.CreateNonNullableInstance(index);
        }

        public override IStandardColumn<DateTime?> CreateNullableDate(short index)
        {
            return DateColumn.CreateNullableInstance(index);
        }

        public override IStandardColumn<DateTime?> CreateNullableDateTime2(short index)
        {
            return DateTime2Column.CreateNullableInstance(index);
        }

        public override IStandardColumn<DateTimeOffset?> CreateNullableDateTimeOffset(short index)
        {
            return DateTimeOffsetColumn.CreateNullableInstance(index);
        }

        public override IStandardColumn<TimeSpan?> CreateNullableTime(short index)
        {
            return TimeColumn.CreateNullableInstance(index);
        }

        protected override int bcp_batch(OdbcConnectionHandle hdbc)
        {
            return NativeMethods.bcp_batch(hdbc);
        }

        protected override bool bcp_bind(OdbcConnectionHandle hdbc, byte* pData, int cbIndicator, int cbData, byte* pTerm, int cbTerm, int eDataType, int idxServerCol)
        {
            return NativeMethods.bcp_bind(hdbc, pData, cbIndicator, cbData, pTerm, cbTerm, eDataType, idxServerCol);
        }

        protected override bool bcp_collen(OdbcConnectionHandle hdbc, int cbData, int idxServerCol)
        {
            return NativeMethods.bcp_collen(hdbc, cbData, idxServerCol);
        }

        protected override int bcp_done(OdbcConnectionHandle hdbc)
        {
            return NativeMethods.bcp_done(hdbc);
        }

        protected override bool bcp_initW(OdbcConnectionHandle hdbc, string szTable, int eDirection)
        {
            return NativeMethods.bcp_initW(hdbc, szTable, Constants.IntPtrZero, Constants.IntPtrZero, eDirection);
        }

        protected override bool bcp_moretext(OdbcConnectionHandle hdbc, int cbData, byte* pData)
        {
            return NativeMethods.bcp_moretext(hdbc, cbData, pData);
        }

        protected override bool bcp_sendrow(OdbcConnectionHandle hdbc)
        {
            return NativeMethods.bcp_sendrow(hdbc);
        }
    }
}
