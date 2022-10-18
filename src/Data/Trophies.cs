//
//  THIS FILE CONTAINS SPOILERS FOR SECRET TROPHIES, PLEASE KEEP IT TO YOURSELF :D
//

using SRCStats.Models;
using SRCStats.Models.SRC;

namespace SRCStats.Data
{
    public class Trophies
    {
        public static bool InitializeTrophies(StatsDbContext _db)
        {
            if (_db.TrophyMetas.Any()) return true;
            _db.TrophyMetas.AddRange(new List<TrophyMeta>()
            {
                new TrophyMeta()
                {
                    Id = "prepac",
                    IsSecret = false,
                    Name = "Pre-Pac",
                    Description = "You were around before the Elo takeover.",
                    Quote = "\"Has it ever been the same since then?\"",
                    Order = 1
                },
                new TrophyMeta()
                {
                    Id = "postpac",
                    IsSecret = false,
                    Name = "Post-Pac",
                    Description = "You weren't around before the Elo takeover.",
                    Quote = "\"Did you ever know what peace feels like?\"",
                    Order = 2
                },
                new TrophyMeta()
                {
                    Id = "mentor",
                    IsSecret = false,
                    Name = "Mentor",
                    Description = "You've written guides for a game.",
                    Quote = "\"You probably don't get as much credit as you deserve.\"",
                    Order = 3
                },
                new TrophyMeta()
                {
                    Id = "supporter",
                    IsSecret = false,
                    Name = "Supporter",
                    Description = "You have an active SRC Supporter subscription.",
                    Quote = "\"Please direct your game boosts to https://www.speedrun.com/goocubelets.\"",
                    Order = 4
                },
                new TrophyMeta()
                {
                    Id = "banned",
                    IsSecret = true,
                    Name = "Banned",
                    Description = "You're banned from the site.",
                    Quote = "\"Lucky...\"",
                    Order = 1
                },
                new TrophyMeta()
                {
                    Id = "staff",
                    IsSecret = true,
                    Name = "Elo Staff",
                    Description = "You're an Elo Staff member.",
                    Quote = "\"haha this is all jokes guys right please raise the api rate limit btw\"",
                    Order = 2
                },
                new TrophyMeta()
                {
                    Id = "stafftest",
                    IsSecret = true,
                    Name = "Staff Test Enjoyer",
                    Description = "You have a run in the game \"Staff Test\"",
                    Quote = "\"https://github.com/speedruncomorg/api/issues/113\"",
                    Order = 3
                },
                new TrophyMeta()
                {
                    Id = "pending",
                    IsSecret = true,
                    Name = "\"A new run is awaiting verification.\"",
                    Description = "You have a run in the verification queue for a game you moderate.",
                    Quote = "\"What are you still doing here?\"",
                    Order = 4
                },
                new TrophyMeta()
                {
                    Id = "obscene",
                    IsSecret = true,
                    Name = "Naughty",
                    Description = "Your username is a naughty word :(",
                    Quote = "\"Sick meme bro!\"",
                    Order = 5
                },
                new TrophyMeta()
                {
                    Id = "broken",
                    IsSecret = true,
                    Name = "API Ruiner",
                    Description = "You have ≥ 10000 runs or verified runs.",
                    Quote = "\"Do you have to break everything?\"",
                    Order = 6
                },
                new TrophyMeta()
                {
                    Id = "promoted",
                    IsSecret = true,
                    Name = "Sellout",
                    Description = "You've ran a promoted game on SRC.",
                    Quote = "\"Chase that bag bestie\"",
                    Order = 7
                },
                new TrophyMeta()
                {
                    Id = "milehigh",
                    IsSecret = true,
                    Name = "Mile High Club",
                    Description = "You've done a run on the Airplane Seats platform.",
                    Quote = "\"Only real ones know.\"",
                    Order = 8
                },
                new TrophyMeta()
                {
                    Id = "zodiac",
                    IsSecret = true,
                    Name = "How",
                    Description = "You've done a run on the Zodiac platform.",
                    Quote = "\"how how how how how how\"",
                    Order = 9
                },
                new TrophyMeta()
                {
                    Id = "subsurf",
                    IsSecret = true,
                    Name = "\"I'm so sorry\"",
                    Description = "You're a moderator for Subway Surfers.",
                    Quote = "\"I'm really really sorry.\"",
                    Order = 10
                },
                new TrophyMeta()
                {
                    Id = "jaypin",
                    IsSecret = true,
                    Name = "hello jaypin",
                    Description = "You're Jaypin88.",
                    Quote = "\"If you could actually load this page and all of his data, then SRC upgraded their infrastructure.\"",
                    Order = 11
                },
                new TrophyMeta()
                {
                    Id = "bacon",
                    IsSecret = true,
                    Name = "Meta's Test Game Killer",
                    Description = "You're YUMmy_Bacon5",
                    Quote = "\"You could've just not told him about it bro.\"",
                    Order = 12
                },
                new TrophyMeta()
                {
                    Id = "pac",
                    IsSecret = true,
                    Name = "Hi Dad",
                    Description = "You're Pac.",
                    Quote = "\"I'm not trying to say that the site was perfect under Pac but like you'd really expect a company like Elo to be able to fi-\"",
                    Order = 13
                },
                new TrophyMeta()
                {
                    Id = "owner",
                    IsSecret = true,
                    Name = "Really Cool",
                    Description = "You're really cool.",
                    Quote = "\"Whoa, they're like, really cool.\"",
                    Order = 14
                }
            });
            _db.SaveChanges();
            return true;
        }

        public static async Task<Trophy[]> PopulateTrophies(StatsDbContext _db, User user, Run[] runs, Run[] verifiedRuns, Game[] moderatedGames, IProgress<List<int>>? progress = null)
        {
            var trophies = new List<Trophy>();
            foreach (var trophyMeta in _db.TrophyMetas)
            {
                Trophy trophy = new();
                trophy.TrophyMeta = trophyMeta;
                switch (trophyMeta.Id)
                {
                    case "prepac":
                        trophy.IsAchieved = ((DateTimeOffset)(user.SignUpDate ?? DateTime.UtcNow)).ToUnixTimeSeconds() < 1602606710;
                        break;
                    case "postpac":
                        trophy.IsAchieved = ((DateTimeOffset)(user.SignUpDate ?? DateTime.UtcNow)).ToUnixTimeSeconds() > 1602606710;
                        break;
                    case "mentor":
                        trophy.IsAchieved = user.GuidesCreated > 0;
                        break;
                    case "supporter":
                        trophy.IsAchieved = user.IsSupporter;
                        break;
                    case "banned":
                        trophy.IsAchieved = user.Role == "banned";
                        break;
                    case "staff":
                        trophy.IsAchieved = user.Role == "admin";
                        break;
                    case "stafftest":
                        trophy.IsAchieved = runs.Where(x => x.GameId == "9dowpwe1").Any();
                        break;
                    case "pending":
                        var a = new APIHandler();
                        trophy.IsAchieved = (await a.Main("GetPendingRuns", arg2: moderatedGames, progress: progress)).Bool ?? false;
                        break;
                    case "obscene":
                        foreach (var word in BadWords.badWords)
                        {
                            if (user.Name.Equals(word, StringComparison.InvariantCultureIgnoreCase))
                            {
                                trophy.IsAchieved = true;
                                break;
                            }
                        }
                        break;
                    case "broken":
                        trophy.IsAchieved = runs.Length == 10000 || verifiedRuns.Length == 10000;
                        break;
                    case "promoted":
                        trophy.IsAchieved = runs.Where(x => x.GameId == "m1zk3031").Any();
                        break;
                    case "milehigh":
                        trophy.IsAchieved = runs.Where(x => x.PlatformId == "gz9qv3e0").Any();
                        break;
                    case "zodiac":
                        trophy.IsAchieved = runs.Where(x => x.PlatformId == "mx6pq4e3").Any();
                        break;
                    case "subsurf":
                        trophy.IsAchieved = moderatedGames.Where(x => x.SiteId == "y65797de").Any();
                        break;
                    case "jaypin":
                        trophy.IsAchieved = user.Name.Equals("Jaypin88", StringComparison.InvariantCultureIgnoreCase);
                        break;
                    case "pac":
                        trophy.IsAchieved = user.Name.Equals("Pac", StringComparison.InvariantCultureIgnoreCase);
                        break;
                    case "owner":
                        trophy.IsAchieved = user.Name.Equals("Sex", StringComparison.InvariantCultureIgnoreCase) || user.Name.Equals("Fireball", StringComparison.InvariantCultureIgnoreCase);
                        break;
                    case "bacon":
                        trophy.IsAchieved = user.Name.Equals("YUMmy_Bacon5", StringComparison.InvariantCultureIgnoreCase);
                        break;
                    default:
                        break;
                }
                trophies.Add(trophy);
            }
            return trophies.ToArray();
        }
    }
}