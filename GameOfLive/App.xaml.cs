using GameOfLife;
using System.Configuration;
using System.Data;
using System.Windows;

namespace GameOfLive
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            BoardSizeWindow sizeWindow = new BoardSizeWindow();
            sizeWindow.ShowDialog();

            if (sizeWindow.IsSizeConfirmed)
            {
                int boardWidth = sizeWindow.BoardWidth;
                int boardHeight = sizeWindow.BoardHeight;

                MainWindow mainWindow = new MainWindow(boardWidth, boardHeight);
                mainWindow.Show();
            }
            else
            {
                this.Shutdown();
            }
        }

    }


}
