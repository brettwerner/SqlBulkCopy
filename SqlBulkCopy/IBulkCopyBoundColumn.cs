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

namespace SqlBulkCopy
{
    /// <summary>
    /// Defines the properties supported by a bound bulk copy column. Data is transferred to SQL Server using program variables.
    /// </summary>
    public interface IBulkCopyBoundColumn : IBulkCopyColumn
    {
        /// <summary>
        /// The length of the column in bytes. For Double-byte character data (<see cref="BulkCopyDataType.NChar"/>, <see cref="BulkCopyDataType.NVarChar"/> and <see cref="BulkCopyDataType.NVarCharMax"/>) the value of this property will be (<see cref="Length"/> * 2).
        /// </summary>
        int ByteLength { get; }

        /// <summary>
        /// Returns <c>true</c> for nullable columns.
        /// </summary>
        bool IsNullable { get; }

        /// <summary>
        /// Returns <c>true</c> for variable length data types: <see cref="BulkCopyDataType.VarBinary"/>, <see cref="BulkCopyDataType.VarBinaryMax"/>, <see cref="BulkCopyDataType.VarChar"/>, <see cref="BulkCopyDataType.VarCharMax"/>, <see cref="BulkCopyDataType.NVarChar"/> and <see cref="BulkCopyDataType.NVarCharMax"/>.
        /// </summary>
        bool IsVariableLength { get; }

        /// <summary>
        /// The length of the column as defined in SQL Server.
        /// </summary>
        /// <seealso cref="ByteLength"/>
        int Length { get; }
    }
}
