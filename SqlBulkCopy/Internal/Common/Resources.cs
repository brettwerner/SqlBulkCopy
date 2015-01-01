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

namespace SqlBulkCopy.Internal.Common
{
    internal static class Resources
    {
        internal static string GetDataTypeDescription(BulkCopyDataType dataType)
        {
            var nullable = (((int)dataType & Constants.NullableDataType) == Constants.NullableDataType);

            dataType = (BulkCopyDataType)((int)dataType & ~Constants.NullableDataType);

            string dataTypeName;

            switch (dataType)
            {
                case BulkCopyDataType.VarBinaryMax:
                    {
                        dataTypeName = "VarBinary(MAX)";
                        break;
                    }
                case BulkCopyDataType.NVarCharMax:
                    {
                        dataTypeName = "NVarChar(MAX)";
                        break;
                    }
                case BulkCopyDataType.VarCharMax:
                    {
                        dataTypeName = "VarChar(MAX)";
                        break;
                    }
                default:
                    {
                        dataTypeName = dataType.ToString("G");
                        break;
                    }
            }

            return dataTypeName + (nullable ? " NULL" : " NOT NULL");
        }
    }
}
