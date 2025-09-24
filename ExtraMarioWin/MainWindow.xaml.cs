using System;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Threading;
using ExtraMarioWin.History;

namespace ExtraMarioWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly KRoster _roster = new();
        private readonly IPerformerHistory _history;
        private readonly IPerformerRosterStorage _rosterStorage;
        public ObservableCollection<KSinger> Singers { get; } = new();

        // Drag-and-drop fields
        private Point _dragStartPoint;
        private ListBoxItem? _draggedItemContainer;

        public MainWindow() : this(new FilePerformerHistory(), new FilePerformerRosterStorage()) {}
        public MainWindow(IPerformerHistory history, IPerformerRosterStorage rosterStorage)
        {
            InitializeComponent();
            _history = history;
            _rosterStorage = rosterStorage;
            DataContext = this;

            RestoreRosterIfAvailable();
            SyncSingersFromRoster();
        }

        private bool IsNameUnique(string? name, KSinger? exclude = null)
        {
            var candidate = (name ?? string.Empty).Trim();
            if (candidate.Length == 0) return false;
            return !_roster.Singers.Any(s => !ReferenceEquals(s, exclude) && string.Equals((s.StageName ?? string.Empty).Trim(), candidate, StringComparison.OrdinalIgnoreCase));
        }

        private void RestoreRosterIfAvailable()
        {
            try
            {
                var restored = _rosterStorage.RestoreRoster();
                if (restored.Count > 0)
                {
                    foreach (var s in restored) _roster.Add(s);
                }
            }
            catch { }
        }

        private void PersistRoster()
        {
            try { _rosterStorage.SaveRoster(_roster.Singers.ToList()); } catch { }
        }

        private void SyncSingersFromRoster()
        {
            Singers.Clear();
            foreach (var s in _roster.Singers)
                Singers.Add(s);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Log the current singer before advancing
            var finished = _roster.Get(0);
            if (finished != null)
            {
                _history.SaveSinger(finished);
            }

            if (_roster.NextSinger())
            {
                PersistRoster();
                SyncSingersFromRoster();
                ScrollCurrentIntoView();
            }
        }

        private void BumpButton_Click(object sender, RoutedEventArgs e)
        {
            if (_roster.Bump())
            {
                PersistRoster();
                SyncSingersFromRoster();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var input = PromptForSingerName();
            if (!string.IsNullOrWhiteSpace(input))
            {
                _roster.Add(new KSinger(Guid.NewGuid(), input.Trim()));
                PersistRoster();
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
                        PersistRoster();
                        Singers.Remove(singer);
                    }
                }
            }
        }

        private void EditSinger_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is KSinger singer)
            {
                var newName = PromptForEditName(singer);
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    singer.StageName = newName.Trim();
                    // refresh displayed text
                    var idx = Singers.IndexOf(singer);
                    if (idx >= 0)
                    {
                        // Replace to raise collection change for some bindings
                        Singers[idx] = Singers[idx];
                    }
                    PersistRoster();
                }
            }
        }

        private string? PromptForEditName(KSinger singer)
        {
            var dialog = new Window
            {
                Title = "Edit Singer",
                Height = 180,
                Width = 320,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Owner = this,
            };

            var grid = new Grid { Margin = new Thickness(10) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var label = new TextBlock { Text = "Singer Name:", Margin = new Thickness(0, 0, 0, 4) };
            Grid.SetRow(label, 0);
            grid.Children.Add(label);

            var nameBox = new TextBox { Margin = new Thickness(0, 0, 0, 4), Text = singer.StageName ?? string.Empty };
            Grid.SetRow(nameBox, 1);
            grid.Children.Add(nameBox);

            var warning = new TextBlock { Foreground = Brushes.DarkRed, Visibility = Visibility.Collapsed, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0,0,0,8) };
            Grid.SetRow(warning, 2);
            grid.Children.Add(warning);

            var panel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            var ok = new Button { Content = "OK", MinWidth = 60, IsDefault = true, Margin = new Thickness(0, 0, 6, 0), IsEnabled = false };
            var cancel = new Button { Content = "Cancel", MinWidth = 60, IsCancel = true };
            ok.Click += (_, _) => { dialog.DialogResult = true; };
            cancel.Click += (_, _) => { dialog.DialogResult = false; };
            panel.Children.Add(ok);
            panel.Children.Add(cancel);
            Grid.SetRow(panel, 3);
            grid.Children.Add(panel);

            void UpdateOk()
            {
                var unique = IsNameUnique(nameBox.Text, singer);
                ok.IsEnabled = unique;
                warning.Visibility = (!unique && !string.IsNullOrWhiteSpace(nameBox.Text)) ? Visibility.Visible : Visibility.Collapsed;
                warning.Text = warning.Visibility == Visibility.Visible ? "This name is already in the roster." : string.Empty;
            }
            nameBox.TextChanged += (_, __) => UpdateOk();
            dialog.Content = grid;
            dialog.Loaded += (_, __) => { nameBox.Focus(); nameBox.CaretIndex = nameBox.Text.Length; UpdateOk(); };

            return dialog.ShowDialog() == true ? nameBox.Text : null;
        }

        private string? PromptForSingerName()
        {
            var dialog = new Window
            {
                Title = "Add Singer",
                Height = 170,
                Width = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Owner = this,
                Content = BuildDialogContent(out TextBox nameBox, out Button okButton, out TextBlock warning)
            };

            // Ensure the TextBox receives keyboard focus when the dialog opens and enable OK only when unique
            nameBox.Focusable = true;
            void UpdateOk()
            {
                var unique = IsNameUnique(nameBox.Text);
                okButton.IsEnabled = unique;
                warning.Visibility = (!unique && !string.IsNullOrWhiteSpace(nameBox.Text)) ? Visibility.Visible : Visibility.Collapsed;
                warning.Text = warning.Visibility == Visibility.Visible ? "This name is already in the roster." : string.Empty;
            }
            nameBox.TextChanged += (_, __) => UpdateOk();
            dialog.Loaded += (_, __) =>
            {
                nameBox.Focus();
                nameBox.CaretIndex = nameBox.Text?.Length ?? 0;
                UpdateOk();
            };
            FocusManager.SetFocusedElement(dialog, nameBox);

            if (dialog.ShowDialog() == true)
            {
                return nameBox.Text;
            }
            return null;
        }

        private Grid BuildDialogContent(out TextBox nameBox, out Button okButton, out TextBlock warning)
        {
            var grid = new Grid { Margin = new Thickness(10) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var label = new TextBlock { Text = "Singer Name:", Margin = new Thickness(0,0,0,4) };
            Grid.SetRow(label, 0);
            grid.Children.Add(label);

            nameBox = new TextBox { Margin = new Thickness(0,0,0,4) };
            Grid.SetRow(nameBox, 1);
            grid.Children.Add(nameBox);

            warning = new TextBlock { Foreground = Brushes.DarkRed, Visibility = Visibility.Collapsed, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0,0,0,8) };
            Grid.SetRow(warning, 2);
            grid.Children.Add(warning);

            var panel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            okButton = new Button { Content = "OK", MinWidth = 60, IsDefault = true, Margin = new Thickness(0,0,6,0), IsEnabled = false };
            var cancel = new Button { Content = "Cancel", MinWidth = 60, IsCancel = true };
            okButton.Click += (_, _) => { ((Window)grid.Parent!).DialogResult = true; };
            cancel.Click += (_, _) => { ((Window)grid.Parent!).DialogResult = false; };
            panel.Children.Add(okButton);
            panel.Children.Add(cancel);
            Grid.SetRow(panel, 3);
            grid.Children.Add(panel);
            return grid;
        }

        private void ScrollCurrentIntoView()
        {
            if (RosterList == null) return;
            RosterList.Dispatcher.InvokeAsync(() =>
            {
                if (RosterList.Items.Count > 0)
                {
                    RosterList.UpdateLayout();
                    RosterList.ScrollIntoView(RosterList.Items[0]);
                }
            }, DispatcherPriority.Background);
        }

        private void RosterList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
            _draggedItemContainer = (e.OriginalSource as DependencyObject)?.FindAncestor<ListBoxItem>();
        }

        private void RosterList_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (_draggedItemContainer == null) return;
            var currentPos = e.GetPosition(null);
            if ((Math.Abs(currentPos.X - _dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                (Math.Abs(currentPos.Y - _dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var listBox = (ListBox)sender;
                var data = _draggedItemContainer.DataContext as KSinger;
                if (data != null)
                {
                    DragDrop.DoDragDrop(listBox, data, DragDropEffects.Move);
                }
                _draggedItemContainer = null;
            }
        }

        private void RosterList_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(KSinger)))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void RosterList_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(KSinger))) return;
            var droppedSinger = (KSinger)e.Data.GetData(typeof(KSinger))!;

            var listBox = (ListBox)sender;
            var pos = e.GetPosition(listBox);
            int targetIndex = GetCurrentIndexFromPoint(listBox, pos);
            int oldIndex = Singers.IndexOf(droppedSinger);
            if (oldIndex < 0) return;

            if (targetIndex < 0) targetIndex = Singers.Count - 1;
            if (targetIndex >= Singers.Count) targetIndex = Singers.Count - 1;

            if (_roster.Move(oldIndex, targetIndex))
            {
                // Update observable collection to match new order
                var item = Singers[oldIndex];
                Singers.RemoveAt(oldIndex);
                Singers.Insert(targetIndex, item);
                PersistRoster();
            }
        }

        private int GetCurrentIndexFromPoint(ListBox listBox, Point point)
        {
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                var container = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromIndex(i);
                if (container == null) continue;
                var bounds = VisualTreeHelper.GetDescendantBounds(container);
                var topLeft = container.TranslatePoint(new Point(0, 0), listBox);
                var rect = new Rect(topLeft, bounds.Size);
                if (rect.Contains(point)) return i;
            }
            return -1;
        }
    }

    internal static class VisualTreeHelpers
    {
        public static T? FindAncestor<T>(this DependencyObject obj) where T : DependencyObject
        {
            var current = obj;
            while (current != null)
            {
                if (current is T t) return t;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
    }
}