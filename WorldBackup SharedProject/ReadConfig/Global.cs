using com.Lavaver.WorldBackup.Global;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using com.Lavaver.WorldBackup.Core;

namespace com.Lavaver.WorldBackup.ReadConfig
{

    internal class WebDAVAccount
    {
        public static string Get()
        {
            try
            {
                XDocument doc = XDocument.Load(GlobalString.SoftwareConfigLocation);
                XElement element = doc.Root.Element("DAVUserName");
                if (element == null)
                {
                    LogConsole.Err("在配置中找不到 DAVUserName 节点，使用默认用户名");
                    return "";
                }
                
                // 使用 ?. 运算符，避免 null 值问题
                string username = element.Value ?? string.Empty; // 若为 null，赋值为空字符串
                
                if (string.IsNullOrEmpty(username))
                {
                    LogConsole.Err("DAVUserName 节点的值为空，使用默认用户名");
                    return "";
                }
                
                return username; // 返回有效的 username
            }
            catch (Exception ex)
            {
                LogConsole.Err($"处理配置文件时发生错误: {ex.Message}");
                return "";
            }
        }
    }
    internal class WebDAVPassword
    {
        public static string Get()
        {
            try
            {
                XDocument doc = XDocument.Load(GlobalString.SoftwareConfigLocation);
                XElement element = doc.Root.Element("DAVPassword");
                if (element == null)
                {
                    LogConsole.Err("在配置中找不到 DAVPassword 节点，使用默认密码");
                    return "";
                }

                // 使用 ?. 运算符，避免 null 值问题
                string password = element.Value ?? string.Empty; // 若为 null，赋值为空字符串

                if (string.IsNullOrEmpty(password))
                {
                    LogConsole.Err("DAVPassword 节点的值为空，使用默认密码");
                    return "";
                }

                return password; // 返回有效的 password
            }
            catch (Exception ex)
            {
                LogConsole.Err($"处理配置文件时发生错误: {ex.Message}");
                return "";
            }
        }
    }
    internal class WebDAVAddress
    {
        public static string Get()
        {
            try
            {
                XDocument doc = XDocument.Load(GlobalString.SoftwareConfigLocation);
                XElement element = doc.Root.Element("DAVHost");
                if (element == null)
                {
                    LogConsole.Err("在配置中找不到 DAVHost 节点，使用默认主机");
                    return "";
                }

                // 使用 ?. 运算符，避免 null 值问题
                string host = element.Value ?? string.Empty; // 若为 null，赋值为空字符串

                if (string.IsNullOrEmpty(host))
                {
                    LogConsole.Err("DAVHost 节点的值为空，使用默认主机");
                    return "";
                }

                return host; // 返回有效的 host
            }
            catch (Exception ex)
            {
                LogConsole.Err($"处理配置文件时发生错误: {ex.Message}");
                return "";
            }
        }
    }
}

