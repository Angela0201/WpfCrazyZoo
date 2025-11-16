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
using CrazyZoo.Domain.Models;
using WpfCrazyZoo.Resources;
using System.Collections.ObjectModel;
using WpfCrazyZoo.ViewModels;
using Microsoft.Win32;
using CrazyZoo.Infrastructure.Infrastructure;
using CrazyZoo.Infrastructure.Logging;

namespace WpfCrazyZoo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ZooViewModel VM { get { return DataContext as ZooViewModel; } }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ZooViewModel();
            if (AnimalsList != null) AnimalsList.SelectedIndex = -1;
            ClearFields();
            RebuildView();
            AnimalsList.SelectedIndex = -1;
        }

        private void RebuildView()
        {
            if (VM == null || VM.ViewAnimals == null) return;
            VM.ViewAnimals.Clear();

            int selectedKind = -1;
            if (FilterKindCombo != null && FilterKindCombo.SelectedItem is ComboBoxItem item)
            {
                int parsed;
                if (int.TryParse(item.Tag != null ? item.Tag.ToString() : "-1", out parsed))
                    selectedKind = parsed;
            }

            foreach (var a in VM.AllAnimals)
            {
                if (selectedKind == -1 || (int)a.Kind == selectedKind)
                    VM.ViewAnimals.Add(a);
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

            foreach (ComboBoxItem cb in KindCombo.Items)
            {
                if (cb.Tag != null && cb.Tag.ToString() == ((int)a.Kind).ToString())
                {
                    KindCombo.SelectedItem = cb;
                    break;
                }
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Window
            {
                Title = Strings.Ui_AddDialog_Title,
                Width = 300,
                Height = 240,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Owner = this
            };

            var grid = new Grid { Margin = new Thickness(10) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var nameLabel = new TextBlock { Text = Strings.Ui_AddDialog_Name, VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(nameLabel, 0); Grid.SetColumn(nameLabel, 0);
            var nameBox = new TextBox { Margin = new Thickness(5, 0, 0, 5) };
            Grid.SetRow(nameBox, 0); Grid.SetColumn(nameBox, 1);

            var ageLabel = new TextBlock { Text = Strings.Ui_AddDialog_Age, VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(ageLabel, 1); Grid.SetColumn(ageLabel, 0);
            var ageBox = new TextBox { Margin = new Thickness(5, 0, 0, 5) };
            Grid.SetRow(ageBox, 1); Grid.SetColumn(ageBox, 1);

            var kindLabel = new TextBlock { Text = Strings.Ui_AddDialog_Kind, VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(kindLabel, 2); Grid.SetColumn(kindLabel, 0);
            var kindCombo = new ComboBox { Margin = new Thickness(5, 0, 0, 10) };
            kindCombo.Items.Add(new ComboBoxItem { Tag = (int)AnimalKind.Cat, Content = "Cat", IsSelected = true });
            kindCombo.Items.Add(new ComboBoxItem { Tag = (int)AnimalKind.Dog, Content = "Dog" });
            kindCombo.Items.Add(new ComboBoxItem { Tag = (int)AnimalKind.Bird, Content = "Bird" });
            Grid.SetRow(kindCombo, 2); Grid.SetColumn(kindCombo, 1);

            var buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            var okBtn = new Button { Content = Strings.Ui_AddDialog_Ok, Width = 80, Margin = new Thickness(0, 0, 5, 0) };
            var cancelBtn = new Button { Content = Strings.Ui_AddDialog_Cancel, Width = 80 };
            buttons.Children.Add(okBtn);
            buttons.Children.Add(cancelBtn);
            Grid.SetRow(buttons, 3); Grid.SetColumnSpan(buttons, 2);

            grid.Children.Add(nameLabel);
            grid.Children.Add(nameBox);
            grid.Children.Add(ageLabel);
            grid.Children.Add(ageBox);
            grid.Children.Add(kindLabel);
            grid.Children.Add(kindCombo);
            grid.Children.Add(buttons);
            dialog.Content = grid;

            okBtn.Click += (s, ev) => { dialog.DialogResult = true; dialog.Close(); };
            cancelBtn.Click += (s, ev) => { dialog.DialogResult = false; dialog.Close(); };

            var result = dialog.ShowDialog();
            if (result != true) return;

            string name = SafeTrim(nameBox.Text);
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

            if (string.IsNullOrWhiteSpace(ageBox.Text))
            {
                MessageBox.Show(Strings.Msg_AgeRequired);
                return;
            }

            int age;
            if (!int.TryParse(ageBox.Text.Trim(), out age))
            {
                MessageBox.Show(Strings.Msg_AgeRequired);
                return;
            }

            if (age < 0 || age > 30)
            {
                MessageBox.Show(Strings.Msg_AgeOutOfRange);
                return;
            }

            int kindCode = (int)AnimalKind.Cat;
            if (kindCombo.SelectedItem is ComboBoxItem item && item.Tag != null)
                int.TryParse(item.Tag.ToString(), out kindCode);

            Animal newAnimal;
            if (kindCode == (int)AnimalKind.Cat) newAnimal = new Cat(name, age);
            else if (kindCode == (int)AnimalKind.Dog) newAnimal = new Dog(name, age);
            else newAnimal = new Bird(name, age);

            VM.AddAnimal(newAnimal);
            RebuildView();
            VM.LogLines.Insert(0, Strings.Msg_Added);
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            var a = AnimalsList.SelectedItem as Animal;
            if (a == null)
            {
                MessageBox.Show(Strings.Msg_NoSelection);
                return;
            }

            var res = MessageBox.Show(Strings.Msg_RemoveConfirmText, Strings.Msg_RemoveConfirmTitle,
                                      MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res != MessageBoxResult.Yes) return;

            VM.RemoveAnimal(a);
            RebuildView();
            VM.LogLines.Insert(0, Strings.Msg_Removed);

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
            VM.LogLines.Insert(0, string.Format(Strings.Msg_Sound, a.Name, sound));
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

            VM.LogLines.Insert(0, string.Format(Strings.Msg_Fed, a.Name, food));
            FoodBox.Clear();
        }

        private void CrazyBtn_Click(object sender, RoutedEventArgs e)
        {
            var a = AnimalsList.SelectedItem as CrazyZoo.Domain.Models.Animal;
            if (a == null)
            {
                MessageBox.Show(WpfCrazyZoo.Resources.Strings.Msg_NoSelection);
                return;
            }

            string resultText;
            if (a is CrazyZoo.Domain.Interfaces.ICrazyAction ca)
                resultText = ca.ActCrazy();
            else
                resultText = WpfCrazyZoo.Resources.Strings.Ui_NoCrazyAction;

            VM.LogLines.Insert(0, string.Format(WpfCrazyZoo.Resources.Strings.Msg_Crazy, resultText));
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
            if (VM.LogLines == null || VM.LogLines.Count == 0)
            {
                MessageBox.Show(Strings.Msg_LogEmpty);
                return;
            }
            var res = MessageBox.Show(Strings.Msg_ClearLogConfirmText, Strings.Msg_ClearLogConfirmTitle,
                                      MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                VM.LogLines.Clear();
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

        private bool nightRunning = false;

        private void NightBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!nightRunning)
            {
                VM.StartNightEvent();
                NightBtn.Content = Strings.Ui_StopNight;
                nightRunning = true;
            }
            else
            {
                VM.StopNightEvent();
                NightBtn.Content = Strings.Ui_StartNight;
                nightRunning = false;
            }
        }

        private void DropFoodBtn_Click(object sender, RoutedEventArgs e)
        {
            VM.RaiseFoodDropped();
        }

        private void SaveLogsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (VM == null || VM.LogLines == null || VM.LogLines.Count == 0)
            {
                MessageBox.Show("Log is empty");
                return;
            }

            var logger = DI.Resolve<ILogger>();
            var dlg = new SaveFileDialog();
            dlg.Filter = logger.FileDialogFilter;
            dlg.AddExtension = true;
            dlg.FileName = "logs." + logger.DefaultExtension;

            var ok = dlg.ShowDialog(this);
            if (ok == true)
            {
                logger.SaveLogs(VM.LogLines, dlg.FileName);
                MessageBox.Show("Logs saved");
            }
        }
    }
}