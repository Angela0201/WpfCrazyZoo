using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrazyZoo.Domain.Interfaces;

namespace CrazyZoo.Domain.Models
{
    public class StoredAnimal : Animal, CrazyZoo.Domain.Interfaces.ICrazyAction
    {
        private bool isFlying;

        public StoredAnimal(string name, int age, AnimalKind kind) : base(name, age, kind)
        {
            isFlying = false;
        }

        public override string MakeSound()
        {
            switch (Kind)
            {
                case AnimalKind.Cat: return "Meow!";
                case AnimalKind.Dog: return "Woof!";
                case AnimalKind.Bird: return "CHIRP!!!";
                default: return "";
            }
        }

        public string ActCrazy()
        {
            if (Kind == AnimalKind.Cat)
                return Name + " stole cheese from the kitchen!";

            if (Kind == AnimalKind.Dog)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < 5; i++)
                {
                    if (i > 0) sb.Append(' ');
                    sb.Append("Woof!");
                }
                return Name + " is barking: " + sb.ToString();
            }

            if (Kind == AnimalKind.Bird)
            {
                isFlying = !isFlying;
                var state = isFlying ? "is flying" : "is not flying";
                return Name + " " + state + " and screams CHIRP!!!";
            }

            return "No crazy action.";
        }
    }
}
