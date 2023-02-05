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
        //Path DLL
        //C:\Users\JP\Documents\GitHub\NETokenizer\tokenizers-main\tokenizers\target\debug\tokenizers.dll
        //E:\github\NETokenizer\tokenizers-main\tokenizers\target\debug/tokenizers.dll
        const string path_dll = @"E:\github\NETokenizer\tokenizers-main\tokenizers\target\debug/tokenizers.dll";
        [StructLayout(LayoutKind.Sequential)]
        public struct RustStringArray
        {
            public UInt64 len;
            public IntPtr data;
        }


        [DllImport(path_dll)]
        static extern Int32 add_numbers(Int32 number1, Int32 number2);

        [DllImport(path_dll)]
        static  extern IntPtr create_tokenizer();

        [DllImport(path_dll)]
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
