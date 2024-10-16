﻿
using System.Windows;
namespace GameOfLife
{
    public partial class BoardSizeWindow : Window
    {
        public int BoardWidth { get; private set; }
        public int BoardHeight { get; private set; }
        public bool IsSizeConfirmed { get; private set; } = false;

        public BoardSizeWindow()
        {
            InitializeComponent();
        }

        private void ConfirmSize_Click(object sender, RoutedEventArgs e)
        {
            bool isWidthValid = int.TryParse(WidthTextBox.Text, out int width);
            bool isHeightValid = int.TryParse(HeightTextBox.Text, out int height);

            if (isWidthValid && isHeightValid && width > 0 && height > 0)
            {
                BoardWidth = width;
                BoardHeight = height;

                IsSizeConfirmed = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Proszę wprowadzić prawidłowe wartości liczbowe dla szerokości i wysokości planszy.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsSizeConfirmed = false;
            this.Close();
        }
    }
}
