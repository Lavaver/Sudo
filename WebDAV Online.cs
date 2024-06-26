using System.Net;

namespace WorldBackup
{
    public class WebDAV
    {
        /// <summary>
        /// 使用 WebDAV 向在线存储服务上传文件
        /// </summary>
        /// <param name="Address">服务器所在地址（一般提供商会在提供相关服务的地方注明地址）</param>
        /// <param name="Account">登陆账户（一般为你注册的用户名或邮箱）</param>
        /// <param name="Password">账户密码 / 访问码（其中访问码需要另行生成，且与主密码是两样东西）</param>
        /// <param name="SourceFilePath">来源文件地址（必须为文件结尾，如：D:\xxx\xxx\xxx.xxx）</param>
        /// <param name="destinationPath">服务器上文件的目标路径（相对于服务器根目录路径。在没有指定的情况下默认为根目录。如果你的服务商特别限制了访问目录源，则你必须要使用该参数）</param>
        public static void Upload(string Address, string Account, string Password, string SourceFilePath, string destinationPath = "/")
        {
            try
            {
                LogConsole.Log("WebDAV Upload", $"正在构建请求（访问码：{Password}）", ConsoleColor.Blue);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Address + destinationPath);
                req.Credentials = new NetworkCredential(Account, Password);//用户名,密码   //CredentialCache.DefaultCredentials使用默认的认证
                req.PreAuthenticate = true;
                req.Method = "PUT";
                req.AllowWriteStreamBuffering = true;

                LogConsole.Log("WebDAV Upload", $"正在准备要上传的文件（{SourceFilePath} => {destinationPath}）", ConsoleColor.Blue);

                // Retrieve request stream
                Stream reqStream = req.GetRequestStream();

                // Open the local file
                FileStream rdr = new FileStream(SourceFilePath, FileMode.Open);

                // Allocate byte buffer to hold file contents
                byte[] inData = new byte[4096];

                LogConsole.Log("WebDAV Upload", $"正在上传文件（{SourceFilePath} => {destinationPath}）。请稍候，这需要 1~10 分钟不等的时间", ConsoleColor.Blue);
                // loop through the local file reading each data block
                //  and writing to the request stream buffer
                int bytesRead = rdr.Read(inData, 0, inData.Length);
                while (bytesRead > 0)
                {
                    reqStream.Write(inData, 0, bytesRead);
                    bytesRead = rdr.Read(inData, 0, inData.Length);
                }

                rdr.Close();
                reqStream.Close();

                req.GetResponse();
                LogConsole.Log("WebDAV Upload", $"完成上传（{SourceFilePath} => {destinationPath}）", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                LogConsole.Log("WebDAV Upload", $"上传文件时发生错误：{ex.Message}",ConsoleColor.Red);
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
                System.Uri myURi = new System.Uri(Address + DestinationPath);
                string sfilePath = SavePath;

                WebRequest req = WebRequest.Create(myURi);
                req.Credentials = new NetworkCredential(Account, Password);
                req.Method = "GET";
                req.Timeout = System.Threading.Timeout.Infinite;
                req.Credentials = CredentialCache.DefaultCredentials;
                WebResponse res = req.GetResponse();
                Stream inStream = res.GetResponseStream();

                FileStream fs = new FileStream(sfilePath, FileMode.OpenOrCreate);

                byte[] inData = new byte[4096];
                int bytesRead = inStream.Read(inData, 0, inData.Length);
                while (bytesRead > 0)
                {
                    fs.Write(inData, 0, bytesRead);
                    bytesRead = inStream.Read(inData, 0, inData.Length);
                }

                fs.Close();

                inStream.Close();
                LogConsole.Log("WebDAV Download", $"完成下载（{DestinationPath} => {SavePath}）", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                LogConsole.Log("WebDAV Upload", $"下载文件时发生错误：{ex.Message}", ConsoleColor.Red);
            }
        }

        /// <summary>
        /// 使用 WebDAV 删除存储在网络存储服务的文件
        /// </summary>
        /// <param name="Address">服务器所在地址（一般提供商会在提供相关服务的地方注明地址）</param>
        /// <param name="DestinationPath">服务器上文件的目标路径（相对于服务器根目录路径。必须指定一个目录）</param>
        /// <param name="Account">登陆账户（一般为你注册的用户名或邮箱）</param>
        /// <param name="Password">账户密码 / 访问码（其中访问码需要另行生成，且与主密码是两样东西）</param>
        public static void Delete(string Address, string Account, string Password, string DestinationPath)
        {
            try
            {
                LogConsole.Log("WebDAV Delect", $"正在删除文件（{DestinationPath}）", ConsoleColor.Blue);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Address + DestinationPath);
                req.Credentials = new NetworkCredential(Account, Password);
                req.PreAuthenticate = true;
                req.Method = "DELETE";
                req.AllowWriteStreamBuffering = true;

                req.GetResponse();
                LogConsole.Log("WebDAV Delect", $"你已成功地删除了 {DestinationPath}", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                LogConsole.Log("WebDAV Delect", $"删除文件时发生错误：{ex.Message}", ConsoleColor.Red);
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
    }
}
