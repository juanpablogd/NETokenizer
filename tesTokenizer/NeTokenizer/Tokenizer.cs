using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace NeTokenizer
{
    /// <summary>
    /// Huggingface's wrapper of the rust library.
    /// </summary>
    public class Tokenizer
    {
        /// <summary>
        /// Native pointer.
        /// </summary>
        private IntPtr _tokenizerPtr;

        public Tokenizer(string tokenizer_path)
        {
            _tokenizerPtr = TokenizerNative.create_tokenizer(tokenizer_path);
        }

        /// <summary>
        /// Gets the native pointer.
        /// </summary>
        /// <returns>Native rust pointer</returns>
        public IntPtr GetNativePtr()
        {
            return _tokenizerPtr;
        }

        public string EncodeStruct(string text)
        {
            var structNative = TokenizerNative.encode_struct(_tokenizerPtr, text);
            RustArray rustArray = Marshal.PtrToStructure<RustArray>(structNative);

            string tokens = PtrToString(rustArray.tokens);



            return string.Empty;
        }

        public DataEncoding Encode(string text)
        {
            int iteration = 0;
            DataEncoding daten = null;
            while (iteration < 5)
            {
                IntPtr prTxt2 = TokenizerNative.encode(_tokenizerPtr, text);
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

            for (int t = 0; t < daten.ids.Length; t++)
            {
                Console.WriteLine($@"{daten.tokens[t]} - {daten.ids[t]}");
            }
            return daten;
        }

        public static DataEncoding encoding_format(string input)
        {
            DataEncoding data = null;
            Regex regex = new Regex(@"(\{.*\]\})", RegexOptions.Compiled);
            Match txt_match = regex.Match(input);
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
    }
}