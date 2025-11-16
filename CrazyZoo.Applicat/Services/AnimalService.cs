using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrazyZoo.Domain.Models;
using CrazyZoo.Domain.Interfaces;
using CrazyZoo.Application.Interfaces;


namespace CrazyZoo.Application.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly IRepository<Animal> repo;

        public AnimalService(IRepository<Animal> repo)
        {
            this.repo = repo;
        }

        public IEnumerable<Animal> GetAll()
        {
            return repo.GetAll();
        }

        public void Add(Animal animal)
        {
            repo.Add(animal);
        }

        public void Remove(Animal animal)
        {
            repo.Remove(animal);
        }
    }
}
