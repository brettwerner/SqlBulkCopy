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
    internal sealed class DateTimeOffsetColumn : StandardDataType, IStandardColumn<DateTimeOffset>, IStandardColumn<DateTimeOffset?>
    {
        private const int DataTypeLengthInBytes = 20;

        private DateTimeOffsetColumn(short index, BulkCopyDataType dataType)
            : base(index, dataType, DataTypeLengthInBytes)
        {
            //
        }

        public static IStandardColumn<DateTimeOffset> CreateNonNullableInstance(short index)
        {
            return new DateTimeOffsetColumn(index, BulkCopyDataType.DateTimeOffset);
        }

        public static IStandardColumn<DateTimeOffset?> CreateNullableInstance(short index)
        {
            return new DateTimeOffsetColumn(index, BulkCopyDataType.NullableDateTimeOffset);
        }

        void ITypedBulkCopyBoundColumn<DateTimeOffset>.SetValue(DateTimeOffset value)
        {
            SetValue(value);
        }

        private void SetValue(DateTimeOffset value)
        {
            /*
             * Output:
             * 
             * SQL_SS_TIMESTAMPOFFSET_STRUCT (refer http://msdn.microsoft.com/en-us/library/bb677267.aspx)
             * 
             * 2 bytes: year
             * 2 bytes: month
             * 2 bytes: day
             * 2 bytes: hour
             * 2 bytes: minute
             * 2 bytes: second
             * 4 bytes: billionths of a second
             * 2 bytes: timezone hour
             * 2 bytes: timezone minute
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
            short timezoneHour;
            short timezoneMinute;

            var ticks = value.Ticks;
            DateTimeFunctions.GetEnhancedDateParts(ticks, out year, out month, out day);
            DateTimeFunctions.GetEnhancedTimeParts(ticks, out hour, out minute, out second, out millisecond);
            DateTimeFunctions.GetEnhancedTimeZoneParts(value.Offset.Ticks, out timezoneHour, out timezoneMinute);

            CopyInt16(0, year);
            CopyInt16(2, month);
            CopyInt16(4, day);
            CopyInt16(6, hour);
            CopyInt16(8, minute);
            CopyInt16(10, second);
            CopyInt32(12, millisecond * Constants.SqlThousandthsToBillionthsMultipler);
            CopyInt16(16, timezoneHour);
            CopyInt16(18, timezoneMinute);
        }

        void ITypedBulkCopyBoundColumn<DateTimeOffset?>.SetValue(DateTimeOffset? value)
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
