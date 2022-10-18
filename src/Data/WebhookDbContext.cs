namespace SRCStats.Data
{
    public class WebhookDbContext
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; } = "srcstats";

        public string WebhooksCollectionName { get; set; } = "webhook-list";
    }
}
