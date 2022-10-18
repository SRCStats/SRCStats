using SRCStats.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace SRCStats.Data
{
    public class WebhookDbService
    {
        private readonly IMongoCollection<WebhookDb> _webhookCollection;

        public WebhookDbService(
            IOptions<WebhookDbContext> webhooksDbContext)
        {
            var client = new MongoClient(webhooksDbContext.Value.ConnectionString);
            var database = client.GetDatabase(webhooksDbContext.Value.DatabaseName);
            _webhookCollection = database.GetCollection<WebhookDb>(webhooksDbContext.Value.WebhooksCollectionName);
        }

        public async Task CreateAsync(WebhookDb webhook)
        {
            var updated = await _webhookCollection.FindOneAndUpdateAsync(x => x.WebhookUrl == webhook.WebhookUrl, Builders<WebhookDb>.Update.Set(u => u.Records, webhook.Records).Set(u => u.Verification, webhook.Verification));
            if (updated == null)
                await _webhookCollection.InsertOneAsync(webhook);
        }
    }
}
