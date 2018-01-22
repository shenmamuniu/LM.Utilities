using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace LM.Utilities
{
    /// <summary>
    /// https://www.cnblogs.com/cynchanpin/p/6791450.html
    /// </summary>
    public class FileZip
    {
        #region 压缩logs文件夹下日志
        public static void CompresslogDic()
        {
            try
            {
                string logFilePath = AppDomain.CurrentDomain.BaseDirectory + "logs";
                DirectoryInfo logsDic = new DirectoryInfo(logFilePath);
                FileInfo[] bipLog = logsDic.GetFiles();
                DateTime dt = DateTime.Now;
                List<FileInfo> logsInOneDay = new List<FileInfo>();
                for (int i = 0; i < bipLog.Length; i++)
                {
                    if (bipLog[i].Name.Substring(bipLog[i].Name.Length - 3) != "zip")
                    {
                        logsInOneDay.Add(bipLog[i]);
                    }
                }
                if (logsInOneDay.Count > 0)
                {
                    try
                    {
                        if (!Directory.Exists(logsDic.FullName + "\\CompressionDirectory"))
                        {
                            Directory.CreateDirectory(logsDic.FullName + "\\CompressionDirectory");
                        }
                        string compressFileName = dt.ToString("yyyy-MM-dd");
                        if (File.Exists(logsDic.FullName + "\\CompressionDirectory\\" + dt.ToString("yyyy-MM-dd") + ".zip"))
                        {
                            Guid guid = Guid.NewGuid();
                            compressFileName = compressFileName + "-" + guid.ToString();
                        }
                        compressFileName += ".zip";
                        Compress(logsInOneDay, logsDic.FullName + "\\CompressionDirectory\\" + compressFileName, 9, 100);
                        foreach (FileInfo fileInfo in logsInOneDay)
                        {
                            try
                            {
                                fileInfo.Delete();
                            }
                            catch (Exception e)
                            {
                                //错误信息记录处理
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //错误信息记录处理
                    }
                }
            }
            catch (Exception e)
            {
                //错误信息记录处理
            }
        }
        #endregion

        #region 压缩logs子文件夹下日志
        public static void CompresslogsDic()
        {
            try
            {
                string logFilePath = AppDomain.CurrentDomain.BaseDirectory + "logs";
                DirectoryInfo logsDic = new DirectoryInfo(logFilePath);
                FileInfo[] bipLog = logsDic.GetFiles();
                DateTime dt = DateTime.Now;
                DirectoryInfo[] subLosgDic = logsDic.GetDirectories();
                foreach (DirectoryInfo bankDic in subLosgDic)
                {
                    dt = DateTime.Now;
                    bipLog = bankDic.GetFiles();
                    List<FileInfo> logsInOneDay = new List<FileInfo>();
                    for (int i = 0; i < bipLog.Length; i++)
                    {
                        if (bipLog[i].Name.Substring(bipLog[i].Name.Length - 3) != "zip")
                        {
                            logsInOneDay.Add(bipLog[i]);
                        }
                    }
                    if (logsInOneDay.Count > 0)
                    {
                        try
                        {
                            if (!Directory.Exists(bankDic.FullName + "\\CompressionDirectory"))
                            {
                                Directory.CreateDirectory(bankDic.FullName + "\\CompressionDirectory");
                            }
                            string compressFileName = dt.ToString("yyyy-MM-dd");
                            if (File.Exists(bankDic.FullName + "\\CompressionDirectory\\" + dt.ToString("yyyy-MM-dd") + ".zip"))
                            {
                                Guid guid = Guid.NewGuid();
                                compressFileName = compressFileName + "-" + guid.ToString();
                            }
                            compressFileName += ".zip";
                            Compress(logsInOneDay, bankDic.FullName + "\\CompressionDirectory\\" + compressFileName, 9, 100);
                            foreach (FileInfo fileInfo in logsInOneDay)
                            {
                                try
                                {
                                    fileInfo.Delete();
                                }
                                catch (Exception e)
                                {
                                    //错误信息记录处理
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            //错误信息记录处理
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //错误信息记录处理
            }
        }
        #endregion
        #region 压缩文件
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileNames">要打包的文件列表</param>
        /// <param name="GzipFileName">目标文件名称</param>
        /// <param name="CompressionLevel">压缩品质级别（0~9）</param>
        /// <param name="SleepTimer">休眠时间（单位毫秒）</param>     
        public static void Compress(List<FileInfo> fileNames, string GzipFileName, int CompressionLevel, int SleepTimer)
        {
            ZipOutputStream s = new ZipOutputStream(File.Create(GzipFileName));
            try
            {
                s.SetLevel(CompressionLevel);   //0 - store only to 9 - means best compression
                foreach (FileInfo file in fileNames)
                {
                    FileStream fs = null;
                    try
                    {
                        fs = file.Open(FileMode.Open, FileAccess.ReadWrite);
                    }
                    catch
                    {
                        continue;
                    }
                    // 将文件分批读入缓冲区
                    byte[] data = new byte[2048];
                    int size = 2048;
                    ZipEntry entry = new ZipEntry(Path.GetFileName(file.Name));
                    entry.DateTime = (file.CreationTime > file.LastWriteTime ? file.LastWriteTime : file.CreationTime);
                    s.PutNextEntry(entry);
                    while (true)
                    {
                        size = fs.Read(data, 0, size);
                        if (size <= 0) break;
                        s.Write(data, 0, size);
                    }
                    fs.Close();
                    Thread.Sleep(SleepTimer);
                }
            }
            finally
            {
                s.Finish();
                s.Close();
            }
        }
        #endregion
        #region 解压缩文件
        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="GzipFile">压缩包文件名称</param>
        /// <param name="targetPath">解压缩目标路径</param>       
        public static void Decompress(string GzipFile, string targetPath)
        {
            //string directoryName = Path.GetDirectoryName(targetPath + "//") + "//";
            string directoryName = targetPath;
            if (!Directory.Exists(directoryName)) Directory.CreateDirectory(directoryName);//生成解压文件夹
            string CurrentDirectory = directoryName;
            byte[] data = new byte[2048];
            int size = 2048;
            ZipEntry theEntry = null;
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(GzipFile)))
            {
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    if (theEntry.IsDirectory)
                    {// 该结点是文件夹
                        if (!Directory.Exists(CurrentDirectory + theEntry.Name)) Directory.CreateDirectory(CurrentDirectory + theEntry.Name);
                    }
                    else
                    {
                        if (theEntry.Name != String.Empty)
                        {
                            //检查多级文件夹是否存在  
                            if (theEntry.Name.Contains("\\"))
                            {
                                string parentDirPath = theEntry.Name.Remove(theEntry.Name.LastIndexOf("\\") + 1);
                                if (!Directory.Exists(parentDirPath))
                                {
                                    Directory.CreateDirectory(CurrentDirectory + parentDirPath);
                                }
                            }
                            //解压文件到指定的文件夹
                            using (FileStream streamWriter = File.Create(CurrentDirectory + theEntry.Name))
                            {
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size <= 0) break;

                                    streamWriter.Write(data, 0, size);
                                }
                                streamWriter.Close();
                            }
                        }
                    }
                }
                s.Close();
            }
        }
        #endregion
        #region 压缩目录
        /// <summary>    
        /// 压缩目录    
        /// </summary>    
        /// <param name="dirPath">要打包的目录</param>    
        /// <param name="GzipFileName">目标文件名称</param>    
        /// <param name="CompressionLevel">压缩品质级别（0~9）</param>    
        /// <param name="deleteDir">是否删除原目录</param>  
        public static void CompressDirectory(string dirPath, string GzipFileName, int CompressionLevel, bool deleteDir)
        {
            //压缩文件为空时默认与压缩目录同一级目录    
            if (GzipFileName == string.Empty)
            {
                GzipFileName = dirPath.Substring(dirPath.LastIndexOf("//") + 1);
                GzipFileName = dirPath.Substring(0, dirPath.LastIndexOf("//")) + "//" + GzipFileName + ".zip";
            }
            //if (Path.GetExtension(GzipFileName) != ".zip")  
            //{  
            //    GzipFileName = GzipFileName + ".zip";  
            //}  


            using (ZipOutputStream zipoutputstream = new ZipOutputStream(File.Create(GzipFileName)))
            {
                zipoutputstream.SetLevel(CompressionLevel);
                ICSharpCode.SharpZipLib.Checksums.Crc32 crc = new ICSharpCode.SharpZipLib.Checksums.Crc32();
                Dictionary<string, DateTime> fileList = GetAllFies(dirPath);
                foreach (KeyValuePair<string, DateTime> item in fileList)
                {
                    Debug.WriteLine(item.Key);
                    FileStream fs = File.OpenRead(item.Key.ToString());
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    ZipEntry entry = new ZipEntry(item.Key.Substring(dirPath.Length));
                    entry.DateTime = item.Value;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipoutputstream.PutNextEntry(entry);
                    zipoutputstream.Write(buffer, 0, buffer.Length);
                }
            }

            if (deleteDir)
            {
                Directory.Delete(dirPath, true);
            }
        }
        #endregion
        #region 获取全部文件
        /// <summary>    
        /// 获取全部文件    
        /// </summary>    
        /// <returns></returns>    
        private static Dictionary<string, DateTime> GetAllFies(string dir)
        {
            Dictionary<string, DateTime> FilesList = new Dictionary<string, DateTime>();
            DirectoryInfo fileDire = new DirectoryInfo(dir);
            if (!fileDire.Exists)
            {
                throw new System.IO.FileNotFoundException("目录:" + fileDire.FullName + "没有找到!");
            }
            GetAllDirFiles(fileDire, FilesList);
            GetAllDirsFiles(fileDire.GetDirectories(), FilesList);
            return FilesList;
        }
        #endregion
        #region 获取一个目录下的全部目录里的文件
        /// <summary>    
        /// 获取一个目录下的全部目录里的文件    
        /// </summary>    
        /// <param name="dirs"></param>    
        /// <param name="filesList"></param>    
        private static void GetAllDirsFiles(DirectoryInfo[] dirs, Dictionary<string, DateTime> filesList)
        {
            foreach (DirectoryInfo dir in dirs)
            {
                foreach (FileInfo file in dir.GetFiles("*.*"))
                {
                    filesList.Add(file.FullName, file.LastWriteTime);
                }
                GetAllDirsFiles(dir.GetDirectories(), filesList);
            }
        }
        #endregion
        #region 获取一个目录下的文件
        /// <summary>    
        /// 获取一个目录下的文件    
        /// </summary>    
        /// <param name="dir">目录名称</param>    
        /// <param name="filesList">文件列表HastTable</param>    
        private static void GetAllDirFiles(DirectoryInfo dir, Dictionary<string, DateTime> filesList)
        {
            foreach (FileInfo file in dir.GetFiles("*.*"))
            {
                filesList.Add(file.FullName, file.LastWriteTime);
            }
        }
        #endregion
    }
}
