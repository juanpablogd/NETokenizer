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
            var tokenizer = Tokenizer.FromFile(@"tokenizer.json");
            
            // measure time taken by encode
            var watch = System.Diagnostics.Stopwatch.StartNew();

            const string inputText= "La inteligencia artificial (IA), en el contexto de las ciencias de la computación, es el conjunto de sistemas o combinación de algoritmos, cuyo propósito es crear máquinas que imitan la inteligencia humana para realizar tareas y pueden mejorar conforme la información que recopilan.";
            var encodeStruct = tokenizer.Encode(inputText, includeSpecialTokens: true, padToMax: 60);

            var decoded = tokenizer.Decode(encodeStruct.Ids);

            watch.Stop();

            // print input text using yellow color
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Input text: {inputText} -----");

            // print encoded ids using green color
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Encoded ids: {string.Join(" ", encodeStruct.Ids)} -----");

            
            // print encoded tokens using yellow color
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Encoded tokens: {string.Join(" ", encodeStruct.Tokens)} -----");


            // print decoded text using cyan
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Decoded text: {decoded} -----");

            // print elapsed time
            Console.WriteLine($"Elapsed time: {watch.ElapsedMilliseconds} ms -----");

            Console.ReadLine();
        }
    }
}
