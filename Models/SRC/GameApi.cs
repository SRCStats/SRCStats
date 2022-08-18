using System.Text.Json.Serialization;

namespace SRCStats.Models.SRC
{
    public class GameApi
    {
        public class GameApiData
        {
            public GameAPI[] Data { get; set; }
            public Pagination Pagination { get; set; }
        }

        public class Pagination
        {
            public int Offset { get; set; }
            public int Max { get; set; }
            public int Size { get; set; }
            public object[] Links { get; set; }
        }

        public class GameAPI
        {
            public string Id { get; set; }
            public Moderators Moderators { get; set; }
            public Assets Assets { get; set; }
        }

        public class Moderators
        {
            public UserAPI[] Data { get; set; }
        }

        public class Assets
        {
            public Logo Logo { get; set; }
            public CoverTiny CoverTiny { get; set; }
            public CoverSmall CoverSmall { get; set; }
            public CoverMedium CoverMedium { get; set; }
            [JsonPropertyName("cover-large")]
            public CoverLarge CoverLarge { get; set; }
            public Icon1 Icon { get; set; }
            [JsonPropertyName("trophy-1st")]
            public Trophy1St Trophy1st { get; set; }
            [JsonPropertyName("trophy-2nd")]
            public Trophy2Nd Trophy2nd { get; set; }
            [JsonPropertyName("trophy-3rd")]
            public Trophy3Rd Trophy3rd { get; set; }
            [JsonPropertyName("trophy-4th")]
            public Trophy4Th Trophy4th { get; set; }
            public Background Background { get; set; }
            public Foreground Foreground { get; set; }
        }

        public class Logo
        {
            public string Uri { get; set; }
        }

        public class CoverTiny
        {
            public string Uri { get; set; }
        }

        public class CoverSmall
        {
            public string Uri { get; set; }
        }

        public class CoverMedium
        {
            public string Uri { get; set; }
        }

        public class CoverLarge
        {
            public string Uri { get; set; }
        }

        public class Icon1
        {
            public string Uri { get; set; }
        }

        public class Trophy1St
        {
            public string Uri { get; set; }
        }

        public class Trophy2Nd
        {
            public string Uri { get; set; }
        }

        public class Trophy3Rd
        {
            public string Uri { get; set; }
        }

        public class Trophy4Th
        {
            public string Uri { get; set; }
        }

        public class Background
        {
            public string Uri { get; set; }
        }

        public class Foreground
        {
            public string Uri { get; set; }
        }

        public class Link1
        {
            public string Rel { get; set; }
            public string Uri { get; set; }
        }

    }
}
