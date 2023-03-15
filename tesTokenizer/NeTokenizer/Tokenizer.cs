using Newtonsoft.Json;
using System;
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

        public string EncodeStruct(string text)
        {
            var structNative = TokenizerNative.encode(_tokenizerPtr, text);
            CSharpArray rustArray = Marshal.PtrToStructure<CSharpArray>(structNative);

            var tokens = PtrToString(rustArray.tokens).Split(' ');
            var ids = PtrToString(rustArray.ids).Split(' ');
            var mas = PtrToString(rustArray.mask).Split(' ');

            return string.Empty;
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