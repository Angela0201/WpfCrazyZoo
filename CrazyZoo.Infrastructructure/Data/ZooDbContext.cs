using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace CrazyZoo.Infrastructure.Data
{
    public class ZooDbContext : DbContext
    {
        public ZooDbContext() : base("ZooDbContext") { }
        public DbSet<AnimalEntity> Animals { get; set; }
    }
}
