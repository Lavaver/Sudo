using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Lavaver.WorldBackup.ThirdUsing.squidbox.SevenZip;

namespace com.Lavaver.WorldBackup.Multivolume
{
    /// <summary>
    /// 分卷解压主类。包括常见分卷压缩格式，可按需选择当前情况下适合的解压方式（例如 winrar 的 [文件名].part[分卷编号].rar ，7-Zip 的 [文件名].7z.[从 001 开头的分卷编号]）。所有的分卷解压方法都包含三个必须参数和一个可选参数：第一个分卷压缩包的路径（string）、分卷压缩包总数（int）、解压位置（string）和缓存路径（可选，string）
    /// </summary>
    public class Unzip
    {
        /// <summary>
        /// 7-Zip 分卷解压
        /// </summary>
        /// <param name="Firstvolume">第一个分卷压缩包的路径</param>
        /// <param name="UnzipPath">解压位置</param>
        public static void SevenZip(string Firstvolume, string UnzipPath)
        {

        }
    }
}
