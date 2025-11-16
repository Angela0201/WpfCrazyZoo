namespace WpfCrazyZoo.Models
{
    public abstract class Animal
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public AnimalKind Kind { get; protected set; }

        protected Animal(string name, int age, AnimalKind kind)
        {
            Name = name;
            Age = age;
            Kind = kind;
        }

        public virtual string Describe()
        {
            return Name + ", " + Age;
        }

        public abstract string MakeSound();
    }
}