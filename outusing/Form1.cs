using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 研究用文件
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnBackUp_Click(object sender, EventArgs e)
        {
            string sourceDirectory = txtSource.Text;
            string targetDirectory = txtTarget.Text;
            if (sourceDirectory.ToLower() == targetDirectory.ToLower())
            {
                Console.WriteLine("源目录和备份目录不能是同一目录！");
                MessageBox.Show("源目录和备份目录不能是同一目录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);    // 源目录
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);    // 备份目录
            Console.WriteLine("源目录: {0}  备份目录: {1}", diSource.FullName, diTarget.FullName);
            Console.WriteLine("源目录: {0}  {1}  {2}", diSource.FullName, diSource.Name, diSource.Root);
            // 因为根目录的Name和FullName完全一样，如果源目录是根目录，则diSource.Name变成绝对路径，
            // 由此Path.Combine会直接返回第二个参数(参见方法的说明)，导致得到的备份目录和源目录一样
            // 实现效果：源目录为根目录时，备份其下所有子目录和文件；非根目录时，只备份该目录本身
            // 指定的备份目录名字和源目录名字相同时，直接备份该目录，如不存在则新建；不同时，视作指定的是所要备份目录的存放目录即父目录
            if (diSource.FullName != diSource.Name && diTarget.Name != diSource.Name)
                diTarget = new DirectoryInfo(Path.Combine(diTarget.FullName, diSource.Name));    // 创建同名目录
            if (!diTarget.Exists) diTarget.Create();    // 如果该目录已存在，则此方法不执行任何操作
            Console.WriteLine("源目录: {0}  备份目录: {1}", diSource.Name, diTarget.FullName);
            txtSource.Enabled = false;
            txtTarget.Enabled = false;
            btnSource.Enabled = false;
            btnTarget.Enabled = false;
            btnBackUp.Enabled = false;
            lblWork.Text = "备份开始！";    // 发现标签的设置并不生效
            bool result = await Task.Run(() => CopyAllAsync(diSource, diTarget));
            if (result)    // if (await CopyAllAsync(diSource, diTarget)) 开始后界面会卡
            {
                lblWork.Text = "备份完成！";
                MessageBox.Show("备份完毕！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else lblWork.Text = "出现错误！";
            txtSource.Enabled = true;
            txtTarget.Enabled = true;
            btnSource.Enabled = true;
            btnTarget.Enabled = true;
            btnBackUp.Enabled = true;
            btnBackUp.Focus();
        }

        public async Task<bool> CopyAllAsync(DirectoryInfo source, DirectoryInfo target)
        {
            try
            {
                foreach (FileInfo fi in source.GetFiles())    // 复制最新文件
                {
                    Console.WriteLine(@"准备复制文件 {0}\{1}", target.FullName, fi.Name);    // Name不含路径，仅文件名
                    FileInfo newfi = new FileInfo(Path.Combine(target.FullName, fi.Name));
                    if (!newfi.Exists || (newfi.Exists && fi.LastWriteTime != newfi.LastWriteTime))
                    // 使用fi.LastWriteTime > newfi.LastWriteTime中途取消备份时会导致文件数据未能完全备份
                    {
                        Console.WriteLine("正在复制文件 {0}", newfi.FullName);
                        lblWork.Text = string.Format("正在复制文件\n{0}", newfi.FullName);
                        if (newfi.Exists && newfi.IsReadOnly) newfi.IsReadOnly = false;
                        // 覆盖或删除只读文件会产生异常：对路径“XXX”的访问被拒绝
                        fi.CopyTo(newfi.FullName, true);    // Copy each file into it's new directory
                    }
                }

                foreach (FileInfo fi2 in target.GetFiles())    // 删除源目录没有而目标目录中有的文件
                {
                    FileInfo newfi2 = new FileInfo(Path.Combine(source.FullName, fi2.Name));
                    if (!newfi2.Exists)
                    {
                        Console.WriteLine("正在删除文件 {0}", fi2.FullName);
                        lblWork.Text = string.Format("正在删除文件\n{0}", fi2.FullName);
                        if (fi2.IsReadOnly) fi2.IsReadOnly = false;
                        fi2.Delete();    // 没有权限(如系统盘需管理员权限)会产生异常，文件不存在不会产生异常
                    }
                }

                foreach (DirectoryInfo di in source.GetDirectories())    // 复制目录(实际上是创建同名目录，和源目录的属性不同步)
                {
                    Console.WriteLine("  {0}  {1}", di.FullName, di.Name);    // Name不含路径，仅本级目录名
                    if (di.Name == "$RECYCLE.BIN" || di.Name == "System Volume Information") continue;
                    Console.WriteLine(@"准备创建目录 {0}\{1}", target.FullName, di.Name);
                    DirectoryInfo newdi = new DirectoryInfo(Path.Combine(target.FullName, di.Name));
                    if (!newdi.Exists)    // 如果CopyAllAsync放在if里的bug: 只要存在同名目录，则不会进行子目录和子文件的检查和更新
                    {
                        Console.WriteLine("正在创建目录 {0}", newdi.FullName);
                        lblWork.Text = string.Format("正在复制目录\n{0}", newdi.FullName);
                        DirectoryInfo diTargetSubDir = target.CreateSubdirectory(di.Name);    // 创建目录
                        Console.WriteLine("完成创建目录 {0}", diTargetSubDir.FullName);
                    }
                    //await CopyAllAsync(di, newdi);
                    if (await CopyAllAsync(di, newdi) == false) return false; ;    // Copy each subdirectory using recursion
                }

                foreach (DirectoryInfo di2 in target.GetDirectories())    // 删除源目录没有而目标目录中有的目录(及其子目录和文件)
                {
                    DirectoryInfo newdi2 = new DirectoryInfo(Path.Combine(source.FullName, di2.Name));
                    if (!newdi2.Exists)
                    {
                        Console.WriteLine("正在删除目录 {0}", di2.FullName);
                        lblWork.Text = string.Format("正在删除目录\n{0}", di2.FullName);
                        if ((di2.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        //if (di2.Attributes.HasFlag(FileAttributes.ReadOnly))    // 作用同上(二选一即可)
                        {
                            Console.WriteLine("ReadOnlyDirectory");
                            di2.Attributes = di2.Attributes & ~FileAttributes.ReadOnly;    // 取消目录的只读属性
                        }
                        di2.Delete(true);    // 如不使用参数则异常"目录不是空的"；只读的目录和文件无法删除(如下级有只读的子目录和文件也不行)
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}\n{1}", e.Message, e.StackTrace);
                MessageBox.Show(e.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnSource_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "请选择源文件夹";
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            folderBrowserDialog1.ShowNewFolderButton = false;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) txtSource.Text = folderBrowserDialog1.SelectedPath;
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "请选择(或创建)备份文件夹";
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer;
            folderBrowserDialog1.ShowNewFolderButton = true;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) txtTarget.Text = folderBrowserDialog1.SelectedPath;
        }
    }
}

// 文件和目录的创建日期为首次全新复制时的创建时间
// 文件复制后修改日期始终保持原先的不变，目录的修改日期为首次全新复制时的创建时间(因为本就是新建)
// 单纯的覆盖不会改变修改时间和创建时间
// 文件发生的属性变化全新复制时可以保留(无法通过更新时间判断)
// 复制过程中取消的问题(针对很大的文件，未等待复制完成时)：
// 在windows文件资源管理器中的复制，不会显式产生占位文件，复制过程中如果取消，windows系统会主动收尾，不会出现遗留文件；
// 本程序使用CopyTo方法复制时，首先会先按照源文件的大小和名字生成一个同样大小和名字的占位文件，
// 如果复制完成，会将新文件的修改时间设为和源文件一致，如果中途取消复制，不会删除该占位文件，且该文件的修改时间和创建时间一致。
// 按本程序目前的设计思路，下次备份会跳过该文件，因此中途取消会导致文件产生无效备份。这是一个很大的问题，暂时没有解决方案。
// 临时的解决方案是：将第一个foreach中的条件判断改一下：将后面的大于号改为不等于号“!=”。