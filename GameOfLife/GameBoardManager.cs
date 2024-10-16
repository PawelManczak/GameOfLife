using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GameOfLife
{
    public class GameBoardManager
    {
        private UniformGrid gameBoardGrid;
        private GameOfLifeEngine gameEngine;
        private string cellShape;
        private Brush cellColor;
        private bool enableAnimations;
        private Border[,] cellBorders;

        public GameBoardManager(UniformGrid gameBoardGrid, GameOfLifeEngine gameEngine)
        {
            this.gameBoardGrid = gameBoardGrid;
            this.gameEngine = gameEngine;
            cellBorders = new Border[gameEngine.Height, gameEngine.Width];
        }

        public void InitializeBoard(string cellShape, Brush cellColor, bool enableAnimations)
        {
            this.cellShape = cellShape;
            this.cellColor = cellColor;
            this.enableAnimations = enableAnimations;

            gameBoardGrid.Rows = gameEngine.Height;
            gameBoardGrid.Columns = gameEngine.Width;
            gameBoardGrid.Children.Clear();

            for (int i = 0; i < gameEngine.Height; i++)
            {
                for (int j = 0; j < gameEngine.Width; j++)
                {
                    Border cellBorder = new Border
                    {
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(0.5),
                        Background = Brushes.White,
                        Tag = new Tuple<int, int>(i, j)
                    };
                    cellBorder.MouseLeftButtonDown += CellBorder_MouseLeftButtonDown;
                    gameBoardGrid.Children.Add(cellBorder);
                    cellBorders[i, j] = cellBorder;
                }
            }
        }

        public void UpdateBoard()
        {
            for (int i = 0; i < gameEngine.Height; i++)
            {
                for (int j = 0; j < gameEngine.Width; j++)
                {
                    UpdateCellVisual(cellBorders[i, j], gameEngine.GetCellState(i, j) == 1);
                }
            }
        }

        public void SetCellShape(string shape)
        {
            cellShape = shape;
            UpdateBoard();
        }

        public void SetCellColor(Brush color)
        {
            cellColor = color;
            UpdateBoard();
        }

        public void SetAnimationsEnabled(bool isEnabled)
        {
            enableAnimations = isEnabled;
        }

        private void CellBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Border clickedCell = sender as Border;
            var coordinates = (Tuple<int, int>)clickedCell.Tag;
            int row = coordinates.Item1;
            int col = coordinates.Item2;

            int currentState = gameEngine.GetCellState(row, col);
            int newState = currentState == 0 ? 1 : 0;
            gameEngine.SetCellState(row, col, newState);

            UpdateCellVisual(clickedCell, newState == 1);
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

                if (enableAnimations)
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
                if (enableAnimations)
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
    }
}

