using System;

namespace GameOfLife
{
    public class GameOfLifeEngine
    {
        private int[,] boardState;
        private int width;
        private int height;

        public int[,] BoardState => boardState;
        public int Width => width;
        public int Height => height;

        public GameOfLifeEngine(int width, int height)
        {
            this.width = width;
            this.height = height;
            boardState = new int[height, width];
        }

        public void SetCellState(int row, int col, int state)
        {
            boardState[row, col] = state;
        }

        public int GetCellState(int row, int col)
        {
            return boardState[row, col];
        }

        public void RandomizeBoard()
        {
            Random random = new Random();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    boardState[i, j] = random.Next(2);
                }
            }
        }

        public void LoadBoardState(int[,] loadedBoardState)
        {
            this.boardState = loadedBoardState;
            this.height = loadedBoardState.GetLength(0);
            this.width = loadedBoardState.GetLength(1);
        }


        public void ClearBoard()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    boardState[i, j] = 0;
                }
            }
        }

        public void NextGeneration(out int born, out int died)
        {
            int[,] newBoardState = new int[height, width];
            born = 0;
            died = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int liveNeighbors = GetLiveNeighbors(i, j);
                    if (boardState[i, j] == 1)
                    {
                        if (liveNeighbors < 2 || liveNeighbors > 3)
                        {
                            newBoardState[i, j] = 0;
                            died++;
                        }
                        else
                        {
                            newBoardState[i, j] = 1;
                        }
                    }
                    else
                    {
                        if (liveNeighbors == 3)
                        {
                            newBoardState[i, j] = 1;
                            born++;
                        }
                        else
                        {
                            newBoardState[i, j] = 0;
                        }
                    }
                }
            }

            boardState = newBoardState;
        }

        private int GetLiveNeighbors(int row, int col)
        {
            int liveNeighbors = 0;

            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    if (i == row && j == col)
                        continue;

                    if (i >= 0 && i < height && j >= 0 && j < width)
                    {
                        if (boardState[i, j] == 1)
                            liveNeighbors++;
                    }
                }
            }

            return liveNeighbors;
        }
    }
}
