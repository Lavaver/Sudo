using System;
using System.Reflection;
using com.Lavaver.WorldBackup.Core;
using com.Lavaver.WorldBackup.Database;
using com.Lavaver.WorldBackup.Rebuild;
using com.Lavaver.WorldBackup.Global;

namespace com.Lavaver.WorldBackup
{
    /// <summary>
    /// 重灾区（确信）
    /// </summary>
    internal class MainProgram
    {
        static async Task Main(string[] args)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var version = entryAssembly?.GetName().Version ?? new Version(0, 0);
            Console.Title = $"WorldBackup CLI+ v{version}";

            LogConsole.Initialize();

            LogConsole.Log("Init", "正在初始化，请稍候", ConsoleColor.Green);

            if (args.Length > 0)
            {
                await HandleCommandLineArgs(args);
            }
            else
            {
                await Init.Run();
            }
        }

        private static async Task HandleCommandLineArgs(string[] args)
        {
            switch (args[0])
            {
                case "-database":
                    ReadDatabase.Run();
                    break;
                case "-clear":
                    Compression_and_Cleanup.Run();
                    break;
                case "-del":
                    HandleDeleteArgs(args);
                    break;
                case "-recovery":
                    RecoveryFile.RestoreData();
                    break;
                case "-config":
                    AfterConfig.Run();
                    break;
                case "-bedrock":
                    Bedrock.Backup.Run();
                    break;
                case "-rebuildconfig":
                    RebuildConfig.Run();
                    break;
                case "-license":
                    var entryAssembly = Assembly.GetEntryAssembly();
                    var version = entryAssembly?.GetName().Version ?? new Version(0, 0, 0, 0);
                    string fileContent = File.ReadAllText(GlobalString.LICENSE);
                    LogConsole.Log("MIT License", fileContent, ConsoleColor.Green);
                    LogConsole.Log("MIT License", "------------------------------", ConsoleColor.Green);
                    LogConsole.Log("MIT License", $"WorldBackup CLI+ 由 Lavaver 授权给你的计算机 {Environment.MachineName} 使用，任何人可在 MIT 许可证下使用本软件、修改、分发、再授权，但必须保留原作者信息和许可证信息。", ConsoleColor.Green);
                    LogConsole.Log("MIT License", $"软件发行版（main 分支，主要更新通道） | 稳定版 {version} | 最后一次构建日期：2024/09/08 | 当前运行时版本 {Environment.Version}", ConsoleColor.Green);
                    break;
                case "-update":
                    await CheckUpdate.Run();
                    break;
                default:
                    LogConsole.Log("Init", "未识别的命令行参数", ConsoleColor.Red);
                    break;
            }
        }

        private static void HandleDeleteArgs(string[] args) 
        {
            if (args.Length < 2) 
            {
                LogConsole.Log("Init", $"用法：{1} -del <data/database/config>", ConsoleColor.Yellow);
                return;
            }

            switch (args[1]) 
            {
                case "data":
                    DelDatabase.DelData();
                    break;
                case "database":
                    DelDatabase.DelFile();
                    break;
                case "config":
                    DelDatabase.DelConfig();
                    break;
                case "log":
                    DelDatabase.DelLog();
                    break;
                default:
                    LogConsole.Log("Init", "不正确的操作模式", ConsoleColor.Red);
                    break;
            }
        }
    }
}
