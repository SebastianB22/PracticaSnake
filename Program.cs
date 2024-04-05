using System;
using System.Collections.Generic;
using System.Threading;

namespace SnakeGame
{
    class Program
    {
        static int width = 40;
        static int height = 20;
        static int score = 0;
        static int delay = 100;
        static bool gameOver = false;
        static Random random = new Random();

        static Snake snake = new Snake();
        static int[] food = new int[2];

        static void Main(string[] args)
        {
            SetupGame();
            DrawBorder();
            DrawFood();
            DrawSnake();

            Thread inputThread = new Thread(ReadInput);
            inputThread.Start();

            while (!gameOver)
            {
                MoveSnake();
                if (IsEatingFood())
                {
                    score++;
                    DrawFood();
                }
                Thread.Sleep(delay);
            }

            GameOver();
        }

        static void SetupGame()
        {
            Console.Title = "JUEGO SNAKE";
            Console.CursorVisible = false;
            Console.SetWindowSize(width + 1, height + 2);
            Console.SetBufferSize(width + 1, height + 2);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Clear();
            Console.SetCursorPosition(width / 2 - 5, height / 2);
            Console.Write("Presiona cualquier tecla");
            Console.ReadKey();
            Console.Clear();
        }

        static void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;

            for (int i = 0; i < width + 2; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("=");
                Console.SetCursorPosition(i, height + 1);
                Console.Write("=");
            }

            for (int i = 1; i < height + 1; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("|");
                Console.SetCursorPosition(width + 1, i);
                Console.Write("|");
            }
        }

        static void DrawFood()
        {
            do
            {
                food[0] = random.Next(1, width);
                food[1] = random.Next(1, height);
            } while (snake.IsColliding(food));

            Console.SetCursorPosition(food[0], food[1]);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("°");
        }

        static void DrawSnake()
        {
            snake.Draw();
        }

        static void MoveSnake()
        {
            snake.Move();
            if (snake.IsColliding() || snake.IsOutOfBounds(width, height))
            {
                gameOver = true;
            }
        }

        static bool IsEatingFood()
        {
            if (snake.Head[0] == food[0] && snake.Head[1] == food[1])
            {
                snake.Grow();
                return true;
            }
            return false;
        }

        static void GameOver()
        {
            Console.SetCursorPosition(width / 2 - 5, height / 2);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("JUEGO TERMINADO AMIGO!");
            Console.SetCursorPosition(width / 2 - 8, height / 2 + 1);
            Console.Write($"RECORD: {score}");
            Console.SetCursorPosition(0, height + 1);
        }

        static void ReadInput()
        {
            while (!gameOver)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        snake.ChangeDirection(0, -1);
                        break;
                    case ConsoleKey.DownArrow:
                        snake.ChangeDirection(0, 1);
                        break;
                    case ConsoleKey.LeftArrow:
                        snake.ChangeDirection(-1, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        snake.ChangeDirection(1, 0);
                        break;
                    default:
                        gameOver = true;
                        break;
                }
            }
        }
    }

    class Snake
    {
        List<int[]> segments = new List<int[]>();
        int dx = 1;
        int dy = 0;

        public int[] Head => segments[0];

        public Snake()
        {
            segments.Add(new int[] { Program.width / 2, Program.height / 2 });
        }

        public void Draw()
        {
            foreach (var segment in segments)
            {
                Console.SetCursorPosition(segment[0], segment[1]);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(">");
            }
        }

        public void Move()
        {
            int[] newHead = { Head[0] + dx, Head[1] + dy };
            segments.Insert(0, newHead);
            Console.SetCursorPosition(newHead[0], newHead[1]);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(">");

            Console.SetCursorPosition(segments[segments.Count - 1][0], segments[segments.Count - 1][1]);
            Console.Write(" ");
            segments.RemoveAt(segments.Count - 1);
        }

        public bool IsColliding()
        {
            for (int i = 1; i < segments.Count; i++)
            {
                if (Head[0] == segments[i][0] && Head[1] == segments[i][1])
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsColliding(int[] point)
        {
            foreach (var segment in segments)
            {
                if (point[0] == segment[0] && point[1] == segment[1])
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsOutOfBounds(int width, int height)
        {
            return Head[0] <= 0 || Head[0] >= width + 1 || Head[1] <= 0 || Head[1] >= height + 1;
        }

        public void ChangeDirection(int x, int y)
        {
            if ((dx == 0 && x != 0) || (dy == 0 && y != 0))
            {
                dx = x;
                dy = y;
            }
        }

        public void Grow()
        {
            int[] lastSegment = segments[segments.Count - 1];
            segments.Add(lastSegment);
        }
    }
}