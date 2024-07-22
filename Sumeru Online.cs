using com.Lavaver.WorldBackup.Core;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace com.Lavaver.WorldBackup.Sumeru
{
    public class AkashaTerminal
    {



        /// <summary>
        /// 使用 WebDAV 向在线存储服务上传文件
        /// </summary>
        /// <param name="Address">服务器所在地址（一般提供商会在提供相关服务的地方注明地址）</param>
        /// <param name="Account">登陆账户（一般为你注册的用户名或邮箱）</param>
        /// <param name="Password">账户密码 / 访问码（其中访问码需要另行生成，且与主密码是两样东西）</param>
        /// <param name="SourceFilePath">来源文件地址（必须为文件结尾，如：D:\xxx\xxx\xxx.xxx）</param>
        /// <param name="destinationPath">服务器上文件的目标路径（相对于服务器根目录路径。在没有指定的情况下默认为根目录。如果你的服务商特别限制了访问目录源，则你必须要使用该参数）</param>
        /// <param name="PreAuthenticate">预认证（默认为开，当认证不通过时可以考虑将其设置为 false）</param>
        /// <param name="Buffer">缓冲区大小。它接受 int 和 long 作为参数输入（默认为 8192，32 位）</param>
        public static void Upload(string Address, string Account, string Password, string SourceFilePath, string destinationPath = "/", bool PreAuthenticate = true, long Buffer = 8192)
        {
            try
            {
                LogConsole.Log("WebDAV Upload", "正在构建请求", ConsoleColor.Blue);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Address + destinationPath);
                req.Credentials = new NetworkCredential(Account, Password);
                if (PreAuthenticate)
                {
                    req.PreAuthenticate = true;
                }
                req.Method = "PUT";
                req.AllowWriteStreamBuffering = true;
                req.Timeout = System.Threading.Timeout.Infinite; // 无限超时时间以确保大文件上传顺利

                LogConsole.Log("WebDAV Upload", $"正在准备要上传的文件（{SourceFilePath} => {destinationPath}）", ConsoleColor.Blue);

                // 使用 using 语句确保资源释放
                using (FileStream rdr = new FileStream(SourceFilePath, FileMode.Open))
                using (Stream reqStream = req.GetRequestStream())
                {
                    byte[] buffer = new byte[Buffer]; // 使用用户自定义缓冲区大小

                    int bytesRead;
                    while ((bytesRead = rdr.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        reqStream.Write(buffer, 0, bytesRead);
                    }
                }

                // 获取响应以确认上传状态
                using (WebResponse response = req.GetResponse())
                {
                    // 可以在这里检查响应状态码等信息
                    LogConsole.Log("WebDAV Upload", $"完成上传（{SourceFilePath} => {destinationPath}）", ConsoleColor.Green);
                }
            }
            catch (Exception ex)
            {
                LogConsole.Log("WebDAV Upload", $"上传文件时发生错误：{ex.Message}", ConsoleColor.Red);
            }
        }

        /// <summary>
        /// 使用 WebDAV 下载存储在网络存储服务的文件
        /// </summary>
        /// <param name="Address">服务器所在地址（一般提供商会在提供相关服务的地方注明地址）</param>
        /// <param name="DestinationPath">服务器上文件的目标路径（相对于服务器根目录路径。在没有指定的情况下默认为根目录。如果你的服务商特别限制了访问目录源，则你必须要使用该参数）</param>
        /// <param name="SavePath">保存路径（必须为完整路径）</param>
        /// <param name="Account">登陆账户（一般为你注册的用户名或邮箱）</param>
        /// <param name="Password">账户密码 / 访问码（其中访问码需要另行生成，且与主密码是两样东西）</param>
        public static void Download(string Address, string Account, string Password, string DestinationPath, string SavePath)
        {
            try
            {
                LogConsole.Log("WebDAV Download", $"正在下载文件（{DestinationPath}）。请稍候", ConsoleColor.Blue);

                // 创建Web请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Address + DestinationPath);
                request.Method = "GET";
                request.Credentials = new NetworkCredential(Account, Password);
                request.Timeout = System.Threading.Timeout.Infinite;

                // 发送请求并获取响应
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // 打开远程文件流
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        // 创建本地文件流
                        using (FileStream fileStream = new FileStream(SavePath, FileMode.Create))
                        {
                            // 将远程流复制到本地文件流
                            responseStream.CopyTo(fileStream);
                        }
                    }
                }

                LogConsole.Log("WebDAV Download", $"完成下载（{DestinationPath} => {SavePath}）", ConsoleColor.Green);
            }
            catch (WebException ex)
            {
                // 处理Web请求异常
                LogConsole.Log("WebDAV Download", $"下载文件时发生Web请求错误：{ex.Message}", ConsoleColor.Red);
            }
            catch (IOException ex)
            {
                // 处理文件操作异常
                LogConsole.Log("WebDAV Download", $"下载文件时发生IO错误：{ex.Message}", ConsoleColor.Red);
            }
            catch (Exception ex)
            {
                // 处理其他异常
                LogConsole.Log("WebDAV Download", $"下载文件时发生错误：{ex.Message}", ConsoleColor.Red);
            }
        }

        /// <summary>
        /// 使用 WebDAV 删除存储在网络存储服务的文件
        /// </summary>
        /// <param name="Address">服务器所在地址（一般提供商会在提供相关服务的地方注明地址）</param>
        /// <param name="DestinationPath">服务器上文件的目标路径（相对于服务器根目录路径。必须指定一个目录）</param>
        /// <param name="Account">登陆账户（一般为你注册的用户名或邮箱）</param>
        /// <param name="Password">账户密码 / 访问码（其中访问码需要另行生成，且与主密码是两样东西）</param>
        /// <param name="PreAuthenticate">预认证（默认为开，当认证不通过时可以考虑将其设置为 false）</param>
        public static void Delete(string Address, string Account, string Password, string DestinationPath, bool PreAuthenticate = true)
        {
            try
            {
                LogConsole.Log("WebDAV Delete", $"正在删除文件（{DestinationPath}）", ConsoleColor.Blue);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Address + DestinationPath);
                req.Credentials = new NetworkCredential(Account, Password);
                if (PreAuthenticate)
                {
                    req.PreAuthenticate = true;
                }
                req.Method = "DELETE";
                req.AllowWriteStreamBuffering = true;

                req.GetResponse();
                LogConsole.Log("WebDAV Delete", $"你已成功地删除了 {DestinationPath}", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                LogConsole.Log("WebDAV Delete", $"删除文件时发生错误：{ex.Message}", ConsoleColor.Red);
            }
        }

        /// <summary>
        /// 使用 WebDAV 在网络存储服务上创建新文件夹
        /// </summary>
        /// <param name="Address">服务器所在地址（一般提供商会在提供相关服务的地方注明地址）</param>
        /// <param name="Account">登陆账户（一般为你注册的用户名或邮箱）</param>
        /// <param name="Password">账户密码 / 访问码（其中访问码需要另行生成，且与主密码是两样东西）</param>
        /// <param name="FolderName">文件夹名称</param>
        public static void NewFolder(string Address, string Account, string Password, string FolderName)
        {
            try
            {
                LogConsole.Log("WebDAV Folder", $"正在创建文件夹（{FolderName}）", ConsoleColor.Blue);
                // Create the HttpWebRequest object.
                HttpWebRequest objRequest = (HttpWebRequest)HttpWebRequest.Create(Address + FolderName);
                // Add the network credentials to the request.
                objRequest.Credentials = new NetworkCredential(Account, Password);//用户名,密码
                // Specify the method.
                objRequest.Method = "MKCOL";

                HttpWebResponse objResponse = (System.Net.HttpWebResponse)objRequest.GetResponse();

                // Close the HttpWebResponse object.
                objResponse.Close();
                LogConsole.Log("WebDAV Folder", $"你已成功地创建了 {FolderName} 文件夹", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                LogConsole.Log("WebDAV Folder", $"删除文件时发生错误：{ex.Message}", ConsoleColor.Red);
            }
        }

        /// <summary>
        /// 以文件形式输出 WebDAV 服务器下的全部文件
        /// </summary>
        /// <param name="Address">服务器所在地址（一般提供商会在提供相关服务的地方注明地址）</param>
        /// <param name="Account">登陆账户（一般为你注册的用户名或邮箱）</param>
        /// <param name="Password">账户密码 / 访问码（其中访问码需要另行生成，且与主密码是两样东西）</param>
        public static void List(string Address, string Account, string Password)
        {
            try
            {
                string sUrl = Address;
                string strXml = "<?xml version=\"1.0\"?> " +
                                "<d:propfind xmlns:d=\"DAV:\" xmlns:o=\"urn:schemas-microsoft-com:office:office/\">" +
                                "<d:prop>" +
                                "<d:displayname/>" +       // 名称
                                "<d:getcontentlength/>" +  // 大小
                                "<d:iscollection/>" +      // 是否文件夹
                                "<d:getlastmodified/>" +   // 最后修改时间
                                "</d:prop>" +
                                "</d:propfind>";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
                request.Credentials = new NetworkCredential(Account, Password);
                request.Method = "PROPFIND";
                request.ContentType = "text/xml";
                request.Headers.Add("Translate", "f");

                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] xmlBytes = Encoding.UTF8.GetBytes(strXml);
                    requestStream.Write(xmlBytes, 0, xmlBytes.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    string responseXml = reader.ReadToEnd();
                    File.WriteAllText(@"list.xml", responseXml);

                    XDocument doc = XDocument.Parse(responseXml);

                    foreach (XElement prop in doc.Descendants(XName.Get("prop", "DAV:")))
                    {
                        XElement displayName = prop.Element(XName.Get("displayname", "DAV:"));
                        XElement contentLength = prop.Element(XName.Get("getcontentlength", "DAV:"));
                        XElement isCollection = prop.Element(XName.Get("iscollection", "DAV:"));
                        XElement lastModified = prop.Element(XName.Get("getlastmodified", "DAV:"));

                        if (displayName != null)
                        {
                            Console.WriteLine("Display Name: " + displayName.Value);
                        }
                        if (contentLength != null)
                        {
                            Console.WriteLine("Content Length: " + contentLength.Value);
                        }
                        if (isCollection != null)
                        {
                            Console.WriteLine("Is Collection: " + isCollection.Value);
                        }
                        if (lastModified != null)
                        {
                            Console.WriteLine("Last Modified: " + lastModified.Value);
                        }

                        Console.WriteLine(); // 输出一个空行分隔每个属性的输出
                    }
                }
            }
            catch (Exception ex)
            {
                LogConsole.Log("WebDAV Download", $"发生了 {0} 个错误：{ex.Message}", ConsoleColor.Red);
            }
        }



    }
}
