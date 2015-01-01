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
    internal sealed class VarBinaryMaxColumn : MaxDataType, IMaxColumn<byte[]>
    {
        private VarBinaryMaxColumn(short index, BulkCopyDataType dataType, BindingFlags options)
            : base(index, dataType, options)
        {
            //
        }

        public static VarBinaryMaxColumn CreateInstance(short index, BulkCopyDataType dataType)
        {
            switch (dataType)
            {
                case BulkCopyDataType.VarBinaryMax:
                    return new VarBinaryMaxColumn(index, dataType, BindingFlags.VariableLengthIn | BindingFlags.VariableLengthOut);
                case BulkCopyDataType.NullableVarBinaryMax:
                    return new VarBinaryMaxColumn(index, dataType, BindingFlags.Nullable | BindingFlags.VariableLengthIn | BindingFlags.VariableLengthOut);
                default:
                    throw new ArgumentOutOfRangeException("dataType");
            }
        }

        void ITypedBulkCopyBoundColumn<byte[]>.SetValue(byte[] value)
        {
            Data = value;
        }
    }
}
