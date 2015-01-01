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
    internal sealed class VarCharMaxColumn : MaxDataType, IMaxColumn<string>
    {
        private readonly Encoding _encoding;

        private VarCharMaxColumn(short index, BulkCopyDataType dataType, Encoding encoding, BindingFlags options)
            : base(index, dataType, options)
        {
            _encoding = encoding;
        }

        public static VarCharMaxColumn CreateInstance(short index, BulkCopyDataType dataType)
        {
            switch (dataType)
            {
                case BulkCopyDataType.VarCharMax:
                    return new VarCharMaxColumn(index, dataType, Encoding.ASCII, BindingFlags.VariableLengthIn | BindingFlags.VariableLengthOut);
                case BulkCopyDataType.NullableVarCharMax:
                    return new VarCharMaxColumn(index, dataType, Encoding.ASCII, BindingFlags.Nullable | BindingFlags.VariableLengthIn | BindingFlags.VariableLengthOut);
                case BulkCopyDataType.NVarCharMax:
                    return new VarCharMaxColumn(index, dataType, Encoding.Unicode, BindingFlags.DoubleByteCharacterSet | BindingFlags.VariableLengthIn | BindingFlags.VariableLengthOut);
                case BulkCopyDataType.NullableNVarCharMax:
                    return new VarCharMaxColumn(index, dataType, Encoding.Unicode, BindingFlags.DoubleByteCharacterSet | BindingFlags.Nullable | BindingFlags.VariableLengthIn | BindingFlags.VariableLengthOut);
                default:
                    throw new ArgumentOutOfRangeException("dataType");
            }
        }

        void ITypedBulkCopyBoundColumn<string>.SetValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Data = null;
            }
            else
            {
                var bytes = _encoding.GetBytes(value);

                Data = bytes;
            }
        }
    }
}
