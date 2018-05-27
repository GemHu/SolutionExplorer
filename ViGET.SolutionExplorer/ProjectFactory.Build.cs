using DothanTech.ViGET.Manager;
using DothanTech.ViGET.ViService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DothanTech.ViGET.SolutionExplorer
{
    public partial class ProjectFactory
    {
        /// <summary>
        /// 编译工作处理事件；
        /// </summary>
        public event StartBuildingEventHandler StartBuilding;

        public event EventHandler BuildStop;

        public bool IsBuilding { get; set; }

        public void BuildSolution(bool? rebuild)
        {
            if (this.TheSolution == null || this.TheSolution.Projects.Count <= 0)
                return;

            List<String> items = this.TheSolution.Projects.Select(proj => proj.ProjectFile).ToList();
            this.BuildProjects(items, rebuild);
        }

        public void BuildProjects(List<String> projectFiles, bool? rebuild)
        {
            if (this.StartBuilding == null)
                return;
            if (projectFiles == null || projectFiles.Count <= 0)
                return;

            // 开始编译
            this.IsBuilding = true;
            StartBuildingEventArgs args = new StartBuildingEventArgs(rebuild);
            foreach (String item in projectFiles)
            {
                args.Add(item);
            }
            this.StartBuilding.Invoke(this, args);
        }

        public void BuildActiveProject(bool? rebuild)
        {
            ProjectManager proj = this.TheSolution == null ? null : this.TheSolution.ActiveProject;
            if (proj == null)
                return;

            this.BuildProjects(new List<string>() { proj.ProjectFile }, rebuild);
        }

        public void BuildCompleted()
        {
            this.IsBuilding = false;
        }

        public void StopBuild()
        {
            if (this.BuildStop == null)
                return;

            this.BuildStop.Invoke(this, EventArgs.Empty);
        }
    }
}
