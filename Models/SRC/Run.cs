using System.ComponentModel.DataAnnotations;

namespace SRCStats.Models.SRC
{
    public class Run
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string SiteId { get; set; }
        [Required]
        public string GameId { get; set; }
        public string? CategoryId { get; set; }
        public string? LevelId { get; set; }
        public string? PlatformId { get; set; }
        public bool? IsCoop { get; set; }
        public bool? IsMobile { get; set; }
        public bool? Verified { get; set; }
    }
}
