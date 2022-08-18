using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Newtonsoft.Json;

using SRCStats.Data;
using SRCStats.Models.SRC;

namespace SRCStats.Models
{
    public class User
    {
        // todo: don't store enumerables on user, make new table for them, don't store runs or anything, only the calculations from them as an archetype
        [Key]
        public int Id { get; set; }
        [Required]
        public string SiteId { get; set; }
        [Required]
        public string Name { get; set; }
        public Location? Location { get; set; }
        public string? Pronouns { get; set; }
        [Required]
        public string Role { get; set; } = "user";
        [Required]
        public NameStyle NameStyle { get; set; } = new NameStyle();
        public DateTime? SignUpDate { get; set; }
        public Uri? Icon { get; set; }
        public Uri? Image { get; set; }
        public Uri? SupporterIcon { get; set; }
        [NotMapped]
        public IEnumerable<Run> Runs { get; set; } = new List<Run>().AsEnumerable();
        [NotMapped]
        public IEnumerable<Game> ModeratedGames { get; set; } = new List<Game>().AsEnumerable();

        [Required]
        public int FullGameRuns { get; set; }
        [Required]
        public int IndividualLevelRuns { get; set; }
        [Required]
        public int ForumPosts { get; set; }
        [NotMapped]
        public IEnumerable<Run> RunsVerified { get; set; } = new List<Run>().AsEnumerable();
        public long TotalRunTime { get; set; }
        public float TotalVerifiedTime { get; set; }
        public int VerifiedRuns { get; set; }
        public int RejectedRuns { get; set; }
        public int GuidesCreated { get; set; }
        public bool IsSupporter { get; set; }

        public IEnumerable<Archetype> Archetypes { get; set; } = new List<Archetype>().AsEnumerable();
        public IEnumerable<DualArchetype> DualArchetypes { get; set; } = new List<DualArchetype>().AsEnumerable();
        public IEnumerable<Trophy> Trophies { get; set; } = new List<Trophy>().AsEnumerable();

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
