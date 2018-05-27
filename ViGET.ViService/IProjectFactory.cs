using DothanTech.ViGET.ViService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DothanTech.ViGET.ViService
{
    public interface IProjectFactory
    {
        /// <summary>
        /// 通过MEF方式获取git管理器；
        /// </summary>
        IGitManager GitManager { get; }

        /// <summary>
        /// 创建新项目(解决方案);
        /// </summary>
        void NewProject();

        /// <summary>
        /// 通过选择对话框的方式打开一个已有项目；
        /// </summary>
        void OpenProject(String projectFile);

        /// <summary>
        /// 通过向导方式，添加新项目；
        /// </summary>
        void AddNewProject();

        /// <summary>
        /// 添加一个已经存在的项目；
        /// </summary>
        void AddExistingProject(String projectFile);

        /// <summary>
        /// 关闭解决方案；
        /// </summary>
        void CloseSolution();

        /// <summary>
        /// 模板添加向导；
        /// </summary>
        ITemplateWizard TempWizard { get; }

        #region Build相关函数

        /// <summary>
        /// 对指定的项目文件执行 Build、Rebuild、Clean等操作；
        /// </summary>
        /// <param name="rebuild">编译状态，三态值：
        ///     true:表示执行Rebuild操作；
        ///     false:表示执行Build操作；
        ///     null:表示执行Clean操作；
        /// </param>
        void BuildProjects(List<String> projectFiles, bool? rebuild);

        /// <summary>
        /// 对整个Solution执行 Build、Rebuild、Clean等操作；
        /// </summary>
        /// <param name="rebuild">三态标识符：
        ///     true:表示执行Rebuild操作；
        ///     false:表示执行Build操作；
        ///     null:表示执行Clean操作；
        /// </param>
        void BuildSolution(bool? rebuild);

        /// <summary>
        /// 对ActiveProject执行 Build、Rebuild、Clean等操作；
        /// </summary>
        /// <param name="rebuild">编译状态，三态值：
        ///     true:表示执行Rebuild操作；
        ///     false:表示执行Build操作；
        ///     null:表示执行Clean操作；
        /// </param>
        void BuildActiveProject(bool? rebuild);

        /// <summary>
        /// 在编译结束后，调用该函数，告诉SolutionExplorer已经结束编译，避免影响其他操作；
        /// </summary>
        void BuildCompleted();

        /// <summary>
        /// 停止编译操作；
        /// </summary>
        void StopBuild();

        /// <summary>
        /// 开始编译事件；
        /// </summary>
        event StartBuildingEventHandler StartBuilding;

        /// <summary>
        /// 停止编译事件，注册该事件后，可以接收停止编译的请求；
        /// </summary>
        event EventHandler BuildStop;

        #endregion

        /// <summary>
        /// 设置打开文档时需要执行的回调函数，通常是通过双击相关文件节点来触发的；
        /// </summary>
        //Func<object, String, bool> OnDocumentOpeningAction { get; set; }
        /// <summary>
        /// 打开文件编辑器事件；
        /// </summary>
        event DocOpeningEventHandler DocOpening;

        /// <summary>
        /// 打开指定的文档编辑器；
        /// </summary>
        /// <param name="file"></param>
        void OpenDocument(String file);
    }

    public interface IViProject
    {
        String ProjectFile { get; }
    }

    public delegate void StartBuildingEventHandler(object sender, StartBuildingEventArgs e);

    public delegate void DocOpeningEventHandler(object sender, DocOpeningEventArgs e);

    public class StartBuildingEventArgs
    {
        public StartBuildingEventArgs(bool? rebuild)
        {
            this.Rebuild = rebuild;
        }

        public StartBuildingEventArgs(String projectFile, bool? rebuild)
            : this(rebuild)
        {
            this.Add(projectFile);
        }

        /// <summary>
        /// 编译状态：
        ///     true:表示Rebuild操作；
        ///     false:表示build操作；
        ///     null:表示Clean操作；
        /// </summary>
        public bool? Rebuild { get; set; }

        public List<String> ProjectFiles { get; set; }

        public void Add(String projectFile)
        {
            if (!File.Exists(projectFile))
                return;

            if (this.ProjectFiles == null)
                this.ProjectFiles = new List<string>();

            this.ProjectFiles.Add(projectFile);
        }
    }

    public class DocOpeningEventArgs : EventArgs
    {
        public DocOpeningEventArgs(String fullPath)
        {
            this.FullPath = fullPath;
        }

        public String FullPath { get; set; }
    }
}
