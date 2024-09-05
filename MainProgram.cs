using System;
using System.Reflection;
using com.Lavaver.WorldBackup.Sumeru;
using com.Lavaver.WorldBackup.Core;
using com.Lavaver.WorldBackup.Database;

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
                Init.Run();
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
                default:
                    LogConsole.Log("Init", "未识别的命令行参数", ConsoleColor.Red);
                    break;
            }
        }

        private static void HandleDeleteArgs(string[] args) 
        {
            if (args.Length < 1) 
            {
                LogConsole.Log("Init", "用法：YourApp.exe -del <data/database>", ConsoleColor.Yellow);
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
                default:
                    LogConsole.Log("Init", "不正确的操作模式", ConsoleColor.Red);
                    break;
            }
        }

        private static void HandleWebDAVArgs(string[] args)
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
