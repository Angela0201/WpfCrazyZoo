using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCrazyZoo.Models;
using WpfCrazyZoo.Resources;
using System.Collections.ObjectModel;

namespace WpfCrazyZoo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Animal> allAnimals;
        private ObservableCollection<Animal> viewAnimals;
        private ObservableCollection<string> logLines;

        public MainWindow()
        {
            allAnimals = new ObservableCollection<Animal>();
            viewAnimals = new ObservableCollection<Animal>();
            logLines = new ObservableCollection<string>();

            allAnimals.Add(new Cat("Murka", 2));
            allAnimals.Add(new Dog("Bobik", 4));
            allAnimals.Add(new Bird("Kesha", 1));

            InitializeComponent();
            this.Title = Strings.AppTitle;

            AnimalsList.ItemsSource = viewAnimals;
            if (AnimalsList != null) AnimalsList.SelectedIndex = -1;
            LogList.ItemsSource = logLines;

            ClearFields();
            RebuildView();
            AnimalsList.SelectedIndex = -1;
        }

        private void RebuildView()
        {
            if (viewAnimals == null) return;

            viewAnimals.Clear();

            int selectedKind = -1;
            if (FilterKindCombo != null && FilterKindCombo.SelectedItem is ComboBoxItem item)
            {
                int parsed;
                if (int.TryParse(item.Tag != null ? item.Tag.ToString() : "-1", out parsed))
                    selectedKind = parsed;
            }

            foreach (var a in allAnimals)
            {
                if (selectedKind == -1 || (int)a.Kind == selectedKind)
                    viewAnimals.Add(a);
            }
        }

        private void AnimalsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AnimalsList == null) return;

            var a = AnimalsList.SelectedItem as Animal;
            if (a == null)
            {
                ClearFields();
                return;
            }

            NameBox.Text = a.Name;
            AgeBox.Text = a.Age.ToString();

            foreach (ComboBoxItem item in KindCombo.Items)
            {
                if (item.Tag != null && item.Tag.ToString() == ((int)a.Kind).ToString())
                {
                    KindCombo.SelectedItem = item;
                    break;
                }
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = SafeTrim(NameBox.Text);
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(Strings.Msg_NameRequired);
                return;
            }

            if (!IsValidName(name))
            {
                MessageBox.Show(Strings.Msg_NameInvalid);
                return;
            }

            if (string.IsNullOrWhiteSpace(AgeBox.Text))
            {
                MessageBox.Show(Strings.Msg_AgeRequired);
                return;
            }

            int age;
            if (!int.TryParse(AgeBox.Text.Trim(), out age))
            {
                MessageBox.Show(Strings.Msg_AgeRequired);
                return;
            }

            if (age < 0 || age > 30)
            {
                MessageBox.Show(Strings.Msg_AgeOutOfRange);
                return;
            }

            int kindCode = -1;
            if (KindCombo.SelectedItem is ComboBoxItem kitem && kitem.Tag != null)
            {
                int.TryParse(kitem.Tag.ToString(), out kindCode);
            }
            if (kindCode < 0)
            {
                MessageBox.Show(Strings.Msg_SelectKind);
                return;
            }

            Animal newAnimal;
            if (kindCode == (int)AnimalKind.Cat) newAnimal = new Cat(name, age);
            else if (kindCode == (int)AnimalKind.Dog) newAnimal = new Dog(name, age);
            else newAnimal = new Bird(name, age);

            allAnimals.Add(newAnimal);
            RebuildView();
            logLines.Add(Strings.Msg_Added);

            AnimalsList.SelectedIndex = -1;
            ClearFields();
            NameBox.Focus();
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            var a = AnimalsList.SelectedItem as Animal;
            if (a == null)
            {
                MessageBox.Show(Strings.Msg_NoSelection);
                return;
            }

            var res = MessageBox.Show(Strings.Msg_RemoveConfirmText, Strings.Msg_RemoveConfirmTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res != MessageBoxResult.Yes) return;

            allAnimals.Remove(a);
            RebuildView();
            logLines.Add(Strings.Msg_Removed);

            AnimalsList.SelectedIndex = -1;
            ClearFields();
        }

        private void SoundBtn_Click(object sender, RoutedEventArgs e)
        {
            var a = AnimalsList.SelectedItem as Animal;
            if (a == null)
            {
                MessageBox.Show(Strings.Msg_NoSelection);
                return;
            }

            string sound = a.MakeSound();
            logLines.Add(string.Format(Strings.Msg_Sound, a.Name, sound));
        }

        private void FeedBtn_Click(object sender, RoutedEventArgs e)
        {
            var a = AnimalsList.SelectedItem as Animal;
            if (a == null)
            {
                MessageBox.Show(Strings.Msg_NoSelection);
                return;
            }

            string food = SafeTrim(FoodBox.Text);
            if (string.IsNullOrWhiteSpace(food))
            {
                MessageBox.Show(Strings.Msg_FoodRequired);
                return;
            }

            if (!IsValidName(food))
            {
                MessageBox.Show(Strings.Msg_FoodInvalid);
                return;
            }

            logLines.Add(string.Format(Strings.Msg_Fed, a.Name, food));
            FoodBox.Clear();
        }

        private void CrazyBtn_Click(object sender, RoutedEventArgs e)
        {
            var a = AnimalsList.SelectedItem as Animal;
            if (a == null)
            {
                MessageBox.Show(Strings.Msg_NoSelection);
                return;
            }

            string resultText;

            if (a is Cat cat) resultText = cat.ActCrazy();
            else if (a is Dog dog) resultText = dog.ActCrazy();
            else if (a is Bird bird) resultText = bird.ActCrazy();
            else resultText = "No crazy action.";

            logLines.Add(string.Format(Strings.Msg_Crazy, resultText));
        }

        private void FilterKindCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RebuildView();
            if (AnimalsList != null) AnimalsList.SelectedIndex = -1;
        }

        private string SafeTrim(string s)
        {
            if (s == null) return "";
            return s.Trim();
        }

        private void ClearFields()
        {
            NameBox.Text = "";
            AgeBox.Text = "";
            KindCombo.SelectedIndex = -1;
            FoodBox.Text = "";
        }

        private void ClearLogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (logLines == null || logLines.Count == 0)
            {
                MessageBox.Show("Log is already empty.");
                return;
            }

            var res = MessageBox.Show("Clear all log entries?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                logLines.Clear();
            }
        }

        private bool IsValidName(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            foreach (var ch in s)
            {
                if (!(char.IsLetter(ch) || ch == ' ' || ch == '-')) return false;
            }
            return true;
        }
    }
}
