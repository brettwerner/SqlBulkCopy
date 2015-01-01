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

using SqlBulkCopy.Internal.Common;

namespace SqlBulkCopy
{
    /// <summary>
    /// Application definition of the supported SQL Server data types.
    /// </summary>
    public enum BulkCopyDataType
    {
        /// <summary>
        /// SQL Server <c>bit</c> data type. Maps to <see cref="System.Boolean"/>. For a nullable bit column, use <see cref="NullableBit"/>.
        /// </summary>
        Bit,

        /// <summary>
        /// SQL Server <c>tinyint</c> data type. Maps to <see cref="System.Byte"/>. For a nullable tinyint column, use <see cref="NullableTinyInt"/>.
        /// <para>Valid values range from 0 to 255.</para>
        /// </summary>
        TinyInt,

        /// <summary>
        /// SQL Server <c>smallint</c> data type. Maps to <see cref="System.Int16"/>. For a nullable smallint column, use <see cref="NullableSmallInt"/>.
        /// <para>Valid values range from -2^15 (-32,768) to 2^15-1 (32,767).</para>
        /// </summary>
        SmallInt,

        /// <summary>
        /// SQL Server <c>int</c> data type. Maps to <see cref="System.Int32"/>. For a nullable int column, use <see cref="NullableInt"/>.
        /// <para>Valid values range from -2^31 (-2,147,483,648) to 2^31-1 (2,147,483,647).</para>
        /// </summary>
        Int,

        /// <summary>
        /// SQL Server <c>bigint</c> data type. Maps to <see cref="System.Int64"/>. For a nullable bigint column, use <see cref="NullableBigInt"/>.
        /// <para>Valid values range from -2^63 (-9,223,372,036,854,775,808) to 2^63-1 (9,223,372,036,854,775,807).</para>
        /// </summary>
        BigInt,

        /// <summary>
        /// SQL Server <c>binary</c> data type. Maps to a 1-dimensional <see cref="System.Byte"/> array. For a nullable binary column, use <see cref="NullableBinary"/>.
        /// <para>The maximum length for this data type is 8000 bytes.</para>
        /// </summary>
        Binary,

        /// <summary>
        /// SQL Server <c>varbinary</c> data type. Maps to a 1-dimensional <see cref="System.Byte"/> array. For a nullable varbinary column, use <see cref="NullableVarBinary"/>.
        /// <para>The maximum length for this data type is 8000 bytes.</para>
        /// <para>The ANSI SQL synonym for varbinary is <c>binary varying</c>.</para>
        /// </summary>
        VarBinary,

        /// <summary>
        /// SQL Server <c>varbinary(max)</c> data type. Maps to a 1-dimensional <see cref="System.Byte"/> array. For a nullable varbinary(max) column, use <see cref="NullableVarBinaryMax"/>.
        /// <para>The maximum length for this data type is (2^31 - 1) bytes.</para>
        /// <para>Use for the SQL Server <c>image</c> data type also.</para>
        /// </summary>
        VarBinaryMax,

        /// <summary>
        /// SQL Server <c>char</c> data type. Maps to <see cref="System.String"/>. For a nullable char column, use <see cref="NullableChar"/>.
        /// <para>The maximum length for this data type is 8000 non-Unicode characters.</para>
        /// <para>The ISO synonym for char is <c>character</c>.</para>
        /// </summary>
        Char,

        /// <summary>
        /// SQL Server <c>nchar</c> data type. Maps to <see cref="System.String"/>. For a nullable nchar column, use <see cref="NullableNChar"/>.
        /// <para>The maximum length for this data type is 4000 Unicode characters.</para>
        /// <para>ISO synonyms for nchar are <c>national char</c> and <c>national character</c>.</para>
        /// </summary>
        NChar,

        /// <summary>
        /// SQL Server <c>varchar</c> data type. Maps to <see cref="System.String"/>. For a nullable varchar column, use <see cref="NullableVarChar"/>.
        /// <para>The maximum length for this data type is 8000 non-Unicode characters.</para>
        /// <para>ISO synonyms for varchar are <c>char varying</c> and <c>character varying</c>.</para>
        /// </summary>
        VarChar,

        /// <summary>
        /// SQL Server <c>varchar(max)</c> data type. Maps to <see cref="System.String"/>. For a nullable varchar(max) column, use <see cref="NullableVarCharMax"/>.
        /// <para>The maximum length for this data type is (2^31 - 1) non-Unicode characters.</para>
        /// <para>ISO synonyms for varchar(max) are <c>char varying</c> and <c>character varying</c>.</para>
        /// <para>Use for the SQL Server <c>text</c> data type also.</para>
        /// </summary>
        VarCharMax,

        /// <summary>
        /// SQL Server <c>nvarchar</c> data type. Maps to <see cref="System.String"/>. For a nullable nvarchar column, use <see cref="NullableNVarChar"/>.
        /// <para>The maximum length for this data type is 4000 Unicode characters.</para>
        /// <para>ISO synonyms for nvarchar are <c>national char varying</c> and <c>national character varying</c>.</para>
        /// </summary>
        NVarChar,

        /// <summary>
        /// SQL Server <c>nvarchar(max)</c> data type. Maps to <see cref="System.String"/>. For a nullable nvarchar(max) column, use <see cref="NullableNVarCharMax"/>.
        /// <para>The maximum length for this data type is ((2^31 - 1) / 2) Unicode characters.</para>
        /// <para>ISO synonyms for nvarchar(max) are <c>national char varying</c> and <c>national character varying</c>.</para>
        /// <para>Use for the SQL Server <c>ntext</c> data type also.</para>
        /// </summary>
        NVarCharMax,

        /// <summary>
        /// SQL Server <c>date</c> data type. Maps to <see cref="System.DateTime"/>. For a nullable date column, use <see cref="NullableDate"/>.
        /// <para>Valid for years between 0001 and 9999.</para>
        /// <para>The SQL Server <c>date</c> data type was introduced in 2008.</para>
        /// </summary>
        Date,

        /// <summary>
        /// SQL Server <c>datetime</c> data type. Maps to <see cref="System.DateTime"/>. For a nullable datetime column, use <see cref="NullableDateTime"/>.
        /// <para>Valid for years between 1753 and 9999.</para>
        /// </summary>
        DateTime,

        /// <summary>
        /// SQL Server <c>smalldatetime</c> data type. Maps to <see cref="System.DateTime"/>. For a nullable smalldatetime column, use <see cref="NullableSmallDateTime"/>.
        /// <para>Valid for dates between 1900-01-01 and 2079-06-06.</para>
        /// </summary>
        SmallDateTime,

        /// <summary>
        /// SQL Server <c>datetime2</c> data type. Maps to <see cref="System.DateTime"/>. For a nullable datetime2 column, use <see cref="NullableDateTime2"/>.
        /// <para>Valid for years between 0001 and 9999.</para>
        /// <para>The SQL Server <c>datetime2</c> data type was introduced in 2008.</para>
        /// </summary>
        DateTime2,

        /// <summary>
        /// SQL Server <c>datetimeoffset</c> data type. Maps to <see cref="System.DateTimeOffset"/>. For a nullable datetimeoffset column, use <see cref="NullableDateTimeOffset"/>.
        /// <para>Valid for years between 0001 and 9999.</para>
        /// <para>The SQL Server <c>datetimeoffset</c> data type was introduced in 2008.</para>
        /// </summary>
        DateTimeOffset,

        /// <summary>
        /// SQL Server <c>time</c> data type. Maps to <see cref="System.DateTime"/>. For a nullable time column, use <see cref="NullableTime"/>.
        /// <para>The SQL Server <c>time</c> data type was introduced in 2008.</para>
        /// </summary>
        Time,

        /// <summary>
        /// SQL Server <c>decimal</c> data type. Maps to <see cref="System.Decimal"/>. For a nullable decimal column, use <see cref="NullableDecimal"/>.
        /// <para>Valid values range from - 10^38 + 1 through 10^38 - 1.</para>
        /// <para>The ISO synonyms for decimal are dec and dec(p, s). Use for the SQL Server <c>numeric</c> data type also.</para>
        /// </summary>
        Decimal,

        /// <summary>
        /// SQL Server <c>money</c> data type. Maps to <see cref="System.Decimal"/>. For a nullable money column, use <see cref="NullableMoney"/>.
        /// <para>Valid values range from -922,337,203,685,477.5808 to 922,337,203,685,477.5807.</para>
        /// </summary>
        Money,

        /// <summary>
        /// SQL Server <c>smallmoney</c> data type. Maps to <see cref="System.Decimal"/>. For a nullable smallmoney column, use <see cref="NullableSmallMoney"/>.
        /// <para>Valid values range from - 214,748.3648 to 214,748.3647.</para>
        /// </summary>
        SmallMoney,

        /// <summary>
        /// SQL Server <c>float</c> data type. Maps to <see cref="System.Double"/>. For a nullable float column, use <see cref="NullableFloat"/>.
        /// <para>Valid values range from - 1.79E+308 to -2.23E-308, 0 and 2.23E-308 to 1.79E+308.</para>
        /// </summary>
        Float,

        /// <summary>
        /// SQL Server <c>real</c> data type. Maps to <see cref="System.Single"/>. For a nullable real column, use <see cref="NullableReal"/>.
        /// <para>Valid values range from - 3.40E + 38 to -1.18E - 38, 0 and 1.18E - 38 to 3.40E + 38.</para>
        /// </summary>
        Real,

        /// <summary>
        /// SQL Server <c>uniqueidentifier</c> data type. Maps to <see cref="System.Guid"/>. For a nullable uniqueidentifier column, use <see cref="NullableUniqueIdentifier"/>.
        /// </summary>
        UniqueIdentifier,

        /// <summary>
        /// SQL Server <c>bit</c> data type. Maps to <see cref="System.Nullable{Boolean}"/>. For a non-nullable bit column, use <see cref="Bit"/>.
        /// </summary>
        NullableBit = Constants.NullableDataType | Bit,

        /// <summary>
        /// SQL Server <c>tinyint</c> data type. Maps to <see cref="System.Nullable{Byte}"/>. For a non-nullable tinyint column, use <see cref="TinyInt"/>.
        /// <para>Valid values range from 0 to 255.</para>
        /// </summary>
        NullableTinyInt = Constants.NullableDataType | TinyInt,

        /// <summary>
        /// SQL Server <c>smallint</c> data type. Maps to <see cref="System.Nullable{Int16}"/>. For a non-nullable smallint column, use <see cref="SmallInt"/>.
        /// <para>Valid values range from -2^15 (-32,768) to 2^15-1 (32,767).</para>
        /// </summary>
        NullableSmallInt = Constants.NullableDataType | SmallInt,

        /// <summary>
        /// SQL Server <c>int</c> data type. Maps to <see cref="System.Nullable{Int32}"/>. For a non-nullable int column, use <see cref="Int"/>.
        /// <para>Valid values range from -2^31 (-2,147,483,648) to 2^31-1 (2,147,483,647).</para>
        /// </summary>
        NullableInt = Constants.NullableDataType | Int,

        /// <summary>
        /// SQL Server <c>bigint</c> data type. Maps to <see cref="System.Nullable{Int64}"/>. For a non-nullable bigint column, use <see cref="BigInt"/>.
        /// <para>Valid values range from -2^63 (-9,223,372,036,854,775,808) to 2^63-1 (9,223,372,036,854,775,807).</para>
        /// </summary>
        NullableBigInt = Constants.NullableDataType | BigInt,

        /// <summary>
        /// SQL Server <c>binary</c> data type. Maps to a 1-dimensional <see cref="System.Byte"/> array. For a non-nullable binary column, use <see cref="Binary"/>.
        /// <para>The maximum length for this data type is 8000 bytes.</para>
        /// </summary>
        NullableBinary = Constants.NullableDataType | Binary,

        /// <summary>
        /// SQL Server <c>varbinary</c> data type. Maps to a 1-dimensional <see cref="System.Byte"/> array. For a non-nullable varbinary column, use <see cref="VarBinary"/>.
        /// <para>The maximum length for this data type is 8000 bytes.</para>
        /// <para>The ANSI SQL synonym for varbinary is <c>binary varying</c>.</para>
        /// </summary>
        NullableVarBinary = Constants.NullableDataType | VarBinary,

        /// <summary>
        /// SQL Server <c>varbinary(max)</c> data type. Maps to a 1-dimensional <see cref="System.Byte"/> array. For a non-nullable varbinary(max) column, use <see cref="VarBinaryMax"/>.
        /// <para>The maximum length for this data type is (2^31 - 1) bytes.</para>
        /// <para>Use for the SQL Server <c>image</c> data type also.</para>
        /// </summary>
        NullableVarBinaryMax = Constants.NullableDataType | VarBinaryMax,

        /// <summary>
        /// SQL Server <c>char</c> data type. Maps to <see cref="System.String"/>. For a non-nullable char column, use <see cref="Char"/>.
        /// <para>The maximum length for this data type is 8000 non-Unicode characters.</para>
        /// <para>The ISO synonym for char is <c>character</c>.</para>
        /// </summary>
        NullableChar = Constants.NullableDataType | Char,

        /// <summary>
        /// SQL Server <c>nchar</c> data type. Maps to <see cref="System.String"/>. For a non-nullable nchar column, use <see cref="NChar"/>.
        /// <para>The maximum length for this data type is 4000 Unicode characters.</para>
        /// <para>ISO synonyms for nchar are <c>national char</c> and <c>national character</c>.</para>
        /// </summary>
        NullableNChar = Constants.NullableDataType | NChar,

        /// <summary>
        /// SQL Server <c>varchar</c> data type. Maps to <see cref="System.String"/>. For a non-nullable varchar column, use <see cref="VarChar"/>.
        /// <para>The maximum length for this data type is 8000 non-Unicode characters.</para>
        /// <para>ISO synonyms for varchar are <c>char varying</c> and <c>character varying</c>.</para>
        /// </summary>
        NullableVarChar = Constants.NullableDataType | VarChar,

        /// <summary>
        /// SQL Server <c>varchar(max)</c> data type. Maps to <see cref="System.String"/>. For a non-nullable varchar(max) column, use <see cref="VarCharMax"/>.
        /// <para>The maximum length for this data type is (2^31 - 1) non-Unicode characters.</para>
        /// <para>ISO synonyms for varchar(max) are <c>char varying</c> and <c>character varying</c>.</para>
        /// <para>Use for the SQL Server <c>text</c> data type also.</para>
        /// </summary>
        NullableVarCharMax = Constants.NullableDataType | VarCharMax,

        /// <summary>
        /// SQL Server <c>nvarchar</c> data type. Maps to <see cref="System.String"/>. For a non-nullable nvarchar column, use <see cref="NVarChar"/>.
        /// <para>The maximum length for this data type is 4000 Unicode characters.</para>
        /// <para>ISO synonyms for nvarchar are <c>national char varying</c> and <c>national character varying</c>.</para>
        /// </summary>
        NullableNVarChar = Constants.NullableDataType | NVarChar,

        /// <summary>
        /// SQL Server <c>nvarchar(max)</c> data type. Maps to <see cref="System.String"/>. For a non-nullable nvarchar(max) column, use <see cref="NVarCharMax"/>.
        /// <para>The maximum length for this data type is ((2^31 - 1) / 2) Unicode characters.</para>
        /// <para>ISO synonyms for nvarchar(max) are <c>national char varying</c> and <c>national character varying</c>.</para>
        /// <para>Use for the SQL Server <c>ntext</c> data type also.</para>
        /// </summary>
        NullableNVarCharMax = Constants.NullableDataType | NVarCharMax,

        /// <summary>
        /// SQL Server <c>date</c> data type. Maps to <see cref="System.Nullable{DateTime}"/>. For a non-nullable date column, use <see cref="Date"/>.
        /// <para>Valid for years between 0001 and 9999.</para>
        /// <para>The SQL Server <c>date</c> data type was introduced in 2008.</para>
        /// </summary>
        NullableDate = Constants.NullableDataType | Date,

        /// <summary>
        /// SQL Server <c>datetime</c> data type. Maps to <see cref="System.Nullable{DateTime}"/>. For a non-nullable datetime column, use <see cref="DateTime"/>.
        /// <para>Valid for years between 1753 and 9999.</para>
        /// </summary>
        NullableDateTime = Constants.NullableDataType | DateTime,

        /// <summary>
        /// SQL Server <c>smalldatetime</c> data type. Maps to <see cref="System.Nullable{DateTime}"/>. For a non-nullable smalldatetime column, use <see cref="SmallDateTime"/>.
        /// <para>Valid for dates between 1900-01-01 and 2079-06-06.</para>
        /// </summary>
        NullableSmallDateTime = Constants.NullableDataType | SmallDateTime,

        /// <summary>
        /// SQL Server <c>datetime2</c> data type. Maps to <see cref="System.Nullable{DateTime}"/>. For a non-nullable datetime2 column, use <see cref="DateTime2"/>.
        /// <para>Valid for years between 0001 and 9999.</para>
        /// <para>The SQL Server <c>datetime2</c> data type was introduced in 2008.</para>
        /// </summary>
        NullableDateTime2 = Constants.NullableDataType | DateTime2,

        /// <summary>
        /// SQL Server <c>datetimeoffset</c> data type. Maps to <see cref="System.Nullable{DateTimeOffset}"/>. For a non-nullable datetimeoffset column, use <see cref="DateTimeOffset"/>.
        /// <para>Valid for years between 0001 and 9999.</para>
        /// <para>The SQL Server <c>datetimeoffset</c> data type was introduced in 2008.</para>
        /// </summary>
        NullableDateTimeOffset = Constants.NullableDataType | DateTimeOffset,

        /// <summary>
        /// SQL Server <c>time</c> data type. Maps to <see cref="System.Nullable{DateTime}"/>. For a non-nullable time column, use <see cref="Time"/>.
        /// <para>The SQL Server <c>time</c> data type was introduced in 2008.</para>
        /// </summary>
        NullableTime = Constants.NullableDataType | Time,

        /// <summary>
        /// SQL Server <c>decimal</c> data type. Maps to <see cref="System.Nullable{Decimal}"/>. For a non-nullable decimal column, use <see cref="Decimal"/>.
        /// <para>Valid values range from - 10^38 + 1 through 10^38 - 1.</para>
        /// <para>The ISO synonyms for decimal are dec and dec(p, s). Use for the SQL Server <c>numeric</c> data type also.</para>
        /// </summary>
        NullableDecimal = Constants.NullableDataType | Decimal,

        /// <summary>
        /// SQL Server <c>money</c> data type. Maps to <see cref="System.Nullable{Decimal}"/>. For a non-nullable money column, use <see cref="Money"/>.
        /// <para>Valid values range from -922,337,203,685,477.5808 to 922,337,203,685,477.5807.</para>
        /// </summary>
        NullableMoney = Constants.NullableDataType | Money,

        /// <summary>
        /// SQL Server <c>smallmoney</c> data type. Maps to <see cref="System.Nullable{Decimal}"/>. For a non-nullable smallmoney column, use <see cref="SmallMoney"/>.
        /// <para>Valid values range from - 214,748.3648 to 214,748.3647.</para>
        /// </summary>
        NullableSmallMoney = Constants.NullableDataType | SmallMoney,

        /// <summary>
        /// SQL Server <c>float</c> data type. Maps to <see cref="System.Nullable{Double}"/>. For a non-nullable float column, use <see cref="Float"/>.
        /// <para>Valid values range from - 1.79E+308 to -2.23E-308, 0 and 2.23E-308 to 1.79E+308.</para>
        /// </summary>
        NullableFloat = Constants.NullableDataType | Float,

        /// <summary>
        /// SQL Server <c>real</c> data type. Maps to <see cref="System.Nullable{Single}"/>. For a non-nullable real column, use <see cref="Real"/>.
        /// <para>Valid values range from - 3.40E + 38 to -1.18E - 38, 0 and 1.18E - 38 to 3.40E + 38.</para>
        /// </summary>
        NullableReal = Constants.NullableDataType | Real,

        /// <summary>
        /// SQL Server <c>uniqueidentifier</c> data type. Maps to <see cref="System.Nullable{Guid}"/>. For a non-nullable uniqueidentifier column, use <see cref="UniqueIdentifier"/>.
        /// </summary>
        NullableUniqueIdentifier = Constants.NullableDataType | UniqueIdentifier,
    }
}
