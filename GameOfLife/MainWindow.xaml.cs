using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace GameOfLife
{
    public partial class MainWindow : Window
    {
        private GameOfLifeEngine gameEngine;
        private GameBoardManager boardManager;
        private GameStateManager gameStateManager = new GameStateManager();
        private ImageSaver imageSaver = new ImageSaver();

        private DispatcherTimer timer;
        private bool isRunning = false;
        private int generation = 0;
        private int totalBorn = 0;
        private int totalDied = 0;

        private string cellShape = "Square";
        private Brush cellColor = Brushes.Black;

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

            if (gameEngine == null)
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
            gameEngine = new GameOfLifeEngine(width, height);
            boardManager = new GameBoardManager(GameBoard, gameEngine);
            boardManager.InitializeBoard(cellShape, cellColor, EnableAnimationsCheckBox.IsChecked == true);

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

            gameEngine.ClearBoard();
            boardManager.UpdateBoard();
        }

        private void RandomizeButton_Click(object sender, RoutedEventArgs e)
        {
            gameEngine.RandomizeBoard();
            boardManager.UpdateBoard();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void NextGeneration()
        {
            gameEngine.NextGeneration(out int born, out int died);
            boardManager.UpdateBoard();

            generation++;
            totalBorn += born;
            totalDied += died;
            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            GenerationTextBlock.Text = generation.ToString();
            BornTextBlock.Text = totalBorn.ToString();
            DiedTextBlock.Text = totalDied.ToString();
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

        private void CellShapeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CellShapeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                cellShape = selectedItem.Content.ToString();

                if (boardManager != null)
                {
                    boardManager.SetCellShape(cellShape);
                }
            }
        }

        private void CellColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CellColorComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string colorName = selectedItem.Content.ToString();

                cellColor = colorName switch
                {
                    "Black" => Brushes.Black,
                    "Red" => Brushes.Red,
                    "Green" => Brushes.Green,
                    "Blue" => Brushes.Blue,
                    "Yellow" => Brushes.Yellow,
                    _ => Brushes.Black,
                };

                if (boardManager != null)
                {
                    boardManager.SetCellColor(cellColor);
                }
            }
        }

        private void EnableAnimationsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (boardManager != null)
            {
                boardManager.SetAnimationsEnabled(EnableAnimationsCheckBox.IsChecked == true);
            }
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            gameStateManager.SaveGameState(gameEngine.BoardState, gameEngine.Width, gameEngine.Height);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var (loadedBoardState, width, height) = gameStateManager.LoadGameState();

            if (loadedBoardState != null)
            {
                if (isRunning)
                {
                    timer.Stop();
                    isRunning = false;
                    StartStopButton.Content = "Start";
                    StepButton.IsEnabled = true;
                }

                gameEngine = new GameOfLifeEngine(width, height);

                gameEngine.LoadBoardState(loadedBoardState);

                boardManager = new GameBoardManager(GameBoard, gameEngine);
                boardManager.InitializeBoard(cellShape, cellColor, EnableAnimationsCheckBox.IsChecked == true);

                boardManager.UpdateBoard();

                MessageBox.Show("Game state loaded successfully.", "Load", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Failed to load game state.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveImageButton_Click(object sender, RoutedEventArgs e)
        {
            imageSaver.SaveAsImage(GameBoard);
        }
    }
}
