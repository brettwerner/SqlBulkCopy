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
using SqlBulkCopy.Internal.Common;

namespace SqlBulkCopy.Internal.BulkCopy
{
    internal sealed class DateTime2Column : StandardDataType, IStandardColumn<DateTime>, IStandardColumn<DateTime?>
    {
        private const int DataTypeLengthInBytes = 16;

        private DateTime2Column(short index, BulkCopyDataType dataType)
            : base(index, dataType, DataTypeLengthInBytes)
        {
            //
        }

        public static IStandardColumn<DateTime> CreateNonNullableInstance(short index)
        {
            return new DateTime2Column(index, BulkCopyDataType.DateTime2);
        }

        public static IStandardColumn<DateTime?> CreateNullableInstance(short index)
        {
            return new DateTime2Column(index, BulkCopyDataType.NullableDateTime2);
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
             * SQL_TIMESTAMP_STRUCT (refer http://msdn.microsoft.com/en-us/library/ms714556.aspx)
             * 
             * 2 bytes: year
             * 2 bytes: month
             * 2 bytes: day
             * 2 bytes: hour
             * 2 bytes: minute
             * 2 bytes: second
             * 4 bytes: billionths of a second
             * 
             * Valid for years between 0001 and 9999.
            */

            short year;
            short month;
            short day;
            short hour;
            short minute;
            short second;
            int millisecond;

            var ticks = value.Ticks;
            DateTimeFunctions.GetEnhancedDateParts(ticks, out year, out month, out day);
            DateTimeFunctions.GetEnhancedTimeParts(ticks, out hour, out minute, out second, out millisecond);

            CopyInt16(0, year);
            CopyInt16(2, month);
            CopyInt16(4, day);
            CopyInt16(6, hour);
            CopyInt16(8, minute);
            CopyInt16(10, second);
            CopyInt32(12, millisecond * Constants.SqlThousandthsToBillionthsMultipler);
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
