using System.Text.Json.Serialization;

namespace DocumentsAPI.DTOs
{
    public class DocSplitInterval
    {
        public int start { get; set; }
        public int end { get; set; }

        [JsonConstructor]
        public DocSplitInterval() { }

        public DocSplitInterval(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        public static DocSplitInterval[]? FromRanges(string? ranges)
        {

            if (string.IsNullOrEmpty(ranges)) return null;

            string[] rng = ranges.Split(',');
            DocSplitInterval[] result = new DocSplitInterval[rng.Length];

            for (int i = 0; i < rng.Length; i++)
            {
                string[] range_int = rng[i].Split("-");
                result[i] = new DocSplitInterval(int.Parse(range_int[0]), int.Parse(range_int[1]));
            }

            return result;

        }
    }
}
