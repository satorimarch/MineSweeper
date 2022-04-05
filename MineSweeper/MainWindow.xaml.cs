using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MineSweeper
{
    public partial class MainWindow : Window
    {
        private enum Theme
        {
            DefaultTheme,
            DarkTheme
        }

        public bool SettingRunning { get; set; }
        internal Timer timer;

        private DateTime startTime;

        public MainWindow()
        {
            InitializeComponent();

            this.Style = (Style)FindResource(typeof(Window));
            grid.ChangeSetting(10, 15, 20);

            timer = new Timer
            {
                Interval = 100,
                Enabled = false
            };

            timer.Elapsed += (o, e) =>
            {
                LabelTime.Dispatcher.BeginInvoke(
                    new Action(
                        delegate
                        {
                            LabelTime.Content = (DateTime.Now - startTime).ToString(@"mm\:ss");
                        }
                    )
                );
            };
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //if (SettingRunning) {
            //    MessageBox.Show("设置正在运行", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            SettingRunning = true;
            SettingWindow setting = new SettingWindow(grid.MapRow, grid.MapColumn, grid.MapMine);
            setting.Owner = this;
            setting.ShowDialog();
        }

        private void Map_OnRestMineChanged()
        {
            LabelMine.Content = grid.RestMine;
        }

        private void Map_OnGameStart()
        {
            startTime = DateTime.Now;
            timer.Start();
        }

        private void Map_OnGameFinish()
        {
            timer.Stop();

            string show = grid.GameWin ? "You Win!" : "You lose...";

            GameFinishWindow finishForm = new GameFinishWindow(show);
            finishForm.Owner = this;
            finishForm.ShowDialog();
            grid.ResetMap();

            //if (MessageBox.Show(show, "Game Over", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK) {
            //map.ResetMap();
            //}

            LabelTime.Content = "00:00";
            LabelMine.Content = grid.MapMine;
        }

        private void MenuItem_Click_Theme(object sender, RoutedEventArgs e)
        {
            string theme = ((MenuItem)sender).Name;

            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Source = new Uri(@"pack://application:,,,/Theme/" + theme + "Theme.xaml", UriKind.RelativeOrAbsolute);
            App.Current.Resources.MergedDictionaries[0] = dictionary;

            grid.ChangeTheme(theme);
        }

        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.Owner = this;
            about.ShowDialog();
        }
    }
}
