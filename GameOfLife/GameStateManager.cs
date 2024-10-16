using System;
using System.IO;
using Microsoft.Win32;

namespace GameOfLife
{
    public class GameStateManager
    {
        public void SaveGameState(int[,] boardState, int width, int height)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text File (*.txt)|*.txt",
                Title = "Save Game State"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    writer.WriteLine(width);
                    writer.WriteLine(height);

                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            writer.Write(boardState[i, j]);
                        }
                        writer.WriteLine();
                    }
                }
            }
        }

        public (int[,] boardState, int width, int height) LoadGameState()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text File (*.txt)|*.txt",
                Title = "Load Game State"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                {
                    int width = int.Parse(reader.ReadLine());
                    int height = int.Parse(reader.ReadLine());

                    int[,] boardState = new int[height, width];

                    for (int i = 0; i < height; i++)
                    {
                        string line = reader.ReadLine();
                        for (int j = 0; j < width; j++)
                        {
                            boardState[i, j] = line[j] - '0';
                        }
                    }

                    return (boardState, width, height);
                }
            }

            return (null, 0, 0); 
        }
    }
}
