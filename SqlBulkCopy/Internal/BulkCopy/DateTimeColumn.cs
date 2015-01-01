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
    internal sealed class DateTimeColumn : StandardDataType, IStandardColumn<DateTime>, IStandardColumn<DateTime?>
    {
        private const int DataTypeLengthInBytes = 8;

        private DateTimeColumn(short index, BulkCopyDataType dataType)
            : base(index, dataType, DataTypeLengthInBytes)
        {
            //
        }

        public static IStandardColumn<DateTime> CreateNonNullableInstance(short index)
        {
            return new DateTimeColumn(index, BulkCopyDataType.DateTime);
        }

        public static IStandardColumn<DateTime?> CreateNullableInstance(short index)
        {
            return new DateTimeColumn(index, BulkCopyDataType.NullableDateTime);
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
             * 4 bytes: days since 1900-01-01.
             * 4 bytes: milliseconds since midnight.
             * 
             * Valid for years between 1753 and 9999.
            */

            var ticks = value.Ticks;

            const long minDateTimeTicks = 552877920000000000;       // DateTime(1753, 1, 1).Ticks;
            const long maxDateTimeTicks = 3155378975999970000;      // DateTime(9999, 12, 31, 23, 59, 59, 997).Ticks;

            if ((ticks < minDateTimeTicks) || (ticks > maxDateTimeTicks))
            {
                throw new ArgumentOutOfRangeException("value", "Year must be between 1753 and 9999.");
            }

            int days;
            int milliseconds;

            DateTimeFunctions.GetDateTimeParts(ticks, out days, out milliseconds);

            CopyInt32(0, days);
            CopyInt32(4, milliseconds);
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
