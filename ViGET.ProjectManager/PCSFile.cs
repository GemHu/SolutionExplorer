using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DothanTech.ViGET.Manager
{
    public class PcsFileInfo
    {
        public static class Extension
        {
            public const String SolutionFile = ".vgsln";
            public const String ProjectFile = ".vgstation";

            public const String CFCProgram = ".CFC";
        }

        public static class FileType
        {
            public const String CFCProgram = "CFC";
            public const String CPU = "CPU1010";
        }

        private static Dictionary<String, PcsFileInfo> _DPcsFileInfos;

        public String type;
        public string extName;

        public PcsFileInfo(String type, string extName)
        {
            this.type = type;
            this.extName = extName;
        }

        /// <summary>
        /// 文件类型与文件后缀、文件Section对应的关联信息。
        /// </summary>
        public static Dictionary<String, PcsFileInfo> DPcsFileInfos
        {
            get
            {
                if (_DPcsFileInfos == null)
                {
                    _DPcsFileInfos = new Dictionary<String, PcsFileInfo>();
                    _DPcsFileInfos[FileType.CFCProgram] = new PcsFileInfo(FileType.CFCProgram, Extension.CFCProgram);
                    
                }

                return _DPcsFileInfos;
            }
        }

        public static PcsFileInfo GetPcsFileInfo(String fileType)
        {
            if (String.IsNullOrEmpty(fileType))
                return null;
            String ext = Path.GetExtension(fileType);

            foreach (PcsFileInfo item in PcsFileInfo.DPcsFileInfos.Values)
            {
                if (item.type.Equals(fileType, StringComparison.OrdinalIgnoreCase))
                    return item;
                if (!String.IsNullOrEmpty(ext) && item.extName.Equals(ext, StringComparison.OrdinalIgnoreCase))
                    return item;
            }

            return null;
        }
    }

    /// <summary>
    /// 工程 .VAR 文件中包含的工程文件信息。
    /// </summary>
    public class PCSFile : IComparable<PCSFile>, IComparable
    {
        public PCSFile(String type, string file)
        {
            this.Type = type;
            this.File = file;
            this.Key = GetFileKey(file);
        }

        public String Type { get; protected set; }
        public string File { get; protected set; }
        public string Key { get; protected set; }

        public int CompareTo(PCSFile other)
        {
            if (other == null) return -1;
            return string.Compare(this.Key, other.Key, true);
        }

        public int CompareTo(object other)
        {
            return this.CompareTo(other as PCSFile);
        }

        /// <summary>
        /// 得到文件名称中的对象关键字。
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <returns>对象关键字</returns>
        public static string GetFileKey(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            return Path.GetFileNameWithoutExtension(fileName).ToUpper();
        }

    }

}
