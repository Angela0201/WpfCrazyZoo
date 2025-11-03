using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfCrazyZoo.Models;

namespace WpfCrazyZoo.Repositories
{
    public class AnimalRepository : IRepository<Animal>
    {
        private readonly List<Animal> items = new List<Animal>();

        public void Add(Animal item) => items.Add(item);
        public void Remove(Animal item) => items.Remove(item);
        public IEnumerable<Animal> GetAll() => items;
        public Animal Find(Func<Animal, bool> predicate) => items.FirstOrDefault(predicate);
    }
}
