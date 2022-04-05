using System.Windows;
using System.Windows.Controls;

namespace MineSweeper.MineSweeperControl
{
    class MyButton : Button
    {

        public int Row { get; set; }
        public int Column { get; set; }

        /// <summary>
        /// 周围的雷的个数
        /// </summary>
        public int AroundMineNum { get; set; }

        // 依赖属性
        public bool IsFlag
        {
            get { return (bool)GetValue(IsFlagProperty); }
            set { SetValue(IsFlagProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMineProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFlagProperty =
            DependencyProperty.Register("IsFlag", typeof(bool), typeof(MyButton));


        public bool IsMine
        {
            get { return (bool)GetValue(IsMineProperty); }
            set { SetValue(IsMineProperty, value); }
        }

        public static readonly DependencyProperty IsMineProperty =
            DependencyProperty.Register("IsMine", typeof(bool), typeof(MyButton));


        public bool IsUncover
        {
            get { return (bool)GetValue(IsUncoverProperty); }
            set { SetValue(IsUncoverProperty, value); }
        }

        public static readonly DependencyProperty IsUncoverProperty =
            DependencyProperty.Register("IsUncover", typeof(bool), typeof(MyButton));


        public MyButton()
        {

        }

    }
}
