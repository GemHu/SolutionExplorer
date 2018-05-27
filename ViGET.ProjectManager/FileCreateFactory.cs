using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DothanTech.ViGET.Manager
{
    /// <summary>
    /// 文件创建工程类，用于统一管理文件的创建；
    /// </summary>
    public class FileCreateFactory
    {
        public static Dictionary<String, Type> DFileTypes
        {
            get
            {
                if (_dFileTypes == null)
                    _dFileTypes = new Dictionary<string, Type>();

                return _dFileTypes;
            }
        }
        private static Dictionary<String, Type> _dFileTypes;

        public static void RegisterFileType(String fileType, Type type)
        {
            if (String.IsNullOrEmpty(fileType) || type == null)
                return;
            if (!type.IsSubclassOf(typeof(ViFileInfo)))
                return;

            fileType = fileType.ToUpper();
            if (DFileTypes.ContainsKey(fileType))
                return;
            DFileTypes.Add(fileType, type);
        }

        static FileCreateFactory()
        {
            // register filetype
            FileCreateFactory.RegisterFileType(PcsFileInfo.FileType.CFCProgram, typeof(ViCFCFile));
            FileCreateFactory.RegisterFileType(PcsFileInfo.FileType.CPU, typeof(ViCPUInfo));
        }

        public static ViFileInfo CreateFile(String file, String fileType)
        {
            if (String.IsNullOrEmpty(file))
                return null;
            if (String.IsNullOrEmpty(fileType))
            {
                PcsFileInfo fileInfo = PcsFileInfo.GetPcsFileInfo(file);
                if (fileInfo == null)
                    return null;

                fileType = fileInfo.type;
            }
            if (String.IsNullOrEmpty(fileType))
                return null;

            try
            {
                fileType = fileType.ToUpper();
                if (!DFileTypes.ContainsKey(fileType))
                    return null;

                Type type = FileCreateFactory.DFileTypes[fileType];
                ViFileInfo info = Activator.CreateInstance(type, file) as ViFileInfo;
                if (info != null)
                    info.Type = fileType;

                return info;
            }
            catch (Exception ee)
            {
                Trace.WriteLine("### [" + ee.Source + "] Exception: " + ee.Message);
                Trace.WriteLine("### " + ee.StackTrace);
                return null;
            }
        }
    }
}
