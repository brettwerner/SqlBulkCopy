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
using System.Text;

namespace SqlBulkCopy.Internal.BulkCopy
{
    internal sealed class CharColumn : StandardDataType, IStandardColumn<string>
    {
        private readonly Encoding _encoding;

        private CharColumn(short index, BulkCopyDataType dataType, short length, Encoding encoding, BindingFlags options)
            : base(index, dataType, length, options)
        {
            _encoding = encoding;
        }

        public static CharColumn CreateInstance(short index, BulkCopyDataType dataType, short length)
        {
            switch (dataType)
            {
                case BulkCopyDataType.Char:
                    return new CharColumn(index, dataType, length, Encoding.ASCII, BindingFlags.VariableLengthIn);
                case BulkCopyDataType.VarChar:
                    return new CharColumn(index, dataType, length, Encoding.ASCII, BindingFlags.VariableLengthIn | BindingFlags.VariableLengthOut);
                case BulkCopyDataType.NullableChar:
                    return new CharColumn(index, dataType, length, Encoding.ASCII, BindingFlags.Nullable | BindingFlags.VariableLengthIn);
                case BulkCopyDataType.NullableVarChar:
                    return new CharColumn(index, dataType, length, Encoding.ASCII, BindingFlags.Nullable | BindingFlags.VariableLengthIn | BindingFlags.VariableLengthOut);
                case BulkCopyDataType.NChar:
                    return new CharColumn(index, dataType, length, Encoding.Unicode, BindingFlags.DoubleByteCharacterSet | BindingFlags.VariableLengthIn);
                case BulkCopyDataType.NVarChar:
                    return new CharColumn(index, dataType, length, Encoding.Unicode, BindingFlags.DoubleByteCharacterSet | BindingFlags.VariableLengthIn | BindingFlags.VariableLengthOut);
                case BulkCopyDataType.NullableNChar:
                    return new CharColumn(index, dataType, length, Encoding.Unicode, BindingFlags.DoubleByteCharacterSet | BindingFlags.Nullable | BindingFlags.VariableLengthIn);
                case BulkCopyDataType.NullableNVarChar:
                    return new CharColumn(index, dataType, length, Encoding.Unicode, BindingFlags.DoubleByteCharacterSet | BindingFlags.Nullable | BindingFlags.VariableLengthIn | BindingFlags.VariableLengthOut);
                default:
                    throw new ArgumentOutOfRangeException("dataType");
            }
        }

        void ITypedBulkCopyBoundColumn<string>.SetValue(string value)
        {
            int length;

            if (value == null || (length = value.Length) == 0)
            {
                SetNull();
            }
            else
            {
                var byteCount = _encoding.GetByteCount(value);

                if (byteCount > ByteLength)
                {
                    throw new ArgumentOutOfRangeException("value", string.Format("Value length exceeds column length. Index: {0}.", Index));
                }

                SetLength(byteCount);
                _encoding.GetBytes(value, 0, length, Data, IndicatorLength);
            }
        }
    }
}
