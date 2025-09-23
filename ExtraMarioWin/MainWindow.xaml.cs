using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExtraMarioWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly KRoster _roster = new();
        public ObservableCollection<KSinger> Singers { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            SyncSingersFromRoster();
        }

        private void SyncSingersFromRoster()
        {
            Singers.Clear();
            foreach (var s in _roster.Singers)
                Singers.Add(s);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_roster.NextSinger())
            {
                SyncSingersFromRoster();
            }
        }

        private void BumpButton_Click(object sender, RoutedEventArgs e)
        {
            if (_roster.Bump())
            {
                SyncSingersFromRoster();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var input = PromptForSingerName();
            if (!string.IsNullOrWhiteSpace(input))
            {
                _roster.Add(new KSinger(Guid.NewGuid(), input.Trim()));
                Singers.Add(_roster.Get(_roster.Count() - 1)!);
            }
        }

        private void RemoveSinger_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is KSinger singer)
            {
                var result = MessageBox.Show(this, $"Do you really want to remove '{singer.StageName}'?", "Remove Singer", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (_roster.Remove(singer))
                    {
                        Singers.Remove(singer);
                    }
                }
            }
        }

        private string? PromptForSingerName()
        {
            var dialog = new Window
            {
                Title = "Add Singer",
                Height = 150,
                Width = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Owner = this,
                Content = BuildDialogContent(out TextBox nameBox)
            };

            if (dialog.ShowDialog() == true)
            {
                return nameBox.Text;
            }
            return null;
        }

        private Grid BuildDialogContent(out TextBox nameBox)
        {
            var grid = new Grid { Margin = new Thickness(10) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var label = new TextBlock { Text = "Singer Name:", Margin = new Thickness(0,0,0,4) };
            Grid.SetRow(label, 0);
            grid.Children.Add(label);

            nameBox = new TextBox { Margin = new Thickness(0,0,0,8) };
            Grid.SetRow(nameBox, 1);
            grid.Children.Add(nameBox);

            var panel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            var ok = new Button { Content = "OK", MinWidth = 60, IsDefault = true, Margin = new Thickness(0,0,6,0) };
            var cancel = new Button { Content = "Cancel", MinWidth = 60, IsCancel = true };
            ok.Click += (_, _) => { ((Window)grid.Parent!).DialogResult = true; };
            cancel.Click += (_, _) => { ((Window)grid.Parent!).DialogResult = false; };
            panel.Children.Add(ok);
            panel.Children.Add(cancel);
            Grid.SetRow(panel, 2);
            grid.Children.Add(panel);
            return grid;
        }
    }
}