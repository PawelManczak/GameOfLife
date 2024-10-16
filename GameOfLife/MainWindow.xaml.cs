using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace GameOfLife
{
    public partial class MainWindow : Window
    {
        private int[,] boardState;
        private int BoardWidth;
        private int BoardHeight;

        private bool isRunning = false;
        private DispatcherTimer timer;
        private int generation = 0;
        private int totalBorn = 0;
        private int totalDied = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(int boardWidth, int boardHeight) : this()
        {
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            InitializeGameBoard(BoardWidth, BoardHeight);
        }

        public void setSize(int boardWidth, int boardHeight)
        {
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            InitializeGameBoard(BoardWidth, BoardHeight);
        }

        private void InitializeGameBoard(int width, int height)
        {
            boardState = new int[height, width]; 

            GameBoard.Rows = height;
            GameBoard.Columns = width;

            GameBoard.Children.Clear();

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Button cell = new Button();
                    cell.Background = Brushes.White; 
                    cell.Tag = new Tuple<int, int>(i, j); 
                    cell.Click += Cell_Click;

                    GameBoard.Children.Add(cell); 
                }
            }

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500); 
            timer.Tick += Timer_Tick;
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                timer.Stop();
                isRunning = false;
                StartStopButton.Content = "Start";
            }
            else
            {
                timer.Start();
                isRunning = true;
                StartStopButton.Content = "Stop";
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void NextGeneration()
        {
            int height = BoardHeight;
            int width = BoardWidth;
            int[,] newBoardState = new int[height, width];
            int born = 0;
            int died = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int liveNeighbors = GetLiveNeighbors(i, j);
                    if (boardState[i, j] == 1)
                    {
                        if (liveNeighbors < 2 || liveNeighbors > 3)
                        {
                            newBoardState[i, j] = 0;
                            died++;
                        }
                        else
                        {
                            newBoardState[i, j] = 1;
                        }
                    }
                    else
                    {
                        if (liveNeighbors == 3)
                        {
                            newBoardState[i, j] = 1;
                            born++;
                        }
                        else
                        {
                            newBoardState[i, j] = 0;
                        }
                    }
                }
            }

            boardState = newBoardState;

            UpdateUI();

            generation++;
            totalBorn += born;
            totalDied += died;
            GenerationTextBlock.Text = generation.ToString();
            BornTextBlock.Text = totalBorn.ToString();
            DiedTextBlock.Text = totalDied.ToString();
        }

        private int GetLiveNeighbors(int row, int col)
        {
            int liveNeighbors = 0;
            int height = BoardHeight;
            int width = BoardWidth;

            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    if (i == row && j == col)
                        continue;

                    if (i >= 0 && i < height && j >= 0 && j < width)
                    {
                        if (boardState[i, j] == 1)
                            liveNeighbors++;
                    }
                }
            }

            return liveNeighbors;
        }

        private void UpdateUI()
        {
            int index = 0;
            int height = BoardHeight;
            int width = BoardWidth;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Button cell = GameBoard.Children[index] as Button;
                    if (boardState[i, j] == 1)
                    {
                        cell.Background = Brushes.Black;
                    }
                    else
                    {
                        cell.Background = Brushes.White;
                    }
                    index++;
                }
            }
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (!isRunning)
            {
                Button clickedCell = sender as Button;
                var coordinates = (Tuple<int, int>)clickedCell.Tag;
                int row = coordinates.Item1;
                int col = coordinates.Item2;

                if (boardState[row, col] == 0)
                {
                    boardState[row, col] = 1;
                    clickedCell.Background = Brushes.Black;
                }
                else
                {
                    boardState[row, col] = 0;
                    clickedCell.Background = Brushes.White;
                }
            }
        }
    }
}
