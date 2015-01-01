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
    internal sealed class IntColumn : StandardDataType, IStandardColumn<int>, IStandardColumn<int?>
    {
        private const int DataTypeLengthInBytes = 4;

        private IntColumn(short index, BulkCopyDataType dataType)
            : base(index, dataType, DataTypeLengthInBytes)
        {
            //
        }

        public static IStandardColumn<int> CreateNonNullableInstance(short index)
        {
            return new IntColumn(index, BulkCopyDataType.Int);
        }

        public static IStandardColumn<int?> CreateNullableInstance(short index)
        {
            return new IntColumn(index, BulkCopyDataType.NullableInt);
        }

        void ITypedBulkCopyBoundColumn<int>.SetValue(int value)
        {
            CopyInt32(0, value);
        }

        void ITypedBulkCopyBoundColumn<int?>.SetValue(int? value)
        {
            if (value.HasValue)
            {
                SetLength(DataTypeLengthInBytes);
                CopyInt32(0, value.Value);
            }
            else
            {
                SetNull();
            }
        }
    }
}
