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
    internal static class DecimalFunctions
    {
        private const int ScaleMask = 0x00FF0000;
        private const uint SignMask = 0x80000000;

        internal static int[] AdjustScale(decimal value, byte requiredScale)
        {
            value = decimal.Round(value, requiredScale);

            var bits = decimal.GetBits(value);
            var scale = GetScale(bits[3]);

            if (scale == requiredScale) return bits;

            unchecked
            {
                for (var i = scale; i < requiredScale; i++)
                {
                    value = value * 1.0M;
                }
            }

            return decimal.GetBits(value);
        }

        internal static byte GetScale(int signScale)
        {
            return (byte)((signScale & ScaleMask) >> 16);
        }

        internal static bool IsNegative(int signScale)
        {
            return ((signScale & SignMask) == SignMask);
        }
    }
}
