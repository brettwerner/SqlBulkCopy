﻿/*
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
using SqlBulkCopy.Internal.BulkCopy;

namespace SqlBulkCopy.Internal.Odbc
{
    internal interface IOdbcConnection : IDisposable
    {
        IOdbcDriver Driver { get; }

        // ODBC
        void Connect(string connectionString);
        bool Connected { get; }
        void Disconnect();
        void ExecuteDirect(string commandText);
        int GetIntAttribute(OdbcConnectionAttribute attribute);
        void SetIntAttribute(OdbcConnectionAttribute attribute, int value);

        // Bulk Copy
        int Batch();
        void Bind<T>(IStandardColumn<T> column);
        void Bind<T>(IMaxColumn<T> column);
        int Done();
        void Initialize(string tableName);
        bool Initialized { get; }
        bool SendMoreText(IMaxDataType column);
        bool SendRow();
        bool SetColumnLength(IMaxDataType column);
    }
}
