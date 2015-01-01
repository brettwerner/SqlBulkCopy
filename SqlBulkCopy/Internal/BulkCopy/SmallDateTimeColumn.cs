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
    internal sealed class SmallDateTimeColumn : StandardDataType, IStandardColumn<DateTime>, IStandardColumn<DateTime?>
    {
        private const int DataTypeLengthInBytes = 4;

        private SmallDateTimeColumn(short index, BulkCopyDataType dataType)
            : base(index, dataType, DataTypeLengthInBytes)
        {
            //
        }

        public static IStandardColumn<DateTime> CreateNonNullableInstance(short index)
        {
            return new SmallDateTimeColumn(index, BulkCopyDataType.SmallDateTime);
        }

        public static IStandardColumn<DateTime?> CreateNullableInstance(short index)
        {
            return new SmallDateTimeColumn(index, BulkCopyDataType.NullableSmallDateTime);
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
             * 2 bytes: days since 1900-01-01.
             * 2 bytes: minutes since midnight.
             * 
             * Valid for dates between 1900-01-01 and 2079-06-06.
            */

            var ticks = value.Ticks;

            const long minSmallDateTimeTicks = 599266080000000000;  // DateTime(1900, 1, 1).Ticks;
            const long maxSmallDateTimeTicks = 655889183699980000;  // DateTime(2079, 6, 6, 23, 59, 29, 998).Ticks;

            if ((ticks < minSmallDateTimeTicks) || (ticks > maxSmallDateTimeTicks))
            {
                throw new ArgumentOutOfRangeException("value", "Date must be between 1900-01-01 and 2079-06-06.");
            }

            short days;
            short minutes;

            DateTimeFunctions.GetSmallDateTimeParts(ticks, out days, out minutes);

            CopyInt16(0, days);
            CopyInt16(2, minutes);
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
