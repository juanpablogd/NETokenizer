using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using static tesTokenizer.Program;

namespace tesTokenizer
{
    public class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RustStringArray
        {
            public UInt64 len;
            public IntPtr data;
        }


        [DllImport(@"E:\github\NETokenizer\tokenizers-main\tokenizers\target\debug/tokenizers.dll")]
        static extern Int32 add_numbers(Int32 number1, Int32 number2);

        [DllImport(@"E:\github\NETokenizer\tokenizers-main\tokenizers\target\debug/tokenizers.dll")]
        static  extern IntPtr create_tokenizer();

        [DllImport(@"E:\github\NETokenizer\tokenizers-main\tokenizers\target\debug/tokenizers.dll")]
        static extern RustStringArray encode(string text);
        static void Main(string[] args)
        {   //Function Test
            var addedNumbers = add_numbers(10, 5);
            var tokenizer = create_tokenizer();
            var rustStringArray = encode("Probando desde app .Net");

            IntPtr[] dataPointers = new IntPtr[rustStringArray.len];
            Marshal.Copy(rustStringArray.data, dataPointers, 0, (int)rustStringArray.len);

            string[] data = new string[rustStringArray.len];
            for (int i = 0; i < (int)rustStringArray.len; i++)
            {
                data[i] = Marshal.PtrToStringAnsi(dataPointers[i]);
            }


            Console.WriteLine(addedNumbers);
            Console.ReadLine();
        }
    }
}
