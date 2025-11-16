using System.Data.Entity;

namespace CrazyZoo.Infrastructure.Data
{
    public class ZooDbContext : DbContext
    {
        public ZooDbContext() : base("ZooDbContext") { }
        public DbSet<AnimalEntity> Animals { get; set; }
    }
}
