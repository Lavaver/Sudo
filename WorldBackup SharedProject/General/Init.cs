using com.Lavaver.WorldBackup.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Lavaver.WorldBackup.Global;
using com.Lavaver.WorldBackup.Database.MySQL;

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

                if (SQLConfig.IsEnabled())
                {
                    LogConsole.Log($"WorldBackup Init Progress ({PID})", "正在初始化 MySQL 数据库配置项，当我们需要你提供 MySQL 数据库的相关信息时，请如实填写。完成后将自动拉旗下个模块。", ConsoleColor.Blue);
                    Auth.CheckAuthFile();
                }

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
                LogConsole.Log($"WorldBackup Init ERROR ({PID})", $"致命性错误：{ex.Message}", ConsoleColor.Red);
            }
        }
    }
}
