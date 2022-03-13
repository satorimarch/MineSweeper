using System;
using System.Collections;
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

    using tuple = Tuple<int, int>;

    /// <summary>
    /// MineSweeperControl.xaml 的交互逻辑
    /// </summary>
    public partial class MineSweeperMap : UserControl
    {
        public int RestMine { get; private set; }
        public int RestButton { get; private set; }

        public int MapRow { get; set; }
        public int MapColumn { get; set; }
        public int MapMine { get; set; }

        public bool GameWin { get; set; }

        private bool gameStart;
        private readonly int[] moveRow = { 1, 1, 1, 0, -1, -1, -1, 0 };
        private readonly int[] moveCol = { -1, 0, 1, 1, 1, 0, -1, -1 };

        public delegate void ChangeRestMineHandler();
        public delegate void GameStartHandler();
        public delegate void GameFinishHandler();

        public event ChangeRestMineHandler OnRestMineChanged;
        public event GameStartHandler OnGameStart;
        public event GameFinishHandler OnGameFinish;

        protected void MineChange()
        {
            if (OnRestMineChanged != null) {
                OnRestMineChanged();
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
        }

        public void ResetMap()
        {
            map.Children.Clear();

            map.Rows = MapRow;
            map.Columns = MapColumn;

            RestButton = MapRow * MapColumn;
            RestMine = MapMine;

            for (int i = 0; i < MapRow; i++) {
                for (int j = 0; j < MapColumn; j++) {

                    MyButton button = new MyButton()
                    {
                        Row = i,
                        Column = j,
                        IsMine = false,
                        IsUnlock = false,
                        Style = (Style)FindResource("DefaultButtonStyle"),
                        //Content = "第" + (i * column + j).ToString() + "个"
                    };
                    button.Click += MyButton_Click;
                    button.PreviewMouseDown += MyButton_MouseDown;

                    map.Children.Add(button);
                }
            }

        }

        /// <summary>
        /// 重置所有地雷
        /// </summary>
        public void SetMine(object sender = null)
        {
            int senderPos = -1;
            if (sender != null) {
                MyButton senderButton = (MyButton)sender;
                senderPos = senderButton.Row * MapColumn + senderButton.Column;
            }

            if (MapMine > map.Children.Count) {
                MessageBox.Show("雷的数量超过总数量", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            int count = map.Children.Count;

            MineChange();

            Random random = new Random();

            for (int i = 1; i <= MapMine; ) {
                int rd = random.Next(0, count);
                MyButton button = (MyButton)map.Children[rd];
                if (!button.IsMine && rd != senderPos)
                {
                    button.IsMine = true;
                    i++;
                    //button.Background = Brushes.Green; // test
                }
            }

            // 计算每个格周围的雷的数量
            for (int i = 0; i < MapRow; i++) {
                for (int j = 0; j < MapColumn; j++) {
                    MyButton currBtn = (MyButton)map.Children[i * MapColumn + j];
                    if (currBtn.IsMine) continue;
                    for (int k = 0; k < 8; k++) {
                        int row = i + moveRow[k];
                        int column = j + moveCol[k];
                        if (!IsButtonInMap(row, column)) continue;
                        MyButton button = (MyButton)map.Children[row * MapColumn + column];
                        if (button.IsMine) {
                            currBtn.AroundMineNum++;
                        }
                    }
                }
            }

            // test: 显示每个格子周围雷数
            //for (int i = 0; i < MapRow; i++) {
            //    for (int j = 0; j < MapColumn; j++) {
            //        MyButton currentButton = (MyButton)map.Children[i * MapColumn + j];
            //        currentButton.Content = currentButton.AroundMineNum.ToString();
            //    }
            //}

            GameStart();
        }

        /// <summary>
        /// 区分右键单击 和 左右键同时单击
        /// </summary>
        private void MyButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Pressed) {
                MyButton_BothDown(sender);
            }
            else if (e.RightButton == MouseButtonState.Pressed) {
                MyButton_RightButtonDown(sender);
            }

        }

        /// <summary>
        /// 左键格子事件
        /// </summary>
        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameStart == false) {
                gameStart = true;
                SetMine(sender);
            }

            MyButton button = (MyButton)sender;

            if (button.IsFlag || button.IsUnlock) {
                return;
            }

            else if (button.IsMine) {

                RestButton--;
                RestMine--;
                MineChange();

                button.IsUnlock = true;
                button.Style = (Style)FindResource("MineButtonStyle");

                GameFinish(false);

                return;
            }
            else {
                UnlockButton(button.Row, button.Column);
            }
        }

        /// <summary>
        /// 右键格子事件, 即flag标记
        /// </summary>
        private void MyButton_RightButtonDown(object sender)
        {
            MyButton button = (MyButton)sender;

            if (button.IsUnlock) {
                return;
            }

            if (!button.IsFlag) { // 未被标记时, 设置标记
                RestButton--;
                RestMine--;

                button.IsFlag = true;
                button.Style = (Style)FindResource("FlagButtonStyle");
            }

            else { // 已被标记时, 取消标记
                RestButton++;
                RestMine++;

                button.IsFlag = false;
                button.Style = (Style)FindResource("DefaultButtonStyle");
            }

            MineChange();
            CheckFinishGame();
        }

        /// <summary>
        /// 左右键同时单击
        /// </summary>
        private void MyButton_BothDown(object sender)
        {
            MyButton button = (MyButton)sender;

            if(button.IsFlag || !button.IsUnlock || button.AroundMineNum == 0) {
                return;
            }

            int countFlag = 0;
            for(int i = 0; i < 8; i++) {
                int currRow = button.Row + moveRow[i];
                int currCol = button.Column + moveCol[i];

                if (IsButtonInMap(currRow, currCol)) {
                    MyButton currBtn = (MyButton)map.Children[currRow * MapColumn + currCol];
                    if (currBtn.IsFlag) countFlag++;
                }
            }

            if (countFlag == button.AroundMineNum) {
                for (int i = 0; i < 8; i++) {
                    int currRow = button.Row + moveRow[i];
                    int currCol = button.Column + moveCol[i];

                    if (IsButtonInMap(currRow, currCol)) {
                        MyButton currBtn = (MyButton)map.Children[currRow * MapColumn + currCol];
                        UnlockButton(currRow, currCol);
                        if (!currBtn.IsFlag && !currBtn.IsUnlock) {
                            MyButton_Click(button, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 判断button是否在地图里
        /// </summary>
        /// <returns>如果在地图里则返回 true</returns>
        private bool IsButtonInMap(int row, int column)
        {
            return row >= 0 && row < MapRow && column >= 0 && column < MapColumn;
        }

        /// <summary>
        /// 点开这个格子和附近的格子(bfs)
        /// </summary>
        private void UnlockButton(int row, int column)
        {
            Queue<tuple> queue = new Queue<tuple>(); // 注意本命名空间最开始有一行: using tuple = Tuple<int, int>;
            queue.Enqueue(Tuple.Create(row, column));
            
            while (queue.Count != 0) {
                tuple pos = queue.Dequeue();

                if (!IsButtonInMap(pos.Item1, pos.Item2)) continue;

                MyButton button = (MyButton)map.Children[pos.Item1 * MapColumn + pos.Item2];

                if (button.IsFlag || button.IsMine || button.IsUnlock) continue;

                button.IsUnlock = true;
                RestButton--;

                if (button.AroundMineNum == 0) {
                    button.Content = "";
                    for (int i = 0; i < 8; i++) {
                        queue.Enqueue(Tuple.Create(pos.Item1 + moveRow[i], pos.Item2 + moveCol[i]));
                    }
                }
                else {
                    button.Content = button.AroundMineNum.ToString();
                }
            }
            CheckFinishGame();
        }


        /// <summary>
        /// 点开这个格子
        /// 如果是空则会递归地点开附近的格子
        /// </summary>
        //private void UnlockButton(int row, int column)
        //{
        //    if (!IsButtonInMap(row, column)) return;

        //    MyButton button = (MyButton)map.Children[row * MapColumn + column];

        //    if (button.IsFlag || button.IsMine || button.IsEnabled == false) return;


        //    button.IsEnabled = false;
        //    RestButton--;

        //    if (button.AroundMineNum == 0) {
        //        button.Content = "";
        //        for (int i = 0; i < 8; i++) {
        //            UnlockButton(row + moveRow[i], column + moveColumn[i]);
        //        }
        //    }
        //    else {
        //        button.Content = button.AroundMineNum.ToString();
        //    }
        //}

        //public void FinishGame(bool win)
        //{
        //    /// <summary>
        //    /// true 为 win
        //    /// false 为 lose
        //    /// </summary>

        //    string show;
        //    if (win) show = "You Win!";
        //    else show = "You lose...";

        //    if (MessageBox.Show(show, "Game Over", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK) {
        //        ResetMap(MapRow, MapColumn);
        //        SetMine(MapMine);
        //    }
        //}

        private void CheckFinishGame()
        {
            if (RestButton == 0 && RestMine == 0) {
                GameFinish(true);
            }
        }

        protected void GameFinish(bool win)
        {
            gameStart = false;
            GameWin = win;
            if (OnGameFinish != null) {
                OnGameFinish();
            }
        }

        public void ChangeSetting(int row, int col, int mineNum)
        {
            MapRow = row;
            MapColumn = col;
            MapMine = mineNum;

            ResetMap();
            RestMine = mineNum;

            MineChange();

            gameStart = false;
        }

        public void ChangeTheme(string theme)
        {
            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Source = new Uri(@"pack://application:,,,../Theme/" + theme + "Theme.xaml", UriKind.RelativeOrAbsolute);
            this.Resources.MergedDictionaries[0] = dictionary;

            foreach (MyButton button in map.Children) {
                if (button.IsMine && button.IsUnlock) {
                    button.Style = (Style)FindResource("MineButtonStyle");
                }
                else if (button.IsFlag) {
                    button.Style = (Style)FindResource("FlagButtonStyle");
                }
                else {
                    button.Style = (Style)FindResource("DefaultButtonStyle");
                }
            }

        }

    }
}
