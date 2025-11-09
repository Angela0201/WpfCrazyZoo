using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfCrazyZoo.Enclosures;
using WpfCrazyZoo.Models;
using WpfCrazyZoo.Repositories;
using System.Timers;
using WpfCrazyZoo.Infrastructure;

namespace WpfCrazyZoo.ViewModels
{
    public class ZooViewModel : INotifyPropertyChanged
    {
        private readonly IRepository<Animal> repo;
        private readonly Enclosure<Animal> enclosure = new Enclosure<Animal>("Main");
        private readonly Random rng = new Random();
        private readonly Timer nightTimer = new Timer(10000);
        public event EventHandler NightEvent;

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
            repo = DI.Resolve<IRepository<Animal>>();

            if (!repo.GetAll().Any())
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
            repo.Add(a);
            enclosure.AddAnimal(a);
        }

        private void Enclosure_AnimalJoinedInSameEnclosure(object sender, Animal e)
        {
            LogLines.Insert(0, e.Name + " joined enclosure");
            foreach (var other in enclosure.Animals.Where(x => !object.ReferenceEquals(x, e)))
            {
                LogLines.Insert(0, other.Name + " noticed " + e.Name);
            }
        }

        private async void Enclosure_FoodDropped(object sender, EventArgs e)
        {
            var order = enclosure.Animals.OrderBy(_ => rng.Next()).ToList();
            foreach (var a in order)
            {
                LogLines.Insert(0, a.Name + " starts eating");
                await System.Threading.Tasks.Task.Delay(GetEatingDelayMs(a));
                LogLines.Insert(0, a.Name + " finished eating");
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
            AllAnimals.Clear();
            foreach (var a in repo.GetAll())
                AllAnimals.Add(a);

            enclosure.Animals.Clear();
            foreach (var a in AllAnimals)
                enclosure.Animals.Add(a);

            RebuildView();
        }

        public void AddAnimal(Animal a)
        {
            repo.Add(a);
            AllAnimals.Add(a);
            enclosure.AddAnimal(a);
            RefreshStats();
        }

        public void RemoveAnimal(Animal a)
        {
            repo.Remove(a);
            AllAnimals.Remove(a);
            enclosure.Animals.Remove(a);
            RefreshStats();
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
                VM_LogNight(a.Name + " is sleeping peacefully");
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
            LogLines.Insert(0, "Night event started.");
        }

        public void StopNightEvent()
        {
            nightTimer.Stop();
            LogLines.Insert(0, "Night event stopped.");
        }

        public void RefreshStats()
        {
            StatsLines.Clear();

            var byKind = AllAnimals
                .GroupBy(a => a.Kind)
                .Select(g => new { Kind = g.Key, Count = g.Count(), Avg = g.Average(x => x.Age) })
                .OrderBy(x => x.Kind.ToString());

            foreach (var s in byKind)
                StatsLines.Add(s.Kind + ": " + s.Count + " (avg " + s.Avg.ToString("F1") + ")");

            var avgAge = AllAnimals.Any() ? AllAnimals.Average(a => a.Age) : 0;
            StatsLines.Add("Average age: " + avgAge.ToString("F1"));

            var oldest = AllAnimals.OrderByDescending(a => a.Age).FirstOrDefault();
            if (oldest != null) StatsLines.Add("Oldest: " + oldest.Name + " (" + oldest.Age + ")");
        }
    }
}