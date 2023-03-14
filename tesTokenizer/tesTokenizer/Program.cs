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

        public static string stantardJson(string input)
        {
            Regex regex = new Regex(@"(\{.*\]\})", RegexOptions.Compiled);
            Match txt_match =  regex.Match(input);
            return txt_match.Value;
        }
        public static DataEncoding encoding_format(string input)
        {
            DataEncoding data = null;
            Regex regex = new Regex(@"(\{.*\]\})", RegexOptions.Compiled);
            Match txt_match =  regex.Match(input);
            try
            {
                data = JsonConvert.DeserializeObject<DataEncoding>(txt_match.Value);
            }
            catch (JsonReaderException jex)
            {
                Console.WriteLine(jex.Message);
            }
            return data;
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

            
            var tokenizerPtr = create_tokenizer(@"dccuchile/bert-base-spanish-wwm-cased");

            /*test_encoder_v4(tokenizerPtr, "göes to élevên");
            test_encoder_v4(tokenizerPtr, "Carlos FonsecA");
            test_encoder_v4(tokenizerPtr, "garzón Dueñas");*/

            for (int i = 0; i < 1000; i++)
            {
                test_encoder_v5(tokenizerPtr, "Carlos Fonseca");
            }

            //test_encoder_v5(tokenizerPtr, "göes to élevên");
            //test_encoder_v5(tokenizerPtr, "garzón Dueñas");

            //test_encoder_v3("garzón  Dueñas");
            //test_encoder_v3("göes to élevên ] garzón Dueñas");

            Console.ReadLine();
        }

        static void test_encoder_v5(IntPtr tokenizerPtr, string texto)
        {
            //Console.WriteLine(stantardJson(data_result2));
            int iteration = 0;
            DataEncoding daten = null;
            while (iteration < 5)
            {
                IntPtr prTxt2 = encode_v5(tokenizerPtr, texto);
                var data_result2 = PtrToString(prTxt2);
                Console.WriteLine($@"NET {data_result2}");
                daten = encoding_format(data_result2);
                if (daten == null)
                {
                    iteration++;
                    Console.WriteLine($@"ITERATION {iteration}");
                    continue;
                }
                else
                {
                    break;
                }
            }
            
            for(int t = 0; t < daten.ids.Length;t++)
            {
                Console.WriteLine($@"{daten.tokens[t]} - {daten.ids[t]}" );
            }
        }

        static void test_encoder_v4(IntPtr tokenizerPtr, string texto)
        {
            IntPtr prTxt2 = encode_v4(tokenizerPtr, texto);
            var data_result2 = PtrToString(prTxt2);
            //Console.WriteLine(data_result2);
            Console.WriteLine(stantardJson(data_result2));
            //string[] star = StringToArray(stantardJson(data_result2));
            //foreach (var token in star)
            //{
            //    Console.WriteLine(token);
            //}
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
