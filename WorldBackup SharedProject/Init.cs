using com.Lavaver.WorldBackup.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Lavaver.WorldBackup.Global;

namespace com.Lavaver.WorldBackup
{
    internal class Init
    {
        /// <summary>
        /// 初始化 WorldBackup 主程序
        /// </summary>
        public static async Task Run()
        {
            int PID = Environment.ProcessId;

            try
            {
                await CheckUpdate.Run();
                LogConsole.Log($"WorldBackup Init Progress ({PID})", "正在检查配置文件...这不需要太长时间", ConsoleColor.Blue);
                CheckConfig.Run();
                LogConsole.Log($"WorldBackup Init Progress ({PID})", "拉起 FirstBackup 模块...", ConsoleColor.Blue);
                FirstBackup.Pullup();
                LogConsole.Log($"WorldBackup Init Progress ({PID})", "拉起后台备份模块...", ConsoleColor.Blue);
                Backup.Pullup();
                while (true)
                {
                    
                }
            }
            catch (Exception ex)
            {
                LogConsole.Log($"WorldBackup Init ERROR ({PID})", $"初始化失败：{ex.Message}", ConsoleColor.Red);
            }
        }
    }
}
