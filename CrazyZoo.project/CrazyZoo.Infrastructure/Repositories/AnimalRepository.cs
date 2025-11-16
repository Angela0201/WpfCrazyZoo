using System;
using System.Collections.Generic;
using System.Linq;
using CrazyZoo.Domain.Models;
using CrazyZoo.Domain.Interfaces;

namespace CrazyZoo.Infrastructure.Repositories
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
