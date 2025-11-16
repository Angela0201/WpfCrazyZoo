using CrazyZoo.Domain.Interfaces;
using System;

namespace CrazyZoo.Domain.Models
{
    public abstract class Animal : IReactToJoin
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public AnimalKind Kind { get; protected set; }
        public int Id { get; set; }

        protected Animal(string name, int age, AnimalKind kind)
        {
            Name = name;
            Age = age;
            Kind = kind;
        }

        public abstract string MakeSound();

        public void OnAnimalJoined(object sender, Animal joined)
        {
            if (joined != this)
                Console.WriteLine(Name + " noticed that " + joined.Name + " joined the enclosure.");
        }
    }
}