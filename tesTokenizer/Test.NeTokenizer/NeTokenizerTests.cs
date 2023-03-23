using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NeTokenizer;

namespace Test.NeTokenizer
{
    [TestClass]
    public class NeTokenizerTests
    {
        [TestMethod]
        public void CreateTokenizerFromFile()
        {
            var tokenizer = Tokenizer.FromRepository("../../../../../bert-base-spanish-wwm-cased/tokenizer.json");

            // check if native pointer is not zero
            Assert.AreNotEqual(tokenizer.GetNativePtr(), IntPtr.Zero);
        }

        [TestMethod]
        public void CreateTokenizerFromFileEncodeDecode()
        {
            var tokenizer = Tokenizer.FromRepository("../../../../../bert-base-spanish-wwm-cased/tokenizer.json");

            string text = "Hola, ¿cómo estás?".ToLower();
            var tokens = tokenizer.Encode(text);
            var decoded = tokenizer.Decode(tokens.Ids);

            // encode again and check if the result is the same as the original text
            var secondEncode = tokenizer.Encode(decoded);
            var secondDecode = tokenizer.Decode(secondEncode.Ids);

            Assert.AreEqual(decoded, secondDecode);
        }
    }
}
