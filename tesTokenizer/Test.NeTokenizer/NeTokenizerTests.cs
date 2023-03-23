using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NeTokenizer;
using System.Security.Cryptography;

namespace Test.NeTokenizer
{
    [TestClass]
    public class NeTokenizerTests
    {
        private string _testText = "Hola, ¿cómo estás?".ToLower();

        private Tokenizer GetTokenizer(bool fromFile)
        {
            if (fromFile)
            {
                return Tokenizer.FromFile("tokenizer.json");
            }
            else
            {
                return Tokenizer.FromRepository("dccuchile/bert-base-spanish-wwm-cased");
            }
        }

        [DataTestMethod]
        [DataRow(true, 1000)]
        [DataRow(false, 1000)]
        public void EncodeDecode1k(bool fromFile, int iterations)
        {
            Tokenizer tokenizer = GetTokenizer(fromFile);

            for (int i = 0; i < iterations; i++)
            {
                var tokens = tokenizer.Encode(_testText);
                var decoded = tokenizer.Decode(tokens.Ids);
                var secondEncode = tokenizer.Encode(decoded);
                var secondDecode = tokenizer.Decode(secondEncode.Ids);
                Assert.AreEqual(secondDecode, decoded);
            }
        }

        [DataTestMethod]
        [DataRow(true, 1000)]
        [DataRow(false, 1000)]
        public void EncodeDecode1kPad(bool fromFile, int iterations)
        {
            Tokenizer tokenizer = GetTokenizer(fromFile);

            for (int i = 0; i < iterations; i++)
            {
                var tokens = tokenizer.Encode(_testText, padToMax:512);
                var decoded = tokenizer.Decode(tokens.Ids);
                var secondEncode = tokenizer.Encode(decoded);
                var secondDecode = tokenizer.Decode(secondEncode.Ids);
                Assert.AreEqual(decoded, secondDecode);
                Assert.AreEqual(tokens.Ids.Length, 512);
            }
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void CheckTokenizerArgumentNull(bool fromFile)
        {
            if (fromFile)
            {
                Assert.ThrowsException<ArgumentNullException>(() => Tokenizer.FromFile(null));
            }
            else
            {
                Assert.ThrowsException<ArgumentNullException>(() => Tokenizer.FromRepository(null));
            }
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void EncodeDecodeEmptyText(bool fromFile)
        {
            Tokenizer tokenizer = GetTokenizer(fromFile);

            var tokens = tokenizer.Encode("");
            var decoded = tokenizer.Decode(tokens.Ids);
            Assert.AreEqual(string.Empty, decoded);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void CreateTokenizer(bool fromFile)
        {
            Tokenizer tokenizer = GetTokenizer(fromFile);

            // check if native pointer is not zero
            Assert.AreNotEqual(tokenizer.GetNativePtr(), IntPtr.Zero);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void CreateTokenizerEncodeDecode(bool fromFile)
        {
            Tokenizer tokenizer = GetTokenizer(fromFile);

            var tokens = tokenizer.Encode(_testText);
            var decoded = tokenizer.Decode(tokens.Ids);

            // encode again and check if the result is the same as the original text
            var secondEncode = tokenizer.Encode(decoded);
            var secondDecode = tokenizer.Decode(secondEncode.Ids);

            Assert.AreEqual(decoded, secondDecode);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void PadTo(bool fromFile)
        {
            Tokenizer tokenizer = GetTokenizer(fromFile);

            // encode with 512 padding
            var tokens = tokenizer.Encode(_testText, padToMax: 512);


            Assert.AreEqual(512, tokens.Ids.Length);
            Assert.AreEqual(512, tokens.WordIds.Length);
            Assert.AreEqual(512, tokens.Mask.Length);
        }

    }
}
