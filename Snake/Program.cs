using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using static System.Console;

namespace Snake
{
    internal class Program
    {
        private const int MapWidth = 30;
        private const int MapHeight = 20;
        private const int FrameMs = 200;

        private const int ScreenWidth = MapWidth * 3;

        private static Random Random = new Random();
        private const int ScreenHeight = MapHeight * 3;

        private const ConsoleColor BorderColor = ConsoleColor.Green;

        private const ConsoleColor HeadColor = ConsoleColor.Blue;

        private const ConsoleColor BodyColor = ConsoleColor.DarkBlue;
        private const ConsoleColor FoodColor = ConsoleColor.Red;
        static void Main()
        {
            SetWindowSize(ScreenWidth, ScreenHeight);
            SetBufferSize(ScreenWidth, ScreenHeight);
            CursorVisible = false;

            while (true)
            {
                StartGame();
                Thread.Sleep(1000);
                ReadKey();
            }
        }

        static Direction ReadMovement(Direction CurrentDirection)
        {
            if (!KeyAvailable)
                return CurrentDirection;

            ConsoleKey key = ReadKey(true).Key;

            CurrentDirection = key switch
            {
                ConsoleKey.UpArrow when CurrentDirection != Direction.Down => Direction.Up,
                ConsoleKey.DownArrow when CurrentDirection != Direction.Up => Direction.Down,
                ConsoleKey.LeftArrow when CurrentDirection != Direction.Right => Direction.Left,
                ConsoleKey.RightArrow when CurrentDirection != Direction.Down => Direction.Right,
                _ => CurrentDirection
            };
            return CurrentDirection;
        }

        static void StartGame()
        {
            Clear();
            DrawBorder();

            Direction CurrentMovement = Direction.Right;

            var snake = new SnakeC(10, 5, HeadColor, BodyColor);
            Pixel food = GenFood(snake);
            food.Draw();

            int score = 0;

            Stopwatch sw = new Stopwatch();
            while (true)
            {
                sw.Restart();

                Direction oldMovement = CurrentMovement;

                while (sw.ElapsedMilliseconds <= FrameMs)
                {
                    if (CurrentMovement == oldMovement)
                        CurrentMovement = ReadMovement(CurrentMovement);
                }

                if (snake.Head.X == food.X && snake.Head.Y == food.Y)
                {
                    snake.Move(CurrentMovement, true);
                    food = GenFood(snake);
                    food.Draw();
                    score++;
                }
                else
                {
                    snake.Move(CurrentMovement);
                }



                if (snake.Head.X == MapWidth - 1 || snake.Head.X == 0 || snake.Head.Y == MapHeight - 1 || snake.Head.Y == 0 || snake.Body.Any(b => b.X == snake.Head.X && b.Y == snake.Head.Y))
                {
                    break;
                }
            }
            snake.Clear();

            SetCursorPosition(ScreenWidth / 3, ScreenWidth / 2);
            WriteLine($"Game over. Score: {score}");
        }

        static Pixel GenFood(SnakeC snake)
        {
            Pixel food;

            do
            {
                food = new Pixel(Random.Next(1, MapWidth - 2), Random.Next(1, MapHeight - 2), FoodColor);
            } while (snake.Head.X == food.X && snake.Head.Y == food.Y||snake.Body.Any(b=>b.X==food.X&&b.Y==food.Y));
            return food;
        }
          

        static void DrawBorder()
        {
            for (int i = 0; i < MapWidth; i++)
            {
                new Pixel(i, 0, BorderColor).Draw();
                new Pixel(i, MapHeight - 1, BorderColor).Draw();
            }

            for (int i = 0; i < MapHeight; i++)
            {
                new Pixel(0, i, BorderColor).Draw();
                new Pixel(MapWidth - 1, i, BorderColor).Draw();
            }

        }
    }
}
