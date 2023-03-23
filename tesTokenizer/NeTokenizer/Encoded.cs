namespace NeTokenizer
{
    public class Encoded
    {
        public string[] Tokens { get; set; }
        public long[] Ids { get; set; }
        public int[] Mask { get; set; }
        public int[] WordIds { get; set; }
    }
}
