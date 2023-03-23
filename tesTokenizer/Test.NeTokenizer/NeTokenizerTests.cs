using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NeTokenizer;

namespace Test.NeTokenizer
{
    [TestClass]
    public class NeTokenizerTests
    {
        private string _testText = "Hola, ¿cómo estás?".ToLower();

        [TestMethod]
        public void EncodeDecodeEmptyText()
        {
            var tokenizer = Tokenizer.FromFile("tokenizer.json");
            var tokens = tokenizer.Encode("");
            var decoded = tokenizer.Decode(tokens.Ids);
            Assert.AreEqual(string.Empty, decoded);
        }

        [TestMethod]
        public void CreateTokenizerFromFile()
        {
            var tokenizer = Tokenizer.FromFile("tokenizer.json");

            // check if native pointer is not zero
            Assert.AreNotEqual(tokenizer.GetNativePtr(), IntPtr.Zero);
        }

        [TestMethod]
        public void CreateTokenizerFromFileEncodeDecode()
        {
            var tokenizer = Tokenizer.FromFile("tokenizer.json");

            var tokens = tokenizer.Encode(_testText);
            var decoded = tokenizer.Decode(tokens.Ids);

            // encode again and check if the result is the same as the original text
            var secondEncode = tokenizer.Encode(decoded);
            var secondDecode = tokenizer.Decode(secondEncode.Ids);

            Assert.AreEqual(decoded, secondDecode);
        }
    }
}
