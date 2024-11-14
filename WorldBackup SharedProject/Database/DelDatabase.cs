using com.Lavaver.WorldBackup.Core;
using com.Lavaver.WorldBackup.Database.MySQL;
using com.Lavaver.WorldBackup.Global;
using System.Xml.Linq;

namespace com.Lavaver.WorldBackup.Database
{
    internal class DelDatabase
    {
        public static void DelFile()
        {
            if (SQLConfig.IsEnabled())
            {
                LogConsole.Log("备份数据库（MySQL）", "正在请求删除备份记录表", ConsoleColor.Yellow);
                Tables.Drop();
                LogConsole.Log("备份数据库（MySQL）", "已删除备份记录表", ConsoleColor.Green);
            }
            else
            {
                if (File.Exists(GlobalString.DatabaseLocation()))
                {
                    File.Delete(GlobalString.DatabaseLocation());
                    LogConsole.Log("备份数据库", "已删除数据库文件", ConsoleColor.Red);
                }
                else
                {
                    LogConsole.Log("备份数据库", "数据库文件不存在", ConsoleColor.Yellow);
                }
            }
        }

        /// <summary>
        /// 删除全部备份
        /// </summary>
        /// <param name="force">强制删除（默认为 false）</param>
        public static void DelAllData()
        {
            var backupLocation = ReadBackupLocation.Get();
            if (!string.IsNullOrEmpty(backupLocation))
            {
                // 删除备份目录下所有文件及子目录
                if (Directory.Exists(backupLocation))
                {
                    Directory.Delete(backupLocation, true);
                    LogConsole.Log("Sudo Remove(All Data)","已完成",ConsoleColor.Green);
                }
                LogConsole.Log("Sudo Remove(All Data)","不存在的目录",ConsoleColor.Red,true);
            }
            LogConsole.Log("Sudo Remove(All Data)","没有配置目标目录",ConsoleColor.Red,true);
        }

        public static void DelData()
        {
            if (!File.Exists(GlobalString.DatabaseLocation()))
            {
                LogConsole.Log("备份数据库", "未找到备份数据库文件。请先正常运行程序以自动备份并生成数据库信息后再试。", ConsoleColor.Red);
                return;
            }

            try
            {
                // 加载 XML 文件
                XDocument doc = XDocument.Load(GlobalString.DatabaseLocation());

                if (doc.Root == null || doc.Root.Elements("Backup").Count() == 0)
                {
                    LogConsole.Log("备份数据库", "未找到备份记录", ConsoleColor.Yellow);
                    return;
                }

                // 查询和解析 XML 内容
                var backups = doc.Descendants("Backup")
                                 .Select(backup => new
                                 {
                                     Element = backup,
                                     Identifier = backup.Element("Identifier")?.Value,
                                     Time = backup.Element("Time")?.Value,
                                     Path = backup.Element("Path")?.Value,
                                 })
                                 .ToList();

                int selectedIndex = 0;

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("{0,-40} {1,-20} {2}", "Identifier", "Time", "Path");
                    Console.WriteLine(new string('-', 100));

                    for (int i = 0; i < backups.Count; i++)
                    {
                        if (i == selectedIndex)
                        {
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }

                        Console.WriteLine("{0,-40} {1,-20} {2}", backups[i].Identifier, backups[i].Time, backups[i].Path);

                        if (i == selectedIndex)
                        {
                            Console.ResetColor();
                        }
                    }

                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.UpArrow)
                    {
                        selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : backups.Count - 1;
                    }
                    else if (key == ConsoleKey.DownArrow)
                    {
                        selectedIndex = (selectedIndex + 1) % backups.Count;
                    }
                    else if (key == ConsoleKey.Enter)
                    {  
                        Console.WriteLine("\n确认要删除该备份吗？(Y/N)");
                        var confirmKey = Console.ReadKey(true).Key;
                        if (confirmKey == ConsoleKey.Y)
                        {
                            try
                            {
                                DeleteDirectoryRecursively($"{backups[selectedIndex].Path}");
                                LogConsole.Log("备份数据库", $"{backups[selectedIndex].Path} => Deleted（删除）", ConsoleColor.Red);
                                backups[selectedIndex].Element.Remove();
                                doc.Save(GlobalString.DatabaseLocation());
                                LogConsole.Log("备份数据库", "删除成功", ConsoleColor.Green);

                                 break;
                            }
                            catch (Exception ex)
                            {
                                LogConsole.Log("备份数据库", $"{backups[selectedIndex].Path} =/> Exception（{ex.Message}）", ConsoleColor.Red);
                                break;
                            }
                        }
                        else if (confirmKey == ConsoleKey.N)
                        {
                            LogConsole.Log("备份数据库", "取消删除", ConsoleColor.Yellow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogConsole.Log("备份数据库", $"读取数据库中存在的备份记录时出现问题：{ex.Message}", ConsoleColor.Red);
            }
        }

        public static void DelConfig()
        {
            if (File.Exists(GlobalString.SoftwareConfigLocation()))
            {
                LogConsole.Log("软件配置", "警告：删除软件配置文件将会导致后续运行时需要重新配置，仅在迫不得已时使用。我们推荐你使用 -rebuild-config 参数重新构建一个新的配置文件以便你重新开始。继续？[Y/N]", ConsoleColor.Yellow);
                var input = Console.ReadLine();
                switch (input)
                {
                    case "y" or "Y":
                        File.Delete(GlobalString.SoftwareConfigLocation());
                        LogConsole.Log("软件配置", "已删除软件配置文件", ConsoleColor.Red);
                        break;
                    case "n" or "N":
                        LogConsole.Log("软件配置", "取消删除软件配置文件", ConsoleColor.Yellow);
                        break;
                    default:
                        LogConsole.Log("软件配置", "输入错误，取消删除软件配置文件", ConsoleColor.Yellow);
                        break;
                }
            }
            else
            {
                LogConsole.Log("软件配置", "软件配置文件不存在", ConsoleColor.Yellow);
            }
        }

        public static void DelLog()
        {
            LogConsole.Log("日志文件管理", "警告：虽然没什么人在日志目录下放重要文件，但为了以防万一，需要确认你真的要删除日志目录及其所有文件。继续？[Y/N]", ConsoleColor.Yellow);
            var input = Console.ReadLine();
            switch (input)
            {
                case "y" or "Y":
                    DeleteDirectoryRecursively(GlobalString.LogLocation);
                    LogConsole.Log("日志文件管理", "已删除日志目录及其所有文件", ConsoleColor.Red);
                    break;
                case "n" or "N":
                    LogConsole.Log("日志文件管理", "取消删除日志目录及其所有文件", ConsoleColor.Yellow);
                    break;
                default:
                    LogConsole.Log("日志文件管理", "输入错误，取消删除日志目录及其所有文件", ConsoleColor.Yellow);
                    break;
            }
        }


        private static void DeleteDirectoryRecursively(string targetDir)
        {
            try
            {
                // 获取所有文件并删除
                foreach (var file in Directory.GetFiles(targetDir))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                // 获取所有子目录并递归删除
                foreach (var dir in Directory.GetDirectories(targetDir))
                {
                    DeleteDirectoryRecursively(dir);
                }

                // 最后删除目标目录自身
                Directory.Delete(targetDir, false);
            }
            catch (Exception ex)
            {
                LogConsole.Log("备份数据库", $"{targetDir} =/> Exception（异常：{ex.Message}）", ConsoleColor.Red);
                throw; // 重新抛出异常以便上层代码处理
            }
        }
    }
}
