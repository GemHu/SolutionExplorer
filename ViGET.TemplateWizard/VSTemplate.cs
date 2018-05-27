using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DothanTech.ViGET.TemplateWizard
{
    /// <summary>
    /// 项目模板解析工具，用于解析VSTemplate文件；
    /// </summary>
    public class VSTemplate : ItemGroup
    {
        /// <summary>
        /// VSTemplate类型；
        /// </summary>
        public enum TemplateType
        {
            Project,        ///< Project
            Item,           ///< ProjectItem
            None            ///< 无效的类型
        }

        public String TemplateFolder
        {
            get
            {
                if (String.IsNullOrEmpty(this.Value))
                    return null;

                return Path.GetDirectoryName(this.Value);
            }
        }

        /// <summary>
        /// vstemplate文件版本号，默认为1.0；
        /// </summary>
        public String Version
        {
            get { return this._version; }
        }
        private String _version;
        /// <summary>
        /// 模板类型；
        /// </summary>
        public TemplateType Type
        {
            get { return this._type; }
        }
        private TemplateType _type = TemplateType.None;

        /// <summary>
        /// Project
        /// </summary>
        public Project Project { get; protected set; }

        /// <summary>
        /// TemplateData
        /// </summary>
        public TemplateData TemplateData
        {
            get { return this.templateData; }
        }
        private TemplateData templateData;

        private VSTemplate(String templateFile)
        {
            this.Value = templateFile;
        }

        public static VSTemplate Parse(String file)
        {
            if (!File.Exists(file))
                return null;

            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            XmlElement root = doc.DocumentElement;
            if (!Constants.TAG.Root.Equals(root.Name))
                return null;

            VSTemplate data = new VSTemplate(file);
            return data.Parse(root) ? data : null;
        }

        public override bool Parse(XmlElement element)
        {
            TemplateType tmpType = TemplateType.Item;
            if (!Enum.TryParse(element.GetAttribute(Constants.Attribute.Type), out tmpType))
                return false;

            this._type = tmpType;
            this._version = element.GetAttribute(Constants.Attribute.Version);

            // parse sub nodes;
            foreach (XmlElement subElement in element.ChildNodes)
            {
                // TemplateData
                if (Constants.TAG.TemplateData.Equals(subElement.Name, StringComparison.OrdinalIgnoreCase))
                {
                    this.templateData = new TemplateData().Parse(subElement);
                }
                else if (Constants.TAG.TemplateContent.Equals(subElement.Name, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (XmlElement contentElement in subElement.ChildNodes)
                    {
                        // Project
                        if (Constants.TAG.Project.Equals(contentElement.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            this.Project = new Project();
                            this.Project.Parse(contentElement);

                        } // ProjectItem
                        else if (Constants.TAG.ProjectItem.Equals(contentElement.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            ProjectItem item = new ProjectItem();
                            item.Parse(contentElement);
                            this.Items.Add(item);
                        }
                    }
                }
            }

            return true;
        }

        public String Copy(string targetLocation, String targetName)
        {
            Dictionary<String, String> options = new Dictionary<string, string>();
            options[Constants.ReplaceParam.ProjectName] = targetName;

            return this.Project.Copy(this.TemplateFolder, targetLocation, options);
        }

        public String GetProjectFile(String targetLocation, String targetName)
        {
            if (String.IsNullOrEmpty(this.Project.TargetFileName) || String.IsNullOrEmpty(targetName))
                return String.Empty;

            String fileName = this.Project.TargetFileName;
            if (this.Project.ReplaceParameters)
                fileName = fileName.Replace(Constants.ReplaceParam.ProjectName, targetName);

            return Path.Combine(targetLocation, fileName);
        }
    }

    public class TemplateData
    {
        public TemplateData()
        {
            this.OwnerTypes = new List<string>();
        }
        /// <summary>
        /// 模板名称；
        /// </summary>
        public String Name { get; protected set; }

        /// <summary>
        /// 模板功能描述；
        /// </summary>
        public String Description { get; protected set; }

        /// <summary>
        /// 模板所属项目名称；
        /// </summary>
        public String Type { get; protected set; }

        /// <summary>
        /// 当前项所属项目类型；
        /// </summary>
        public List<String> OwnerTypes { get; protected set; }

        /// <summary>
        /// 图标名称；
        /// </summary>
        public String Icon { get; protected set; }

        /// <summary>
        /// 排列顺序优先级；
        /// </summary>
        public int SortOrder { get; protected set; }

        /// <summary>
        /// 默认模板名称；
        /// </summary>
        public String DefaultName { get; protected set; }

        public TemplateData Parse(XmlElement element)
        {
            foreach (XmlNode item in element.ChildNodes)
            {
                if (Constants.TAG.Name.Equals(item.Name))
                {
                    this.Name = item.InnerText;
                }
                else if (Constants.TAG.Description.Equals(item.Name))
                {
                    this.Description = item.InnerText;
                }
                else if (Constants.TAG.Type.Equals(item.Name))
                {
                    this.Type = item.InnerText;
                }
                else if (Constants.TAG.DefaultName.Equals(item.Name))
                {
                    this.DefaultName = item.InnerText;
                }
                else if (Constants.TAG.Icon.Equals(item.Name))
                {
                    this.Icon = item.InnerText;
                }
                else if (Constants.TAG.SortOrder.Equals(item.Name))
                {
                    int value = 0;
                    this.SortOrder = (Int32.TryParse(item.InnerText, out value) ? value : Int32.MaxValue);
                }
                else if (Constants.TAG.Category.Equals(item.Name))
                {
                    foreach (XmlNode type in item.ChildNodes)
                    {
                        if (Constants.TAG.OwnerType.Equals(type.Name))
                        {
                            this.OwnerTypes.Add(type.InnerText);
                        }
                    }
                }
            }

            return this;
        }
    }

    public class ProjectItem
    {
        public String TargetFileName { get; protected set; }
        public bool ReplaceParameters { get; protected set; }
        public String Value { get; protected set; }

        public virtual bool Parse(XmlElement element)
        {
            String attr;
            bool ret;

            this.TargetFileName = element.GetAttribute(Constants.Attribute.TargetFileName);
            this.Value = String.IsNullOrEmpty(element.InnerText) ? String.Empty : element.InnerText.Trim();

            attr = element.GetAttribute(Constants.Attribute.ReplaceParameters);
            this.ReplaceParameters = (Boolean.TryParse(attr, out ret) ? ret : false);

            return true;
        }

        /// <summary>
        /// 复制模板文件到指定的位置；
        /// </summary>
        /// <param name="location"></param>
        public virtual String Copy(String sourceLocation, String targetLocation, Dictionary<String, String> options = null)
        {
            // <ProjectItem
            //  TargetFileName="$safeprojectname$.VAR"
            //  ReplaceParameters="true">
            //  ViGET.VAR
            //</ProjectItem>
            // 1、源文件参数检查；
            if (String.IsNullOrEmpty(this.Value) || String.IsNullOrEmpty(sourceLocation))
                return String.Empty;
            // 2、目标文件检测
            if (String.IsNullOrEmpty(targetLocation) || String.IsNullOrEmpty(this.TargetFileName))
                return String.Empty;

            String sourceName = this.Value;
            String sourceFile = Path.Combine(sourceLocation, sourceName);
            if (!File.Exists(sourceFile))
                return String.Empty;

            // 3、获取目标文件；
            String targetName = String.IsNullOrEmpty(this.TargetFileName) ? sourceName : this.TargetFileName;
            if (this.ReplaceParameters && options != null)
            {
                if (options.ContainsKey(Constants.ReplaceParam.ProjectName))
                    targetName = targetName.Replace(Constants.ReplaceParam.ProjectName, options[Constants.ReplaceParam.ProjectName]);
                if (options.ContainsKey(Constants.ReplaceParam.ENV))
                    targetName = targetName.Replace(Constants.ReplaceParam.ENV, options[Constants.ReplaceParam.ENV]);
            }
            String targetFile = Path.Combine(targetLocation, targetName);

            // 3、如果目标文件所在的文件夹不存在，需要创建文件夹，否则会抛异常；
            String targetFolder = Path.GetDirectoryName(targetFile);
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);
            // 4、文件复制；
            File.Copy(sourceFile, targetFile);

            return targetFile;
        }
    }

    /// <summary>
    /// 包含 ProjectItem列表的相关标签
    /// </summary>
    public class ItemGroup : ProjectItem
    {
        /// <summary>
        /// ProjectItem属性列表；
        /// </summary>
        public List<ProjectItem> Items
        {
            get { return this._items; }
        }
        private List<ProjectItem> _items = new List<ProjectItem>();

        public override bool Parse(XmlElement element)
        {
            base.Parse(element);
            // 解析当前Node下的ProjectItem元素；
            foreach (XmlElement subNode in element.ChildNodes)
            {
                if (Constants.TAG.ProjectItem.Equals(subNode.Name, StringComparison.OrdinalIgnoreCase))
                {
                    ProjectItem subData = new ProjectItem();
                    subData.Parse(subNode);
                    this.Items.Add(subData);
                }
            }

            return true;
        }

        public override string Copy(string sourceLocation, string targetLocation, Dictionary<string, string> options = null)
        {
            foreach (ProjectItem item in this.Items)
            {
                item.Copy(sourceLocation, targetLocation, options);
            }

            return base.Copy(sourceLocation, targetLocation, options);
        }
    }

    /// <summary>
    /// 对应VSTemplate中的Project标签
    /// </summary>
    public class Project : ItemGroup
    {
        public override bool Parse(XmlElement element)
        {
            if (!base.Parse(element))
                return false;

            this.Value = element.GetAttribute(Constants.Attribute.File);
            return true;
        }
    }
}
