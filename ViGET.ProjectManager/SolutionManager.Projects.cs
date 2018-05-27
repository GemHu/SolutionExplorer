using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace DothanTech.ViGET.Manager
{
    /// <summary>
    /// Projects相关代码；
    /// </summary>
    public partial class SolutionManager
    {
        public List<ProjectManager> Projects
        {
            get
            {
                return this.Children.Where(item =>
                {
                    return item is ProjectManager;
                }).Select(item => item as ProjectManager).ToList();
            }
        }

        #region Active Project

        /// <summary>
        /// 当前 Active 工程。
        /// </summary>
        public ProjectManager ActiveProject
        {
            get
            {
                ProjectManager target = null;
                this.LoopChild<ProjectManager>(item =>
                {
                    if (item.IsActive)
                    {
                        target = item;
                        return false;
                    }
                    return true;
                });

                return target;
            }
        }

        #endregion

        public ProjectManager OpenProject(String projectFile)
        {
            if (!File.Exists(projectFile))
                return null;
            if (!ProjectManager.IsProjectFile(projectFile))
                return null;

            ProjectManager project = new ProjectManager(projectFile);
            project.Load();

            this.AddChild(project);
            this.UpdateActiveCpu();
            this.SaveFile(this.FullName);

            return project;
        }

        public bool RemoveProject(ProjectManager project)
        {
            if (project == null)
                return false;
            if (!this.Children.Contains(project))
                return false;

            project.Close();
            this.RemoveChild(project);
            this.SaveFile(this.FullName);

            // 激活工程被关闭，尝试激活另外一个工程
            this.UpdateActiveCpu();

            // 发出工程被关闭的通知事件
            //this.RaiseProjectOpened(project, null);
            return true;
        }

        /// <summary>
        /// 根据工程中的文件名称，得到所属的工程。
        /// </summary>
        /// <param name="fileName">工程中文件的全路径名称，可以为工程中的任意一个文件。</param>
        /// <returns>文件所属的工程。</returns>
        public ProjectManager GetProject(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;
            if (this.Projects.Count <= 0)
                return null;

            if (fileName.IndexOf(Path.DirectorySeparatorChar) >= 0)
            {
                foreach (ProjectManager v in this.Projects)
                {
                    if (fileName.StartsWith(v.ProjectPath, StringComparison.OrdinalIgnoreCase))
                        return v;
                }
            }
            else
            {
                foreach (ProjectManager v in this.Projects)
                {
                    if (fileName.Equals(v.ProjectName, StringComparison.OrdinalIgnoreCase))
                        return v;
                }
            }

            return null;
        }

        #region CPU 相关方法

        public void LoopCPUs(Func<ViCPUInfo, bool> func)
        {
            this.LoopChild<ProjectManager>((project) =>
            {
                project.LoopChild<ViCPUInfo>((cpu) =>
                {
                    if (!func(cpu))
                        return false;
                    return true;
                });
                return true;
            });
        }

        public ViCPUInfo ActiveCPU
        {
            get
            {
                ViCPUInfo target = null;
                this.LoopCPUs(item =>
                {
                    if (item.IsActive)
                    {
                        target = item;
                        return false;
                    }
                    return true;
                });

                return target;
            }
        }

        /// <summary>
        /// 当项目发生变化的时候，需要实施的去检测并更新ActiveCPU；
        /// </summary>
        public void UpdateActiveCpu()
        {
            if (this.ActiveCPU != null)
                return;

            this.LoopCPUs((cpu) =>
            {
                cpu.IsActive = true;
                return false;
            });
        }

        #endregion
    }
}
