using System.Runtime.InteropServices;
using System;

namespace NeTokenizer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CSharpArray
    {
        public IntPtr tokens;
        public IntPtr ids;
        public IntPtr mask;
        public IntPtr word_ids;
    }

    internal class TokenizerNative
    {
        const string path_dll = @"tokenizers.dll";
        [DllImport(path_dll)]
        public static extern IntPtr create_tokenizer(string tokenizer_path);
        
        [DllImport(path_dll)]
        public static extern IntPtr encode(IntPtr tokenizer, [MarshalAs(UnmanagedType.LPUTF8Str)] string utf8Text, bool includeSpecialTokens, int padToMax);

        [DllImport(path_dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr encode_struct(IntPtr tokenizer, [MarshalAs(UnmanagedType.LPUTF8Str)] string utf8Text);
    }
}
