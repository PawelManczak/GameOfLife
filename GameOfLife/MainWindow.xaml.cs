using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GameOfLife
{
    public partial class MainWindow : Window
    {
        private int[,] boardState; // Stores the state of the board (0 - dead, 1 - alive)
        private int BoardWidth;
        private int BoardHeight;

        private bool isRunning = false;
        private DispatcherTimer timer;
        private int generation = 0;
        private int totalBorn = 0;
        private int totalDied = 0;

        private string cellShape = "Square";
        private Brush cellColor = Brushes.Black;

        public MainWindow()
        {
            InitializeComponent();

            // Hide the main window until the board size is set
            this.Visibility = Visibility.Hidden;

            // Show the board size selection window
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
            // Initialize the board state array
            boardState = new int[BoardHeight, BoardWidth]; // [rows, columns]

            // Set the number of rows and columns for the UniformGrid
            GameBoard.Rows = BoardHeight;
            GameBoard.Columns = BoardWidth;

            // Clear existing elements
            GameBoard.Children.Clear();

            // Create cells representing the cells of the board
            for (int i = 0; i < BoardHeight; i++)
            {
                for (int j = 0; j < BoardWidth; j++)
                {
                    Border cellBorder = new Border();
                    cellBorder.BorderBrush = Brushes.Gray;
                    cellBorder.BorderThickness = new Thickness(0.5);

                    // Set the background to white (dead cell)
                    cellBorder.Background = Brushes.White;

                    cellBorder.Tag = new Tuple<int, int>(i, j); // Store cell coordinates
                    cellBorder.MouseLeftButtonDown += Cell_Click; // Handle click event

                    GameBoard.Children.Add(cellBorder); // Add cell to UniformGrid
                }
            }

            // Initialize the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(SpeedSlider.Value); // Use slider value for interval
            timer.Tick += Timer_Tick;

            // Reset statistics
            generation = 0;
            totalBorn = 0;
            totalDied = 0;
            UpdateStatistics();
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                // Stop the simulation
                timer.Stop();
                isRunning = false;
                StartStopButton.Content = "Start";
                StepButton.IsEnabled = true; // Enable the Step button
            }
            else
            {
                // Start the simulation
                timer.Start();
                isRunning = true;
                StartStopButton.Content = "Stop";
                StepButton.IsEnabled = false; // Disable the Step button while running
            }
        }

        private void StepButton_Click(object sender, RoutedEventArgs e)
        {
            // Perform a single step
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
            // Stop the simulation if it's running
            if (isRunning)
            {
                timer.Stop();
                isRunning = false;
                StartStopButton.Content = "Start";
                StepButton.IsEnabled = true;
            }

            // Hide the main window
            this.Visibility = Visibility.Hidden;

            // Reset statistics
            generation = 0;
            totalBorn = 0;
            totalDied = 0;
            UpdateStatistics();

            // Show the board size selection window again
            BoardWidth = 0;
            BoardHeight = 0;
            ShowBoardSizeWindow();
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
                        // Alive cell
                        if (liveNeighbors < 2 || liveNeighbors > 3)
                        {
                            // Cell dies
                            newBoardState[i, j] = 0;
                            died++;
                        }
                        else
                        {
                            // Cell survives
                            newBoardState[i, j] = 1;
                        }
                    }
                    else
                    {
                        // Dead cell
                        if (liveNeighbors == 3)
                        {
                            // Cell becomes alive
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

            // Update the board state
            boardState = newBoardState;

            // Update the UI
            UpdateUI();

            // Update statistics
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

                    // Check if indices are within the bounds of the board
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
                    cellBorder.Child = null; // Clear previous content

                    if (boardState[i, j] == 1)
                    {
                        // Live cell
                        Shape shape;

                        if (cellShape == "Circle")
                        {
                            shape = new Ellipse();
                        }
                        else // "Square"
                        {
                            shape = new Rectangle();
                        }

                        shape.Fill = cellColor;
                        shape.Stretch = Stretch.Uniform;
                        cellBorder.Background = Brushes.White; // Keep background white
                        cellBorder.Child = shape;
                    }
                    else
                    {
                        // Dead cell
                        cellBorder.Background = Brushes.White;
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
            // Change the cell state on click only when the simulation is not running
            if (!isRunning)
            {
                Border clickedCell = sender as Border;
                var coordinates = (Tuple<int, int>)clickedCell.Tag;
                int row = coordinates.Item1;
                int col = coordinates.Item2;

                // Toggle cell state
                if (boardState[row, col] == 0)
                {
                    // Cell becomes alive
                    boardState[row, col] = 1;
                    UpdateCellVisual(clickedCell, true);
                }
                else
                {
                    // Cell becomes dead
                    boardState[row, col] = 0;
                    UpdateCellVisual(clickedCell, false);
                }
            }
        }

        private void UpdateCellVisual(Border cellBorder, bool isAlive)
        {
            cellBorder.Child = null; // Clear previous content

            if (isAlive)
            {
                Shape shape;

                if (cellShape == "Circle")
                {
                    shape = new Ellipse();
                }
                else // "Square"
                {
                    shape = new Rectangle();
                }

                shape.Fill = cellColor;
                shape.Stretch = Stretch.Uniform;
                cellBorder.Background = Brushes.White; // Keep background white
                cellBorder.Child = shape;
            }
            else
            {
                cellBorder.Background = Brushes.White;
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

            switch (colorName)
            {
                case "Black":
                    cellColor = Brushes.Black;
                    break;
                case "Red":
                    cellColor = Brushes.Red;
                    break;
                case "Green":
                    cellColor = Brushes.Green;
                    break;
                case "Blue":
                    cellColor = Brushes.Blue;
                    break;
                case "Yellow":
                    cellColor = Brushes.Yellow;
                    break;
                default:
                    cellColor = Brushes.Black;
                    break;
            }

            UpdateUI();
        }
    }
}
