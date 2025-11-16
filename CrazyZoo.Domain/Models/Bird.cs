using CrazyZoo.Domain.Interfaces;

namespace CrazyZoo.Domain.Models
{
    public class Bird : Animal, Flyable, ICrazyAction
    {
        private bool isFlying;

        public Bird(string name, int age) : base(name, age, AnimalKind.Bird)
        {
            isFlying = false;
        }

        public bool IsFlying
        {
            get { return isFlying; }
        }

        public override string MakeSound()
        {
            return "CHIRP!!!";
        }

        public void Fly()
        {
            isFlying = !isFlying;
        }

        public string ActCrazy()
        {
            Fly();
            string state = isFlying ? "is flying" : "is not flying";
            return Name + " " + state + " and screams CHIRP!!!";
        }
    }
}
