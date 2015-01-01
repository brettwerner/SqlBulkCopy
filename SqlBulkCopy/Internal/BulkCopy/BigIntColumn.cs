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
    internal sealed class BigIntColumn : StandardDataType, IStandardColumn<long>, IStandardColumn<long?>
    {
        private const int DataTypeLengthInBytes = 8;

        private BigIntColumn(short index, BulkCopyDataType dataType)
            : base(index, dataType, DataTypeLengthInBytes)
        {
            //
        }

        public static IStandardColumn<long> CreateNonNullableInstance(short index)
        {
            return new BigIntColumn(index, BulkCopyDataType.BigInt);
        }

        public static IStandardColumn<long?> CreateNullableInstance(short index)
        {
            return new BigIntColumn(index, BulkCopyDataType.NullableBigInt);
        }

        void ITypedBulkCopyBoundColumn<long>.SetValue(long value)
        {
            CopyInt64(0, value);
        }

        void ITypedBulkCopyBoundColumn<long?>.SetValue(long? value)
        {
            if (value.HasValue)
            {
                SetLength(DataTypeLengthInBytes);
                CopyInt64(0, value.Value);
            }
            else
            {
                SetNull();
            }
        }
    }
}
