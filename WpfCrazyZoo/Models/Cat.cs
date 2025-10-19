using WpfCrazyZoo.Interfaces;

namespace WpfCrazyZoo.Models
{
    public class Cat : Animal, ICrazyAction
    {
        public Cat(string name, int age) : base(name, age, AnimalKind.Cat) { }

        public override string MakeSound()
        {
            return "Meow!";
        }

        public string ActCrazy()
        {
            return Name + " stole cheese from the kitchen!";
        }
    }
}
