using System;
using System.Reflection;
using WorldBackup.Sumeru;

namespace WorldBackup
{
    internal class MainProgram
    {
        static void Main(string[] args)
        {
            Console.Title = $"WorldBackup | Version {Assembly.GetEntryAssembly().GetName().Version} | Powered By Fontaine Core";

            LogConsole.Initialize();

            LogConsole.Log("Init", "正在初始化，请稍候", ConsoleColor.Green);

            // 检查是否有命令行参数
            if (args.Length > 0)
            {
                // 如果存在 -database 参数，则调用 ReadDatabase.Run() 方法
                if (args[0] == "-database")
                {
                    ReadDatabase.Run();
                }
                else if (args[0] == "-clean")
                {
                    DelDatabase.DelFile();
                }
                else if (args[0] == "-deldata")
                {
                    DelDatabase.DelData();
                }
                else if (args[0] == "-recovery")
                {
                    RecoveryFile.RestoreData();
                }
                else if (args[0] == "-WebDAV")
                {
                    if (args.Length < 6)
                    {
                        LogConsole.Log("Init", "用法: YourApp.exe -WebDAV <Address> <Account> <Password> <SourceFilePath> [<DestinationPath>] <Upload/Download/Delete/NewFolder>", ConsoleColor.Yellow);
                        return; 
                    }
                    string address = args[1];
                    string account = args[2];
                    string password = args[3];
                    string sourceFilePath = args[4];
                    string destinationPath = args.Length > 5 ? args[5] : "/"; // 默认为根路径

                    if (args[6] == "Upload")
                    {
                        // 调用上传方法
                        AkashaTerminal.Upload(address, account, password, sourceFilePath, destinationPath);
                    }
                    else if ( args[6] == "Download")
                    {
                        AkashaTerminal.Download(address, account, password, destinationPath, sourceFilePath);
                    }
                    else if ( args[6] == "Delete")
                    {
                        AkashaTerminal.Delete(address, account, password, destinationPath);
                    }
                    else if (args[6] == "NewFolder")
                    {
                        AkashaTerminal.NewFolder(address, account, password, destinationPath);
                    }
                    else if (args[6] == "List")
                    {
                        AkashaTerminal.List(address, account, password);
                    }
                    else
                    {
                        LogConsole.Log("Init", "不正确的操作模式", ConsoleColor.Red);
                        return;
                    }
                }
                else if (args[0] == "-config")
                {
                    AfterConfig.Run();
                }
                else
                {
                    LogConsole.Log("Init", "未识别的命令行参数", ConsoleColor.Red);
                }
            }
            else
            {
                // 如果没有命令行参数，则调用 Init.Run() 方法
                Init.Run();
            }
        }
    }
}