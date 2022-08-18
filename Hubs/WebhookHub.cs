using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SRCStats.Data;
using SRCStats.Models;
using SRCStats.Models.SRC;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SRCStats.Hubs
{
    public class WebhookHub : Hub
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // todo: support abbreviations
        public async Task GetGame(string game)
        {
            var aG = new APIHandler();
            var result = (await aG.Main("GetGame", game)).FullGames;
            var bestMatch = result.Where(x => String.Equals(x.Names.International, game, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() ?? result.OrderByDescending(x => x.BoostDistinctDonors).FirstOrDefault();
            if (bestMatch != null)
                Clients.Client(Context.ConnectionId).SendAsync("ConfirmGame", bestMatch);
            else
                Clients.Clients(Context.ConnectionId).SendAsync("NotAGame");
        }

        public async Task GetUser(string user)
        {
            var aU = new APIHandler();
            var result = (await aU.Main("GetUser", user)).User;
            if (result != null && string.Equals(result.Names.International, user, StringComparison.InvariantCultureIgnoreCase))
                Clients.Client(Context.ConnectionId).SendAsync("ConfirmUser", new { result.Names.International, result.Id });
            else
                Clients.Client(Context.ConnectionId).SendAsync("NotAUser");
        }
    }
}
