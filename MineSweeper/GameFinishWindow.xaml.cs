using System.Windows;

namespace MineSweeper
{
    /// <summary>
    /// GameFinishForm.xaml 的交互逻辑
    /// </summary>

    public partial class GameFinishWindow : Window
    {
        public GameFinishWindow(string str)
        {
            InitializeComponent();

            TextBlockGameFinish.Text = str;
        }
    }
}
