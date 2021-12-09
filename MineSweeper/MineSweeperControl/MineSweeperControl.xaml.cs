using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
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

namespace MineSweeper.MineSweeperControl
{
    /// <summary>
    /// MineSweeperControl.xaml 的交互逻辑
    /// </summary>
    public partial class MineSweeperMap : UserControl
    {

        public int RestMine   { get; private set; }
        public int RestButton { get; private set; }

        public int MapRow    { get; set; }
        public int MapColumn { get; set; }
        public int MapMine   { get; set; }

        private readonly int[] moveRow    = { 1, 1, 1, 0, -1, -1, -1, 0 };
        private readonly int[] moveColumn = { -1, 0, 1, 1, 1, 0, -1, -1 };

        public delegate void ChangeRestMineHandler();
        public delegate void ChangeRestButtonHandler();
        public delegate void GameStartHandler();

        public event ChangeRestMineHandler OnRestMineChanged;
        public event ChangeRestButtonHandler OnRestButtonChanged;
        public event GameStartHandler OnGameStart;

        protected void MineChange()
        {
            if(OnRestMineChanged != null) {
                OnRestMineChanged();
            }
        }

        protected void ButtonChange()
        {
            if (OnRestButtonChanged != null) {
                OnRestButtonChanged();
            }
        }

        protected void GameStart()
        {
            if (OnGameStart != null) {
                OnGameStart();
            }
        }


        public MineSweeperMap()
        {
            InitializeComponent();

            OnRestButtonChanged += new ChangeRestButtonHandler(CheckFinishGame); // 注册
        }

        private void CheckFinishGame()
        {
            if(RestButton == 0 && RestMine == 0) {
                FinishGame(true);
            }
        }


        public void ResetMap(int row, int column)
        {
            map.Children.Clear();

            MapRow = map.Rows = row;
            MapColumn = map.Columns = column;

            RestButton = MapRow * MapColumn;
            ButtonChange();

            for (int i = 0; i < row; i++) {
                for (int j = 0; j < column; j++) {

                    MyButton button = new MyButton()
                    {
                        Row = i,
                        Column = j,
                        IsMine = false,
                        //Content = "第" + (i * column + j).ToString() + "个"
                    };
                    button.Click += MyButton_Click;
                    button.MouseDown += MyButton_RightButtonDown;

                    map.Children.Add(button);
                }
            }

        }

        /// <summary>
        /// 右键格子事件
        /// </summary>
        private void MyButton_RightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyButton button = (MyButton)sender;
            if (!button.IsFlag) {
                RestButton--;
                RestMine--;

                button.Style = (Style)FindResource("MyButtonStyle");
                button.IsFlag = true;
                //button.Background = Brushes.Blue;
            }
            else {
                RestButton++;
                RestMine++;
                MineChange();

                button.Style = (Style)FindResource("MyButtonStyleDefault");
                button.IsFlag = false;
            }

            MineChange();
            ButtonChange();
        }


        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            /// <summary>
            /// 左键格子事件
            /// </summary>


            MyButton button = (MyButton)sender;

            if (button.IsFlag) {
                return;
            }
            else if (button.IsMine) {

                RestButton--;
                RestMine--;
                MineChange();
                ButtonChange();

                button.Background = Brushes.Red;
                
                FinishGame(false);
                
                return;
            }
            else {
                UnlockButton(button.Row, button.Column);
                ButtonChange();
            }
        }

        private bool IsButtonInMap(int row, int column)
        {
            if (row < 0 || row >= MapRow || column < 0 || column >= MapColumn) return false;
            return true;
        }


        private void UnlockButton(int row, int column)
        {
            /// <summary>
            /// 点开这个格子(包含胜利判定)
            /// 如果是空则会递归地点开附近的格子
            /// </summary>

            if (!IsButtonInMap(row, column)) return;

            MyButton button = (MyButton)map.Children[row * MapColumn + column];

            if (button.IsFlag || button.IsMine || button.IsEnabled == false) return;


            button.IsEnabled = false;
            RestButton--;

            if (button.AroundMineNum == 0) {
                button.Content = "";
                for (int i = 0; i < 8; i++) {
                    UnlockButton(row + moveRow[i], column + moveColumn[i]);
                }
            }
            else {
                button.Content = button.AroundMineNum.ToString();
            }
        }

        virtual public void FinishGame(bool win)
        {
            /// <summary>
            /// true 为 win
            /// false 为 lose
            /// </summary>

            string show;
            if (win) show = "You Win!";
            else show = "You lose...";

            if (MessageBox.Show(show, "Game Over", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK) {
                ResetMap(MapRow, MapColumn);
                SetMine(MapMine);
            }
        }

        public void SetMine(int mineNum)
        {
            /// <summary>
            /// 重置所有地雷
            /// </summary>

            
            if (mineNum > map.Children.Count) {
                MessageBox.Show("雷的数量超过总数量", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            int count = map.Children.Count;
            RestMine = mineNum;

            MineChange();

            MapMine = mineNum;

            Random random = new Random();

            for (int i = 1; i <= mineNum;) {
                int rd = random.Next(0, count);
                MyButton button = (MyButton)map.Children[rd];
                if (button.IsMine) {
                    continue;
                }
                button.IsMine = true;
                //button.Background = Brushes.Green; // test
                i++;
            }

            // 计算每个格周围的雷的数量
            for (int i = 0; i < MapRow; i++) {
                for (int j = 0; j < MapColumn; j++) {
                    MyButton currentButton = (MyButton)map.Children[i * MapColumn + j];
                    if (currentButton.IsMine) continue;
                    for (int k = 0; k < 8; k++) {
                        int row = i + moveRow[k];
                        int column = j + moveColumn[k];
                        if (!IsButtonInMap(row, column)) continue;
                        MyButton button = (MyButton)map.Children[row*MapColumn+column];
                        if (button.IsMine) {
                            currentButton.AroundMineNum++;
                        }
                    }
                }
            }

            // test: 显示每个格周围雷数
            //for (int i = 0; i < MapRow; i++) {
            //    for (int j = 0; j < MapColumn; j++) {
            //        MyButton currentButton = (MyButton)map.Children[i * MapColumn + j];
            //        currentButton.Content = currentButton.AroundMineNum.ToString();
            //    }
            //}

            GameStart();
        }
    }
}
