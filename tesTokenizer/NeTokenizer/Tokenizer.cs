using System;

namespace NeTokenizer
{
    /// <summary>
    /// Huggingface's wrapper of the rust library.
    /// </summary>
    internal class Tokenizer
    {
        /// <summary>
        /// Native pointer.
        /// </summary>
        private IntPtr _tokenizer;

        public Tokenizer(string tokenizer_path)
        {
            _tokenizer = TokenizerNative.create_tokenizer(tokenizer_path);
        }

        public string[] Encode(string text)
        {
            var result = TokenizerNative.encode_v5(_tokenizer, text);
            return TokenizerNative.PtrToStringArray(result);
        }
    }
}
