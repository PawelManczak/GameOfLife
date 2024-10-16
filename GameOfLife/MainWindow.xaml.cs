using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
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

        private string cellShape = "Square";
        private Brush cellColor = Brushes.Black;

        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            this.Visibility = Visibility.Hidden;
            ShowBoardSizeWindow();
        }

        private void ShowBoardSizeWindow()
        {
            BoardSizeWindow sizeWindow = new BoardSizeWindow();
            sizeWindow.BoardSizeConfirmed += SizeWindow_BoardSizeConfirmed;
            sizeWindow.ShowDialog();

            if (BoardWidth == 0 || BoardHeight == 0)
            {
                this.Close();
            }
            else
            {
                this.Visibility = Visibility.Visible;
            }
        }

        private void SizeWindow_BoardSizeConfirmed(int width, int height)
        {
            BoardWidth = width;
            BoardHeight = height;
            InitializeGameBoard();
        }

        private void InitializeGameBoard()
        {
            boardState = new int[BoardHeight, BoardWidth];
            GameBoard.Rows = BoardHeight;
            GameBoard.Columns = BoardWidth;
            GameBoard.Children.Clear();

            for (int i = 0; i < BoardHeight; i++)
            {
                for (int j = 0; j < BoardWidth; j++)
                {
                    Border cellBorder = new Border
                    {
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(0.5),
                        Background = Brushes.White,
                        Tag = new Tuple<int, int>(i, j)
                    };
                    cellBorder.MouseLeftButtonDown += Cell_Click;
                    GameBoard.Children.Add(cellBorder);
                }
            }

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(SpeedSlider.Value)
            };
            timer.Tick += Timer_Tick;

            generation = 0;
            totalBorn = 0;
            totalDied = 0;
            UpdateStatistics();
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                timer.Stop();
                isRunning = false;
                StartStopButton.Content = "Start";
                StepButton.IsEnabled = true;
            }
            else
            {
                timer.Start();
                isRunning = true;
                StartStopButton.Content = "Stop";
                StepButton.IsEnabled = false;
            }
        }

        private void StepButton_Click(object sender, RoutedEventArgs e)
        {
            NextGeneration();
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timer != null)
            {
                timer.Interval = TimeSpan.FromMilliseconds(SpeedSlider.Value);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                timer.Stop();
                isRunning = false;
                StartStopButton.Content = "Start";
                StepButton.IsEnabled = true;
            }

            this.Visibility = Visibility.Hidden;

            generation = 0;
            totalBorn = 0;
            totalDied = 0;
            UpdateStatistics();

            BoardWidth = 0;
            BoardHeight = 0;
            ShowBoardSizeWindow();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                timer.Stop();
                isRunning = false;
                StartStopButton.Content = "Start";
                StepButton.IsEnabled = true;
            }

            for (int i = 0; i < BoardHeight; i++)
            {
                for (int j = 0; j < BoardWidth; j++)
                {
                    boardState[i, j] = 0;
                }
            }
            UpdateUI();
        }

        private void RandomizeButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < BoardHeight; i++)
            {
                for (int j = 0; j < BoardWidth; j++)
                {
                    boardState[i, j] = random.Next(2);
                }
            }
            UpdateUI();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void NextGeneration()
        {
            int[,] newBoardState = new int[BoardHeight, BoardWidth];
            int born = 0;
            int died = 0;

            for (int i = 0; i < BoardHeight; i++)
            {
                for (int j = 0; j < BoardWidth; j++)
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
            UpdateStatistics();
        }

        private int GetLiveNeighbors(int row, int col)
        {
            int liveNeighbors = 0;

            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    if (i == row && j == col)
                        continue;

                    if (i >= 0 && i < BoardHeight && j >= 0 && j < BoardWidth)
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
            for (int i = 0; i < BoardHeight; i++)
            {
                for (int j = 0; j < BoardWidth; j++)
                {
                    Border cellBorder = GameBoard.Children[index] as Border;
                    cellBorder.Child = null;

                    if (boardState[i, j] == 1)
                    {
                        UpdateCellVisual(cellBorder, true);
                    }
                    else
                    {
                        UpdateCellVisual(cellBorder, false);
                    }
                    index++;
                }
            }
        }

        private void UpdateStatistics()
        {
            GenerationTextBlock.Text = generation.ToString();
            BornTextBlock.Text = totalBorn.ToString();
            DiedTextBlock.Text = totalDied.ToString();
        }

        private void Cell_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!isRunning)
            {
                Border clickedCell = sender as Border;
                var coordinates = (Tuple<int, int>)clickedCell.Tag;
                int row = coordinates.Item1;
                int col = coordinates.Item2;

                if (boardState[row, col] == 0)
                {
                    boardState[row, col] = 1;
                    UpdateCellVisual(clickedCell, true);
                }
                else
                {
                    boardState[row, col] = 0;
                    UpdateCellVisual(clickedCell, false);
                }
            }
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (GameBoardScaleTransform != null)
            {
                double newZoom = ZoomSlider.Value;

                DoubleAnimation scaleXAnimation = new DoubleAnimation
                {
                    To = newZoom,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase()
                };

                DoubleAnimation scaleYAnimation = new DoubleAnimation
                {
                    To = newZoom,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase()
                };

                GameBoardScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnimation);
                GameBoardScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnimation);
            }
        }

        private void UpdateCellVisual(Border cellBorder, bool isAlive)
        {
            cellBorder.Child = null;
            Shape shape = cellShape == "Circle" ? new Ellipse() : (Shape)new Rectangle();
            if (isAlive)
            {
                shape.Fill = cellColor;
                shape.Stretch = Stretch.Uniform;
                cellBorder.Background = Brushes.White;
                cellBorder.Child = shape;

                if (EnableAnimationsCheckBox.IsChecked == true)
                {
                    DoubleAnimation fadeInAnimation = new DoubleAnimation
                    {
                        From = 0.0,
                        To = 1.0,
                        Duration = TimeSpan.FromMilliseconds(300),
                        EasingFunction = new QuadraticEase()
                    };
                    shape.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
                }
            }
            else
            {
                if (EnableAnimationsCheckBox.IsChecked == true)
                {
                    DoubleAnimation fadeOutAnimation = new DoubleAnimation
                    {
                        From = 1.0,
                        To = 0.0,
                        Duration = TimeSpan.FromMilliseconds(300),
                        EasingFunction = new QuadraticEase()
                    };
                    shape.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);

                    fadeOutAnimation.Completed += (s, e) =>
                    {
                        cellBorder.Background = Brushes.White;
                        cellBorder.Child = null;
                    };
                }
                else
                {
                    cellBorder.Background = Brushes.White;
                    cellBorder.Child = null;
                }
            }
        }

        private void CellShapeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cellShape = (CellShapeComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            UpdateUI();
        }

        private void CellColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string colorName = (CellColorComboBox.SelectedItem as ComboBoxItem).Content.ToString();

            cellColor = colorName switch
            {
                "Black" => Brushes.Black,
                "Red" => Brushes.Red,
                "Green" => Brushes.Green,
                "Blue" => Brushes.Blue,
                "Yellow" => Brushes.Yellow,
                _ => Brushes.Black,
            };

            UpdateUI();
        }
    }
}
