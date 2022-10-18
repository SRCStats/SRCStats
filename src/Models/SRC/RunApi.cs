using System.Text.Json.Serialization;

namespace SRCStats.Models.SRC
{
    public class RunApi
    {
        public class RunApiData
        {
            public RunAPI[] Data { get; set; }
            public Pagination Pagination { get; set; }
        }

        public class Pagination
        {
            public int Offset { get; set; }
            public int Max { get; set; }
            public int Size { get; set; }
            public Link[] Links { get; set; }
        }

        public class Link
        {
            public string Rel { get; set; }
            public string Uri { get; set; }
        }

        public class RunAPI
        {
            public string Id { get; set; }
            public string Weblink { get; set; }
            public string Game { get; set; }
            public string Level { get; set; }
            public string Category { get; set; }
            public Videos Videos { get; set; }
            public string Comment { get; set; }
            public Status Status { get; set; }
            public Player[] Players { get; set; }
            public string Date { get; set; }
            public DateTime? Submitted { get; set; }
            public Times Times { get; set; }
            public System System { get; set; }
            public Splits Splits { get; set; }
            public Dictionary<string,string> Values { get; set; }
            public Link2[] Links { get; set; }
        }

        public class Videos
        {
            public Link1[] Links { get; set; }
        }

        public class Link1
        {
            public string Uri { get; set; }
        }

        public class Status
        {
            [JsonPropertyName("status")]
            public string RunStatus { get; set; }
            public string Examiner { get; set; }
            public DateTime VerifyDate { get; set; }
            public string Reason { get; set; }
        }

        public class Times
        {
            public string Primary { get; set; }
            public float Primary_t { get; set; }
            public string RealTime { get; set; }
            public float RealTime_t { get; set; }
            public string RealTime_NoLoads { get; set; }
            public float RealTime_NoLoads_T { get; set; }
            public string InGame { get; set; }
            public float InGame_t { get; set; }
        }

        public class System
        {
            public string Platform { get; set; }
            public bool Emulated { get; set; }
            public string Region { get; set; }
        }

        public class Splits
        {
            public string Rel { get; set; }
            public string Uri { get; set; }
        }

        public class Player
        {
            public string Rel { get; set; }
            public string Id { get; set; }
            public string Uri { get; set; }
        }

        public class Link2
        {
            public string Rel { get; set; }
            public string Uri { get; set; }
        }
    }
}
