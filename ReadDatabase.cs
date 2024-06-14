using System;
using System.IO;  // 引入 System.IO 命名空间以便进行文件检查
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace WorldBackup
{
    internal class ReadDatabase
    {
        public static void Run()
        {
            LogConsole.Log("备份数据库", "正在读取数据库中存在的备份记录...", ConsoleColor.Green);
            Read();
        }

        private static void Read()
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
                var backups = doc.Descendants("Backup").Select(backup => new
                {
                    Identifier = backup.Element("Identifier")?.Value,
                    Time = backup.Element("Time")?.Value,
                    Path = backup.Element("Path")?.Value,
                    ParsedTime = DateTime.TryParse(backup.Element("Time")?.Value, out var parsedTime) ? parsedTime : (DateTime?)null
                })
                .Where(b => b.ParsedTime.HasValue)
                .OrderByDescending(b => b.ParsedTime)
                .ToList();

                if (!backups.Any())
                {
                    LogConsole.Log("备份数据库", "没有有效的备份记录", ConsoleColor.Yellow);
                    return;
                }

                // 打印表格头
                Console.WriteLine("{0,-40} {1,-20} {2}", "Identifier", "Time", "Path");
                Console.WriteLine(new string('-', 100));

                // 打印最新的备份记录（置顶显示）
                var latestBackup = backups.First();
                Console.WriteLine("{0,-40} {1,-20} {2} {3}", latestBackup.Identifier, latestBackup.Time, latestBackup.Path, "* Latest");

                // 打印其余备份记录
                foreach (var backup in backups.Skip(1))
                {
                    Console.WriteLine("{0,-40} {1,-20} {2}", backup.Identifier, backup.Time, backup.Path);
                }

                LogConsole.Log("备份数据库", "完成读取", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                LogConsole.Log("备份数据库", $"读取数据库中存在的备份记录时出现问题：{ex.Message}", ConsoleColor.Red);
            }
        }


    }
}