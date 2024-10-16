﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

            // Create buttons representing the cells of the board
            for (int i = 0; i < BoardHeight; i++)
            {
                for (int j = 0; j < BoardWidth; j++)
                {
                    Button cell = new Button();
                    cell.Background = Brushes.White; // Dead cell
                    cell.Tag = new Tuple<int, int>(i, j); // Store cell coordinates
                    cell.Click += Cell_Click; // Handle click event

                    GameBoard.Children.Add(cell); // Add cell to UniformGrid
                }
            }

            // Initialize the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500); // Adjust speed as needed
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
            }
            else
            {
                // Start the simulation
                timer.Start();
                isRunning = true;
                StartStopButton.Content = "Stop";
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

        private void UpdateStatistics()
        {
            GenerationTextBlock.Text = generation.ToString();
            BornTextBlock.Text = totalBorn.ToString();
            DiedTextBlock.Text = totalDied.ToString();
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            // Change the cell state on click only when the simulation is not running
            if (!isRunning)
            {
                Button clickedCell = sender as Button;
                var coordinates = (Tuple<int, int>)clickedCell.Tag;
                int row = coordinates.Item1;
                int col = coordinates.Item2;

                // Toggle cell state
                if (boardState[row, col] == 0)
                {
                    // Cell becomes alive
                    boardState[row, col] = 1;
                    clickedCell.Background = Brushes.Black;
                }
                else
                {
                    // Cell becomes dead
                    boardState[row, col] = 0;
                    clickedCell.Background = Brushes.White;
                }
            }
        }
    }
}
