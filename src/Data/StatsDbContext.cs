using Microsoft.EntityFrameworkCore;

using SRCStats.Models;
using SRCStats.Models.SRC;

namespace SRCStats.Data
{
    public class StatsDbContext : DbContext
    {
        public StatsDbContext(DbContextOptions<StatsDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<ArchetypeMeta> ArchetypeMetas { get; set; }
        public DbSet<TrophyMeta> TrophyMetas { get; set; }
        public DbSet<InProgress> InProgress { get; set; }
    }
}
