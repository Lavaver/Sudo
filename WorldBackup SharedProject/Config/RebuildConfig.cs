using System;
using System.Collections.Generic;
using System.Text;
using com.Lavaver.WorldBackup.Global;
using com.Lavaver.WorldBackup.Core;

namespace com.Lavaver.WorldBackup.Rebuild
{
    internal class RebuildConfig
    {
        public static void Run()
        {
            if (!File.Exists(GlobalString.SoftwareConfigLocation()))
            {
                LogConsole.Log("RebuildConfig", "正在直接构建新的配置文件", ConsoleColor.Blue);
                CheckConfig.Run();
                LogConsole.Log("RebuildConfig", "配置文件构建完成", ConsoleColor.Green);
            }
            else
            {
                LogConsole.Log("RebuildConfig", "注意：重构配置文件会重置所有配置项，请确认是否继续？[Y/N]", ConsoleColor.Yellow);
                string? input = Console.ReadLine();
                if (input == "y" || input == "Y")
                {
                    LogConsole.Log("RebuildConfig", "正在重构配置文件", ConsoleColor.Blue);
                    try
                    {
                        File.Delete(GlobalString.SoftwareConfigLocation());
                    }
                    catch (Exception ex)
                    {
                        LogConsole.Log("RebuildConfig", "配置文件重构失败，原因：" + ex.Message, ConsoleColor.Red);
                        return;
                    }
                    CheckConfig.Run();
                    LogConsole.Log("RebuildConfig", "配置文件重构完成", ConsoleColor.Green);
                }
                else if (input == "n" || input == "N")
                {
                    LogConsole.Log("RebuildConfig", "已取消配置文件重构", ConsoleColor.Yellow);
                }
                else
                {
                    LogConsole.Log("RebuildConfig", "无效的抉择。", ConsoleColor.Red);
                }
            }
        }
    }
}
