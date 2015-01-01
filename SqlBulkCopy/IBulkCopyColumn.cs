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
    /// Defines the properties supported by all bulk copy columns.
    /// </summary>
    /// <seealso cref="IBulkCopyBoundColumn"/>
    public interface IBulkCopyColumn : IDisposable
    {
        /// <summary>
        /// The data type of the column in SQL Server.
        /// </summary>
        BulkCopyDataType DataType { get; }

        /// <summary>
        /// Text description for the <see cref="DataType"/>.
        /// </summary>
        string DataTypeDescription { get; }

        /// <summary>
        /// Ordinal position of the column. Zero bound.
        /// </summary>
        short Index { get; }

        /// <summary>
        /// Returns <c>true</c> when the column is bound.
        /// </summary>
        bool IsBound { get; }
    }
}
