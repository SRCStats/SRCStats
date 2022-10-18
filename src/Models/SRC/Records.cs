using static SRCStats.Models.SRC.RunApi;

namespace SRCStats.Models.SRC
{
    public class Records
    {
        public class RecordData
        {
            public RecordAPI[] Data { get; set; }
            public Pagination Pagination { get; set; }
        }

        public class Pagination
        {
            public int Offset { get; set; }
            public int Max { get; set; }
            public int Size { get; set; }
            public object[] Links { get; set; }
        }

        public class RecordAPI
        {
            public string Category { get; set; }
            public string Level { get; set; }
            public Runs[] Runs { get; set; }
        }

        public class Runs
        {
            public int Place { get; set; }
            public RunAPI Run { get; set; }
        }
    }
}
