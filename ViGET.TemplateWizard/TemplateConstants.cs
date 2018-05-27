using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DothanTech.ViGET.TemplateWizard
{
    static class Constants
    {
        public static class Config
        {
            public const String DefaultLocationKey = "DefaultLocation";
            public static String DefaultLocationValue = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
        /// <summary>
        /// 模板文件相关后缀名；
        /// </summary>
        public static class Extension
        {
            public const String Template = ".vstemplate";
        }

        /// <summary>
        /// VSTemplate文件标签常量；
        /// </summary>
        public static class TAG
        {
            /// <summary>
            /// VSTemplate根节点；
            /// </summary>
            public const String Root = "VSTemplate";

            public const String TemplateData = "TemplateData";
            // TemplateData子标签
            public const String Name = "Name";
            public const String Type = "Type";
            public const String Category = "Category";
            public const String OwnerType = "OwnerType";
            public const String Icon = "Icon";
            public const String Description = "Description";
            public const String SortOrder = "SortOrder";
            public const String DefaultName = "DefaultName";

            public const String TemplateContent = "TemplateContent";
            public const String Project = "Project";
            public const String ProjectItem = "ProjectItem";

        }

        /// <summary>
        /// VSTemplate中的标签属性常量；
        /// </summary>
        public static class Attribute
        {
            // VSTemplate标签属性
            public const String Type = "Type";
            public const String Version = "Version";


            // Project标签属性
            public const String File = "File";
            public const String TargetFileName = "TargetFileName";
            public const String ReplaceParameters = "ReplaceParameters";
        }

        public static class ReplaceParam
        {
            public const String ProjectName = "$safeprojectname$";
            public const String ENV = "$ENV$";
        }

        public static class Message
        {
            public const String Title = "ViGET";
            public const String SolutionIsExist = "The project cannot be created, becourse another project already exists in the folder '{0}'";
            public const String ProjectIsExist = "The solution already contains an item named '{0}'";
            public const String FileIsExist = "A file with the name '{0}' already exists. Please give a unique name to the item you are adding, or delete the existing item first.";
        }
    }
}
