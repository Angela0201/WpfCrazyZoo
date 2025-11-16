using CrazyZoo.Application.Interfaces;
using CrazyZoo.Domain.Interfaces;
using CrazyZoo.Domain.Models;
using CrazyZoo.Infrastructure.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using WpfCrazyZoo.Resources;

namespace WpfCrazyZoo.ViewModels
{
    public class ZooViewModel : INotifyPropertyChanged
    {
        private readonly IAnimalService service;
        private readonly Enclosure<Animal> enclosure = new Enclosure<Animal>("Main");
        private readonly Random rng = new Random();
        private readonly Timer nightTimer = new Timer(10000);
        public event EventHandler NightEvent;
        private bool _initialized = false;

        public ObservableCollection<Animal> AllAnimals { get; } = new ObservableCollection<Animal>();
        public ObservableCollection<Animal> ViewAnimals { get; } = new ObservableCollection<Animal>();
        public ObservableCollection<string> LogLines { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> StatsLines { get; } = new ObservableCollection<string>();

        private Animal _selectedAnimal;
        public Animal SelectedAnimal
        {
            get { return _selectedAnimal; }
            set
            {
                if (!object.Equals(_selectedAnimal, value))
                {
                    _selectedAnimal = value;
                    OnPropertyChanged();
                }
            }
        }

        public ZooViewModel()
        {
            service = DI.Resolve<IAnimalService>();

            if (!service.GetAll().Any())
            {
                AddInitial(new Cat("Murka", 2));
                AddInitial(new Dog("Bobik", 4));
                AddInitial(new Bird("Kesha", 1));
            }

            enclosure.AnimalJoinedInSameEnclosure += Enclosure_AnimalJoinedInSameEnclosure;
            enclosure.FoodDropped += Enclosure_FoodDropped;

            NightEvent += ZooViewModel_NightEvent;
            nightTimer.Elapsed += (s, e) => NightEvent?.Invoke(this, EventArgs.Empty);
            nightTimer.AutoReset = true;

            RefreshFromRepo();
            RefreshStats();
        }

        private void AddInitial(Animal a)
        {
            service.Add(a);
        }

        private void Enclosure_AnimalJoinedInSameEnclosure(object sender, Animal e)
        {
            VM_LogNight(string.Format(Strings.Msg_JoinedEnclosure, e.Name));
            foreach (var other in enclosure.Animals.Where(x => !object.ReferenceEquals(x, e)))
            {
                VM_LogNight(string.Format(Strings.Msg_AnimalNoticed, other.Name, e.Name));
            }
        }

        private async void Enclosure_FoodDropped(object sender, EventArgs e)
        {
            var order = enclosure.Animals.OrderBy(_ => rng.Next()).ToList();
            foreach (var a in order)
            {
                VM_LogNight(string.Format(Strings.Msg_EatingStart, a.Name));
                await System.Threading.Tasks.Task.Delay(GetEatingDelayMs(a));
                VM_LogNight(string.Format(Strings.Msg_EatingFinished, a.Name));
            }
        }

        private int GetEatingDelayMs(Animal a)
        {
            if (a is Cat) return 500;
            if (a is Dog) return 800;
            if (a is Bird) return 300;
            return 600;
        }

        public void RefreshFromRepo()
        {
            var items = service.GetAll().Select(ToDomain).ToList();

            AllAnimals.Clear();
            foreach (var a in items)
                AllAnimals.Add(a);

            for (int i = enclosure.Animals.Count - 1; i >= 0; i--)
            {
                var ex = enclosure.Animals[i];

                bool stillExists = items.Any(a =>
                    (ex.Id > 0 && a.Id == ex.Id) ||
                    (ex.Id == 0 && a.Id == 0 &&
                     a.Name == ex.Name && a.Age == ex.Age && a.Kind == ex.Kind));

                if (!stillExists)
                {
                    UnsubscribeAnimalIfNeeded(ex);
                    enclosure.Animals.RemoveAt(i);
                }
            }

            foreach (var a in items)
            {
                bool alreadyInside = enclosure.Animals.Any(x =>
                    (a.Id > 0 && x.Id == a.Id) ||
                    (a.Id == 0 && x.Id == 0 &&
                     x.Name == a.Name && x.Age == a.Age && x.Kind == a.Kind));

                if (!alreadyInside)
                {
                    SubscribeAnimalIfNeeded(a);
                    if (_initialized)
                        enclosure.AddAnimal(a);
                    else
                        enclosure.Animals.Add(a);
                }
            }

            RebuildView();

            RefreshStats();
            _initialized = true;
        }

        private void SubscribeAnimalIfNeeded(Animal a)
        {
            var r = a as IReactToJoin;
            if (r != null)
                enclosure.AnimalJoinedInSameEnclosure += r.OnAnimalJoined;
        }

        private Animal ToDomain(Animal a)
        {
            var sa = a as StoredAnimal;
            if (sa == null) return a;

            switch (sa.Kind)
            {
                case AnimalKind.Cat: return new Cat(sa.Name, sa.Age);
                case AnimalKind.Dog: return new Dog(sa.Name, sa.Age);
                case AnimalKind.Bird: return new Bird(sa.Name, sa.Age);
                default: return a;
            }
        }

        private void UnsubscribeAnimalIfNeeded(Animal a)
        {
            var r = a as IReactToJoin;
            if (r != null)
                enclosure.AnimalJoinedInSameEnclosure -= r.OnAnimalJoined;
        }

        public void AddAnimal(Animal a)
        {
            service.Add(a);
            RefreshFromRepo();
        }

        public void AddAnimal(string name, int age, AnimalKind kind)
        {
            Animal newAnimal;
            if (kind == AnimalKind.Cat)
                newAnimal = new Cat(name, age);
            else if (kind == AnimalKind.Dog)
                newAnimal = new Dog(name, age);
            else if (kind == AnimalKind.Bird)
                newAnimal = new Bird(name, age);
            else
                newAnimal = new Cat(name, age);

            AddAnimal(newAnimal);
        }

        public void RemoveAnimal(Animal a)
        {
            if (a == null) return;
            service.Remove(a);
            RefreshFromRepo();
        }

        public void RebuildView()
        {
            ViewAnimals.Clear();
            foreach (var a in AllAnimals)
                ViewAnimals.Add(a);
        }

        public void RaiseFoodDropped()
        {
            enclosure.DropFood();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            var h = PropertyChanged;
            if (h != null) h(this, new PropertyChangedEventArgs(propName));
        }

        private void ZooViewModel_NightEvent(object sender, EventArgs e)
        {
            foreach (var a in AllAnimals)
            {
                VM_LogNight(string.Format(Strings.Msg_Sleeping, a.Name));
            }
        }

        private void VM_LogNight(string text)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                LogLines.Insert(0, text);
            });
        }

        public void StartNightEvent()
        {
            nightTimer.Start();
            VM_LogNight(Strings.Msg_NightStarted);
        }

        public void StopNightEvent()
        {
            nightTimer.Stop();
            VM_LogNight(Strings.Msg_NightStopped);
        }

        public void RefreshStats()
        {
            StatsLines.Clear();

            var byKind = AllAnimals
                .GroupBy(a => a.Kind)
                .Select(g => new { Kind = g.Key, Count = g.Count(), Avg = g.Average(x => x.Age) })
                .OrderBy(x => x.Kind.ToString());

            foreach (var s in byKind)
                StatsLines.Add(string.Format(Strings.Msg_StatsByKind, s.Kind, s.Count, s.Avg.ToString("F1")));

            var avgAge = AllAnimals.Any() ? AllAnimals.Average(a => a.Age) : 0;
            StatsLines.Add(string.Format(Strings.Msg_StatsAverageAge, avgAge.ToString("F1")));

            var oldest = AllAnimals.OrderByDescending(a => a.Age).FirstOrDefault();
            if (oldest != null) StatsLines.Add(string.Format(Strings.Msg_StatsOldest, oldest.Name, oldest.Age));
        }
    }
}