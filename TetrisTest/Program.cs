using System;
using System.Collections.Generic;
using System.Threading;

namespace TetrisTest
{
    class Program
    {
        #region Variables
        static int tetrisRows = 20;
        static int tetrisCols = 10;
        static int infoCols = 8;
        static int consoleRows = 1 + tetrisRows + 1;
        static int consoleCols = 1 + tetrisCols + 1 + infoCols + 1;
        static List<bool[,]> tetrisFigures = new List<bool[,]>()
        {

            new bool[,] //I
            {
                {true, true, true, true }
            },

            new bool[,] //O 
            {
                {true, true },
                {true, true }
            },

            new bool[,] //T 
            {
                {false, true, false },
                {true,  true, true }
            },

            new bool[,] //S
            {
                {false, true, true },
                { true, true, false}
            },

            new bool[,] //Z 
            {
                {true, true, false },
                {false, true, true }
            },

            new bool[,]//J 
            {
                {true, false, false },
                {true, true, true }
            },

            new bool[,]//L  
            {
                {false, false, true },
                {true, true, true }
            }
        };
        static int[] scorePerLines = { 0, 40, 100, 300, 1200 };
        static int score = 0;
        static int frame = 0;
        static int frameToMoveFigure = 15;
        static bool[,] currentFigure = null;
        static int currentFigureRow = 0;
        static int currentFigureCol = 0;
        static bool[,] tetrisField = new bool[tetrisRows, tetrisCols];
        static Random random = new Random();
        #endregion

        static void Main(string[] args)
        {
            Console.Title = "Tetris Test";
            Console.CursorVisible = false;
            Console.WindowHeight = consoleRows + 1;
            Console.BufferHeight = consoleRows + 1;
            Console.WindowWidth = consoleCols;
            Console.BufferWidth = consoleCols;
            currentFigure = tetrisFigures[random.Next(0, tetrisFigures.Count)];

            while (true)
            {
                //user input
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape)
                    {
                        return;
                    }

                    if (key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.A)
                    {
                        // move the current figure left
                        if (currentFigureCol >= 1)
                        {
                            currentFigureCol--;
                        }
                    }
                    if (key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.D)
                    {
                        // move the current figure right
                        if (currentFigureCol < tetrisCols - currentFigure.GetLength(1))
                        {
                            currentFigureCol++;
                        }
                    }
                    if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.S)
                    {
                        // move the current figure down
                        frame = 1;
                        score++;
                        currentFigureRow++;
                    }

                    if (key.Key == ConsoleKey.Spacebar || key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W)
                    {
                        RotateCurrentFigure();
                    }


                }
                //change state
                if (frame % frameToMoveFigure == 0)
                {
                    currentFigureRow++;
                    frame = 0;
                    score++;
                }

                if (Collision(currentFigure))
                {

                    AddCurrentFigureToTetrisField();
                    currentFigure = tetrisFigures[random.Next(0, tetrisFigures.Count)];
                    currentFigureRow = 0;
                    currentFigureCol = 0;
                    int lines = CheckForFullLines();
                    score += scorePerLines[lines];

                    if (Collision(currentFigure))
                    {
                        var scoreAsString = score.ToString();
                        scoreAsString += new string(' ', 7 - scoreAsString.Length);
                        Write("╔══════════╗", 5, 5, ConsoleColor.DarkYellow);
                        Write("║   GAME   ║", 6, 5, ConsoleColor.DarkYellow);
                        Write("║   OVER!  ║", 7, 5, ConsoleColor.DarkYellow);
                        Write($"║   {scoreAsString}║", 8, 5, ConsoleColor.DarkYellow);
                        Write("╚══════════╝", 9, 5, ConsoleColor.DarkYellow);
                        Console.ReadKey();
                        Thread.Sleep(100000);
                        return;
                    }
                }

                //redraw UI
                DrawConsoleBorder();
                DrawInfo();
                DrawTetrisField();
                DrawCurrentFigure();

                frame++;
                Thread.Sleep(40);
            }
        }

        private static void RotateCurrentFigure()
        {
            var newFigure = new bool[currentFigure.GetLength(1), currentFigure.GetLength(0)];

            for (int row = 0; row < currentFigure.GetLength(0); row++)
            {
                for (int col = 0; col < currentFigure.GetLength(1); col++)
                {
                    newFigure[col, currentFigure.GetLength(0) - row - 1] = currentFigure[row, col];
                }
            }

            if (!Collision(newFigure))
            {
                currentFigure = newFigure;
            }

        }

        static int CheckForFullLines()
        {
            int lines = 0;
            for (int row = 0; row < tetrisField.GetLength(0); row++)
            {
                bool rowIsFull = true;
                for (int col = 0; col < tetrisField.GetLength(1); col++)
                {
                    if (tetrisField[row, col] == false)
                    {
                        rowIsFull = false;
                        break;
                    }
                }

                if (rowIsFull)
                {
                    for (int rowToMove = row; rowToMove >= 1; rowToMove--)
                    {
                        for (int col = 0; col < tetrisField.GetLength(1); col++)
                        {
                            tetrisField[rowToMove, col] = tetrisField[rowToMove - 1, col];
                        }
                    }

                    lines++;
                }
            }


            return lines;
        }

        static void AddCurrentFigureToTetrisField()
        {
            for (int row = 0; row < currentFigure.GetLength(0); row++)
            {
                for (int col = 0; col < currentFigure.GetLength(1); col++)
                {
                    if (currentFigure[row, col])
                    {
                        tetrisField[currentFigureRow + row, currentFigureCol + col] = true;
                    }
                }
            }


        }

        static bool Collision(bool[,] figure)
        {

            if (currentFigureCol > tetrisCols - figure.GetLength(1))
            {
                return true;
            }

            if (currentFigureRow + figure.GetLength(0) == tetrisRows)
            {
                return true;
            }

            for (int row = 0; row < figure.GetLength(0); row++)
            {
                for (int col = 0; col < figure.GetLength(1); col++)
                {
                    if (figure[row, col] && tetrisField[currentFigureRow + row + 1, currentFigureCol + col])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static void DrawConsoleBorder()
        {
            Console.SetCursorPosition(0, 0);
            string line = "╔";
            line += new string('═', tetrisCols);
            line += "╦";
            line += new string('═', infoCols);
            line += "╗";
            Console.Write(line);

            for (int i = 0; i < tetrisRows; i++)
            {
                string middleLine = "║";
                middleLine += new string(' ', tetrisCols);
                middleLine += "║";
                middleLine += new string(' ', infoCols);
                middleLine += "║";
                Console.Write(middleLine);
            }

            string endLine = "╚";
            endLine += new string('═', tetrisCols);
            endLine += "╩";
            endLine += new string('═', infoCols);
            endLine += "╝";
            Console.Write(endLine);
        }

        static void DrawInfo()
        {
            Write("Score:", 1, 3 + tetrisCols, ConsoleColor.Cyan);
            Write(score.ToString(), 2, 3 + tetrisCols, ConsoleColor.Cyan);

            Write("Frame:", 4, 3 + tetrisCols, ConsoleColor.Cyan);
            Write(frame.ToString(), 5, 3 + tetrisCols, ConsoleColor.Cyan);

            Write("Pos:", 7, 3 + tetrisCols, ConsoleColor.Cyan);
            Write($"{currentFigureRow},{currentFigureCol}", 8, 3 + tetrisCols, ConsoleColor.Cyan);

            Write("Keys:", 10, 3 + tetrisCols, ConsoleColor.Cyan);
            Write($"   ^  ", 11, 3 + tetrisCols, ConsoleColor.Cyan);
            Write($" < v >", 12, 3 + tetrisCols, ConsoleColor.Cyan);
        }

        static void DrawTetrisField()
        {
            for (int row = 0; row < tetrisField.GetLength(0); row++)
            {
                for (int col = 0; col < tetrisField.GetLength(1); col++)
                {
                    if (tetrisField[row, col])
                    {
                        Write("@", row + 1, col + 1, ConsoleColor.Cyan);
                    }
                }
            }
        }

        static void DrawCurrentFigure()
        {
            for (int row = 0; row < currentFigure.GetLength(0); row++)
            {
                for (int col = 0; col < currentFigure.GetLength(1); col++)
                {
                    if (currentFigure[row, col])
                    {
                        Write("@", row + 1 + currentFigureRow, col + 1 + currentFigureCol, ConsoleColor.Blue);
                    }
                }
            }
            Console.ResetColor();

        }

        static void Write(string word, int row, int col, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(col, row);
            Console.WriteLine(word);
            Console.ResetColor();
        }
    }
}
