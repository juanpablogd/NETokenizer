using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NeTokenizer;

namespace Test.NeTokenizer
{
    [TestClass]
    public class NeTokenizerTests
    {
        [TestMethod]
        public void CreateTokenizer()
        {
            var tokenizer = new Tokenizer(@"dccuchile/bert-base-spanish-wwm-cased");

            // check if native pointer is not zero
            Assert.AreNotEqual(tokenizer.GetNativePtr(), IntPtr.Zero);
        }

        [TestMethod]
        public void CreateTokenizerEncode()
        {
            var tokenizer = new Tokenizer(@"dccuchile/bert-base-spanish-wwm-cased");

            
        }
    }
}
