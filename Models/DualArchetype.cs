using System.ComponentModel.DataAnnotations;

namespace SRCStats.Models
{
    public class DualArchetype
    {
        [Key]
        public int Id { get; set; }
        public int? Value { get; set; }
        public string? OptionalParam { get; set; }
        [Required]
        public ArchetypeMeta ArchetypeMeta { get; set; }
    }
}
