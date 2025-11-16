using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfCrazyZoo.Enclosures;
using WpfCrazyZoo.Data;

namespace WpfCrazyZoo.Data
{
    public class ZooDbContext : DbContext
    {
        public ZooDbContext() : base("ZooDbContext") { }
        public DbSet<AnimalEntity> Animals { get; set; }
    }
}
