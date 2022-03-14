using System.Windows;
using System.Windows.Controls;

namespace MineSweeper.MineSweeperControl
{
    class MyButton : Button
    {

        public int Row { get; set; }
        public int Column { get; set; }

        public int AroundMineNum { get; set; } // 显示周围的雷的个数
        public bool IsMine { get; set; } // 是否是雷
        public bool IsFlag { get; set; } // 是否被标记

        // 依赖属性
        public static readonly DependencyProperty IsUlockProperty = DependencyProperty.Register("IsUnlock", typeof(bool), typeof(MyButton));
        public bool IsUnlock
        {
            get { return (bool)GetValue(IsUlockProperty); }
            set { SetValue(IsUlockProperty, value); }
        }

        public MyButton()
        {

        }

    }
}
