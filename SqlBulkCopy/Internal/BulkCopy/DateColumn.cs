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

namespace SqlBulkCopy.Internal.BulkCopy
{
    internal sealed class DateColumn : StandardDataType, IStandardColumn<DateTime>, IStandardColumn<DateTime?>
    {
        private const int DataTypeLengthInBytes = 6;

        private DateColumn(short index, BulkCopyDataType dataType)
            : base(index, dataType, DataTypeLengthInBytes)
        {
            //
        }

        public static IStandardColumn<DateTime> CreateNonNullableInstance(short index)
        {
            return new DateColumn(index, BulkCopyDataType.Date);
        }

        public static IStandardColumn<DateTime?> CreateNullableInstance(short index)
        {
            return new DateColumn(index, BulkCopyDataType.NullableDate);
        }

        void ITypedBulkCopyBoundColumn<DateTime>.SetValue(DateTime value)
        {
            SetValue(value);
        }

        private void SetValue(DateTime value)
        {
            /*
             * Output:
             * 
             * SQL_DATE_STRUCT (refer http://msdn.microsoft.com/en-us/library/ms714556.aspx)
             * 
             * 2 bytes: year
             * 2 bytes: month
             * 2 bytes: day
             * 
             * Valid for years between 0001 and 9999.
            */

            short year;
            short month;
            short day;

            DateTimeFunctions.GetEnhancedDateParts(value.Ticks, out year, out month, out day);

            CopyInt16(0, year);
            CopyInt16(2, month);
            CopyInt16(4, day);
        }

        void ITypedBulkCopyBoundColumn<DateTime?>.SetValue(DateTime? value)
        {
            if (value.HasValue)
            {
                SetLength(DataTypeLengthInBytes);
                SetValue(value.Value);
            }
            else
            {
                SetNull();
            }
        }
    }
}
