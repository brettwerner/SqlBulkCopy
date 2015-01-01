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
    internal sealed class DecimalColumn : StandardDataType, IStandardColumn<decimal>, IStandardColumn<decimal?>
    {
        private const int DataTypeLengthInBytes = 19;

        private DecimalColumn(short index, BulkCopyDataType dataType)
            : base(index, dataType, DataTypeLengthInBytes)
        {
            //
        }

        public static IStandardColumn<decimal> CreateNonNullableInstance(short index)
        {
            return new DecimalColumn(index, BulkCopyDataType.Decimal);
        }

        public static IStandardColumn<decimal?> CreateNullableInstance(short index)
        {
            return new DecimalColumn(index, BulkCopyDataType.NullableDecimal);
        }

        void ITypedBulkCopyBoundColumn<decimal>.SetValue(decimal value)
        {
            SetValue(value);
        }

        private void SetValue(decimal value)
        {
            var bits = decimal.GetBits(value);

            /*
             * Input:
             * 
             * 4 ints, 96-bit integer part of the decimal is contained in the initial 3 elements.
             * 
             * element 0: low 32 bits
             * element 1: middle 32 bits
             * element 2: high 32 bits
             * element 3: contains scale (8 bits from 16-23 containing a value between 0 and 28) and sign (bit 31)
             * 
             * Output:
             * 
             * 1 byte: precision (always 38)
             * 1 byte: scale
             * 1 byte: sign
             * 4 bytes: low 32 bits
             * 4 bytes: middle 32 bits
             * 4 bytes: high 32 bits
             * 4 bytes: unused
            */

            const byte negative = 0;
            const byte positive = 1;

            CopyByte(0, 38);
            CopyByte(1, DecimalFunctions.GetScale(bits[3]));
            CopyByte(2, DecimalFunctions.IsNegative(bits[3]) ? negative : positive);
            CopyInt32(3, bits[0]);
            CopyInt32(7, bits[1]);
            CopyInt32(11, bits[2]);
        }

        void ITypedBulkCopyBoundColumn<decimal?>.SetValue(decimal? value)
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
