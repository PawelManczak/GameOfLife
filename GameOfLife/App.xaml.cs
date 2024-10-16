using GameOfLife;
using System.Configuration;
using System.Data;
using System.Windows;


namespace GameOfLife
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            BoardSizeWindow sizeWindow = new BoardSizeWindow();
            sizeWindow.ShowDialog();

            if (sizeWindow.IsSizeConfirmed)
            {
                int boardWidth = sizeWindow.BoardWidth;
                int boardHeight = sizeWindow.BoardHeight;

                Console.WriteLine(boardHeight);
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
