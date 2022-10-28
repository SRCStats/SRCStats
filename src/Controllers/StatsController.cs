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
        [Route("users")]
        public IActionResult Users()
        {
            return View();
        }

        [HttpGet]
        [Route("users/{Username?}")]
        public IActionResult Users(string Username)
        {
            if (_cache.TryGetValue(Username, out User? user))
            {
                Debug.WriteLine($"User {Username} found in cache.");
            }
            else
            {
                user = _db.Users.Include(x => x.Location.Country.Names).Include(x => x.NameStyle).ThenInclude(style => style.ColorTo).Include(x => x.NameStyle).ThenInclude(style => style.ColorFrom).Include(x => x.NameStyle).ThenInclude(style => style.Color).Include(x => x.Archetypes).ThenInclude(x => x.ArchetypeMeta).Include(x => x.DualArchetypes).ThenInclude(x => x.ArchetypeMeta).Include(x => x.Trophies).ThenInclude(x => x.TrophyMeta).Where(x => x.Name.Equals(Username)).FirstOrDefault();
                var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(24));
                _cache.Set(Username, user, cacheOptions);
            }
            if (user != null)
            {
                return View("[Username]", user);
            }
            else
            {
                // todo: implement option to start the search process from the page
                return NotFound();
            }
        }
    }
}
