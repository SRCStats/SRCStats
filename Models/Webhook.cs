using System.ComponentModel.DataAnnotations;

namespace SRCStats.Models
{
    public class Webhook
    {
        [Key]
        public int Id { get; set; }
        public Uri WebhookUrl { get; set; }
        public string? RCategories { get; set; }
        public string? RUsers { get; set; }
        public string? REvents { get; set; }
        public string? VContext { get; set; }
        public string? VIDs { get; set; }
        public string? VEvents { get; set; }
    }
}

