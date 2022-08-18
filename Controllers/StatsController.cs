using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

using SRCStats.Data;
using SRCStats.Models;

namespace SRCStats.Controllers
{
    public class StatsController : Controller
    {
        private readonly StatsDbContext _db;
        IMemoryCache _cache;

        public StatsController(StatsDbContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        [HttpGet]
        [Route("User/{Username?}")]
        public IActionResult Users(string Username)
        {
            if (Username == null)
            {
                return View();
            }
            if (_cache.TryGetValue(Username, out User? user))
            {
                Debug.WriteLine($"User {Username} found in cache.");
            }
            else
            {
                user = _db.Users.Include(x => x.Location.Country.Names).Include(x => x.NameStyle).ThenInclude(style => style.ColorTo).Include(x => x.NameStyle).ThenInclude(style => style.ColorFrom).Include(x => x.NameStyle).ThenInclude(style => style.Color).Include(x => x.Archetypes).ThenInclude(x => x.ArchetypeMeta).Include(x => x.DualArchetypes).ThenInclude(x => x.ArchetypeMeta).Include(x => x.Trophies).ThenInclude(x => x.TrophyMeta).Where(x => x.Name.Equals(Username)).FirstOrDefault();
            }
            if (user != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24));
                _cache.Set(Username, user, cacheOptions);
                // todo: make this a partial view?
                return View(user);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
