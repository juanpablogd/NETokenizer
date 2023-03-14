using NeTokenizer;
using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace tesTokenizer
{
    public class Program
    {
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
        
        public static string stantardJson(string input)
        {
            Regex regex = new Regex(@"(\{.*\]\})", RegexOptions.Compiled);
            Match txt_match =  regex.Match(input);
            return txt_match.Value;
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
        {
            // create tokenizer
            var tokenizer = new Tokenizer(@"dccuchile/bert-base-spanish-wwm-cased");
            //var encoded = tokenizer.Encode("prueba texto encode");
            var encodeStruct = tokenizer.EncodeStruct("prueba texto encode");
  
            //var tokenizerPtr = create_tokenizer(@"dccuchile/bert-base-spanish-wwm-cased");

            //var tokenizerPtr = create_tokenizer(@"dccuchile/bert-base-spanish-wwm-cased");
            // var tokenizerPtr = create_tokenizer_local(@"pre_trained\bert-base-spanish-wwm-cased\tokenizer.json");


            //for (int i = 0; i < 1000; i++)
            //{
            //    test_encoder_v5(tokenizerPtr, "Carlos Fonseca");
            //}

            Console.ReadLine();
        }
    }
}
