using com.Lavaver.WorldBackup.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace com.Lavaver.WorldBackup
{
    internal class AfterConfig
    {
        public static void Run()
        {
            while (true)
            {
                LogConsole.Log("配置","请选择一个选项",ConsoleColor.Blue);
                Console.WriteLine("1. 来源文件夹");
                Console.WriteLine("2. 备份到");
                Console.WriteLine("3. NTP 服务器");
                Console.WriteLine("0. 退出");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HandleChoice("source");
                        break;
                    case "2":
                        HandleChoice("backupto");
                        break;
                    case "3":
                        HandleChoice("NTP-Server");
                        break;
                    case "0":
                        return;
                    default:
                        LogConsole.Log("配置", "选项无效", ConsoleColor.Red);
                        break;
                }
            }
        }

        private static void HandleChoice(string elementName)
        {
            string configFilePath = "WorldBackupConfig.xml";

            try
            {
                XDocument doc = XDocument.Load(configFilePath);
                XElement element = doc.Root.Element(elementName);

                if (element != null)
                {
                    LogConsole.Log("配置", $"当前配置为 {element.Value}", ConsoleColor.Blue);
                    Console.Write("输入新路径：");
                    string newPath = Console.ReadLine();

                    if (!string.IsNullOrEmpty(newPath))
                    {
                        element.Value = newPath;
                        doc.Save(configFilePath);
                        Console.WriteLine("配置已更新。");
                    }
                    else
                    {
                        LogConsole.Log("配置", "配置未更新。因为你没有填写配置", ConsoleColor.Red);
                    }
                }
                else
                {
                    LogConsole.Log("配置", $"没有元素 {elementName}", ConsoleColor.Red);
                }
            }
            catch (Exception ex)
            {
                LogConsole.Log("配置", $"{ex.Message}", ConsoleColor.Red);
            }
        }
    }
}
