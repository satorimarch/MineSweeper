using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        /// <summary>
        /// 重置地图
        /// </summary>
        public void ResetMap()
        {
            grid.Children.Clear();

            grid.Rows = MapRow;
            grid.Columns = MapColumn;

            RestButton = MapRow * MapColumn;
            RestMine = MapMine;

            for (int i = 0; i < MapRow; i++) {
                for (int j = 0; j < MapColumn; j++) {

                    MyButton button = new MyButton()
                    {
                        Row = i,
                        Column = j,
                        IsMine = false,
                        IsUncover = false,
                        Style = (Style)FindResource("DefaultButtonStyle"),
                        //Content = "第" + (i * column + j).ToString() + "个"
                    };
                    button.Click += MyButton_Click;
                    button.PreviewMouseDown += DiscernMouseDown;

                    grid.Children.Add(button);
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

            int count = MapColumn * MapRow;

            if (MapMine > count) {
                MessageBox.Show("雷的数量超过总数量", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            MineChange();

            Random random = new Random();

            for (int i = 1; i <= MapMine;) {
                int rd = random.Next(0, count);
                MyButton button = (MyButton)grid.Children[rd];
                if (!button.IsMine && rd != senderPos) {
                    button.IsMine = true;
                    i++;
                    //button.Background = Brushes.Green; // test
                }
            }

            // 计算每个格周围的雷的数量
            for (int i = 0; i < MapRow; i++) {
                for (int j = 0; j < MapColumn; j++) {
                    MyButton currBtn = (MyButton)grid.Children[i * MapColumn + j];
                    if (!currBtn.IsMine) {
                        for (int k = 0; k < 8; k++) {
                            int row = i + moveRow[k];
                            int column = j + moveCol[k];
                            if (!IsButtonInMap(row, column)) continue;
                            MyButton button = (MyButton)grid.Children[row * MapColumn + column];
                            if (button.IsMine) {
                                currBtn.AroundMineNum++;
                            }
                        }
                    }
                }
            }

            GameStart();
        }

        /// <summary>
        /// 区分右键单击 和 左右键同时单击
        /// </summary>
        private void DiscernMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Pressed) {
                Chord_BothButtonDown(sender);
            }
            else if (e.RightButton == MouseButtonState.Pressed) {
                Flag_RightButtonDown(sender);
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

            if (!button.IsFlag && !button.IsUncover) {
                if (button.IsMine) {

                    RestButton--;
                    RestMine--;
                    MineChange();

                    button.IsUncover = true;
                    button.Style = (Style)FindResource("MineButtonStyle");

                    GameFinish(false);
                }
                else {
                    UncoverButton(button.Row, button.Column);
                }
            }
            CheckFinishGame();
        }

        /// <summary>
        /// 右键格子事件, 即flag标记
        /// </summary>
        private void Flag_RightButtonDown(object sender)
        {
            MyButton button = (MyButton)sender;

            if (button.IsUncover) {
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
        private void Chord_BothButtonDown(object sender)
        {
            MyButton button = (MyButton)sender;

            if (button.IsFlag || !button.IsUncover || button.AroundMineNum == 0) {
                return;
            }

            int countFlag = 0;
            for (int i = 0; i < 8; i++) {
                int currRow = button.Row + moveRow[i];
                int currCol = button.Column + moveCol[i];

                if (IsButtonInMap(currRow, currCol)) {
                    MyButton currBtn = (MyButton)grid.Children[currRow * MapColumn + currCol];
                    if (currBtn.IsFlag) countFlag++;
                }
            }

            if (countFlag == button.AroundMineNum) {
                for (int i = 0; i < 8; i++) {
                    int currRow = button.Row + moveRow[i];
                    int currCol = button.Column + moveCol[i];

                    if (IsButtonInMap(currRow, currCol)) {
                        MyButton currBtn = (MyButton)grid.Children[currRow * MapColumn + currCol];
                        if (!currBtn.IsFlag && !currBtn.IsUncover) {
                            MyButton_Click(currBtn, null);
                            if (!gameStart) break;
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
        private void UncoverButton(int row, int column)
        {
            Queue<tuple> queue = new Queue<tuple>(); // 注意本命名空间最开始有一行: using tuple = Tuple<int, int>;
            queue.Enqueue(Tuple.Create(row, column));

            while (queue.Count != 0) {
                tuple pos = queue.Dequeue();

                if (!IsButtonInMap(pos.Item1, pos.Item2)) continue;

                MyButton button = (MyButton)grid.Children[pos.Item1 * MapColumn + pos.Item2];

                if (button.IsFlag || button.IsMine || button.IsUncover) continue;

                button.IsUncover = true;
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
        }

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

            foreach (MyButton button in grid.Children) {
                if (button.IsMine && button.IsUncover) {
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
