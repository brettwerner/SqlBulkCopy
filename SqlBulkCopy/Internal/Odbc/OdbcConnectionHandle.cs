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
using SqlBulkCopy.Internal.BulkCopy;
using SqlBulkCopy.Internal.Common;

namespace SqlBulkCopy.Internal.Odbc
{
    internal sealed class OdbcConnectionHandle : OdbcHandle, IOdbcConnection
    {
        private bool _connected;
        private readonly IOdbcDriver _driver;
        private bool _initialized;
        private OdbcEnvironmentHandle _parentHandle;

        internal OdbcConnectionHandle(OdbcEnvironmentHandle parentHandle, IOdbcDriver driver)
        {
            if (parentHandle == null) throw new ArgumentNullException("parentHandle");
            if (driver == null) throw new ArgumentNullException("driver");

            _driver = driver;

            handle = OdbcMethods.AllocateConnectionHandle(parentHandle);

            var success = false;

            parentHandle.DangerousAddRef(ref success);

            if (!success)
            {
                throw new Exception("Unable to increment ODBC environment handle reference count.");
            }

            _parentHandle = parentHandle;
        }

        #region properties

        IOdbcDriver IOdbcConnection.Driver
        {
            get { return _driver; }
        }

        public override OdbcHandleType HandleType
        {
            get { return OdbcHandleType.Connection; }
        }

        public override bool IsInvalid
        {
            get { return (handle == Constants.IntPtrZero); }
        }

        #endregion

        #region methods

        protected override bool ReleaseHandle()
        {
            try
            {
                var releaseHandle = handle;

                if (releaseHandle != Constants.IntPtrZero)
                {
                    handle = Constants.IntPtrZero;

                    if (_connected)
                    {
                        OdbcMethods.Disconnect(releaseHandle);
                        _connected = false;
                    }

                    OdbcMethods.ReleaseHandle(OdbcHandleType.Connection, releaseHandle);
                }
            }
            finally
            {
                var parentHandle = _parentHandle;

                if (parentHandle != null)
                {
                    _parentHandle = null;
                    parentHandle.DangerousRelease();
                }
            }

            return true;
        }

        #endregion

        #region IOdbcConnection - ODBC

        void IOdbcConnection.Connect(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("connectionString is required.");

            if (_connected)
            {
                throw new InvalidOperationException("The instance is already connected.");
            }

            OdbcMethods.SetBcpFlag(this);
            OdbcMethods.Connect(this, connectionString);
            _connected = true;
        }

        bool IOdbcConnection.Connected
        {
            get { return _connected; }
        }

        void IOdbcConnection.Disconnect()
        {
            if (!_connected) return;

            OdbcMethods.Disconnect(this);
            _connected = false;
        }

        void IOdbcConnection.ExecuteDirect(string commandText)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            using (var statementHandle = new OdbcStatementHandle(this))
            {
                OdbcMethods.ExecuteDirect(statementHandle, commandText);
            }
        }

        int IOdbcConnection.GetIntAttribute(OdbcConnectionAttribute attribute)
        {
            return OdbcMethods.GetIntConnectionAttribute(this, attribute);
        }

        void IOdbcConnection.SetIntAttribute(OdbcConnectionAttribute attribute, int value)
        {
            OdbcMethods.SetIntConnectionAttribute(this, attribute, value);
        }

        #endregion

        #region IOdbcConnection - Bulk Copy

        int IOdbcConnection.Batch()
        {
            return _driver.Batch(this);
        }

        void IOdbcConnection.Bind<T>(IStandardColumn<T> column)
        {
            _driver.Bind(this, column);
        }

        void IOdbcConnection.Bind<T>(IMaxColumn<T> column)
        {
            _driver.Bind(this, column);
        }

        int IOdbcConnection.Done()
        {
            return _driver.Done(this);
        }

        void IOdbcConnection.Initialize(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentException("tableName is required.");

            _driver.Initialize(this, tableName);
            _initialized = true;
        }

        bool IOdbcConnection.Initialized
        {
            get { return _initialized; }
        }

        bool IOdbcConnection.SendMoreText(IMaxDataType column)
        {
            if (column == null) throw new ArgumentNullException("column");

            return _driver.SendMoreText(this, column);
        }

        bool IOdbcConnection.SendRow()
        {
            return _driver.SendRow(this);
        }

        bool IOdbcConnection.SetColumnLength(IMaxDataType column)
        {
            if (column == null) throw new ArgumentNullException("column");

            return _driver.SetColumnLength(this, column);
        }

        #endregion
    }
}