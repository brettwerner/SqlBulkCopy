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
    internal sealed class MoneyColumn : StandardDataType, IStandardColumn<decimal>, IStandardColumn<decimal?>
    {
        private const int MoneyDataTypeLengthInBytes = 8;
        private const int SmallMoneyDataTypeLengthInBytes = 4;

        private MoneyColumn(short index, BulkCopyDataType dataType, int length)
            : base(index, dataType, length)
        {
            //
        }

        public static IStandardColumn<decimal> CreateNonNullableMoneyInstance(short index)
        {
            return new MoneyColumn(index, BulkCopyDataType.Money, MoneyDataTypeLengthInBytes);
        }

        public static IStandardColumn<decimal?> CreateNullableMoneyInstance(short index)
        {
            return new MoneyColumn(index, BulkCopyDataType.NullableMoney, MoneyDataTypeLengthInBytes);
        }

        public static IStandardColumn<decimal> CreateNonNullableSmallMoneyInstance(short index)
        {
            return new MoneyColumn(index, BulkCopyDataType.SmallMoney, SmallMoneyDataTypeLengthInBytes);
        }

        public static IStandardColumn<decimal?> CreateNullableSmallMoneyInstance(short index)
        {
            return new MoneyColumn(index, BulkCopyDataType.NullableSmallMoney, SmallMoneyDataTypeLengthInBytes);
        }

        void ITypedBulkCopyBoundColumn<decimal>.SetValue(decimal value)
        {
            SetValue(value);
        }

        private void SetValue(decimal value)
        {
            const decimal minMoney = -922337203685477.5808M;
            const decimal maxMoney = 922337203685477.5807M;

            const decimal minSmallMoney = -214748.3648M;
            const decimal maxSmallMoney = 214748.3647M;

            if (Length == SmallMoneyDataTypeLengthInBytes)
            {
                if ((value < minSmallMoney) || (value > maxSmallMoney))
                {
                    throw new ArgumentOutOfRangeException("value", "SmallMoney value must be between -214748.3648M and 214748.3647M.");
                }
            }
            else
            {
                if ((value < minMoney) || (value > maxMoney))
                {
                    throw new ArgumentOutOfRangeException("value", "Money value must be between -922337203685477.5808M and 922337203685477.5807M.");
                }
            }

            // Output scale must be 4.

            const int moneyScale = 4;

            var bits = DecimalFunctions.AdjustScale(value, moneyScale);
            var output = ((long)(uint)bits[1]) << 32 | (uint)bits[0];

            if (DecimalFunctions.IsNegative(bits[3]))
            {
                output = -output;
            }

            if (Length == SmallMoneyDataTypeLengthInBytes)
            {
                CopyInt32(0, (int)output);
            }
            else
            {
                CopyInt32(0, (int)(output >> 32));
                CopyInt32(4, (int)output);
            }
        }

        void ITypedBulkCopyBoundColumn<decimal?>.SetValue(decimal? value)
        {
            if (value.HasValue)
            {
                SetLength(Length);
                SetValue(value.Value);
            }
            else
            {
                SetNull();
            }
        }
    }
}
