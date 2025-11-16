using System.Text;
using CrazyZoo.Domain.Interfaces;

namespace CrazyZoo.Domain.Models
{
    public class Dog : Animal, ICrazyAction
    {
        public Dog(string name, int age) : base(name, age, AnimalKind.Dog) { }

        public override string MakeSound()
        {
            return "Woof!";
        }

        public string ActCrazy()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                if (i > 0) sb.Append(' ');
                sb.Append("Woof!");
            }
            return Name + " is barking: " + sb.ToString();
        }
    }
}
