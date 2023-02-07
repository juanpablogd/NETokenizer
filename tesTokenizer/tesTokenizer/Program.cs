using System;
using System.Runtime.InteropServices;
using System.Text;
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
        public static extern IntPtr print_string([MarshalAs(UnmanagedType.LPUTF8Str)] string utf8Text);

        [DllImport(path_dll)]
        static extern RustStringArray encode(string text);

        [DllImport(path_dll)]
        public static extern IntPtr encode_v3([MarshalAs(UnmanagedType.LPUTF8Str)] string utf8Text);

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

        private static string GetUTF8TextFromPointer(IntPtr pointer)
        {
            string ansiText = Marshal.PtrToStringAnsi(pointer);
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(ansiText);
            return Encoding.UTF8.GetString(utf8Bytes);
        }

        private static string GetUTF8TextFromPointerV1(IntPtr pointer)
        {
            int size = 0;
            while (Marshal.ReadByte(pointer, size) != 0)
            {
                size++;
            }
            byte[] buffer = new byte[size];
            Marshal.Copy(pointer, buffer, 0, size);
            return Encoding.UTF8.GetString(buffer);
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
        static int mayorIndexOf(string texto)
        {
            int mayor = -1;
            int indice = texto.IndexOf(']');
            while (indice != -1)
            {
                if (indice > mayor)
                {
                    mayor = indice;
                }
                indice = texto.IndexOf(']', indice + 1);
            }
            return mayor;
        }

        static string stantardJson(string texto)
        {
            int inicio = texto.IndexOf('[');
            int fin = mayorIndexOf(texto);
            return texto.Substring(inicio, fin - inicio + 1);
        }

        private static string[] StringToArray(string ptext) // aPtr is nul-terminated
        {
            string[] result = null;
            if (string.IsNullOrEmpty(ptext)) return result;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            result = serializer.Deserialize<string[]>(ptext);
            return result;
        }

        static void Main(string[] args)
        {   //Function Test
            //var addedNumbers = add_numbers(10, 5);
            //Console.WriteLine(addedNumbers);
            //IntPtr prTxt = print_string("göes to élevên garzón Dueñas");
            //var data_result = PtrToStringUtf8(prTxt);
            test_encoder_v3("garzón  Dueñas");
            test_encoder_v3("göes to élevên ] garzón Dueñas");

            Console.ReadLine();
        }
        static void test_encoder_v3(string texto)
        {
            IntPtr prTxt2 = encode_v3(texto);
            var data_result2 = PtrToString(prTxt2); //Console.WriteLine(stantardJson(data_result2));
            string[] star = StringToArray(stantardJson(data_result2));
            foreach (var token in star)
            {
                Console.WriteLine(token);
            }
        }
    }
}
