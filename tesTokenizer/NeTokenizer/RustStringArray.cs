using System.Runtime.InteropServices;
using System;

namespace NeTokenizer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RustStringArray
    {
        public UInt64 len;
        public IntPtr data;
    }
}
