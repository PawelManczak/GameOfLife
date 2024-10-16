using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameOfLive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
   public partial class MainWindow : Window
{
    private int BoardWidth;
    private int BoardHeight;

    public MainWindow(int boardWidth, int boardHeight)
    {
        InitializeComponent();
        BoardWidth = boardWidth;
        BoardHeight = boardHeight;
        // Inicjalizacja planszy gry (np. automatu komórkowego) tutaj
        InitializeGameBoard(BoardWidth, BoardHeight);
    }

    private void InitializeGameBoard(int width, int height)
    {
    }
}

}