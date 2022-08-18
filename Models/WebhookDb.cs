namespace SRCStats.Models
{
    public class WebhookDb
    {
        public Uri WebhookUrl { get; set; }
        public RecordsFieldDb? Records { get; set; }
        public VerificationFieldDb? Verification { get; set; }
    }

    public class RecordsFieldDb
    {
        public string[]? Categories { get; set; }
        public string[]? Users { get; set; }
        public string Events { get; set; }
    }

    public class VerificationFieldDb
    {
        public string Context { get; set; }
        public string[] IDs { get; set; }
        public string[] Events { get; set; }
    }
}
