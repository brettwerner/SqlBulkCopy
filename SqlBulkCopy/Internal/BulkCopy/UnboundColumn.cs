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
using SqlBulkCopy.Internal.Common;

namespace SqlBulkCopy.Internal.BulkCopy
{
    internal sealed class UnboundColumn : IBulkCopyColumn
    {
        private readonly BulkCopyDataType _dataType;
        private readonly short _index;

        internal UnboundColumn(short index, BulkCopyDataType dataType)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", index, "The argument must be >= 0.");
            }

            _index = index;
            _dataType = dataType;
        }

        void IDisposable.Dispose()
        {
            //
        }

        public BulkCopyDataType DataType
        {
            get { return _dataType; }
        }

        public string DataTypeDescription
        {
            get { return Resources.GetDataTypeDescription(_dataType); }
        }

        public short Index
        {
            get { return _index; }
        }

        public bool IsBound
        {
            get { return false; }
        }
    }
}
