using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace NeTokenizer
{
    /// <summary>
    /// Huggingface's wrapper of the rust library.
    /// </summary>
    public class Tokenizer
    {
        /// <summary>
        /// Native pointer.
        /// </summary>
        private IntPtr _tokenizerPtr;

        public Tokenizer(string tokenizer_path)
        {
            _tokenizerPtr = TokenizerNative.create_tokenizer(tokenizer_path);
        }

        /// <summary>
        /// Gets the native pointer.
        /// </summary>
        /// <returns>Native rust pointer</returns>
        public IntPtr GetNativePtr()
        {
            return _tokenizerPtr;
        }

        public Encoded Encode(string text,bool includeSpecialTokens = false, int padToMax = -1)
        {
            var wordIds = new List<int>();
            var nativeStructPtr = TokenizerNative.encode(_tokenizerPtr, text, includeSpecialTokens, padToMax);
            CSharpArray rustArray = Marshal.PtrToStructure<CSharpArray>(nativeStructPtr);

            var tokens = PtrToString(rustArray.tokens).Split(' ');
            var ids = PtrToString(rustArray.ids).Split(' ');
            var mask = PtrToString(rustArray.mask).Split(' ');

            int wordIndex = -1;

            // loop tokens with for loop and check if it starts with ##, if it does, then it is a subword
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].StartsWith("##"))
                {
                    // add subword to the previous word
                    wordIds.Add(wordIndex);
                }
                else if (tokens[i].StartsWith("[") && tokens[i].EndsWith("]"))
                {
                    // -100 as None
                    wordIds.Add(-100);
                }
                else
                {
                    wordIndex++;
                    wordIds.Add(wordIndex);
                }
            }

            // print wordIds with its respective token
            for (int i = 0; i < tokens.Length; i++)
            {
                Console.WriteLine($"{tokens[i]} {wordIds[i]}");
            }


            return new Encoded
            {
                Ids = Array.ConvertAll(ids, int.Parse),
                Mask = Array.ConvertAll(mask, int.Parse),
                Tokens = tokens
            };
        }

        public static string PtrToString(IntPtr intPtr)
        {
            try
            {
                int len = 0;
                while (Marshal.ReadByte(intPtr, len) != 0) ++len;
                byte[] buffer = new byte[len];
                Marshal.Copy(intPtr, buffer, 0, buffer.Length);
                string result = Encoding.UTF8.GetString(buffer);
                return result;
            }
            catch (AccessViolationException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return string.Empty;
        }
    }
}