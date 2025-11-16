using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfCrazyZoo.Models
{
    public class StoredAnimal : Animal
    {
        public StoredAnimal(string name, int age, AnimalKind kind) : base(name, age, kind) { }
        public override string MakeSound() { return ""; }
    }
}
