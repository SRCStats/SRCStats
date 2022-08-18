using System.Diagnostics;

using Newtonsoft.Json;

using SRCStats.Models;
using SRCStats.Models.SRC;

namespace SRCStats.Data
{
    public class Archetypes
    {
        public static bool InitializeArchetypes(StatsDbContext _db)
        {
            if (_db.ArchetypeMetas.Any()) return true;
            _db.ArchetypeMetas.AddRange(new List<ArchetypeMeta>()
            {
                new ArchetypeMeta()
                {
                    Id = "besties",
                    IsDual = false,
                    Name = "Besties",
                    Description = "A calculation of how many games you share moderation with one specific person on.",
                    MaxDesc = "You're inseperable from your bestie, {0}. Whatever games you mod, they mod with you."
                },
                new ArchetypeMeta()
                {
                    Id = "modcount",
                    IsDual = true,
                    Name = "Normal",
                    Description = "A calculation of the sizes of game moderation teams that you're a part of.",
                    MaxName = "It's a Party!",
                    MaxDesc = "You sure like to be part of big moderation teams. Are you sure you manage to get things done?",
                    MinName = "Alone",
                    MinDesc = "You don't need any support to moderate your games. Other people just tend to get in your way, anyways."
                },
                new ArchetypeMeta()
                {
                    Id = "verifier",
                    IsDual = true,
                    Name = "Normal",
                    Description = "A calculation of how many runs you verify for the games you moderate.",
                    MaxName = "Greedy",
                    MaxDesc = "Save some runs for the rest of your mod team!",
                    MinName = "Lazy",
                    MinDesc = "Are you just a mod for show? Get out there and do your job."
                },
                new ArchetypeMeta()
                {
                    Id = "forumruns",
                    IsDual = true,
                    Name = "Normal",
                    Description = "A calculation of how many forum posts you make compared to the runs you submit.",
                    MaxName = "Strictly Business",
                    MaxDesc = "You have no time for some phpBB board, you have some world records to take.",
                    MinName = "Forum Patroller",
                    MinDesc = "I promise you aren't as cool as you think you are."
                },
                new ArchetypeMeta()
                {
                    Id = "ranking",
                    IsDual = true,
                    Name = "Average",
                    Description = "A calculation of your average ranking on leaderboards.",
                    MaxName = "Power Hungry",
                    MaxDesc = "Either you're really good at the games you play, or you don't submit runs if they aren't top 3...",
                    MinName = "Cozy at the Bottom",
                    MinDesc = "There's no stress, speedrunning is just for fun to you! ...Or maybe you're just bad."
                },
                new ArchetypeMeta()
                {
                    Id = "singlecoop",
                    IsDual = true,
                    Name = "Normal",
                    Description = "A ratio of Single Player runs to Co-op runs",
                    MaxName = "Popular",
                    MaxDesc = "Running with you must be a guaranteed WR! Either that or you have abandonment issues.",
                    MinName = "Loner",
                    MinDesc = "You should really try running Co-op with friends sometime... if you have any."
                },
                new ArchetypeMeta()
                {
                    Id = "child",
                    IsDual = false,
                    Name = "Child",
                    Description = "A ratio of runs performed on Mobile versus other platforms.",
                    MaxDesc = "Remember not to stay up past your bedtime while you're running Jetpack Joyride! :D"
                },
                new ArchetypeMeta()
                {
                    Id = "indiemain",
                    IsDual = true,
                    Name = "Normal",
                    Description = "A ratio of games ran based on their runner count.",
                    MaxName = "Mainstream Runner",
                    MaxDesc = "\"no dude you just don't get it this game has so many one-frames it's crazy i'm gonna be the next kanno of this game\"",
                    MinName = "Indie Appreciator",
                    MinDesc = "No one understands \"Bigfoot: Collision Course\" like you do...."
                },
                new ArchetypeMeta()
                {
                    Id = "themer",
                    IsDual = false,
                    Name = "Theme Appreciator",
                    Description = "A ratio of moderated games with customized themes.",
                    MaxDesc = "Every good game needs a good theme, and no one understands that like you do."
                },
                new ArchetypeMeta()
                {
                    Id = "probvalid",
                    IsDual = false,
                    Name = "\"Yeah it's probably valid\"",
                    Description = "A calculation of the amount of long runs verified by you.",
                    MaxDesc = "Are you sure you watched all of those runs to make sure they're valid...?"
                },
                new ArchetypeMeta()
                {
                    Id = "farmer",
                    IsDual = false,
                    Name = "Mod Farmer",
                    Description = "A calculation of games that were likely mod farmed by you.",
                    MaxDesc = "Everyone has a hobby, yours is just a bad one. Maybe try knitting."
                },
                new ArchetypeMeta()
                {
                    Id = "monitor",
                    IsDual = false,
                    Name = "Hall Monitor",
                    Description = "A ratio of forum posts on games you don't run.",
                    MaxDesc = "Maybe you should let the community be involved in those things..."
                }
            });
            _db.SaveChanges();
            return true;
        }

        public static async Task<Archetype[]> PopulateArchetypes(StatsDbContext _db, User user, Run[] runs, Run[] verifiedRuns, Game[] moderatedGames, IProgress<List<int>>? progress = null)
        {
            var archetypes = new List<Archetype>();
            foreach (var archetypeMeta in _db.ArchetypeMetas)
            {
                Archetype archetype = new()
                {
                    ArchetypeMeta = archetypeMeta
                };
                switch (archetypeMeta.Id)
                {
                    case "besties":
                        var modIds = new List<string>();
                        foreach (var game in moderatedGames)
                        {
                            foreach (var mod in game.Moderators)
                            {
                                modIds.Add(mod);
                            }
                        }
                        if (modIds.Count == 0)
                        {
                            archetype.Value = 0;
                            archetype.OptionalParam = "nobody";
                            break;
                        }
                        var bestie = modIds.Count != 1 ? modIds.GroupBy(i => i).OrderByDescending(group => group.Count()).Select(group => group.Key).Where(x => x != null && x != user.Name).First() : "nobody";
                        int count = modIds.Where(x => x == bestie).Count();
                        archetype.Value = (int)Math.Round((double)((decimal)count / (decimal)moderatedGames.Length) * 100);
                        archetype.OptionalParam = bestie;
                        break;
                    case "child":
                        archetype.Value = runs.Length == 0 ? 0 : (int)Math.Round((((decimal)runs.Where(x => x.IsMobile == true).Count() / (decimal)runs.Length) * 100));
                        break;
                    case "farmer":
                        int farmed = 0;
                        foreach (var game in moderatedGames)
                        {
                            if (runs.Where(x => x.GameId == game.SiteId).Count() <= 2)
                            {
                                farmed++;
                            }
                        }
                        archetype.Value = farmed == 0 ? 0 : (int)Math.Round((double)((decimal)farmed / (decimal)moderatedGames.Length) * 100);
                        break;
                    case "monitor":
                        // not implemented yet
                        archetype.Value = 0;
                        break;
                    case "probvalid":
                        int calc = (int)Math.Round(((decimal)user.TotalVerifiedTime / 691200) * 100);
                        archetype.Value = calc > 100 ? 100 : calc;
                        break;
                    case "themer":
                        if (!moderatedGames.Any())
                        {
                            archetype.Value = 0;
                            break;
                        }
                        double total = 0;
                        foreach (var game in moderatedGames)
                        {
                            total += game.ThemeRate;
                        }
                        archetype.Value = (int)Math.Round((double)total / (moderatedGames.Length * 10) * 100);
                        break;
                    default:
                        break;
                }
                if (archetype.Value != null)
                    archetypes.Add(archetype);
            }
            return archetypes.ToArray();
        }

        public static async Task<DualArchetype[]> PopulateDualArchetypes(StatsDbContext _db, User user, Run[] runs, Run[] verifiedRuns, Game[] moderatedGames, IProgress<List<int>>? progress = null)
        {
            var archetypes = new List<DualArchetype>();
            foreach (var archetypeMeta in _db.ArchetypeMetas)
            {
                DualArchetype archetype = new()
                {
                    ArchetypeMeta = archetypeMeta
                };
                switch (archetypeMeta.Id)
                {
                    case "forumruns":
                        int forumPosts = user.ForumPosts;
                        int runCount = user.Runs.Count();
                        archetype.Value = (forumPosts + runCount == 0) ? 0 : (int)Math.Round((double)(((decimal)runCount / (decimal)(forumPosts + runCount)) * 200) - 100);
                        break;
                    case "indiemain":
                        var aI = new APIHandler();
                        long totalRunners = (await aI.Main("GetGameRunCount", arg3: user.Runs, progress: progress)).Long ?? user.Runs.Count();
                        try
                        {
                            archetype.Value = (int)Math.Round(((decimal)totalRunners / (runs.Count() * 1000)) * 200) - 100;
                            archetype.Value = archetype.Value > 100 ? 100 : archetype.Value;
                        }
                        catch
                        {
                            archetype.Value = 0;
                        }
                        break;
                    case "modcount":
                        int mods = 0;
                        int weight = 5;
                        foreach (var game in moderatedGames)
                        {
                            mods += game.Moderators.Length;
                        }
                        // todo: i hate this math but i just cant think of another algorithm
                        archetype.Value = mods == 0 ? 0 : (int)Math.Round((double)((decimal)(mods - moderatedGames.Length) / ((decimal)moderatedGames.Length * weight)) * 200) - 100;
                        archetype.Value = archetype.Value > 100 ? 100 : archetype.Value;
                        break;
                    case "ranking":
                        var aR = new APIHandler();
                        var records = (await aR.Main("GetRecords", user.SiteId, arg3: user.Runs, progress: progress)).Records;
                        List<decimal> placements = new List<decimal>();
                        foreach (var record in records)
                        {
                            var run = record.Value.Where(x => x.Run.Players.Where(x => x.Id == user.SiteId).Any()).FirstOrDefault();
                            if (run != null)
                            {
                                placements.Add((decimal)run.Place / (decimal)record.Value.Length);
                            }
                        }
                        var total = placements.Sum();
                        archetype.Value = (int)Math.Round((Math.Abs(total - placements.Count) / placements.Count) * 200) - 100;
                        break;
                    case "singlecoop":
                        archetype.Value = runs.Length == 0 ? 0 : (int)Math.Round((double)((decimal)runs.Where(x => x.IsCoop == true).Count() / (decimal)runs.Length) * 200) - 100;
                        break;
                    case "verifier":
                        // not implemented yet
                        archetype.Value = 0;
                        break;
                    default:
                        break;
                }
                if (archetype.Value != null)
                    archetypes.Add(archetype);
            }
            return archetypes.ToArray();
        }
    }
}
