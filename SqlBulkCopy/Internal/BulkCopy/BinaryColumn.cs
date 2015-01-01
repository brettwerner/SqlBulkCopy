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
    internal sealed class BinaryColumn : StandardDataType, IStandardColumn<byte[]>
    {
        private BinaryColumn(short index, BulkCopyDataType dataType, short length, BindingFlags options)
            : base(index, dataType, length, options)
        {
            //
        }

        public static BinaryColumn CreateInstance(short index, BulkCopyDataType dataType, short length)
        {
            switch (dataType)
            {
                case BulkCopyDataType.Binary:
                    return new BinaryColumn(index, dataType, length, BindingFlags.VariableLengthIn);
                case BulkCopyDataType.VarBinary:
                    return new BinaryColumn(index, dataType, length, BindingFlags.VariableLengthIn | BindingFlags.VariableLengthOut);
                case BulkCopyDataType.NullableBinary:
                    return new BinaryColumn(index, dataType, length, BindingFlags.Nullable | BindingFlags.VariableLengthIn);
                case BulkCopyDataType.NullableVarBinary:
                    return new BinaryColumn(index, dataType, length, BindingFlags.Nullable | BindingFlags.VariableLengthIn | BindingFlags.VariableLengthOut);
                default:
                    throw new ArgumentOutOfRangeException("dataType");
            }
        }

        void ITypedBulkCopyBoundColumn<byte[]>.SetValue(byte[] value)
        {
            int length;

            if (value == null || (length = value.Length) == 0)
            {
                SetNull();
            }
            else
            {
                if (length > ByteLength)
                {
                    throw new ArgumentOutOfRangeException("value", string.Format("Value length exceeds column length. Index: {0}.", Index));
                }

                CopyFrom(value, length);
            }
        }
    }
}
