using System.Runtime.InteropServices;
using System;

namespace NeTokenizer
{
    internal class TokenizerNative
    {
        const string path_dll = @"E:\github\NETokenizer\ext\tokenizers\target\debug\tokenizers.dll";
        [DllImport(path_dll)]
        public static extern IntPtr create_tokenizer(string tokenizer_path);

        [DllImport(path_dll)]
        public static extern IntPtr print_string([MarshalAs(UnmanagedType.LPUTF8Str)] string utf8Text);

        [DllImport(path_dll)]
        static extern RustStringArray encode(string text);

        [DllImport(path_dll)]
        public static extern IntPtr encode_v3([MarshalAs(UnmanagedType.LPUTF8Str)] string utf8Text);

        [DllImport(path_dll)]
        public static extern IntPtr encode_v4(IntPtr tokenizer, [MarshalAs(UnmanagedType.LPUTF8Str)] string utf8Text);

        [DllImport(path_dll)]
        public static extern IntPtr encode_v5(IntPtr tokenizer, [MarshalAs(UnmanagedType.LPUTF8Str)] string utf8Text);
    }
}
