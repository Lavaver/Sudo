using com.Lavaver.WorldBackup.Core;
using com.Lavaver.WorldBackup.Database.MySQL;
using com.Lavaver.WorldBackup.Global;
using System.IO.Compression;
using System.Xml.Linq;

namespace com.Lavaver.WorldBackup
{
    /// <summary>
    /// 备份压缩与清理（公共）类
    /// </summary>
    public class Compression_and_Cleanup
    {
        public static int selectedIndex = 0; // 声明 selectedIndex 作为类的私有静态变量
        public static void Run()
        {
            if (SQLConfig.IsEnabled())
            {
                string returnData = Tables.GetBackupTable();
                if (returnData == null)
                {
                    LogConsole.Log("备份数据库", "未找到备份记录", ConsoleColor.Yellow);
                    return;
                }
                else
                {
                    LogConsole.Log("备份数据库", "正在读备份记录", ConsoleColor.Green);
                    SQL_Run(returnData);
                    return;
                }
            }
            else
            {
                if (!File.Exists(GlobalString.DatabaseLocation))
                {
                    LogConsole.Log("备份数据库", "未找到备份数据库文件。请先正常运行程序以自动备份并生成数据库信息后再试。", ConsoleColor.Red);
                    return;
                }
            }

            if (!File.Exists(GlobalString.SoftwareConfigLocation))
            {
                LogConsole.Log("配置文件", "未找到配置文件。请检查配置文件路径。", ConsoleColor.Red);
                return;
            }

            try
            {
                // 加载 XML 文件
                XDocument doc = XDocument.Load(GlobalString.DatabaseLocation);
                XDocument configDoc = XDocument.Load(GlobalString.DatabaseLocation);

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
                                     Time = DateTime.Parse(backup.Element("Time")?.Value), // 解析备份时间
                                     Path = backup.Element("Path")?.Value,
                                 })
                                 .ToList();

                List<int> selectedIndexes = new List<int>(); // 存储多选备份的索引

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("{0,-40} {1,-20} {2}", "Identifier", "Time", "Path");
                    Console.WriteLine(new string('-', 100));

                    // 仅显示时间大于或等于5个月的备份记录
                    var validBackups = backups.Where(b => (DateTime.Now - b.Time).TotalDays >= 5 * 30);

                    if (!validBackups.Any())
                    {
                        Console.WriteLine("暂无过时备份。你可以按下 Ctrl + C 结束运行程序");
                        Console.ReadKey();
                        continue;
                    }

                    foreach (var backup in validBackups)
                    {
                        int index = backups.IndexOf(backup);

                        if (selectedIndexes.Contains(index))
                        {
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else if (index == selectedIndex)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkCyan;
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                        Console.WriteLine("{0,-40} {1,-20} {2}", backup.Identifier, backup.Time.ToString("yyyy-MM-dd HH:mm:ss"), backup.Path);

                        Console.ResetColor();
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
                    else if (key == ConsoleKey.Enter) // 支持多选备份
                    {
                        if (!selectedIndexes.Contains(selectedIndex))
                        {
                            selectedIndexes.Add(selectedIndex);
                        }
                        else
                        {
                            selectedIndexes.Remove(selectedIndex);
                        }
                    }
                    else if (key == ConsoleKey.F2) // 递归删除选中备份
                    {
                        foreach (var index in selectedIndexes.OrderByDescending(x => x))
                        {
                            DeleteBackup(backups[index]);
                            backups.RemoveAt(index);
                            backups[selectedIndex].Element.Remove();
                        }
                        selectedIndexes.Clear();
                        doc.Save(GlobalString.DatabaseLocation);
                    }
                    else if (key == ConsoleKey.F1) // 压缩选中备份
                    {
                        foreach (var index in selectedIndexes.OrderByDescending(x => x))
                        {
                            CompressBackup(backups[index]);
                            backups[selectedIndex].Element.Remove();
                        }
                        selectedIndexes.Clear();
                        
                        doc.Save(GlobalString.DatabaseLocation);
                    }
                }
            }
            catch (Exception ex)
            {
                LogConsole.Log("备份数据库", $"读取数据库中存在的备份记录时出现问题：{ex.Message}", ConsoleColor.Red);
            }
        }

        static void SQL_Run(string returnData)
        {
            // 将纯文本数据转换为 XML 数据
            XDocument doc = XDocument.Parse(returnData);

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
                                      Time = DateTime.Parse(backup.Element("Time")?.Value), // 解析备份时间
                                      Path = backup.Element("Path")?.Value,
                                  })
                                 .ToList();

            List<int> selectedIndexes = new List<int>(); // 存储多选备份的索引

            while (true)
            {
                Console.Clear();
                Console.WriteLine("{0,-40} {1,-20} {2}", "Identifier", "Time", "Path");
                Console.WriteLine(new string('-', 100));

                // 仅显示时间大于或等于5个月的备份记录
                var validBackups = backups.Where(b => (DateTime.Now - b.Time).TotalDays >= 5 * 30);

                if (!validBackups.Any())
                {
                    Console.WriteLine("暂无过时备份。你可以按下 Ctrl + C 结束运行程序");
                    Console.ReadKey();
                    continue;
                }

                foreach (var backup in validBackups)
                {
                    int index = backups.IndexOf(backup);

                    if (selectedIndexes.Contains(index))
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else if (index == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.WriteLine("{0,-40} {1,-20} {2}", backup.Identifier, backup.Time.ToString("yyyy-MM-dd HH:mm:ss"), backup.Path);

                    Console.ResetColor();
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
                else if (key == ConsoleKey.Enter) // 支持多选备份
                {
                    if (!selectedIndexes.Contains(selectedIndex))
                    {
                        selectedIndexes.Add(selectedIndex);
                    }
                    else
                    {
                        selectedIndexes.Remove(selectedIndex);
                    }
                }
                else if (key == ConsoleKey.F2) // 递归删除选中备份
                {
                    foreach (var index in selectedIndexes.OrderByDescending(x => x))
                    {
                        DeleteBackup(backups[index]);
                        backups.RemoveAt(index);
                        backups[selectedIndex].Element.Remove();
                    }
                    selectedIndexes.Clear();
                    doc.Save(GlobalString.DatabaseLocation);
                }
                else if (key == ConsoleKey.F1) // 压缩选中备份
                {
                    foreach (var index in selectedIndexes.OrderByDescending(x => x))
                    {
                        CompressBackup(backups[index]);
                        backups[selectedIndex].Element.Remove();
                    }
                    selectedIndexes.Clear();

                    doc.Save(GlobalString.DatabaseLocation);
                }
            }
        }

        public static void DeleteBackup(dynamic backup)
        {
            LogConsole.Log("备份压缩与清理", $"正在删除 {backup}", ConsoleColor.Blue);

            try
            {
                string targetDir = backup;
                // 获取所有文件并删除
                foreach (string file in Directory.GetFiles(targetDir))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                // 获取所有子目录并递归删除
                foreach (string dir in Directory.GetDirectories(targetDir))
                {
                    DeleteBackup(dir);
                }

                // 最后删除目标目录自身
                Directory.Delete(targetDir, false);
            }
            catch (Exception ex)
            {
                string targetDir = backup;
                LogConsole.Log("备份压缩与清理", $"{targetDir} =/> Exception（异常：{ex.Message}）", ConsoleColor.Red, true);
                throw; // 重新抛出异常以便上层代码处理
            }
        }

        

        public static void CompressBackup(dynamic backup)
        {
            LogConsole.Log("备份压缩与清理", $"正在压缩 {backup.Identifier}", ConsoleColor.Blue);

            string backupPath = backup;
            string zipFilePath = backupPath + ".zip";

            try
            {
                LogConsole.Log("备份压缩与清理", $"Compressing backup folder: {backupPath}", ConsoleColor.Blue);

                // 创建压缩文件
                using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                {
                    // 获取备份文件夹中的所有文件和子文件夹
                    var files = Directory.GetFiles(backupPath, "*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        // 计算文件在压缩包中的路径
                        var entryName = Path.GetRelativePath(backupPath, file);
                        zipArchive.CreateEntryFromFile(file, entryName);
                    }
                }

                LogConsole.Log("备份压缩与清理", $"压缩完成：{zipFilePath}", ConsoleColor.Blue);
            }
            catch (Exception ex)
            {
                LogConsole.Log("备份压缩与清理", $"压缩失败：{ex.Message}", ConsoleColor.Red, true);
                throw; // 重新抛出异常以便上层代码处理
            }
        }
    }
}
