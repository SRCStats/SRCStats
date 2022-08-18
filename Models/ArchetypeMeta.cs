using System.ComponentModel.DataAnnotations;

namespace SRCStats.Models
{
    public class ArchetypeMeta
    {
        public string Id { get; set; }
        [Required]
        public bool IsDual { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string MaxDesc { get; set; }
        public string? MaxName { get; set; }
        public string? MinName { get; set; }
        public string? MinDesc { get; set; }

        public List<Archetype> Archetypes { get; set; }
    }
}
