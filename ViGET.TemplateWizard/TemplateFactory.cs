using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DothanTech.ViGET.TemplateWizard
{
    /// <summary>
    /// 向导，用于添加或者新建项目，或相关项的向导；
    /// </summary>
    public class TemplateFactory
    {
        private List<VSTemplate> projectTemplates;
        private List<VSTemplate> projectItemTemplates;

        #region Properties

        public String ProjectTemplatePath
        {
            get
            {
                return String.Format("{0}Templates/Projects/", AppDomain.CurrentDomain.BaseDirectory);
            }
        }

        public String ProjectItemTemplatePath
        {
            get
            {
                return String.Format("{0}Templates/ProjectItems/", AppDomain.CurrentDomain.BaseDirectory);
            }
        }

        public static Dictionary<String, List<VSTemplate>> sGetProjectTemplates()
        {
            Dictionary<String, List<VSTemplate>> dTemplates = new Dictionary<string, List<VSTemplate>>();
            List<VSTemplate> tempList = new TemplateFactory().GetProjectTemplates();
            foreach (VSTemplate item in tempList)
            {
                if (item.TemplateData == null)
                    continue;

                String key = item.TemplateData.Type;
                List<VSTemplate> childList;
                if (dTemplates.ContainsKey(key))
                {
                    childList = dTemplates[key];
                }
                else
                {
                    childList = new List<VSTemplate>();
                    dTemplates[key] = childList;
                }
                childList.Add(item);
            }

            return dTemplates;
        }

        /// <summary>
        /// 获取所有的项目模板；
        /// </summary>
        /// <returns></returns>
        public List<VSTemplate> GetProjectTemplates()
        {
            if (this.projectTemplates == null)
            {
                this.projectTemplates = new List<VSTemplate>();
                this.LoadTemplates(this.ProjectTemplatePath, ref this.projectTemplates);
            }

            return this.projectTemplates;
        }

        public static List<VSTemplate> sGetProjectItemTemplates(String ownerType)
        {
            return new TemplateFactory().GetProjectItemTemplates(ownerType);
        }

        /// <summary>
        /// 获取指定项目类型的所有项；
        /// 如果ProjectType为null，或者String.Empty,则返回所有项；
        /// </summary>
        /// <param name="projectType"></param>
        /// <returns></returns>
        public List<VSTemplate> GetProjectItemTemplates(String projectType = null)
        {
            if (this.projectTemplates == null)
            {
                this.projectItemTemplates = new List<VSTemplate>();
                this.LoadTemplates(this.ProjectItemTemplatePath, ref this.projectItemTemplates);
            }

            if (String.IsNullOrEmpty(projectType))
                return this.projectItemTemplates;

            List<VSTemplate> templates = new List<VSTemplate>();
            foreach (VSTemplate item in this.projectItemTemplates)
            {
                if (String.IsNullOrEmpty(projectType))
                {
                    templates.Add(item);
                }
                else
                {
                    foreach (String type in item.TemplateData.OwnerTypes)
                    {
                        if (projectType.Equals(type, StringComparison.OrdinalIgnoreCase))
                        {
                            templates.Add(item);
                            break;
                        }
                    }
                }
            }

            return templates;
        }

        #endregion

        private void LoadTemplates(String path, ref List<VSTemplate> templates)
        {
            if (File.Exists(path))
            {
                // 如果后缀名一致，则直接解析文件；
                if (path.EndsWith(Constants.Extension.Template, StringComparison.OrdinalIgnoreCase))
                {

                    VSTemplate template = VSTemplate.Parse(path);
                    if (template != null)
                    {
                        if (templates == null)
                            templates = new List<VSTemplate>();
                        templates.Add(template);
                    }
                }

                return;
            }

            if (Directory.Exists(path))
            {
                IEnumerable<String> files = Directory.EnumerateFiles(path);
                foreach (String file in files)
                {
                    this.LoadTemplates(file, ref templates);
                }
                IEnumerable<String> folders = Directory.EnumerateDirectories(path);
                foreach (String folder in folders)
                {
                    this.LoadTemplates(folder, ref templates);
                }
            }
        }
    }
}
