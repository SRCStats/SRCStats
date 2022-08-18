using System.ComponentModel.DataAnnotations;

namespace SRCStats.Models
{
    public class InProgress
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
