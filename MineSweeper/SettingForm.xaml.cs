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
using System.Windows.Shapes;

namespace MineSweeper
{
    /// <summary>
    /// SettingForm.xaml 的交互逻辑
    /// </summary>
    public partial class SettingForm : Window
    {
        public SettingForm(int row, int column, int mineNum)
        {
            InitializeComponent();

            TextBoxRow.Text = row.ToString();
            TextBoxColumn.Text = column.ToString();
            TextBoxMine.Text = mineNum.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int row, column, mineNum;
            if(!int.TryParse(TextBoxRow.Text, out row) ||
               !int.TryParse(TextBoxColumn.Text, out column) ||
               !int.TryParse(TextBoxMine.Text, out mineNum)) {
                MessageBox.Show("输入有误", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else {
                ((MainWindow)Owner).ChangeSetting(row, column, mineNum);
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            ((MainWindow)Owner).SettingRunning = false;
        }
    }
}
