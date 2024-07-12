using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using com.Lavaver.WorldBackup.Core;

namespace WorldBackup
{
    internal class Backup
    {
        private static System.Timers.Timer timer;
        public static void Pullup()
        {
            // 创建一个定时器，间隔为 15 分钟（900000 毫秒）
            timer = new System.Timers.Timer(900000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);
            LogConsole.Log($"WorldBackup Backup", $"按下 Ctrl + C 离开程序", ConsoleColor.Blue);
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            LogConsole.Log($"WorldBackup Backup", $"在 {timer} 执行备份", ConsoleColor.Blue);
            FirstBackup.Pullup();
        }

        private static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            // 阻止程序立即退出
            args.Cancel = true;
            StopAutoBackup();
        }

        public static void StopAutoBackup()
        {
            timer.Stop();
            timer.Dispose();

            LogConsole.Log($"WorldBackup Backup", "感谢使用 WorldBackup ，下次见！", ConsoleColor.Blue);
            Environment.Exit(0);
        }
    }
}
