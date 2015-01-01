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

namespace SqlBulkCopy
{
    /// <summary>
    /// Provides a template for a standard implementation of a bulk copy importer.
    /// </summary>
    /// <typeparam name="TRowData"></typeparam>
    public abstract class BulkCopyImporter<TRowData>
    {
        private readonly int _batchSize;
        private readonly IBulkCopy _bulkCopy;
        private int _pending;

        /// <summary>
        /// Initializes the instance.
        /// </summary>
        /// <param name="bulkCopy"></param>
        /// <param name="batchSize"></param>
        protected BulkCopyImporter(IBulkCopy bulkCopy, int batchSize)
        {
            if (bulkCopy == null) throw new ArgumentNullException("bulkCopy");

            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException("batchSize", "The argument must be greater than zero.");
            }

            _bulkCopy = bulkCopy;
            _batchSize = batchSize;
        }

        /// <summary>
        /// The implementation should use this method to set column values for the row.
        /// </summary>
        /// <param name="data">An object containing row data.</param>
        protected abstract void SetRowValues(TRowData data);

        /// <summary>
        /// Sends a row of data from program variables to SQL Server. 
        /// </summary>
        /// <returns><c>true</c> indicates success.</returns>
        public bool SendRow(TRowData data)
        {
            if (data == null) throw new ArgumentNullException("data");

            SetRowValues(data);

            _pending++;

            if (_pending < _batchSize)
            {
                return _bulkCopy.SendRow();
            }

            if (!_bulkCopy.SendRow())
            {
                return false;
            }

            if (_bulkCopy.Batch() != _pending)
            {
                return false;
            }

            _pending = 0;

            return true;
        }

        /// <summary>
        /// Ends a bulk copy transfer.
        /// </summary>
        /// <returns><c>true</c> indicates success.</returns>
        public virtual bool Done()
        {
            if (_bulkCopy.Done() != _pending)
            {
                return false;
            }

            _pending = 0;

            return true;
        }
    }
}
