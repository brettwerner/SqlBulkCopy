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
using System.Globalization;
using System.Text;

namespace SqlBulkCopy.Internal.BulkCopy
{
    internal sealed class VarCharConversionColumn : StandardDataType, 
        IStandardColumn<DateTime>, IStandardColumn<DateTime?>, 
        IStandardColumn<TimeSpan>, IStandardColumn<TimeSpan?>, 
        IStandardColumn<DateTimeOffset>, IStandardColumn<DateTimeOffset?>
    {
        private readonly Encoding _encoding;
        private readonly string _format;

        private const string DateFormat = "yyyy-MM-dd";                                 // ISO 8601 date.
        private const int DateFormatLength = 10;

        private const string DateTime2Format = "yyyy-MM-dd HH:mm:ss.FFFFFFF";           // ISO 8601 local date and time.
        private const int MaxDateTime2FormatLength = 27;

        private const string DateTimeOffsetFormat = "yyyy-MM-dd HH:mm:ss.FFFFFFF K";    // ISO 8601 date and time with UTC time zone offset.
        private const int MaxDateTimeOffsetFormatLength = 34;

        // TimeSpan.ToString(String, IFormatProvider) was not introduced until .NET 4.0, so we use String.Format(IFormatProvider, String, Object[]) instead.

        private const string TimeFormat = "{0:c}";                                      // ISO 8601 time (HH:mm:ss.FFFFFFF).
        private const int MaxTimeFormatLength = 16;

        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        private VarCharConversionColumn(short index, BulkCopyDataType dataType, short length, string format, BindingFlags options = BindingFlags.None)
            : base(index, dataType, length, options | BindingFlags.RequiresConversion)
        {
            _encoding = Encoding.ASCII;
            _format = format;
        }

        public static IStandardColumn<DateTime> CreateNonNullableDateInstance(short index)
        {
            return new VarCharConversionColumn(index, BulkCopyDataType.Date, DateFormatLength, DateFormat);
        }

        public static IStandardColumn<DateTime?> CreateNullableDateInstance(short index)
        {
            return new VarCharConversionColumn(index, BulkCopyDataType.NullableDate, DateFormatLength, DateFormat);
        }

        public static IStandardColumn<DateTime> CreateNonNullableDateTime2Instance(short index)
        {
            return new VarCharConversionColumn(index, BulkCopyDataType.DateTime2, MaxDateTime2FormatLength, DateTime2Format, BindingFlags.VariableLengthIn);
        }

        public static IStandardColumn<DateTime?> CreateNullableDateTime2Instance(short index)
        {
            return new VarCharConversionColumn(index, BulkCopyDataType.NullableDateTime2, MaxDateTime2FormatLength, DateTime2Format, BindingFlags.VariableLengthIn);
        }

        public static IStandardColumn<DateTimeOffset> CreateNonNullableDateTimeOffsetInstance(short index)
        {
            return new VarCharConversionColumn(index, BulkCopyDataType.DateTimeOffset, MaxDateTimeOffsetFormatLength, DateTimeOffsetFormat, BindingFlags.VariableLengthIn);
        }

        public static IStandardColumn<DateTimeOffset?> CreateNullableDateTimeOffsetInstance(short index)
        {
            return new VarCharConversionColumn(index, BulkCopyDataType.NullableDateTimeOffset, MaxDateTimeOffsetFormatLength, DateTimeOffsetFormat, BindingFlags.VariableLengthIn);
        }

        public static IStandardColumn<TimeSpan> CreateNonNullableTimeInstance(short index)
        {
            return new VarCharConversionColumn(index, BulkCopyDataType.Time, MaxTimeFormatLength, TimeFormat, BindingFlags.VariableLengthIn);
        }

        public static IStandardColumn<TimeSpan?> CreateNullableTimeInstance(short index)
        {
            return new VarCharConversionColumn(index, BulkCopyDataType.NullableTime, MaxTimeFormatLength, TimeFormat, BindingFlags.VariableLengthIn);
        }

        void ITypedBulkCopyBoundColumn<DateTime>.SetValue(DateTime value)
        {
            SetValue(value.ToString(_format, Culture));
        }

        void ITypedBulkCopyBoundColumn<DateTimeOffset>.SetValue(DateTimeOffset value)
        {
            SetValue(value.ToString(_format, Culture));
        }

        void ITypedBulkCopyBoundColumn<TimeSpan>.SetValue(TimeSpan value)
        {
            SetValue(string.Format(Culture, _format, value));
        }

        void ITypedBulkCopyBoundColumn<DateTime?>.SetValue(DateTime? value)
        {
            if (value.HasValue)
            {
                SetValue(value.Value.ToString(_format, Culture));
            }
            else
            {
                SetNull();
            }
        }

        void ITypedBulkCopyBoundColumn<DateTimeOffset?>.SetValue(DateTimeOffset? value)
        {
            if (value.HasValue)
            {
                SetValue(value.Value.ToString(_format, Culture));
            }
            else
            {
                SetNull();
            }
        }

        void ITypedBulkCopyBoundColumn<TimeSpan?>.SetValue(TimeSpan? value)
        {
            if (value.HasValue)
            {
                SetValue(string.Format(Culture, _format, value.Value));
            }
            else
            {
                SetNull();
            }
        }

        private void SetValue(string value)
        {
            var length = value.Length;

            if (IndicatorLength > 0)
            {
                // NB all formats containing milliseconds will be variable length as trailing zeros are not included.

                SetLength(length);
            }

            _encoding.GetBytes(value, 0, length, Data, IndicatorLength);
        }
    }
}
