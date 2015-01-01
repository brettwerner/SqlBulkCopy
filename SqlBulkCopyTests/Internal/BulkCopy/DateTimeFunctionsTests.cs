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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlBulkCopy.Internal.BulkCopy;

namespace SqlBulkCopyTests.Internal.BulkCopy
{
    [TestClass]
    public class DateTimeFunctionsTests
    {
        private const int MinutesPerHour = 60;
        private const int HoursPerDay = 24;
        private const int MinutesPerDay = MinutesPerHour * HoursPerDay;
        private static readonly DateTime DateTimeZeroDate = new DateTime(1900, 1, 1);

        [TestMethod]
        public void GetDateTimeReturnsCorrectValues()
        {
            var minDateTime = new DateTime(1753, 1, 1);
            var maxDateTime = new DateTime(9999, 12, 31);

            VerifyGetDateTimeParts(minDateTime);
            VerifyGetDateTimeParts(maxDateTime);
        }

        [TestMethod]
        public void GetDateTimeRoundsMillisecondsCorrectly()
        {
            var value = new DateTime(1998, 1, 1, 23, 59, 59, 989);

            int days;
            int expectedMilliseconds;

            // datetime values are rounded to 1/300th of a second, in increments of .000, .003, or .007 seconds.

            // Refer http://msdn.microsoft.com/en-us//library/ms187819.aspx: Rounding of datetime Fractional Second Precision.

            DateTimeFunctions.GetDateTimeParts(value.Ticks, out days, out expectedMilliseconds);

            for (var i = 0; i < 1000; i++)
            {
                value = value.AddMilliseconds(1);

                switch (value.Millisecond % 10)
                {
                    case 2:
                    case 5:
                    case 9:
                        {
                            expectedMilliseconds++;
                            break;
                        }
                }

                if (value == value.Date)
                {
                    expectedMilliseconds = 0;
                }

                VerifyGetDateTimePartsMilliseconds(value, expectedMilliseconds);
            }
        }

        [TestMethod]
        public void GetEnhancedDatePartsReturnsCorrectValues()
        {
            var minDate = new DateTime(1, 1, 1);
            var maxDate = new DateTime(9999, 12, 31);
            var y400LastDay = new DateTime(2000, 12, 31);

            VerifyGetEnhancedDateParts(minDate);
            VerifyGetEnhancedDateParts(maxDate);
            VerifyGetEnhancedDateParts(y400LastDay);
        }

        [TestMethod]
        public void GetEnhancedTimePartsReturnsCorrectValues()
        {
            var minDateTime2 = new DateTime(1, 1, 1, 0, 0, 0, 0);
            var maxDateTime2 = new DateTime(9999, 12, 31, 23, 59, 59, 999);
            var now = DateTime.Now;

            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);

            VerifyGetEnhancedTimeParts(minDateTime2);
            VerifyGetEnhancedTimeParts(maxDateTime2);
            VerifyGetEnhancedTimeParts(now);
        }

        [TestMethod]
        public void GetEnhancedTimeZonePartsReturnsCorrectValues()
        {
            var minDateTimeOffset = new DateTimeOffset(1, 1, 1, 0, 0, 0, 0, new TimeSpan(-14, 0, 0));
            var maxDateTimeOffset = new DateTimeOffset(9999, 12, 31, 23, 59, 59, 999, new TimeSpan(14, 0, 0));

            VerifyGetEnhancedTimeZoneParts(minDateTimeOffset);
            VerifyGetEnhancedTimeZoneParts(maxDateTimeOffset);
        }

        [TestMethod]
        public void GetSmallDateTimeReturnsCorrectValues()
        {
            var minSmallDateTime = new DateTime(1900, 1, 1);
            var maxSmallDateTime = new DateTime(2079, 6, 6, 23, 59, 29, 998);
            var smallDateTime1 = new DateTime(2014, 1, 1, 0, 0, 29, 999);   // verify proper rounding up
            var smallDateTime2 = new DateTime(2014, 1, 1, 0, 0, 30, 0);     // verify proper rounding up
            var smallDateTime3 = new DateTime(2014, 1, 1, 23, 59, 29, 999); // verify proper rounding up
            var smallDateTime4 = new DateTime(2014, 1, 1, 23, 59, 30, 0);   // verify proper rounding up

            VerifyGetSmallDateTimeParts(minSmallDateTime);
            VerifyGetSmallDateTimeParts(maxSmallDateTime);
            VerifyGetSmallDateTimeParts(smallDateTime1);
            VerifyGetSmallDateTimeParts(smallDateTime2);
            VerifyGetSmallDateTimeParts(smallDateTime3);
            VerifyGetSmallDateTimeParts(smallDateTime4);
        }

        private static void VerifyGetDateTimeParts(DateTime value)
        {
            var expectedDays = value.Subtract(DateTimeZeroDate).Days;
            var expectedMilliseconds = (((value.Hour * 3600) + (value.Minute * 60) + (value.Second))) * 300;
            int actualDays;
            int actualMilliseconds;

            DateTimeFunctions.GetDateTimeParts(value.Ticks, out actualDays, out actualMilliseconds);

            Assert.AreEqual(expectedDays, actualDays, "days");
            Assert.AreEqual(expectedMilliseconds, actualMilliseconds, "milliseconds");
        }

        private static void VerifyGetDateTimePartsMilliseconds(DateTime value, int expectedMilliseconds)
        {
            int actualDays;
            int actualMilliseconds;

            DateTimeFunctions.GetDateTimeParts(value.Ticks, out actualDays, out actualMilliseconds);

            Assert.AreEqual(expectedMilliseconds, actualMilliseconds, "milliseconds");
        }

        private static void VerifyGetEnhancedDateParts(DateTime value)
        {
            short year;
            short month;
            short day;
            DateTimeFunctions.GetEnhancedDateParts(value.Ticks, out year, out month, out day);

            Assert.AreEqual(value.Year, year, "year");
            Assert.AreEqual(value.Month, month, "month");
            Assert.AreEqual(value.Day, day, "day");
        }

        private static void VerifyGetEnhancedTimeParts(DateTime value)
        {
            short hour;
            short minute;
            short second;
            int millisecond;
            DateTimeFunctions.GetEnhancedTimeParts(value.Ticks, out hour, out minute, out second, out millisecond);

            Assert.AreEqual(value.Hour, hour, "hour");
            Assert.AreEqual(value.Minute, minute, "minute");
            Assert.AreEqual(value.Second, second, "second");
            Assert.AreEqual(value.Millisecond, millisecond, "millisecond");
        }

        private static void VerifyGetEnhancedTimeZoneParts(DateTimeOffset value)
        {
            short timezoneHour;
            short timezoneMinute;
            DateTimeFunctions.GetEnhancedTimeZoneParts(value.Offset.Ticks, out timezoneHour, out timezoneMinute);

            Assert.AreEqual(value.Offset.Hours, timezoneHour, "timezoneHour");
            Assert.AreEqual(value.Offset.Minutes, timezoneMinute, "timezoneMinute");
        }

        private static void VerifyGetSmallDateTimeParts(DateTime value)
        {
            var expectedDays = (short)value.Subtract(DateTimeZeroDate).Days;
            var expectedMinutes = (short)((value.Hour * 60) + value.Minute);

            // smalldatetime values are rounded to the nearest minutes.

            // Values that are 29.998 seconds or less are rounded down to the nearest minute.
            // Values of 29.999 seconds or more are rounded up to the nearest minute.

            // Refer http://msdn.microsoft.com/en-us/library/ms182418.aspx.

            var second = value.Second;

            if (second >= 30)
            {
                expectedMinutes++;
                if (expectedMinutes == MinutesPerDay)
                {
                    expectedMinutes = 0;
                    expectedDays++;
                }
            }
            else if (second == 29)
            {
                var millisecond = value.Millisecond;

                if (millisecond == 999)
                {
                    expectedMinutes++;
                    if (expectedMinutes == MinutesPerDay)
                    {
                        expectedMinutes = 0;
                        expectedDays++;
                    }
                }
            }

            short actualDays;
            short actualMinutes;

            DateTimeFunctions.GetSmallDateTimeParts(value.Ticks, out actualDays, out actualMinutes);

            Assert.AreEqual(expectedDays, actualDays, "days");
            Assert.AreEqual(expectedMinutes, actualMinutes, "minutes");
        }
    }
}
