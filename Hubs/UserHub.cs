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
    public class UserHub : Hub
    {
        private readonly StatsDbContext _db;
        IMemoryCache _cache;

        public UserHub(StatsDbContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        public async Task FetchUser(string userName, bool force)
        {
            // fetch user & _fedata
            // fetch user's runs
            // fetch runs verified
            // fetch games moderated
            // fetch pending runs
            // fetch game _fedatas
            APIHandler a = new();
            if (string.IsNullOrWhiteSpace(userName))
            {
                Clients.Client(Context.ConnectionId).SendAsync("Error", "No username was inputted!");
                return;
            }
            if (_db.InProgress.Where(x => x.Type == "user" && x.Name == userName).Any())
            {
                Clients.Client(Context.ConnectionId).SendAsync("Error", "That user is currently being fetched already! Please try searching for another user or wait for a few minutes and try again.");
                return;
            }
            else
            {
                Clients.Client(Context.ConnectionId).SendAsync("Init");
                _db.InProgress.Add(new InProgress { Type = "user", Name = userName });
                await _db.SaveChangesAsync();
            }
            var userCheck = _db.Users.Include(x => x.Location.Country.Names).Include(x => x.NameStyle).ThenInclude(style => style.ColorTo).Include(x => x.NameStyle).ThenInclude(style => style.ColorFrom).Include(x => x.Archetypes).ThenInclude(x => x.ArchetypeMeta).Include(x => x.DualArchetypes).ThenInclude(x => x.ArchetypeMeta).Include(x => x.Trophies).ThenInclude(x => x.TrophyMeta).Where(x => x.Name.Equals(userName)).FirstOrDefault();
            if (userCheck != null)
            {
                // force a search if specified in post
                if (!force)
                {
                    _db.RemoveRange(_db.InProgress.Where(x => x.Type == "user" && x.Name == userName));
                    await _db.SaveChangesAsync();
                    Clients.Client(Context.ConnectionId).SendAsync("Complete", userCheck.Name);
                    return;
                }
                else
                {
                    // todo: have cascade deletion happen here
                    _db.Users.Remove(userCheck);
                }
            }
            await _db.SaveChangesAsync();
            var progress = new Progress<List<int>>(percent =>
            {
                if (percent[0] == -16614)
                    Clients.Client(Context.ConnectionId).SendAsync("AwaitingThread", percent[0]);
                else
                    Clients.Client(Context.ConnectionId).SendAsync("ReceiveProgress", percent[0], percent[1], 1);
            });
            Clients.Client(Context.ConnectionId).SendAsync("ReceiveProgress", 0, 2, 1);
            var _user = (await a.Main("GetUser", userName, progress: progress)).User;
            if (_user == null)
            {
                Clients.Client(Context.ConnectionId).SendAsync("Error", "There is no user with that name on the site!");
                _db.RemoveRange(_db.InProgress.Where(x => x.Type == "user" && x.Name == userName));
                await _db.SaveChangesAsync();
                return;
            }
            User user = new()
            {
                SiteId = _user.Id,
                Name = _user.Names.International ?? throw new ArgumentException("UserAPI.Names.International returned null!"),
                Location = _user.Location,
                Pronouns = _user.Pronouns,
                Role = _user.Role,
                NameStyle = _user.NameStyle,
                SignUpDate = _user.Signup,
                Image = _user.Assets.Image?.Uri,
                Icon = _user.Assets.Icon?.Uri,
                SupporterIcon = _user.Assets.SupporterIcon?.Uri
            };
            Clients.Client(Context.ConnectionId).SendAsync("ReceiveProgress", 1, 2, 1);

            var returnVal = (await a.Main("GetUserStats", user.SiteId, progress: progress)).UserStats;
            JsonNode stats = returnVal.Item1;
            JArray modStatsArray = returnVal.Item2 ?? new JArray();
            try
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                user.ForumPosts = stats["userStats"]["totalComments"].GetValue<int>();
                user.TotalRunTime = stats["userStats"]["totalRunTime"].GetValue<long>();
                user.GuidesCreated = stats["userStats"]["guidesCreated"].GetValue<int>();
                user.IsSupporter = stats["user"]["isSupporter"].GetValue<bool>();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
            catch
            {
                user.ForumPosts = 0;
                user.TotalRunTime = 0;
                user.GuidesCreated = 0;
                user.IsSupporter = false;
            }
            Clients.Client(Context.ConnectionId).SendAsync("ReceiveProgress", 2, 2, 1);

            progress = new Progress<List<int>>(percent =>
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (stats != null && percent[0] < stats["userStats"]["totalRuns"].GetValue<int>())
                    percent[1] = stats["userStats"]["totalRuns"].GetValue<int>();
                if (percent[0] == -16614)
                    // todo: check if we actually need percent[0] passed through, i dont think we do but im too scared to change it rn
                    Clients.Client(Context.ConnectionId).SendAsync("AwaitingThread", percent[0]);
                else
                    Clients.Client(Context.ConnectionId).SendAsync("ReceiveProgress", percent[0] == 0 ? 1 : percent[0], percent[1], 2);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            });
            var runs = (await a.Main("GetUserRuns", user.SiteId, progress: progress)).Runs;
            foreach (var run in runs)
            {
                if (user.Runs == null)
                {
                    var firstRun = new Run
                    {
                        SiteId = run.Id,
                        GameId = run.Game,
                        CategoryId = run.Category,
                        LevelId = run.Level,
                        PlatformId = run.System.Platform,
                        IsCoop = run.Players.Length > 1,
                        IsMobile = run.System.Platform == "gde3xgek" || run.System.Platform == "lq60nl94",
                        Verified = run.Status.RunStatus == "verified"
                    };
                    user.Runs = new List<Run> { firstRun }.AsEnumerable();
                }
                else
                {
                    user.Runs = user.Runs.Concat(new List<Run>
                    {
                        new Run
                        {
                            SiteId = run.Id,
                            GameId = run.Game,
                            CategoryId = run.Category,
                            LevelId = run.Level,
                            PlatformId = run.System.Platform,
                            IsCoop = run.Players.Length > 1,
                            IsMobile = run.System.Platform == "gde3xgek" || run.System.Platform == "lq60nl94",
                            Verified = run.Status.RunStatus == "verified"
                        }
                    }.AsEnumerable());
                }
            }
            if (user.Runs != null)
            {
                user.FullGameRuns = user.Runs.Where(x => x.LevelId == null).Count();
                user.IndividualLevelRuns = user.Runs.Where(x => x.LevelId != null).Count();
                user.Runs = user.Runs.ToList();
            }
            else
            {
                user.FullGameRuns = 0;
                user.IndividualLevelRuns = 0;
                user.Runs = new List<Run>().AsEnumerable();
            }

            int _verified = 0;
            if (stats != null && modStatsArray != null)
            {
                foreach (var game in modStatsArray)
                {
                    _verified += (int)(game["totalRuns"] ?? 0);
                }
            }
            progress = new Progress<List<int>>(percent =>
            {
                if (percent[1] < _verified)
                    percent[1] = _verified;
                if (percent[0] == -16614)
                    Clients.Client(Context.ConnectionId).SendAsync("AwaitingThread", percent[0]);
                else
                    Clients.Client(Context.ConnectionId).SendAsync("ReceiveProgress", percent[0] == 0 ? 1 : percent[0], percent[1], 3);
            });
            var runsVerified = (await a.Main("GetRunsVerified", user.SiteId, progress: progress)).Runs;
            foreach (var run in runsVerified)
            {
                user.TotalVerifiedTime += run.Times.Primary_t;
                _ = run.Status.RunStatus == "verified" ? user.VerifiedRuns++ : user.RejectedRuns++;
                if (user.RunsVerified == null)
                {
                    var firstRun = new Run
                    {
                        SiteId = run.Id,
                        GameId = run.Game,
                        CategoryId = run.Category,
                        LevelId = run.Level,
                        PlatformId = run.System.Platform,
                        IsCoop = run.Players.Length > 1,
                        IsMobile = run.System.Platform == "gde3xgek" || run.System.Platform == "lq60nl94",
                        Verified = run.Status.RunStatus == "verified"
                    };
                    user.RunsVerified = new List<Run> { firstRun }.AsEnumerable();
                }
                else
                {
                    user.RunsVerified = user.RunsVerified.Concat(new List<Run>
                    {
                        new Run
                        {
                            SiteId = run.Id,
                            GameId = run.Game,
                            CategoryId = run.Category,
                            LevelId = run.Level,
                            PlatformId = run.System.Platform,
                            IsCoop = run.Players.Length > 1,
                            IsMobile = run.System.Platform == "gde3xgek" || run.System.Platform == "lq60nl94",
                            Verified = run.Status.RunStatus == "verified"
                        }
                    }.AsEnumerable());
                }
            }
            if (user.RunsVerified != null)
            {
                user.RunsVerified = user.RunsVerified.ToList();
            }
            else
            {
                user.RunsVerified = new List<Run>().AsEnumerable();
            }

            progress = new Progress<List<int>>(percent =>
            {
                if (modStatsArray != null && percent[1] < modStatsArray.Count)
                    percent[1] = modStatsArray.Count;
                if (percent[0] == -16614)
                    Clients.Client(Context.ConnectionId).SendAsync("AwaitingThread", percent[0]);
                else
                    Clients.Client(Context.ConnectionId).SendAsync("ReceiveProgress", percent[0] == 0 ? 1 : percent[0], percent[1], 4);
            });
            var gamesModerated = (await a.Main("GetGamesModerated", user.SiteId, progress: progress)).Games;
            foreach (var game in gamesModerated)
            {
                List<string> mods = new();
                var gameTemp = new Game { SiteId = game.Id };
                foreach (var mod in game.Moderators.Data)
                {
                    if (mod.Names?.International != null)
                        mods.Add(mod.Names.International);
                }
                gameTemp.Moderators = mods.ToArray();
                // not implemented yet
                gameTemp.ThemeRate = 10;

                if (user.ModeratedGames == null)
                {
                    user.ModeratedGames = new List<Game> { gameTemp }.AsEnumerable();
                }
                else
                {
                    user.ModeratedGames = user.ModeratedGames.Concat(new List<Game> { gameTemp }.AsEnumerable());
                }
            }

            if (user.ModeratedGames == null)
            {
                user.ModeratedGames = new List<Game>().AsEnumerable();
            }

            /* not needed
            progress = new Progress<List<int>>(percent =>
            {   
                if (percent[0] == -16614)
                    Clients.Client(Context.ConnectionId).SendAsync("AwaitingThread", percent[0]);
                else
                    Clients.Client(Context.ConnectionId).SendAsync("ReceiveProgress", percent[0], percent[1], null);
            });
            */
            user.Archetypes = await Archetypes.PopulateArchetypes(_db, user, (user.Runs ?? new List<Run>()).ToArray(), (user.RunsVerified ?? new List<Run>()).ToArray(), (user.ModeratedGames ?? new List<Game>()).ToArray());

            progress = new Progress<List<int>>(percent =>
            {
                if (percent[0] == -16614)
                    Clients.Client(Context.ConnectionId).SendAsync("AwaitingThread", percent[0]);
                else
                    Clients.Client(Context.ConnectionId).SendAsync("ReceiveProgress", percent?[0], percent?[1], percent?[2]);
            });
            user.DualArchetypes = await Archetypes.PopulateDualArchetypes(_db, user, (user.Runs ?? new List<Run>()).ToArray(), (user.RunsVerified ?? new List<Run>()).ToArray(), (user.ModeratedGames ?? new List<Game>()).ToArray(), progress);

            progress = new Progress<List<int>>(percent =>
            {
                if (percent[0] == -16614)
                    Clients.Client(Context.ConnectionId).SendAsync("AwaitingThread", percent[0]);
                else
                    Clients.Client(Context.ConnectionId).SendAsync("ReceiveProgress", percent[0], percent[1], 7);
            });
            user.Trophies = await Trophies.PopulateTrophies(_db, user, (user.Runs ?? new List<Run>()).ToArray(), (user.RunsVerified ?? new List<Run>()).ToArray(), (user.ModeratedGames ?? new List<Game>()).ToArray(), progress);

            Clients.Client(Context.ConnectionId).SendAsync("Finalize");

            _db.Users.Add(user);
            _db.RemoveRange(_db.InProgress.Where(x => x.Type == "user" && x.Name == user.Name));
            await _db.SaveChangesAsync();

            _cache.Remove(user.Name);
            await Clients.Client(Context.ConnectionId).SendAsync("Complete", user.Name);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
