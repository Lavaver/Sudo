using System;
using System.Reflection;
using com.Lavaver.WorldBackup.Sumeru;
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
            var version = entryAssembly?.GetName().Version ?? new Version(0, 0, 0, 0);
            Console.Title = $"WorldBackup CLI+ | Version {version}";

            LogConsole.Initialize();

            LogConsole.Log("Init", "正在初始化，请稍候", ConsoleColor.Green);

            if (args.Length > 0)
            {
                HandleCommandLineArgs(args);
            }
            else
            {
                await Init.Run();
            }
        }

        private static void HandleCommandLineArgs(string[] args)
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
                case "-WebDAV":
                    HandleWebDAVArgs(args);
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
                    LogConsole.Log("MIT License", $"软件发行版（main 分支，主要更新通道） | RL (Release Version) {version}", ConsoleColor.Green);
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
                default:
                    LogConsole.Log("Init", "不正确的操作模式", ConsoleColor.Red);
                    break;
            }
        }

        private static void HandleWebDAVArgs(string[] args)
        {
            if (args.Length < 6)
            {
                LogConsole.Log("Init", $"用法: {1} -WebDAV <Address> <Account> <Password> <SourceFilePath> [<DestinationPath>] [<PreAuthenticate:true/false>] [<Buffer>] <Upload/Download/Delete/NewFolder>", ConsoleColor.Yellow);
                return;
            }

            string address = args[1];
            string account = args[2];
            string password = args[3];
            string sourceFilePath = args[4];
            string destinationPath = args.Length > 5 ? args[5] : "/"; // 默认为根路径
            bool preAuthenticate = Convert.ToBoolean(args[6]);
            long buffer = Convert.ToInt64(args[7]);

            switch (args[8])
            {
                case "Upload":
                    AkashaTerminal.Upload(address, account, password, sourceFilePath, destinationPath, preAuthenticate, buffer);
                    break;
                case "Download":
                    AkashaTerminal.Download(address, account, password, destinationPath, sourceFilePath);
                    break;
                case "Delete":
                    AkashaTerminal.Delete(address, account, password, destinationPath, preAuthenticate);
                    break;
                case "NewFolder":
                    AkashaTerminal.NewFolder(address, account, password, destinationPath);
                    break;
                case "List":
                    AkashaTerminal.List(address, account, password);
                    break;
                default:
                    LogConsole.Log("Init", "不正确的操作模式", ConsoleColor.Red);
                    break;
            }
        }
    }
}
