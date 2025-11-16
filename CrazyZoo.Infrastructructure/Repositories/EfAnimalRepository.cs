using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using CrazyZoo.Infrastructure.Data;
using CrazyZoo.Domain.Models;
using CrazyZoo.Domain.Interfaces;

namespace CrazyZoo.Infrastructure.Repositories
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
            var entities = _ctx.Animals.AsNoTracking().ToList();
            var result = new List<Animal>(entities.Count);

            foreach (var e in entities)
            {
                switch ((AnimalKind)e.Kind)
                {
                    case AnimalKind.Cat:
                        result.Add(new Cat(e.Name, e.Age));
                        break;
                    case AnimalKind.Dog:
                        result.Add(new Dog(e.Name, e.Age));
                        break;
                    case AnimalKind.Bird:
                        result.Add(new Bird(e.Name, e.Age));
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        public Animal Find(Func<Animal, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }
    }
}
