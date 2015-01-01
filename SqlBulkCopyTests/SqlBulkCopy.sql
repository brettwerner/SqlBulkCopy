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

Use [SqlBulkCopy]

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SqlBulkCopyTest]') AND type in (N'U'))
drop table SqlBulkCopyTest
go

/*
select * from SqlBulkCopyTest
*/

create table SqlBulkCopyTest
(
	identityColumn int identity(1,1) not null,

	nonNullableBit bit not null,
	nonNullableTinyInt tinyint not null,
	nonNullableSmallInt smallint not null,
	nonNullableInt int not null,
	nonNullableBigInt bigint not null,
	nonNullableBinary binary(4) not null,
	nonNullableVarBinary varbinary(10) not null,
	nonNullableChar char(4) not null,
	nonNullableNChar nchar(8) not null,
	nonNullableVarChar varchar(10) not null,
	nonNullableNVarChar nvarchar(12) not null,
	nonNullableDateTime datetime not null,
	nonNullableSmallDateTime smalldatetime not null,
	nonNullableDate date not null,
	nonNullableDateTime2 datetime2 not null,
	nonNullableDateTimeOffset datetimeoffset not null,
	nonNullableTime time not null,
	nonNullableDecimal1 decimal(9,4) not null,
	nonNullableDecimal2 decimal(38,6) not null,
	nonNullableSmallMoney smallmoney not null,
	nonNullableMoney money not null,
	nonNullableFloat float not null,
	nonNullableReal real not null,
	nonNullableUniqueIdentifier uniqueidentifier not null,
	nonNullableVarBinaryMax varbinary(MAX) not null,
	nonNullableVarCharMax varchar(MAX) not null,
	nonNullableNVarCharMax nvarchar(MAX) not null,
	nullableBit bit null,
	nullableTinyInt tinyint null,
	nullableSmallInt smallint null,
	nullableInt int null,
	nullableBigInt bigint null,
	nullableBinary binary(4) null,
	nullableVarBinary varbinary(10) null,
	nullableChar char(4) null,
	nullableNChar nchar(8) null,
	nullableVarChar varchar(10) null,
	nullableNVarChar nvarchar(12) null,
	nullableDateTime datetime null,
	nullableSmallDateTime smalldatetime null,
	nullableDate date null,
	nullableDateTime2 datetime2 null,
	nullableDateTimeOffset datetimeoffset null,
	nullableTime time null,
	nullableDecimal1 decimal(9,4) null,
	nullableDecimal2 decimal(38,6) null,
	nullableSmallMoney smallmoney null,
	nullableMoney money null,
	nullableFloat float null,
	nullableReal real null,
	nullableUniqueIdentifier uniqueidentifier null,
	nullableVarBinaryMax varbinary(MAX) null,
	nullableVarCharMax varchar(MAX) null,
	nullableNVarCharMax nvarchar(MAX) null,

	constraint PK_SqlBulkCopyTest primary key clustered (identityColumn)
)
go
