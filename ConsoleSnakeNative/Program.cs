using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ConsoleSnake
{
    internal class Snake
    {
        public const char Head = '#';
        public const char Body = '.';
        public const char Food = 'F';
        public const int FieldSize = 17;
        public static Vector2 SnakeDirection;
        public static List<Vector2> SnakeBody;
        public static Vector2 FoodPosition;
        public static Random Random;
        public static Timer GameTick;
        public static int TickTime = 100;
        public static Vector2 NextDirection;
        static void Main(string[] args)
        {

            Random = new Random();
            //Console.SetWindowSize(FieldSize, FieldSize * 2);
            NextDirection = Vector2.UnitX;
            SnakeBody = new List<Vector2>() { new Vector2(0, (float)Math.Round(FieldSize / 2d)) };
            GenerateFood();
            GameTick = new Timer(TickTime);
            GameTick.Elapsed += Tick;
            GameTick.AutoReset = true;
            GameTick.Enabled = true;
            GameTick.Start();
            while (true)
            {
                var input = Console.ReadKey().Key;
                switch (input)
                {
                    case ConsoleKey.UpArrow:
                        if (SnakeDirection != Vector2.UnitY)
                            NextDirection = -Vector2.UnitY;
                        break;
                    case ConsoleKey.DownArrow:
                        if (SnakeDirection != -Vector2.UnitY)
                            NextDirection = Vector2.UnitY;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (SnakeDirection != Vector2.UnitX)
                            NextDirection = -Vector2.UnitX;
                        break;
                    case ConsoleKey.RightArrow:
                        if (SnakeDirection != -Vector2.UnitX)
                            NextDirection = Vector2.UnitX;
                        break;
                    case ConsoleKey.N:
                        Console.Clear();
                        NextDirection = Vector2.UnitX;
                        SnakeBody = new List<Vector2>() { new Vector2(0, (float)Math.Round(FieldSize / 2d)) };
                        GenerateFood();
                        GameTick.Start();
                        break;
                    case ConsoleKey.E: return;
                }
            }
            Console.ReadLine();
        }


        public static void Tick(object sender, ElapsedEventArgs e)
        {
            SnakeDirection = NextDirection;
            var nextHead = SnakeBody[0] + SnakeDirection;
            //if (nextHead.X == -1)
            //    GameTick.Stop();
            if (nextHead.X == -1) nextHead.X = FieldSize - 2;
            if (nextHead.X == FieldSize - 1) nextHead.X = 0;
            if (nextHead.Y == -1) nextHead.Y = FieldSize - 2;
            if (nextHead.Y == FieldSize - 1) nextHead.Y = 0;
            bool go = false;
            if (SnakeBody.Contains(nextHead))
            {
                SnakeBody.Remove(nextHead);
                SnakeBody.Insert(0, nextHead);
                go = true;
            }
            else
            {
                SnakeBody.Insert(0, nextHead);
                if (nextHead != FoodPosition)
                    SnakeBody.RemoveAt(SnakeBody.Count - 1);
                else
                {
                    GenerateFood();
                }
            }

            var snakeImg = SnakeBody.Select(i => (i, Body)).ToList();
            snakeImg[0] = (snakeImg[0].ToTuple().Item1, Head);
            snakeImg.Add((FoodPosition, Food));
            Render(snakeImg, go);
            if (go)
                GameTick.Stop();
        }

        public static void Render(List<(Vector2, char)> image, bool gameOver)
        {
            StringBuilder img = new StringBuilder(string.Join("", Enumerable.Repeat(" ", FieldSize * FieldSize)));
            foreach (var pix in image)
            {
                img[(int)(pix.Item1.X + pix.Item1.Y * FieldSize)] = pix.Item2;
            }
            for (int i = 0; i < FieldSize; i++)
            {
                img[(i + 1) * FieldSize - 1] = '|';
                img[FieldSize * FieldSize - 1 - i] = '-';
                img[(FieldSize * FieldSize) - 1] = '+';
            }
            Console.SetCursorPosition(0, 0);
            var stringed = img.ToString();
            stringed = string.Join("\n", ChunkString(stringed, FieldSize));
            stringed += $"\nСчет: {SnakeBody.Count - 1}";
            if (gameOver)
                stringed += "\nИгра окончена\nДля начала новой игры нажмите N";
            Console.WriteLine(stringed);
            Console.SetCursorPosition(0, FieldSize + 1);
        }

        public static void GenerateFood()
        {
            var food = new Vector2(Random.Next(FieldSize - 1), Random.Next(FieldSize - 1));
            while (SnakeBody.Any(x => x == food))
            {
                food = new Vector2(Random.Next(FieldSize - 1), Random.Next(FieldSize - 1));
            }
            FoodPosition = food;
        }

        public static List<string> ChunkString(string str, int size)
        {
            var list = new List<string>();
            var iter = str;
            while (iter.Length > size)
            {
                list.Add(new string(iter.Take(size).ToArray()));
                iter = new string(iter.Skip(size).ToArray());
            }
            list.Add(iter);
            return list;
        }

    }
}
