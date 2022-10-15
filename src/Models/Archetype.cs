using System.ComponentModel.DataAnnotations;

namespace SRCStats.Models
{
    public class Archetype
    {
        [Key]
        public int Id { get; set; }
        public int? Value { get; set; }
        public string? OptionalParam { get; set; }
        [Required]
        public ArchetypeMeta ArchetypeMeta { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}
