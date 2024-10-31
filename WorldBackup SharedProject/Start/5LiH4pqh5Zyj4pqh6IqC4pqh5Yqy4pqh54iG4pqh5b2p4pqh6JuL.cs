using com.Lavaver.WorldBackup.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.Lavaver.WorldBackup.Start
{
    internal class _5LiH4pqh5Zyj4pqh6IqC4pqh5Yqy4pqh54iG4pqh5b2p4pqh6JuL
    {
        static int ballX = 0, ballY = 0;
        static int ballDirectionX = 1, ballDirectionY = 1;
        static int paddleY = 10;
        static int score = 0;
        static int width = 20, height = 20;

        public static void _6K6p5oiR5Lus54uC5qyi5ZCn77yB()
        {
            var today = DateTime.Today;
            if (today.Month == 10 && today.Day == 31)
            {
                _44CKMyBBIOS6jCDnu7Qg57uPIOWFuCDlpKcg5L2c44CL();
            }
            else
            {
                LogConsole.Log("Init", "现在使用该参数太早了...", ConsoleColor.Red);
            }
        }

        static void _44CKMyBBIOS6jCDnu7Qg57uPIOWFuCDlpKcg5L2c44CL()
        {
            Console.CursorVisible = false;

            while (true)
            {
                _57uY5Yi2();
                _6L6T5YWl();
                _5pu05paw();
                Thread.Sleep(100); // 控制游戏速度
            }
        }

        static void _57uY5Yi2()
        {
            Console.Clear();

            // 绘制边界
            for (int i = 0; i < width + 2; i++)
                Console.Write("#");

            Console.WriteLine();

            for (int y = 0; y < height; y++)
            {
                Console.Write("#"); // 左边界

                for (int x = 0; x < width; x++)
                {
                    if (x == ballX && y == ballY)
                        Console.Write("O"); // 球
                    else if (x == 1 && y >= paddleY && y < paddleY + 3)
                        Console.Write("|"); // 琴
                    else
                        Console.Write(" ");
                }

                Console.WriteLine("#"); // 右边界
            }

            for (int i = 0; i < width + 2; i++)
                Console.Write("#");

            Console.WriteLine($"\n得分: {score}");
        }

        static void _6L6T5YWl()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.UpArrow && paddleY > 0) paddleY--;
                if (key == ConsoleKey.DownArrow && paddleY < height - 3) paddleY++;
            }
        }

        static void _5pu05paw()
        {
            // 更新球的位置
            ballX += ballDirectionX;
            ballY += ballDirectionY;

            // 碰撞检测
            if (ballY <= 0 || ballY >= height - 1) ballDirectionY *= -1; // 碰到顶部或底部
            if (ballX >= width)
            {
                ballDirectionX *= -1; // 向左反弹
                score++;
            }
            if (ballX <= 0 && ballY >= paddleY && ballY < paddleY + 3)
            {
                ballDirectionX *= -1; // 与球拍碰撞
            }

            if (ballX < 0) // 如果球出界
            {
                Console.Clear();
                Console.WriteLine("游戏结束！");
                Console.WriteLine($"最终得分: {score}");
                Environment.Exit(0);
            }
        }
    }
}
