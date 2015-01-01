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

namespace SqlBulkCopy.Internal.BulkCopy
{
    internal sealed class BitColumn : StandardDataType, IStandardColumn<bool>, IStandardColumn<bool?>
    {
        private const int DataTypeLengthInBytes = 1;

        private const byte ByteFalse = 0;
        private const byte ByteTrue = 1;

        private BitColumn(short index, BulkCopyDataType dataType)
            : base(index, dataType, DataTypeLengthInBytes)
        {   
            //
        }

        public static IStandardColumn<bool> CreateNonNullableInstance(short index)
        {
            return new BitColumn(index, BulkCopyDataType.Bit);
        }

        public static IStandardColumn<bool?> CreateNullableInstance(short index)
        {
            return new BitColumn(index, BulkCopyDataType.NullableBit);
        }

        void ITypedBulkCopyBoundColumn<bool>.SetValue(bool value)
        {
            CopyByte(0, value ? ByteTrue : ByteFalse);
        }

        void ITypedBulkCopyBoundColumn<bool?>.SetValue(bool? value)
        {
            if (value.HasValue)
            {
                SetLength(DataTypeLengthInBytes);
                CopyByte(0, value.Value ? ByteTrue : ByteFalse);
            }
            else
            {
                SetNull();
            }
        }
    }
}
