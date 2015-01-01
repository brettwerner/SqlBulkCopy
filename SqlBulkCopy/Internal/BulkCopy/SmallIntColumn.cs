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
    internal sealed class SmallIntColumn : StandardDataType, IStandardColumn<short>, IStandardColumn<short?>
    {
        private const int DataTypeLengthInBytes = 2;

        private SmallIntColumn(short index, BulkCopyDataType dataType)
            : base(index, dataType, DataTypeLengthInBytes)
        {
            //
        }

        public static IStandardColumn<short> CreateNonNullableInstance(short index)
        {
            return new SmallIntColumn(index, BulkCopyDataType.SmallInt);
        }

        public static IStandardColumn<short?> CreateNullableInstance(short index)
        {
            return new SmallIntColumn(index, BulkCopyDataType.NullableSmallInt);
        }

        void ITypedBulkCopyBoundColumn<short>.SetValue(short value)
        {
            CopyInt16(0, value);
        }

        void ITypedBulkCopyBoundColumn<short?>.SetValue(short? value)
        {
            if (value.HasValue)
            {
                SetLength(DataTypeLengthInBytes);
                CopyInt16(0, value.Value);
            }
            else
            {
                SetNull();
            }
        }
    }
}
