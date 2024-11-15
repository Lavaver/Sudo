﻿using com.Lavaver.WorldBackup.Core;
using com.Lavaver.WorldBackup.Database.MySQL;
using com.Lavaver.WorldBackup.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
                if (SQLConfig.IsEnabled())
                {
                    Console.WriteLine("4. 关闭 MySQL 模式");
                }
                else
                {
                    Console.WriteLine("4. 启用 MySQL 模式");
                }
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
                    case "4":
                        if (SQLConfig.IsEnabled())
                        {
                            HandleSqlClose();
                        }
                        else
                        {
                            HandleSqlCreate();
                        }
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
            try
            {
                var doc = XDocument.Load(GlobalString.SoftwareConfigLocation());
                var element = doc.Root.Element(elementName);

                if (element != null)
                {
                    LogConsole.Log("配置", $"当前配置为 {element.Value}", ConsoleColor.Blue);
                    Console.Write("输入新路径：");
                    var newPath = Console.ReadLine();

                    if (!string.IsNullOrEmpty(newPath))
                    {
                        element.Value = newPath;
                        doc.Save(GlobalString.SoftwareConfigLocation());
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

        private static void HandleSqlClose()
        {
            try
            {
                // 加载 XML 文档
                var xmlDoc = XDocument.Load(GlobalString.SoftwareConfigLocation());
                var element = xmlDoc.Root.Element("MySQL");
                
                // 删除 MySQL 节点
                element.Remove();
                xmlDoc.Save(GlobalString.SoftwareConfigLocation());

                LogConsole.Log("配置", "已完成", ConsoleColor.Green);
            }
            catch(Exception ex)
            {
                LogConsole.Log("配置", $"{ex.Message}", ConsoleColor.Red);
            }
        }

        static void HandleSqlCreate()
        {
            try
            {
                // 加载 XML 文档
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(GlobalString.SoftwareConfigLocation());

                // 创建 MySQL 节点
                var mySqlNode = xmlDoc.CreateElement("MySQL");
                mySqlNode.InnerText = "true"; 

                xmlDoc.DocumentElement.AppendChild(mySqlNode);

                // 保存修改后的 XML 文档
                xmlDoc.Save(GlobalString.SoftwareConfigLocation());

                LogConsole.Log("配置", "已完成", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                LogConsole.Log("配置", $"{ex.Message}", ConsoleColor.Red);
            }
        }
    }
}
