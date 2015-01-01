using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlBulkCopy;
using SqlBulkCopy.Internal.BulkCopy;

namespace SqlBulkCopyTests.Internal.BulkCopy
{
    [TestClass]
    public class CharColumnTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetValueThrowsExceptionForValueLongerThanDeclaredColumnLength()
        {
            const int length = 1;

            ITypedBulkCopyBoundColumn<string> sut = CharColumn.CreateInstance(0, BulkCopyDataType.Char, length);

            sut.SetValue(new string('-', length + 1));
        }
    }
}
