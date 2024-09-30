using System;
using System.Timers;
using com.Lavaver.WorldBackup.Core;

namespace com.Lavaver.WorldBackup
{
    internal class Backup
    {
        public static System.Timers.Timer timer;

        public static void Pullup()
        {
            // 创建一个定时器，间隔为 15 分钟（900000 毫秒）
            timer = new System.Timers.Timer(900000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
            Console.CancelKeyPress += OnExit;
            LogConsole.Log("WorldBackup Backup", "后台备份模块已启动，每隔 15 分钟自动备份一次。如需停止，请按下 Ctrl + C 离开程序", ConsoleColor.Blue);
        }

        public static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                LogConsole.Log("WorldBackup Backup", $"在 {timer.Interval} 毫秒后执行备份", ConsoleColor.Blue);
                FirstBackup.Pullup();
            }
            catch (Exception ex)
            {
                LogConsole.Log("WorldBackup Backup", $"备份过程中发生异常: {ex.Message}", ConsoleColor.Red);
            }
        }

        public static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            // 阻止程序立即退出
            args.Cancel = true;
            StopAutoBackup();
        }

        public static void StopAutoBackup()
        {
            timer.Stop();
            timer.Dispose();

            LogConsole.Log("WorldBackup Backup", "感谢使用 WorldBackup ，下次见！", ConsoleColor.Blue);
            Environment.Exit(0);
        }
    }
}
