using System.ComponentModel.DataAnnotations;

namespace SRCStats.Models
{
    public class TrophyMeta
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public bool IsSecret { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Quote { get; set; }
        public int Order { get; set; }

        public List<Trophy> Trophies { get; set; }
    }
}