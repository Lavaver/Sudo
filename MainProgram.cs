using System;

namespace WorldBackup
{
    internal class MainProgram
    {
        static void Main(string[] args)
        {
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
                else if (args[0] == "-deldatabase")
                {
                    DelDatabase.DelData();
                }
                else if (args[0] == "-recovery")
                {
                    RecoveryFile.RestoreData();
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