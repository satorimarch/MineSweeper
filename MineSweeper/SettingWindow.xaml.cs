using System.Windows;

namespace MineSweeper
{
    /// <summary>
    /// SettingForm.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow(int row, int column, int mineNum)
        {
            InitializeComponent();

            this.Style = (Style)FindResource(typeof(Window));

            TextBoxRow.Text = row.ToString();
            TextBoxColumn.Text = column.ToString();
            TextBoxMine.Text = mineNum.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int row, column, mineNum;
            if (!int.TryParse(TextBoxRow.Text, out row) ||
               !int.TryParse(TextBoxColumn.Text, out column) ||
               !int.TryParse(TextBoxMine.Text, out mineNum)) {
                MessageBox.Show("输入有误, 请输入整数", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else if (mineNum >= row * column) {
                MessageBox.Show("输入有误, 雷数必须小于总格数", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else {
                MainWindow mainWindow = (MainWindow)Owner;
                mainWindow.grid.ChangeSetting(row, column, mineNum);
                mainWindow.timer.Stop();
                mainWindow.LabelTime.Content = "00:00";
            }

            this.Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            ((MainWindow)Owner).SettingRunning = false;
        }
    }
}
