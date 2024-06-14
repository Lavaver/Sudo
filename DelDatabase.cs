using System.Xml.Linq;

namespace WorldBackup
{
    internal class DelDatabase
    {
        public static void DelFile()
        {
            string xmlFilePath = "备份数据库.xml";

            if (File.Exists(xmlFilePath))
            {
                File.Delete(xmlFilePath);
                LogConsole.Log("备份数据库", "已删除数据库文件", ConsoleColor.Red);
            }
            else
            {
                LogConsole.Log("备份数据库", "数据库文件不存在", ConsoleColor.Yellow);
            }
        }

        public static void DelData()
        {
            string xmlFilePath = "备份数据库.xml";

            if (!File.Exists(xmlFilePath))
            {
                LogConsole.Log("备份数据库", "未找到备份数据库文件。请先正常运行程序以自动备份并生成数据库信息后再试。", ConsoleColor.Red);
                return;
            }

            try
            {
                // 加载 XML 文件
                XDocument doc = XDocument.Load(xmlFilePath);

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
                                Directory.Delete($"{backups[selectedIndex].Path}");
                                LogConsole.Log("备份数据库", $"{backups[selectedIndex].Path} => Delected（删除）", ConsoleColor.Red);
                                backups[selectedIndex].Element.Remove();
                                doc.Save(xmlFilePath);
                                LogConsole.Log("备份数据库", "删除成功", ConsoleColor.Green);
                                break;
                            }
                            catch (Exception ex)
                            {
                                LogConsole.Log("备份数据库", $"{backups[selectedIndex].Path} =/> Exception（异常：{ex.Message}）", ConsoleColor.Red);
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
    }
}
