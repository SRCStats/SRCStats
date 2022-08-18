namespace SRCStats.Models.SRC
{
    public class FullGameApiData
    {
        public FullGameAPI[] Data { get; set; }
        public Pagination Pagination { get; set; }
    }

    public class Pagination
    {
        public int Offset { get; set; }
        public int Max { get; set; }
        public int Size { get; set; }
    }

    public class FullGameAPI
    {
        public string Id { get; set; }
        public Names Names { get; set; }
        public int BoostReceived { get; set; }
        public int BoostDistinctDonors { get; set; }
        public string Abbreviation { get; set; }
        public string Weblink { get; set; }
        public string Discord { get; set; }
        public int Released { get; set; }
        public string Releasedate { get; set; }
        public Ruleset Ruleset { get; set; }
        public bool Romhack { get; set; }
        public string[] Gametypes { get; set; }
        public string[] Platforms { get; set; }
        public string[] Regions { get; set; }
        public string[] Genres { get; set; }
        public string[] Engines { get; set; }
        public string[] Developers { get; set; }
        public string[] Publishers { get; set; }
        public DateTime? Created { get; set; }
        public Categories Categories { get; set; }
    }

    public class Ruleset
    {
        public bool Showmilliseconds { get; set; }
        public bool Requireverification { get; set; }
        public bool Requirevideo { get; set; }
        public string[] Runtimes { get; set; }
        public string Defaulttime { get; set; }
        public bool Emulatorsallowed { get; set; }
    }

    public class Categories
    {
        public CategoryApi[] Data { get; set; }
    }

    public class CategoryApi
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Weblink { get; set; }
        public string Type { get; set; }
        public string Rules { get; set; }
        public Players Players { get; set; }
        public bool Miscellaneous { get; set; }
    }

    public class Players
    {
        public string Type { get; set; }
        public int Value { get; set; }
    }

}
