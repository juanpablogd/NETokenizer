using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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

        private Tokenizer(IntPtr nativeptr) => _tokenizerPtr = nativeptr;

        /// <summary>
        /// Creates a tokenizer from a Huggingface's repository.
        /// </summary>
        /// <param name="repository">Huggingface's repository name.</param>
        /// <returns>New tokenizer.</returns>
        /// <exception cref="ArgumentNullException">repository cannot be null or empty.</exception>
        public static Tokenizer FromRepository(string repository)
        {
            // throw repository cannot be null or empty
            if (string.IsNullOrEmpty(repository))
                throw new ArgumentNullException(nameof(repository));
            var nativePtr = TokenizerNative.create_tokenizer(repository);
            return new Tokenizer(nativePtr);
        }

        /// <summary>
        /// Creates a tokenizer from a file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>Instance of the new tokenizer created from a file.</returns>
        public static Tokenizer FromFile(string filename)
        {
            // throw filename cannot be null or empty
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));
            var nativePtr = TokenizerNative.create_tokenizer_local(filename);
            return new Tokenizer(nativePtr);
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
            var nativeStructPtr = TokenizerNative.encode(_tokenizerPtr, text, includeSpecialTokens, padToMax);
            CSharpArray rustArray = Marshal.PtrToStructure<CSharpArray>(nativeStructPtr);

            var tokens = PtrToString(rustArray.tokens).Split(' ');
            var ids = PtrToString(rustArray.ids).Split(' ');
            var mask = PtrToString(rustArray.mask).Split(' ');
            var wordIdsStr = PtrToString(rustArray.wordids).Split(' ');

            return new Encoded
            {
                Ids = Array.ConvertAll(ids, long.Parse),
                Mask = Array.ConvertAll(mask, int.Parse),
                Tokens = tokens,
                WordIds = Array.ConvertAll(wordIdsStr, int.Parse)
            };
        }


        public string Decode(long[] tokens)
        {
            var decodePtr = TokenizerNative.decode(tokens.Length, tokens, _tokenizerPtr);
            var resultDecoded = PtrToString(decodePtr);
            return resultDecoded;
        }

        /// <summary>
        /// Converts a native pointer to a string. 
        /// </summary>
        /// <param name="intPtr">Native pointer.</param>
        /// <returns>String of the content from the native pointer.</returns>
        internal static string PtrToString(IntPtr intPtr)
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