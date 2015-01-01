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
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SqlBulkCopyTests
{
    internal static class TestHelper
    {
        public static void AssertThrows<T>(Action action) where T : Exception
        {
            AssertThrows<T>(action, null);
        }

        public static void AssertThrows<T>(Action action, string message) where T : Exception
        {
            try
            {
                action.Invoke();

                message = string.Format("Expected exception of type {0}.", typeof(T).Name);

                Assert.Fail(message);
            }
            catch (T ex)
            {
                if (message != null)
                {
                    Assert.AreEqual(message, ex.Message);
                }
            }
        }

        public static void SetPrivateFieldValue<T>(this object obj, string propName, T val)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            var t = obj.GetType();

            FieldInfo fi = null;

            while (fi == null && t != null)
            {
                fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                t = t.BaseType;
            }

            if (fi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));

            fi.SetValue(obj, val);
        }
    }
}
