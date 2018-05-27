using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DothanTech.ViGET.Manager
{
    internal static class Constants
    {
        public static class Extention
        {
            public const String Solution = ".vgsln";
            public const String Project = ".vgstation";
        }

        public static class TAG
        {
            public const String Guid = "Guid";
            public const String PropertyGroup = "PropertyGroup";
            public const String ItemGroup = "ItemGroup";
            public const String File = "File";
            public const String Global = "Global";
            public const String Name = "Name";
            public const String Folder = "Folder";

            // Solution
            public const String Soluton = "Solution";
            public const String Projects = "Projects";

            // Project
            public const String Project = "Project";
            public const String CPUs = "CPUs";
            public const String CPU = "CPU";
            public const String GlobalVariable = "GlobalVariable";
            public const String Settings = "Settings";
            public const String Connection = "Connection";
            public const String Tasks = "Tasks";
            public const String Task = "Task";
            public const String Files = "Files";
        }

        public static class Attribute
        {
            public const String Version = "version";
            public const String EncyptCode = "";
            public const String CreateDate = "createDate";
            public const String Author = "author";
            public const String LastModifyDate = "lastModifyDate";
            public const String FilePath = "filePath";
            public const String Name = "name";
            public const String IsActive = "isActive";
            public const String HwType = "hwType";
            public const String HasShmVars = "hasShmVars";
            public const String TcpPort = "tcpPort";
            public const String TcpIp = "tcpIp";
            public const String Cyclic = "cyclic";
            public const String Interrupt = "interrupt";
            public const String Type = "type";
            public const String Priority = "priority";
            public const String FileNumber = "fileNumber";
            public const String ExectionOrder = "exectionOrder";
            public const String FileName = "fileName";
            public const String Guid = "Guid";
            public const String FileType = "fileType";
            public const String Linked = "link";

        }

        public static class Other
        {
            public const String DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }
    }
}
