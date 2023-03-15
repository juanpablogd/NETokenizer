using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeTokenizer
{
    public class Encoded
    {
        public string[] Tokens { get; set; }
        public int[] Ids { get; set; }
        public int[] Mask { get; set; }
        
    }
}
