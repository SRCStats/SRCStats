using Newtonsoft.Json.Linq;
using SRCStats.Models;
using SRCStats.Models.SRC;
using System.Text.Json.Nodes;
using static SRCStats.Models.SRC.GameApi;
using static SRCStats.Models.SRC.Records;
using static SRCStats.Models.SRC.RunApi;

namespace SRCStats.Data
{
    public class APIReturn
    {
        public UserAPI? User { get; set; }
        public Tuple<JsonNode?, JArray?>? UserStats { get; set; }
        public IEnumerable<RunAPI> Runs { get; set; } = new List<RunAPI>().AsEnumerable();
        public IEnumerable<GameAPI> Games { get; set; } = new List<GameAPI>().AsEnumerable();
        public IEnumerable<FullGameAPI> FullGames { get; set; } = new List<FullGameAPI>().AsEnumerable();
        public Dictionary<string, Runs[]> Records { get; set; } = new Dictionary<string, Runs[]>();
        public bool? Bool { get; set; }
        public long? Long { get; set; }
    }
}
