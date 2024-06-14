using System;
using System.IO;
using System.Windows.Forms;

namespace 复制目录及其内容
{
    class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Test(@"d:\data");
            //Test(@"d:\1234");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static void Test(string dirName)    // 测试已存在目录、创建新目录并删除
        {
            try
            {
                if (Directory.Exists(dirName))
                {
                    Console.WriteLine(dirName + ": This path exists already.");
                    return;
                }
                Directory.CreateDirectory(dirName);
                Console.WriteLine("The directory was created successfully.");
                Console.WriteLine("按任意键将删除新创建的目录: " + dirName);
                Console.ReadKey(true);
                Directory.Delete(dirName);
                Console.WriteLine("The directory was deleted successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }
    }
}
