using System.Windows;

namespace MineSweeper
{
    /// <summary>
    /// AboutForm.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            this.Style = (Style)FindResource(typeof(Window));
        }
    }
}
