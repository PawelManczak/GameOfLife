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

            MainWindow mainWindow = new MainWindow();
            BoardSizeWindow sizeWindow = new BoardSizeWindow();
            sizeWindow.ShowDialog();

           
            if (sizeWindow.IsSizeConfirmed)
            {
                int boardWidth = sizeWindow.BoardWidth;
                int boardHeight = sizeWindow.BoardHeight;

               
                mainWindow.setSize(boardWidth, boardHeight);
                mainWindow.ShowDialog();
               
            }
            else
            {
                this.Shutdown();
            }
        }
    }
}
