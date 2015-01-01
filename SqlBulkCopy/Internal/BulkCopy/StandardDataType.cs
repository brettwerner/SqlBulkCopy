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
using System.Runtime.InteropServices;
using SqlBulkCopy.Internal.Common;

namespace SqlBulkCopy.Internal.BulkCopy
{
    internal unsafe abstract class StandardDataType : IStandardDataType
    {
        private const int IndicatorLengthInBytes = 4;

        private readonly int _byteLength;
        private readonly byte[] _data;
        private readonly BulkCopyDataType _dataType;
        private GCHandle _handle;
        private readonly short _index;
        private readonly byte _indicatorLength;
        private readonly BindingFlags _options;
        private readonly byte* _pointer;

        #region ctor -> Finalize

        protected StandardDataType(short index, BulkCopyDataType dataType, int length, BindingFlags options = BindingFlags.None)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", index, "The argument must be >= 0.");
            }

            int byteLength;

            if ((options & BindingFlags.DoubleByteCharacterSet) == 0)
            {
                if (length < 1 || length > Constants.SqlMaximumByteLength)
                {
                    throw new ArgumentOutOfRangeException("length", length, "The argument must be between 1 and 8000.");
                }

                byteLength = length;
            }
            else
            {
                if (length < 1 || length > Constants.SqlMaximumDoubleByteCharacterSetByteLength)
                {
                    throw new ArgumentOutOfRangeException("length", length, "The argument must be between 1 and 4000.");
                }

                byteLength = length * 2;
            }

            if (dataType >= BulkCopyDataType.NullableBit)
            {
                options |= BindingFlags.Nullable;
            }

            if ((options & (BindingFlags.Nullable | BindingFlags.VariableLengthIn | BindingFlags.VariableLengthOut)) != 0)
            {
                _indicatorLength = IndicatorLengthInBytes;
            }

            _byteLength = byteLength;
            _data = new byte[_indicatorLength + byteLength];
            _dataType = dataType;
            _handle = GCHandle.Alloc(_data, GCHandleType.Pinned);
            _index = index;
            _options = options;
            _pointer = (byte*)_handle.AddrOfPinnedObject();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~StandardDataType()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_handle.IsAllocated)
            {
                _handle.Free();
            }
        }

        #endregion

        #region properties

        public int ByteLength
        {
            get { return _byteLength; }
        }

        public byte[] Data
        {
            get { return _data; }
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

        public byte IndicatorLength
        {
            get { return _indicatorLength; }
        }

        public bool IsBound
        {
            get { return true; }
        }

        public bool IsNullable
        {
            get { return ((_options & BindingFlags.Nullable) != 0); }
        }

        public bool IsVariableLength
        {
            get { return ((_options & BindingFlags.VariableLengthOut) != 0); }
        }

        public int Length
        {
            get
            {
                if ((_options & BindingFlags.DoubleByteCharacterSet) != 0)
                {
                    return _byteLength/2;
                }

                return _byteLength;
            }
        }

        public BindingFlags Options
        {
            get { return _options; }
        }

        public byte* Pointer
        {
            get { return _pointer; }
        }

        public bool RequiresConversion
        {
            get { return ((_options & BindingFlags.RequiresConversion) != 0); }
        }

        #endregion

        #region methods

        protected void CopyByte(int index, byte value)
        {
            if (index < 0 || index > _byteLength - 1)
            {
                throw new ArgumentOutOfRangeException("index", index, "Index out of bounds.");
            }

            unchecked
            {
                _data[_indicatorLength + index] = value;
            }
        }

        protected void CopyInt16(int index, short value)
        {
            if (index < 0 || index > _byteLength - 2)
            {
                throw new ArgumentOutOfRangeException("index", index, "Index out of bounds.");
            }

            unchecked
            {
                *((short*)(Pointer + IndicatorLength + index)) = value;
            }
        }

        protected void CopyInt32(int index, int value)
        {
            if (index < 0 || index > _byteLength - 4)
            {
                throw new ArgumentOutOfRangeException("index", index, "Index out of bounds.");
            }

            unchecked
            {
                *((int*)(Pointer + IndicatorLength + index)) = value;
            }
        }

        protected void CopyInt64(int index, long value)
        {
            if (index < 0 || index > _byteLength - 8)
            {
                throw new ArgumentOutOfRangeException("index", index, "Index out of bounds.");
            }

            unchecked
            {
                *((long*)(Pointer + IndicatorLength + index)) = value;
            }
        }

        protected void CopyFrom(byte[] bytes, int length)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");

            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("length", length, "The argument must be greater than zero.");
            }

            if (length > _byteLength)
            {
                throw new ArgumentOutOfRangeException("length", length, "Value exceeds column length.");
            }

            if (_indicatorLength > 0)
            {
                SetIndicator(length);
            }

            Buffer.BlockCopy(bytes, 0, _data, _indicatorLength, length);
        }

        protected void SetLength(int length)
        {
            if (_indicatorLength == 0)
            {
                throw new InvalidOperationException("The column is neither nullable nor variable length.");
            }

            if (length < 0 || length > _byteLength)
            {
                throw new ArgumentOutOfRangeException("length", length, string.Format("The argument must be between 1 and {0}.", _byteLength));
            }

            SetIndicator(length);
        }

        protected void SetNull()
        {
            if (_indicatorLength == 0)
            {
                throw new InvalidOperationException("The column is neither nullable nor variable length.");
            }

            SetIndicator(IsNullable ? Constants.SqlNullData : 0);
        }

        private void SetIndicator(int value)
        {
            *((int*)(Pointer)) = value;
        }

        #endregion
    }
}
