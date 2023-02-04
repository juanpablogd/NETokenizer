using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace tesTokenizer
{
    public class Program
    {
        [DllImport("tokenizers.dll")]
        static extern Int32 add_numbers(Int32 number1, Int32 number2);
        static void Main(string[] args)
        {   //Function Test
            var addedNumbers = add_numbers(10, 5);
            Console.WriteLine(addedNumbers);
        }
    }
}
