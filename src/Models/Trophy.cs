using System.ComponentModel.DataAnnotations;

namespace SRCStats.Models
{
    public class Trophy
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public bool IsAchieved { get; set; }
        public string? OptionalParam { get; set; }
        [Required]
        public TrophyMeta TrophyMeta { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}
