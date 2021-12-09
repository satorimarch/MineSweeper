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
        public bool SettingRunning { get; set; }
        DispatcherTimer timer = new DispatcherTimer();
        DateTime startTime;

        public MainWindow()
        {
            InitializeComponent();

            map.ResetMap(10, 10);
            map.SetMine(10);

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            string temp = (currentTime - startTime).ToString(@"mm\:ss");
            LabelTime.Content = temp;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (SettingRunning) {
                MessageBox.Show("设置正在运行", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            SettingRunning = true;
            SettingForm setting = new SettingForm(map.MapRow, map.MapColumn, map.MapMine);
            setting.Owner = this;
            setting.Show();
        }


        public void ChangeSetting(int row, int column, int mineNum)
        {
            map.ResetMap(row, column);
            map.SetMine(mineNum);
        }

        private void Map_OnRestMineChanged()
        {
            LabelMine.Content = map.RestMine;
        }

        private void map_OnGameStart()
        {
            startTime = DateTime.Now;
            timer.Start();
        }

    }
}
