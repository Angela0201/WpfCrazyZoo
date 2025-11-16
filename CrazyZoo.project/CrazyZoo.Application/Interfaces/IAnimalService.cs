using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrazyZoo.Domain.Models;

namespace CrazyZoo.Application.Interfaces
{
    public interface IAnimalService
    {
        IEnumerable<Animal> GetAll();
        void Add(Animal animal);
        void Remove(Animal animal);
    }
}
