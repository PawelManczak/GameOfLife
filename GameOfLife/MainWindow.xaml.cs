using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameOfLife
{
    public partial class MainWindow : Window
    {
        private int[,] boardState; // Przechowywanie stanu planszy (0 - martwa, 1 - żywa)
        private int BoardWidth;
        private int BoardHeight;

        public MainWindow(int boardWidth, int boardHeight)
        {
            InitializeComponent();
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;

            InitializeGameBoard(BoardWidth, BoardHeight);
        }

        private void InitializeGameBoard(int width, int height)
        {
            // Tworzymy tablicę dwuwymiarową, która będzie przechowywać stan planszy
            boardState = new int[width, height];

            // Ustawiamy liczby wierszy i kolumn dla UniformGrid
            GameBoard.Rows = height;
            GameBoard.Columns = width;

            // Tworzymy przyciski reprezentujące komórki planszy
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Button cell = new Button();
                    cell.Background = Brushes.White; // Martwa komórka
                    cell.Tag = new Tuple<int, int>(i, j); // Przechowujemy współrzędne komórki
                    cell.Click += Cell_Click; // Obsługa kliknięcia

                    GameBoard.Children.Add(cell); // Dodanie komórki do UniformGrid
                }
            }
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            // Zmieniamy stan komórki po kliknięciu
            Button clickedCell = sender as Button;
            var coordinates = (Tuple<int, int>)clickedCell.Tag;
            int row = coordinates.Item1;
            int col = coordinates.Item2;

            // Zmieniamy stan komórki
            if (boardState[row, col] == 0)
            {
                // Komórka żywa
                boardState[row, col] = 1;
                clickedCell.Background = Brushes.Black;
            }
            else
            {
                // Komórka martwa
                boardState[row, col] = 0;
                clickedCell.Background = Brushes.White;
            }
        }
    }
}
