using System;
using System.Reflection;
using com.Lavaver.WorldBackup.Sumeru;
using com.Lavaver.WorldBackup.Core;

namespace com.Lavaver.WorldBackup
{
    /// <summary>
    /// 如果你要说我为什么不创建文件夹做分类？我只想对你说你太天真了，首先这个项目基本没什么人看，其次是项目初期本身就是不用文件夹整理有历史包袱，最后是这个程序有非常多代码文件都依靠底层核心（例如 LogConsole）支持，动一个项目编译不过去<br/>
    /// 最重要的是根据我的习惯一旦新建文件夹把代码放进去那必须要下意识遵循 Java 包名规则，如果有爱好者想用新版代码就必须要把那些组件命名空间全部改为包名非常麻烦
    /// </summary>
    internal class MainProgram
    {
        static void Main(string[] args)
        {
            Console.Title = $"WorldBackup {Assembly.GetEntryAssembly().GetName().Version}";

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
                else if (args[0] == "-deldatabase")
                {
                    DelDatabase.DelFile();
                }
                else if (args[0] == "-clear")
                {
                    Compression_and_Cleanup.Run();
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
                        LogConsole.Log("Init", "用法: YourApp.exe -WebDAV <Address> <Account> <Password> <SourceFilePath> [<DestinationPath>] [<PreAuthenticate:true/false>] [<Buffer>] <Upload/Download/Delete/NewFolder>", ConsoleColor.Yellow);
                        return; 
                    }
                    string address = args[1];
                    string account = args[2];
                    string password = args[3];
                    string sourceFilePath = args[4];
                    string destinationPath = args.Length > 5 ? args[5] : "/"; // 默认为根路径
                    bool PreAuthenticate = Convert.ToBoolean(args[6]);
                    long Buffer = Convert.ToInt64(args[7]);

                    if (args[8] == "Upload")
                    {
                        // 调用上传方法
                        AkashaTerminal.Upload(address, account, password, sourceFilePath, destinationPath, PreAuthenticate, Buffer);
                    }
                    else if ( args[8] == "Download")
                    {
                        AkashaTerminal.Download(address, account, password, destinationPath, sourceFilePath);
                    }
                    else if ( args[8] == "Delete")
                    {
                        AkashaTerminal.Delete(address, account, password, destinationPath, PreAuthenticate);
                    }
                    else if (args[8] == "NewFolder")
                    {
                        AkashaTerminal.NewFolder(address, account, password, destinationPath);
                    }
                    else if (args[8] == "List")
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
                else if (args[0] == "-bedrock")
                {
                    Bedrock.Backup.Run();
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