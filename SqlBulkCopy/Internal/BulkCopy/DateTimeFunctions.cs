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
    internal static class DateTimeFunctions
    {
        private const int MillisecondsPerSecond = 1000;
        private const int SecondsPerMinute = 60;
        private const int MinutesPerHour = 60;
        private const int HoursPerDay = 24;

        private const int MinutesPerDay = MinutesPerHour * HoursPerDay;
        private const int SecondsPerDay = SecondsPerMinute * MinutesPerHour * HoursPerDay;

        private const long TicksPerMillisecond = 10000L;
        private const long TicksPerSecond = TicksPerMillisecond * MillisecondsPerSecond;
        private const long TicksPerMinute = TicksPerSecond * SecondsPerMinute;
        private const long TicksPerHour = TicksPerMinute * MinutesPerHour;
        private const long TicksPerDay = TicksPerHour * HoursPerDay;

        private const int SqlDateTimeMillisecondsPerSecond = 300;
        private const Double SqlDateTimeSecondsPerMillisecond = 3.333;

        private const int DaysPerYear = 365;
        private const int DaysPer4Years = DaysPerYear * 4 + 1;
        private const int DaysPer100Years = DaysPer4Years * 25 - 1;
        private const int DaysPer400Years = DaysPer100Years * 4 + 1;
        private static readonly int[] DaysToMonth365 = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365};
        private static readonly int[] DaysToMonth366 = { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366};

        internal static void GetDateTimeParts(long ticks, out int days, out int milliseconds)
        {
            // datetime values are rounded to 1/300th of a second, in increments of .000, .003, or .007 seconds.

            // Refer http://msdn.microsoft.com/en-us//library/ms187819.aspx: Rounding of datetime Fractional Second Precision.

            const long defaultDateTimeTicks = 599266080000000000;   // DateTime(1900, 1, 1).Ticks;

            days = (int)(((ticks - (ticks % TicksPerDay)) - defaultDateTimeTicks) / TicksPerDay);

            var seconds = (int)((ticks / TicksPerSecond) % SecondsPerDay);
            var millisecond = (int)((ticks / TicksPerMillisecond) % MillisecondsPerSecond);

            milliseconds = (seconds * SqlDateTimeMillisecondsPerSecond) + (int)Math.Round(millisecond / SqlDateTimeSecondsPerMillisecond);
        }

        internal static void GetEnhancedDateParts(long ticks, out short year, out short month, out short day)
        {
            // d = number of days since 01-01-0001
            var d = (int)(ticks / TicksPerDay);

            // y400 = number of whole 400-year periods since 01-01-0001
            var y400 = d / DaysPer400Years;

            // d = day number within 400-year period
            d -= y400 * DaysPer400Years;

            // y100 = number of whole 100-year periods within 400-year period
            var y100 = d / DaysPer100Years;

            // last 100-year period has an extra day, so decrement result if 4
            if (y100 == 4)
            {
                y100 = 3;
            }

            // d = day number within 100-year period
            d -= y100 * DaysPer100Years;

            // y4 = number of whole 4-year periods within 100-year period
            var y4 = d / DaysPer4Years;

            // d = day number within 4-year period
            d -= y4 * DaysPer4Years;

            // y1 = number of whole years within 4-year period
            var y1 = d / DaysPerYear;

            // last year has an extra day, so decrement result if 4
            if (y1 == 4)
            {
                y1 = 3;
            }

            year = (short)(y400 * 400 + y100 * 100 + y4 * 4 + y1 + 1);

            // d = day number within year
            d -= y1 * DaysPerYear;

            var leapYear = y1 == 3 && (y4 != 24 || y100 == 3);

            var days = leapYear ? DaysToMonth366 : DaysToMonth365;

            month = (short)(d / 32 + 1);

            while (days[month] <= d)
            {
                month++;
            }

            day = (short)(d - days[month - 1] + 1);
        }

        internal static void GetEnhancedTimeParts(long ticks, out short hour, out short minute, out short second, out int millisecond)
        {
            hour = (short)((ticks / TicksPerHour) % HoursPerDay);
            minute = (short)((ticks / TicksPerMinute) % MinutesPerHour);
            second = (short)((ticks / TicksPerSecond) % SecondsPerMinute);
            millisecond = (int)((ticks / TicksPerMillisecond) % MillisecondsPerSecond);
        }

        internal static void GetEnhancedTimeZoneParts(long ticks, out short hour, out short minute)
        {
            hour = (short)((ticks / TicksPerHour) % HoursPerDay);
            minute = (short)((ticks / TicksPerMinute) % MinutesPerHour);
        }

        internal static void GetSmallDateTimeParts(long ticks, out short days, out short minutes)
        {
            // smalldatetime values are rounded to the nearest minute.

            // Values that are 29.998 seconds or less are rounded down to the nearest minute.
            // Values of 29.999 seconds or more are rounded up to the nearest minute.

            // Refer http://msdn.microsoft.com/en-us/library/ms182418.aspx.

            const long minSmallDateTimeTicks = 599266080000000000;  // DateTime(1900, 1, 1).Ticks;

            days = (short)(((ticks - (ticks % TicksPerDay)) - minSmallDateTimeTicks) / TicksPerDay);
            minutes = (short)((ticks / TicksPerMinute) % MinutesPerDay);

            var second = (int)((ticks / TicksPerSecond) % SecondsPerMinute);

            if (second >= 30)
            {
                minutes++;

                if (minutes != MinutesPerDay) return;

                minutes = 0;
                days++;
            }
            else if (second == 29)
            {
                var millisecond = (int)((ticks / TicksPerMillisecond) % MillisecondsPerSecond);

                if (millisecond != 999) return;

                minutes++;

                if (minutes != MinutesPerDay) return;

                minutes = 0;
                days++;
            }
        }
    }
}
