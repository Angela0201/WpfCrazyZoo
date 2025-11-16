using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using WpfCrazyZoo.Data;
using WpfCrazyZoo.Models;

namespace WpfCrazyZoo.Repositories
{
    public class EfAnimalRepository : IRepository<Animal>
    {
        private readonly ZooDbContext _ctx;

        public EfAnimalRepository(ZooDbContext ctx)
        {
            _ctx = ctx;
        }

        public void Add(Animal item)
        {
            var e = new AnimalEntity
            {
                Name = item.Name,
                Age = item.Age,
                Kind = (int)item.Kind
            };
            _ctx.Animals.Add(e);
            _ctx.SaveChanges();
        }

        public void Remove(Animal item)
        {
            var toDelete = _ctx.Animals.FirstOrDefault(x =>
                x.Name == item.Name &&
                x.Age == item.Age &&
                x.Kind == (int)item.Kind);
            if (toDelete != null)
            {
                _ctx.Animals.Remove(toDelete);
                _ctx.SaveChanges();
            }
        }

        public IEnumerable<Animal> GetAll()
        {
            var list = _ctx.Animals.AsNoTracking().ToList();
            var result = new List<Animal>();
            foreach (var e in list)
            {
                result.Add(new StoredAnimal(e.Name, e.Age, (AnimalKind)e.Kind));
            }
            return result;
        }

        public Animal Find(Func<Animal, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }
    }
}
