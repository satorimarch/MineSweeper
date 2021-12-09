using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MineSweeper.MineSweeperControl
{
    class MyButton : Button
    {
        //public Image imgae = new Image();

        public int Row { get; set; }
        public int Column { get; set; }

        public int AroundMineNum { get; set; } // 显示周围的雷的个数
        public bool IsMine { get; set; } // 判断是否是雷
        public bool IsFlag { get; set; }

        public MyButton()
        {

        }

    }
}
