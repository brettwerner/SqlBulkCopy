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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using SqlBulkCopy.Internal.Common;

namespace SqlBulkCopy.Internal.Odbc
{
    internal static class OdbcMethods
    {
        private enum OdbcReturnCode : short  // Defined in sql.h
        {
// ReSharper disable UnusedMember.Local
            Error = -1,          // ERROR
            InvalidHandle = -2,  // INVALID_HANDLE
            NoData = 100,        // NO_DATA
            Success = 0,         // SUCCESS
            SuccessWithInfo = 1  // SUCCESS_WITH_INFO
// ReSharper restore UnusedMember.Local
        }

        private static class NativeMethods
        {
            #region odbc32.dll

            [DllImport("odbc32.dll", SetLastError = false)]
            internal static extern OdbcReturnCode SQLAllocHandle(OdbcHandleType handleType, IntPtr inputHandle, out IntPtr outputHandle);

            [DllImport("odbc32.dll", SetLastError = false)]
            internal static extern OdbcReturnCode SQLAllocHandle(OdbcHandleType handleType, OdbcEnvironmentHandle inputHandle, out IntPtr outputHandle);

            [DllImport("odbc32.dll", SetLastError = false)]
            internal static extern OdbcReturnCode SQLAllocHandle(OdbcHandleType handleType, OdbcConnectionHandle inputHandle, out IntPtr outputHandle);

            [DllImport("odbc32.dll", SetLastError = false)]
            internal static extern OdbcReturnCode SQLDisconnect(OdbcConnectionHandle connectionHandle);

            [DllImport("odbc32.dll", SetLastError = false)]
            internal static extern OdbcReturnCode SQLDisconnect(IntPtr connectionHandle);

            [DllImport("odbc32.dll", CharSet = CharSet.Unicode, SetLastError = false)]
            internal static extern OdbcReturnCode SQLDriverConnectW(OdbcConnectionHandle connectionHandle, IntPtr windowHandle, [In, MarshalAs(UnmanagedType.LPWStr)] string inConnectionString, short inConnectionStringLength, StringBuilder outConnectionStringBuffer, short bufferLength, out short bufferLengthNeeded, short fDriverCompletion);

            [DllImport("odbc32.dll", CharSet = CharSet.Unicode, SetLastError = false)]
            internal static extern OdbcReturnCode SQLExecDirectW(OdbcStatementHandle statementHandle, [In, MarshalAs(UnmanagedType.LPWStr)] string statementText, int textLength);

            [DllImport("odbc32.dll", SetLastError = false)]
            internal static extern OdbcReturnCode SQLFreeHandle(OdbcHandleType handleType, IntPtr handle);

            [DllImport("odbc32.dll", SetLastError = false)]
            internal static extern OdbcReturnCode SQLGetConnectAttr(IntPtr connectionHandle, OdbcConnectionAttribute attribute, out int value, int valueLengthShouldBeZero, int valueLengthNeededShouldBeZero);

            [DllImport("odbc32.dll", SetLastError = false)]
            internal static extern OdbcReturnCode SQLGetConnectAttr(OdbcConnectionHandle connectionHandle, OdbcConnectionAttribute attribute, out int value, int valueLengthShouldBeZero, int valueLengthNeededShouldBeZero);

            [DllImport("odbc32.dll", CharSet = CharSet.Unicode, SetLastError = false)]
            internal static extern OdbcReturnCode SQLGetConnectAttrW(IntPtr connectionHandle, OdbcConnectionAttribute attribute, StringBuilder valueBuffer, int bufferLength, out int bufferLengthNeeded);

            [DllImport("odbc32.dll", CharSet = CharSet.Unicode, SetLastError = false)]
            internal static extern OdbcReturnCode SQLGetDiagRecW(OdbcHandleType handleType, OdbcHandle handle, short recordNumber, StringBuilder stateBuffer, out int nativeError, StringBuilder messageBuffer, short bufferLength, out short bufferLengthNeeded);

            [DllImport("odbc32.dll", SetLastError = false)]
            internal static extern OdbcReturnCode SQLGetEnvAttr(OdbcEnvironmentHandle environmentHandle, OdbcEnvironmentAttribute attribute, out int value, int valueLengthShouldBeZero, int valueLengthNeededShouldBeZero);

            [DllImport("odbc32.dll", SetLastError = false)]
            internal static extern OdbcReturnCode SQLSetConnectAttr(IntPtr connectionHandle, OdbcConnectionAttribute attribute, int value, int valueLengthShouldBeZero);

            [DllImport("odbc32.dll", SetLastError = false)]
            internal static extern OdbcReturnCode SQLSetConnectAttr(OdbcConnectionHandle connectionHandle, OdbcConnectionAttribute attribute, int value, int valueLengthShouldBeZero);

            [DllImport("odbc32.dll", CharSet = CharSet.Unicode, SetLastError = false)]
            internal static extern OdbcReturnCode SQLSetConnectAttrW(IntPtr connectionHandle, OdbcConnectionAttribute attribute, [In, MarshalAs(UnmanagedType.LPWStr)] string value, int stringLength);

            [DllImport("odbc32.dll", SetLastError = false)]
            internal static extern OdbcReturnCode SQLSetEnvAttr(OdbcEnvironmentHandle environmentHandle, OdbcEnvironmentAttribute attribute, int value, int valueLengthShouldBeZero);

            #endregion
        }

        #region internal methods

        internal static IntPtr AllocateConnectionHandle(OdbcEnvironmentHandle environmentHandle)
        {
            if (environmentHandle == null) throw new ArgumentNullException("environmentHandle");

            IntPtr handle;
            var result = NativeMethods.SQLAllocHandle(OdbcHandleType.Connection, environmentHandle, out handle);

            if ((result == OdbcReturnCode.Success) || (result == OdbcReturnCode.SuccessWithInfo)) return handle;

            var ex = GetException(environmentHandle, "Unable to allocate ODBC connection handle.");

            throw ex;
        }

        internal static IntPtr AllocateEnvironmentHandle()
        {
            IntPtr handle;
            var result = NativeMethods.SQLAllocHandle(OdbcHandleType.Environment, Constants.IntPtrZero, out handle);

            if ((result != OdbcReturnCode.Success) && (result != OdbcReturnCode.SuccessWithInfo))
            {
                throw new Exception("Unable to allocate ODBC environment handle.");
            }

            return handle;
        }

        internal static IntPtr AllocateStatementHandle(OdbcConnectionHandle connectionHandle)
        {
            if (connectionHandle == null) throw new ArgumentNullException("connectionHandle");

            IntPtr handle;
            var result = NativeMethods.SQLAllocHandle(OdbcHandleType.Statement, connectionHandle, out handle);

            if ((result == OdbcReturnCode.Success) || (result == OdbcReturnCode.SuccessWithInfo)) return handle;

            var ex = GetException(connectionHandle, "Unable to allocate ODBC statement handle.");

            throw ex;
        }

        internal static void Connect(OdbcConnectionHandle connectionHandle, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            const short noPrompt = 0;  // SQL_DRIVER_NOPROMPT: the Driver Manager copies the connection string specified by the application.

            short bufferLengthNeeded;
            var result = NativeMethods.SQLDriverConnectW(connectionHandle, Constants.IntPtrZero, connectionString, (short)connectionString.Length, null, 0, out bufferLengthNeeded, noPrompt);

            if ((result == OdbcReturnCode.Success) || (result == OdbcReturnCode.SuccessWithInfo)) return;

            var ex = GetException(connectionHandle, "Unable to connect to the database.");

            throw ex;
        }

        internal static void Disconnect(OdbcConnectionHandle connectionHandle)
        {
            if (connectionHandle == null) throw new ArgumentNullException("connectionHandle");

            var result = NativeMethods.SQLDisconnect(connectionHandle);

            if ((result == OdbcReturnCode.Success) || (result == OdbcReturnCode.SuccessWithInfo)) return;

            var ex = GetException(connectionHandle, "Unable to disconnect the database.");

            throw ex;
        }

        internal static void Disconnect(IntPtr connectionHandle)
        {
            var result = NativeMethods.SQLDisconnect(connectionHandle);

            if ((result != OdbcReturnCode.Success) && (result != OdbcReturnCode.SuccessWithInfo))
            {
                throw new Exception("Unable to disconnect the database.");
            }
        }

        internal static void ExecuteDirect(OdbcStatementHandle statementHandle, string commandText)
        {
            if (commandText == null) throw new ArgumentNullException("commandText");

            var result = NativeMethods.SQLExecDirectW(statementHandle, commandText, commandText.Length);

            if ((result == OdbcReturnCode.Success) || (result == OdbcReturnCode.SuccessWithInfo)) return;

            var ex = GetException(statementHandle, "Unable to execute command text.");

            throw ex;
        }

        internal static int GetIntConnectionAttribute(OdbcConnectionAttribute attribute)
        {
            int value;
            var result = NativeMethods.SQLGetConnectAttr(Constants.IntPtrZero, attribute, out value, 0, 0);

            if ((result != OdbcReturnCode.Success) && (result != OdbcReturnCode.SuccessWithInfo))
            {
                var ex = new Exception(string.Format("Unable to get ODBC connection attribute '{0:G}'.", attribute));

                throw ex;
            }

            return value;
        }

        internal static int GetIntConnectionAttribute(OdbcConnectionHandle connectionHandle, OdbcConnectionAttribute attribute)
        {
            if (connectionHandle == null) throw new ArgumentNullException("connectionHandle");

            int value;
            var result = NativeMethods.SQLGetConnectAttr(connectionHandle, attribute, out value, 0, 0);

            if ((result != OdbcReturnCode.Success) && (result != OdbcReturnCode.SuccessWithInfo))
            {
                var ex = GetException(connectionHandle, string.Format("Unable to get ODBC connection attribute '{0:G}'.", attribute));

                throw ex;
            }

            return value;
        }

        internal static string GetStringConnectionAttribute(OdbcConnectionAttribute attribute)
        {
            int bufferLengthNeeded;
            var buffer = new StringBuilder(256);
            var result = NativeMethods.SQLGetConnectAttrW(Constants.IntPtrZero, attribute, buffer, buffer.Capacity * 2, out bufferLengthNeeded);

            if (result == OdbcReturnCode.SuccessWithInfo)
            {
                var neededCapacity = ((bufferLengthNeeded / 2) + 1);

                if (buffer.Capacity < neededCapacity)
                {
                    buffer.Capacity = neededCapacity;
                    result = NativeMethods.SQLGetConnectAttrW(Constants.IntPtrZero, attribute, buffer, buffer.Capacity * 2, out bufferLengthNeeded);
                }
            }

            if ((result != OdbcReturnCode.Success) && (result != OdbcReturnCode.SuccessWithInfo))
            {
                var ex = new Exception(string.Format("Unable to get ODBC connection attribute '{0:G}'.", attribute));

                throw ex;
            }

           return buffer.ToString();
        }

        internal static int GetIntEnvironmentAttribute(OdbcEnvironmentHandle environmentHandle, OdbcEnvironmentAttribute attribute)
        {
            if (environmentHandle == null) throw new ArgumentNullException("environmentHandle");

            int value;
            var result = NativeMethods.SQLGetEnvAttr(environmentHandle, attribute, out value, 0, 0);

            if ((result != OdbcReturnCode.Success) && (result != OdbcReturnCode.SuccessWithInfo))
            {
                var ex = GetException(environmentHandle, string.Format("Unable to get ODBC environment attribute '{0:G}'.", attribute));

                throw ex;
            }

            return value;
        }

        internal static bool ReleaseHandle(OdbcHandleType handleType, IntPtr handle)
        {
            return (NativeMethods.SQLFreeHandle(handleType, handle) == OdbcReturnCode.Success);
        }

        internal static void SetBcpFlag(OdbcConnectionHandle connectionHandle)
        {
            if (connectionHandle == null) throw new ArgumentNullException("connectionHandle");

            const int bcpOn = 1;  // SQL_BCP_ON: enables bulk copy functions on a connection.

            var result = NativeMethods.SQLSetConnectAttr(connectionHandle, OdbcConnectionAttribute.EnableBcp, bcpOn, 0);

            if ((result == OdbcReturnCode.Success) || (result == OdbcReturnCode.SuccessWithInfo)) return;

            var ex = GetException(connectionHandle, "Unable to set BCP connection flag.");

            throw ex;
        }

        internal static void SetIntConnectionAttribute(OdbcConnectionAttribute attribute, int value)
        {
            var result = NativeMethods.SQLSetConnectAttr(Constants.IntPtrZero, attribute, value, 0);

            if ((result != OdbcReturnCode.Success) && (result != OdbcReturnCode.SuccessWithInfo))
            {
                var ex = new Exception(string.Format("Unable to set ODBC connection attribute '{0:G}'.", attribute));

                throw ex;
            }
        }

        internal static void SetStringConnectionAttribute(OdbcConnectionAttribute attribute, string value)
        {
            if (value == null) throw new ArgumentNullException("value");

            var result = NativeMethods.SQLSetConnectAttrW(Constants.IntPtrZero, attribute, value, value.Length * 2);

            if ((result != OdbcReturnCode.Success) && (result != OdbcReturnCode.SuccessWithInfo))
            {
                var ex = new Exception(string.Format("Unable to set ODBC connection attribute '{0:G}'.", attribute));

                throw ex;
            }
        }

        internal static void SetIntConnectionAttribute(OdbcConnectionHandle connectionHandle, OdbcConnectionAttribute attribute, int value)
        {
            if (connectionHandle == null) throw new ArgumentNullException("connectionHandle");

            var result = NativeMethods.SQLSetConnectAttr(connectionHandle, attribute, value, 0);

            if ((result != OdbcReturnCode.Success) && (result != OdbcReturnCode.SuccessWithInfo))
            {
                var ex = GetException(connectionHandle, string.Format("Unable to set ODBC connection attribute '{0:G}'.", attribute));

                throw ex;
            }
        }

        internal static void SetIntEnvironmentAttribute(OdbcEnvironmentHandle environmentHandle, OdbcEnvironmentAttribute attribute, int value)
        {
            if (environmentHandle == null) throw new ArgumentNullException("environmentHandle");

            var result = NativeMethods.SQLSetEnvAttr(environmentHandle, attribute, value, 0);

            if ((result != OdbcReturnCode.Success) && (result != OdbcReturnCode.SuccessWithInfo))
            {
                var ex = GetException(environmentHandle, string.Format("Unable to set ODBC environment attribute '{0:G}'.", attribute));

                throw ex;
            }
        }

        #endregion

        #region error handling

        private sealed class OdbcError
        {
            public OdbcError(string state, string message)
            {
                State = state;
                Message = message;
            }

            public string State { get; private set; }
            public string Message { get; private set; }
        }

        private static Exception CreateException(string message, IEnumerable<OdbcError> errors)
        {
            var builder = new StringBuilder(message);

            foreach (var error in errors)
            {
                builder.Append(Environment.NewLine);
                builder.Append(string.Format("[{0}] {1}", error.State, error.Message));
            }

            return new Exception(builder.ToString());
        }

        private static OdbcReturnCode GetDiagnosticRecord(OdbcHandle handle, short recordNumber, out string state, StringBuilder messageBuffer, out short bufferLengthNeeded)
        {
            var stateBuffer = new StringBuilder(5);
            int nativeError;

            var result = NativeMethods.SQLGetDiagRecW(handle.HandleType, handle, recordNumber, stateBuffer, out nativeError, messageBuffer, (short)messageBuffer.Capacity, out bufferLengthNeeded);

            if ((result == OdbcReturnCode.Success) || (result == OdbcReturnCode.SuccessWithInfo))
            {
                state = stateBuffer.ToString();
            }
            else
            {
                state = null;
            }

            return result;
        }

        internal static Exception GetException(OdbcHandle handle, string message)
        {
            var errors = new List<OdbcError>();
            var messageBuffer = new StringBuilder(512);
            short recordNumber = 1;

            while (true)
            {
                string state;
                short bufferLengthNeeded;

                var result = GetDiagnosticRecord(handle, recordNumber, out state, messageBuffer, out bufferLengthNeeded);

                if (result == OdbcReturnCode.SuccessWithInfo)
                {
                    var neededCapacity = bufferLengthNeeded + 1;

                    if (messageBuffer.Capacity < neededCapacity)
                    {
                        messageBuffer.Capacity = neededCapacity;
                        result = GetDiagnosticRecord(handle, recordNumber, out state, messageBuffer, out bufferLengthNeeded);
                    }
                }

                if ((result != OdbcReturnCode.Success) && (result != OdbcReturnCode.SuccessWithInfo))
                {
                    break;
                }

                errors.Add(new OdbcError(state, messageBuffer.ToString()));
                recordNumber++;
            }

            return CreateException(message, errors);
        }

        #endregion
    }
}
