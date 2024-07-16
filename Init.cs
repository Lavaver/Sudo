using com.Lavaver.WorldBackup.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.Lavaver.WorldBackup
{
    internal class Init
    {
        /// <summary>
        /// 初始化 WorldBackup 主程序
        /// </summary>
        public static void Run()
        {
            int PID = Environment.ProcessId;

            try
            {
                LogConsole.Log($"WorldBackup Init Progress ({PID})", "正在检查配置文件...这不需要太长时间", ConsoleColor.Blue);
                CheckConfig.Run();
                LogConsole.Log($"WorldBackup Init Progress ({PID})", "拉起 FirstBackup 模块...", ConsoleColor.Blue);
                FirstBackup.Pullup();
                LogConsole.Log($"WorldBackup Init Progress ({PID})", "拉起 Backup 模块...", ConsoleColor.Blue);
                Backup.Pullup();
                while (true)
                {
                    // 其他逻辑或保持主线程活动的代码
                }
            }
            catch (Exception ex)
            {
                LogConsole.Log($"WorldBackup Init ERROR ({PID})", $"初始化失败：{ex.Message}", ConsoleColor.Red);
            }
        }
    }
}
