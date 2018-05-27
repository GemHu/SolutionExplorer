/// <summary>
/// @file   DZUtilityFileName.cs
///	@brief  文件名和文件操作相关的便捷函数。
/// @author	DothanTech 刘伟宏
/// 
/// Copyright(C) 2011~2014, DothanTech. All rights reserved.
/// </summary>

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace DothanTech.Helpers
{
    /// <summary>
    /// 文件名和文件操作相关的便捷函数。
    /// </summary>
    public static class FileName
    {
        public enum FilePathType
        {
            None,
            Relative,
            Absolute,
        }

        static public string GetFilePath(string csFileName)
        {
            int nPos = csFileName.LastIndexOf('\\');
            if (nPos < 0) return "";
            return csFileName.Substring(0, nPos + 1);
        }

        [DllImport("kernel32.dll ", CharSet = CharSet.Auto)]
        public static extern int GetShortPathName([MarshalAs(UnmanagedType.LPTStr)] string path, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder shortPath, int shortPathLength);

        /// <summary>
        /// Get 8.3 File name
        /// </summary>
        /// <param name="csFileName"></param>
        /// <returns></returns>
        static public string GetShortFilePath(string csFileName)
        {
            try
            {
                // not a absolute path but a relative one
                if (!Path.IsPathRooted(csFileName))
                {
                    return csFileName;
                }

                StringBuilder shortNameBuffer = new StringBuilder(256);
                int bufferSize = shortNameBuffer.Capacity;

                int result = GetShortPathName(csFileName, shortNameBuffer, bufferSize);

                return shortNameBuffer.ToString();
            }
            catch (Exception ee)
            {
                Trace.WriteLine("### [" + ee.Source + "] Exception: " + ee.Message);
                Trace.WriteLine("### " + ee.StackTrace);

                return csFileName;
            }
        }

        static public string GetSafePath(string csPath)
        {
            if (string.IsNullOrEmpty(csPath))
                return csPath;

            if (csPath.EndsWith("\\"))
                return csPath;

            return csPath + "\\";
        }

        static public string GetPathNoEndSlash(string csPath)
        {
            if (string.IsNullOrEmpty(csPath))
                return csPath;

            if (!csPath.EndsWith("\\"))
                return csPath;

            return csPath.Substring(0, csPath.Length - 1);
        }

        static public FilePathType GetFilePathType(string csFileName)
        {
            if (string.IsNullOrEmpty(csFileName))
                return FilePathType.None;

            if (csFileName.IndexOf('\\') < 0)
                return FilePathType.None;

            if (csFileName[0] == '\\' && csFileName[1] == '\\')
                return FilePathType.Absolute;

            if (csFileName.Length >= 2 && csFileName[1] == ':')
                return FilePathType.Absolute;

            return FilePathType.Relative;
        }

        static public string GetAbsoluteFileName(string csBasePath, string csFileName)
        {
            if (string.IsNullOrEmpty(csBasePath))
                return csFileName;
            if (string.IsNullOrEmpty(csFileName))
                return csFileName;

            switch (csFileName[0])
            {
                case '.':
                    if (csFileName.Length >= 3 &&
                        csFileName[1] == '\\')
                    {
                        return GetSafePath(csBasePath) + csFileName.Substring(2);
                    }
                    break;
                case '\\':
                    if (csFileName.Length >= 2 &&
                        csFileName[1] != '\\')
                    {
                        return GetSafePath(csBasePath) + csFileName.Substring(1);
                    }
                    break;
                default:
                    if (csFileName.Length >= 2 &&
                        csFileName[1] != ':')
                    {
                        return GetSafePath(csBasePath) + csFileName;
                    }
                    break;
            }

            return csFileName;
        }

        // return relative file name, leading with '\'.
        static public string GetRelativeFileName(string csBasePath, string csFileName)
        {
            if (string.IsNullOrEmpty(csBasePath))
                return csFileName;
            if (string.IsNullOrEmpty(csFileName))
                return csFileName;

            csBasePath = GetSafePath(csBasePath);
            if (csFileName.Length > csBasePath.Length &&
                csFileName.StartsWith(csBasePath, StringComparison.OrdinalIgnoreCase))
            {
                return csFileName.Substring(csBasePath.Length - 1);
            }

            return csFileName;
        }

        static public string GetFilePathWithoutExtension(string csFileName)
        {
            if (string.IsNullOrEmpty(csFileName))
                return csFileName;

            int pos1 = csFileName.LastIndexOf('.');
            if (pos1 < 0) return csFileName;
            int pos2 = csFileName.LastIndexOf('\\');
            if (pos1 < pos2) return csFileName;

            return csFileName.Substring(0, pos1);
        }

        static public string GetNewExtFileName(string csFileName, string csNewExt)
        {
            csFileName = GetFilePathWithoutExtension(csFileName);
            if (string.IsNullOrEmpty(csNewExt)) return csFileName;

            if (csNewExt[0] != '.') csNewExt = '.' + csNewExt;
            return csFileName + csNewExt;
        }

        static public string GetFirstFile(string pattern)
        {
            string path = GetFilePath(pattern);
            if (string.IsNullOrEmpty(path))
                return "";
            pattern = System.IO.Path.GetFileName(pattern);
            if (string.IsNullOrEmpty(pattern))
                return "";

            string[] files = System.IO.Directory.GetFiles(path, pattern);
            if (files == null || files.Count() <= 0)
                return "";

            return files[0];
        }

        static public DateTime GetFileWriteTime(string file)
        {
            try
            {
                return File.GetLastWriteTime(file);
            }
            catch (Exception e)
            {
                Trace.WriteLine("### [" + e.Source + "] Exception: " + e.Message);
                Trace.WriteLine("### " + e.StackTrace);
                return DummyTime;
            }
        }

        static public bool SetFileWriteTime(string file, DateTime writeTime)
        {
            try
            {
                File.SetLastWriteTime(file, writeTime);
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine("### [" + e.Source + "] Exception: " + e.Message);
                Trace.WriteLine("### " + e.StackTrace);
                return false;
            }
        }

        static public int CompareFileWriteTime(string file1, string file2)
        {
            if (File.Exists(file1))
            {
                if (File.Exists(file2))
                {
                    DateTime time1 = GetFileWriteTime(file1);
                    DateTime time2 = GetFileWriteTime(file2);
                    if (time1 > time2)
                        return 1;
                    else if (time1 == time2)
                        return 0;
                    else
                        return -1;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                //if (File.Exists(file2))
                return -1;
                //else
                //    return 0;
            }
        }

        static public DateTime DummyTime = new DateTime(1971, 2, 3, 4, 5, 6);

        static public bool CopyFile(string srcFile, string destFile)
        {
            try
            {
                if (!File.Exists(srcFile))
                    return false;

                if (srcFile.Equals(destFile, StringComparison.OrdinalIgnoreCase))
                    return false;

                File.Copy(srcFile, destFile, true);

                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine("### [" + e.Source + "] Exception: " + e.Message);
                Trace.WriteLine("### " + e.StackTrace);
                return false;
            }
        }

        static public bool MoveFile(string srcFile, string destFile)
        {
            try
            {
                if (!File.Exists(srcFile))
                    return false;

                if (srcFile.Equals(destFile, StringComparison.OrdinalIgnoreCase))
                    return true;

                if (File.Exists(destFile))
                {
                    File.Copy(srcFile, destFile, true);
                    File.Delete(srcFile);
                }
                else
                {
                    File.Move(srcFile, destFile);
                }

                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine("### [" + e.Source + "] Exception: " + e.Message);
                Trace.WriteLine("### " + e.StackTrace);
                return false;
            }
        }

        static public bool DeleteFile(string file)
        {
            try
            {
                if (!File.Exists(file))
                    return false;

                File.Delete(file);
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine("### [" + e.Source + "] Exception: " + e.Message);
                Trace.WriteLine("### " + e.StackTrace);
                return false;
            }
        }

        #region Delete file to RecycleBin

        private const int FO_DELETE = 0x3;
        private const ushort FOF_NOCONFIRMATION = 0x10;
        private const ushort FOF_ALLOWUNDO = 0x40;
        private const ushort FOF_NOERRORUI = 0x0400;

        [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int SHFileOperation([In, Out] _SHFILEOPSTRUCT str);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class _SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            public UInt32 wFunc;
            public string pFrom;
            public string pTo;
            public UInt16 fFlags;
            public Int32 fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        public static bool DeleteToRecycleBin(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;

            // remove end \\ char otherwise SHFileOperation() will failed
            if (path.EndsWith(@"\"))
                path = path.Substring(0, path.Length - 1);

            if (!File.Exists(path) && !Directory.Exists(path))
                return false;

            try
            {
                _SHFILEOPSTRUCT pm = new _SHFILEOPSTRUCT();
                pm.wFunc = FO_DELETE;
                pm.pFrom = path + '\0';
                pm.pTo = null;
                pm.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION | FOF_NOERRORUI;
                return (SHFileOperation(pm) == 0);
            }
            catch (Exception ee)
            {
                Trace.WriteLine("### [" + ee.Source + "] Exception: " + ee.Message);
                Trace.WriteLine("### " + ee.StackTrace);
                return false;
            }
        }

        #endregion

        /// <summary>
        /// 判断当前文件是否存在，如果存在，则进行递增判断，直至找到一个可用的文件为止；
        /// </summary>
        /// <param name="file">带判断文件；</param>
        /// <param name="check">文件名称检测函数：
        ///     参数1：待检测文件全路径；
        ///     参数2：待检测文件名称；
        ///     返回值：true：表示文件名可用，false：表示文件名不可用；
        /// </param>
        /// <param name="format">生成递增文件的格式；</param>
        /// <param name="maxIndex">地边查找的最大索引值，默认为1024；</param>
        /// <returns>获取到的可用文件；</returns>
        public static String IncreateFileName(String file, Func<String, String, bool> check = null, String format = null, int maxIndex = 1024)
        {
            if (String.IsNullOrEmpty(file) || !File.Exists(file))
                return file;
            if (String.IsNullOrEmpty(format) || !Regex.IsMatch(format, @".*\{0}.*\{1}.*\{2}.*"))
                format = "{0}{1}{2}";

            String ext = Path.GetExtension(file);
            String fileName = Path.GetFileNameWithoutExtension(file);
            String filePath = Path.GetDirectoryName(file);
            String currName;
            String currFile;

            for (int i = 1; i < maxIndex; i++)
            {
                currName = String.Format(format, fileName, i, ext);
                currFile = Path.Combine(filePath, currName);
                if (check != null)
                {
                    // 文件名不可用
                    if (!check(currFile, currName))
                        continue;
                }
                if (!File.Exists(currFile))
                    return currFile;
            }

            return String.Empty;
        }
    }
}
