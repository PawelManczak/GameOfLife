using System;
using System.Windows;

namespace GameOfLife
{
    public partial class BoardSizeWindow : Window
    {
        // Delegate definition
        public delegate void BoardSizeConfirmedHandler(int width, int height);

        // Event definition
        public event BoardSizeConfirmedHandler BoardSizeConfirmed;

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
                // Raise the event
                BoardSizeConfirmed?.Invoke(width, height);
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter valid numeric values for board width and height.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
