using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using static tesTokenizer.Program;
using System.Web.Script.Serialization;

namespace tesTokenizer
{
    public class Program
    {
        //Path DLL
        //C:\Users\JP\Documents\GitHub\NETokenizer\tokenizers-main\tokenizers\target\debug\tokenizers.dll
        //E:\github\NETokenizer\tokenizers-main\tokenizers\target\debug/tokenizers.dll
        const string path_dll = @"C:\Users\JP\Documents\GitHub\NETokenizer\tokenizers-main\tokenizers\target\debug\tokenizers.dll";
        [StructLayout(LayoutKind.Sequential)]
        public struct RustStringArray
        {
            public UInt64 len;
            public IntPtr data;
        }

        [DllImport(path_dll)]
        static extern Int32 add_numbers(Int32 number1, Int32 number2);

        [DllImport(path_dll)]
        static extern IntPtr create_tokenizer();

        [DllImport(path_dll)]
        static extern RustStringArray encode(string text);

        [DllImport(path_dll)]
        static extern string encode_v0(string tokenizer);
        [DllImport(path_dll, CharSet = CharSet.Ansi, SetLastError = true)]
        static extern IntPtr encode_v1([MarshalAs(UnmanagedType.LPUTF8Str)] string tokenizer);

        [DllImport(path_dll)]
        public static extern IntPtr print_string([MarshalAs(UnmanagedType.LPUTF8Str)] string utf8Text);

        [DllImport(path_dll, EntryPoint = "how_many_characters")]
        public static extern uint HowManyCharacters(string s);

        private static string PtrToStringUtf8(IntPtr ptr) // aPtr is nul-terminated
        {
            if (ptr == IntPtr.Zero)
                return "";
            int len = 0;
            while (System.Runtime.InteropServices.Marshal.ReadByte(ptr, len) != 0)
                len++;
            if (len == 0)
                return "";
            byte[] array = new byte[len];
            System.Runtime.InteropServices.Marshal.Copy(ptr, array, 0, len);
            return System.Text.Encoding.UTF8.GetString(array);
        }

        private static string[] StringToArray(string ptext) // aPtr is nul-terminated
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string[] result = serializer.Deserialize<string[]>(ptext);
            return result;
        }

        static void Main(string[] args)
        {   //Function Test
            var addedNumbers = add_numbers(10, 5);
            IntPtr prTxt  = print_string("göes to élevên garzón Dueñas");
            var data_result = PtrToStringUtf8(prTxt);

            IntPtr ptr_tokens = encode_v1("Probando desde app .Net");
            var txt = PtrToStringUtf8(ptr_tokens);
            var ar_tokens = StringToArray(txt);
            Console.WriteLine(ar_tokens?.Length);
            Marshal.FreeCoTaskMem((IntPtr)ptr_tokens);

            //var rustStringArray = encode("Probando desde app .Net");

            //IntPtr[] dataPointers = new IntPtr[rustStringArray.len];
            //Marshal.Copy(rustStringArray.data, dataPointers, 0, (int)rustStringArray.len);

            //string[] data = new string[rustStringArray.len];
            //for (int i = 0; i < (int)rustStringArray.len; i++)
            //{
            //    data[i] = Marshal.PtrToStringAnsi(dataPointers[i]);
            //}


            //Console.WriteLine(addedNumbers);
            Console.ReadLine();
        }
    }
}
