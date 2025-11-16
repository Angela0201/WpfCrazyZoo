using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyZoo.Domain.Models
{
    public class Enclosure<T> where T : Animal
    {
        public string Name { get; set; }
        public List<T> Animals { get; } = new List<T>();

        public event EventHandler<T> AnimalJoinedInSameEnclosure;
        public event EventHandler FoodDropped;

        public Enclosure(string name)
        {
            Name = name;
        }

        public void AddAnimal(T animal)
        {
            Animals.Add(animal);
            AnimalJoinedInSameEnclosure?.Invoke(this, animal);
        }

        public void DropFood()
        {
            FoodDropped?.Invoke(this, EventArgs.Empty);
        }
    }
}
