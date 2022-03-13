using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
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
using MineSweeper.MineSweeperControl;

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
        internal DispatcherTimer timer = new DispatcherTimer();
        private DateTime startTime;

        public MainWindow()
        {
            InitializeComponent();

            this.Style = (Style)FindResource(typeof(Window));

            map.ChangeSetting(10, 15, 20);

            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            LabelTime.Content = (DateTime.Now - startTime).ToString(@"mm\:ss");
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //if (SettingRunning) {
            //    MessageBox.Show("设置正在运行", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            //    return;
            //}

            SettingRunning = true;
            SettingWindow setting = new SettingWindow(map.MapRow, map.MapColumn, map.MapMine);
            setting.Owner = this;
            setting.ShowDialog();
        }

        private void Map_OnRestMineChanged()
        {
            LabelMine.Content = map.RestMine;
        }

        private void Map_OnGameStart()
        {
            startTime = DateTime.Now;
            timer.Start();
        }

        private void Map_OnGameFinish()
        {
            timer.Stop();

            string show = map.GameWin ? "You Win!" : "You lose...";

            GameFinishWindow finishForm = new GameFinishWindow(show);
            finishForm.Owner = this;
            finishForm.ShowDialog();
            map.ResetMap();

            //if (MessageBox.Show(show, "Game Over", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK) {
            //map.ResetMap();
            //}

            LabelTime.Content = "00:00";
            LabelMine.Content = map.MapMine;
        }

        private void MenuItem_Click_Theme(object sender, RoutedEventArgs e)
        {
            string theme = ((MenuItem)sender).Name;

            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Source = new Uri(@"pack://application:,,,/Theme/" + theme + "Theme.xaml", UriKind.RelativeOrAbsolute);
            App.Current.Resources.MergedDictionaries[0] = dictionary;

            map.ChangeTheme(theme);
        }

        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.Owner = this;
            about.ShowDialog();
        }
    }
}
