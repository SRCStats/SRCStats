using System.ComponentModel.DataAnnotations;

namespace SRCStats.Models.SRC
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        public string SiteId { get; set; }
        public string[] Moderators { get; set; }
        public double ThemeRate { get; set; }
    }
}
