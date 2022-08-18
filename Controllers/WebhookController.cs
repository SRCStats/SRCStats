using Microsoft.AspNetCore.Mvc;
using SRCStats.Data;
using SRCStats.Models;
using System.Diagnostics;

namespace SRCStats.Controllers
{
    public class WebhookController : Controller
    {
        private readonly WebhookDbService _webhookService;

        public WebhookController(WebhookDbService webhookService)
        {
            _webhookService = webhookService;
        }

        [Route("webhook")]
        [Route("webhooks")]
        public IActionResult Webhooks()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Webhook webhook)
        {
            WebhookDb webhookDb = new()
            {
                WebhookUrl = webhook.WebhookUrl,
                Records = new RecordsFieldDb()
                {
                    Categories = webhook.RCategories?.Split(','),
                    Users = webhook.RUsers?.Split(','),
                    Events = webhook.REvents
                }
            };
            await _webhookService.CreateAsync(webhookDb);
            // todo: use correct status code here
            return RedirectToAction("Webhooks");
        }
    }
}
