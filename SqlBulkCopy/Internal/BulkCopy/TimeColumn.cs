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
    internal sealed class TimeColumn : StandardDataType, IStandardColumn<TimeSpan>, IStandardColumn<TimeSpan?>
    {
        private const int DataTypeLengthInBytes = 12;

        private TimeColumn(short index, BulkCopyDataType dataType)
            : base(index, dataType, DataTypeLengthInBytes)
        {
            //
        }

        public static IStandardColumn<TimeSpan> CreateNonNullableInstance(short index)
        {
            return new TimeColumn(index, BulkCopyDataType.Time);
        }

        public static IStandardColumn<TimeSpan?> CreateNullableInstance(short index)
        {
            return new TimeColumn(index, BulkCopyDataType.NullableTime);
        }

        void ITypedBulkCopyBoundColumn<TimeSpan>.SetValue(TimeSpan value)
        {
            SetValue(value);
        }

        private void SetValue(TimeSpan value)
        {
            /*
             * Output:
             * 
             * SQL_SS_TIME2_STRUCT (refer http://msdn.microsoft.com/en-us/library/bb677267.aspx)
             * 
             * 2 bytes: hour
             * 2 bytes: minute
             * 2 bytes: second
             * 2 bytes: (undocumented)
             * 4 bytes: billionths of a second
             * 
             * Seconds since midnight, with precision up to 7 decimal places.
            */

            short hour;
            short minute;
            short second;
            int millisecond;

            DateTimeFunctions.GetEnhancedTimeParts(value.Ticks, out hour, out minute, out second, out millisecond);

            CopyInt16(0, hour);
            CopyInt16(2, minute);
            CopyInt16(4, second);
            CopyInt32(8, millisecond * Constants.SqlThousandthsToBillionthsMultipler);
        }

        void ITypedBulkCopyBoundColumn<TimeSpan?>.SetValue(TimeSpan? value)
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
