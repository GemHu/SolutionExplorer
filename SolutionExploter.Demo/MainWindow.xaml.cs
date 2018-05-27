using DothanTech.ViGET.GitManager;
using DothanTech.ViGET.SolutionExplorer;
using DothanTech.ViGET.TemplateWizard;
using DothanTech.ViGET.ViService;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SolutionExploter.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IPartImportsSatisfiedNotification
    {
        [Import(typeof(UcSolutionExplorer))]
        public UserControl SolutionExplorer { get; set; }

        [Import]
        public IProjectFactory ProjectFactory { get; set; }

        #region Life Cycle
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            this.Compose();
            this.Closing += this.OnMainWindowClosing;
        }

        private void OnMainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("确定要退出主程序吗？", "Closing", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                this.ProjectFactory.CloseSolution();
            }
            else
            {
                e.Cancel = true;
            }
        }

        public void OnImportsSatisfied()
        {
            this.ProjectFactory.DocOpening += this.onDocumentOpening;
            this.ProjectFactory.StartBuilding += this.OnStartBuilding;
            this.ProjectFactory.BuildStop += this.OnBuildStop;
        }

        /// <summary>
        /// 组装所有的MEF模块；
        /// </summary>
        private void Compose()
        {
            var catalog = new AggregateCatalog();
            // MEF组装方法一、通过文件夹的方式加载组件
            // 缺点：加载速度慢，优点：操作简单
            //var dir = new DirectoryCatalog(".");
            //catalog.Catalogs.Add(dir);

            // MEF组装方法二、只加载需要的程序集；
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(TemplateWizard).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ProjectFactory).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ViGitManager).Assembly));
            
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }

        #endregion

        #region 通过双击SolutionExplorer来打开文件编辑器

        private void onDocumentOpening(object sender, DocOpeningEventArgs e)
        {
            MessageBox.Show(String.Format("FilePath:{0}", e.FullPath), "Open Document", MessageBoxButton.OK);
        }

        #endregion

        #region 编译相关测试程序

        private bool stopBuild;

        private void OnStartBuilding(object sender, StartBuildingEventArgs e)
        {
            if (e.ProjectFiles == null)
                return;

            String buildState = e.Rebuild == false ? "Build" : (e.Rebuild == true ? "Rebuild" : "Clean");
            String msg = String.Join("\n", e.ProjectFiles);
            MessageBox.Show(String.Format("BuildState : {0};\nBuildProjects:\n{1}", buildState, msg), "Building", MessageBoxButton.OK);

            // 模拟编译操作
            ThreadPool.QueueUserWorkItem(this.SimulateBuildWork, e.ProjectFiles);
        }

        private void OnBuildStop(object sender, EventArgs e)
        {
            this.stopBuild = true;
            Trace.WriteLine("Build Stop Stop Stop Stop Stop Stop!");
        }

        /// <summary>
        /// 模拟编译工作
        /// </summary>
        /// <param name="args"></param>
        private void SimulateBuildWork(object args)
        {
            Trace.WriteLine("\n\n\n\n\n");
            // 重置 buildStop状态；
            stopBuild = false;
            List<String> projects = args as List<String>;

            foreach (String item in projects)
            {
                if (stopBuild)
                    break;
                // Start build project
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Trace.WriteLine(String.Format("Start to build {0}", item));
                }));
                
                // 假设解析出10个文件，每个文件执行1s时间
                for (int i = 0; i < 10; i++)
                {
                    if (stopBuild)
                        break;

                    // 模拟文件编译操作；
                    Thread.Sleep(1 * 1000);
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Trace.WriteLine(String.Format("Build File{0} of \"{1}\" success!", i, item));
                    }));
                }

                // End build project
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Trace.WriteLine(String.Format("End to build {0}", item));
                }));
            }

            // 编译完成
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Trace.WriteLine(String.Format("Build Complete"));
                this.ProjectFactory.BuildCompleted();
            }));
        }

        #endregion

        #region Project相关命令

        #region New Project

        public ICommand NewProjectCmd
        {
            get
            {
                if (this._newProjectCmd == null)
                {
                    this._newProjectCmd = new MyCommand(() =>
                    {
                        this.ProjectFactory.NewProject();
                    }, null);
                }

                return this._newProjectCmd;
            }
        }
        private ICommand _newProjectCmd;

        #endregion

        #region Close Solution

        public ICommand CloseSolutionCmd
        {
            get
            {
                if (this._closeSolutionCmd == null)
                {
                    this._closeSolutionCmd = new MyCommand(() =>
                    {
                        this.ProjectFactory.CloseSolution();
                    }, () =>
                    {
                        return true;
                    });
                }
                return _closeSolutionCmd;
            }
        }
        private ICommand _closeSolutionCmd;

        #endregion

        #region Open Project

        public ICommand OpenProjectCmd
        {
            get
            {
                if (this._openProjectCmd == null)
                {
                    this._openProjectCmd = new MyCommand(() =>
                    {
                        this.ProjectFactory.OpenProject(null);
                    }, () =>
                    {
                        return true;
                    });
                }
                return _openProjectCmd;
            }
        }
        private ICommand _openProjectCmd;

        #endregion

        #region Add New Project

        public ICommand AddNewProjectCmd
        {
            get
            {
                if (this._addNewProjectCmd == null)
                {
                    this._addNewProjectCmd = new MyCommand(() =>
                    {
                        this.ProjectFactory.AddNewProject();
                    }, () => { return true; });
                }
                return _addNewProjectCmd;
            }
        }
        private ICommand _addNewProjectCmd;

        #endregion

        #region Add Existing Project

        public ICommand AddExistingProjectCmd
        {
            get
            {
                if (this._addExistingProjectCmd == null)
                {
                    this._addExistingProjectCmd = new MyCommand(() =>
                    {
                        this.ProjectFactory.AddExistingProject(null);
                    }, () =>
                    {
                        return true;
                    });
                }
                return _addExistingProjectCmd;
            }
        }
        private ICommand _addExistingProjectCmd;

        #endregion

        #endregion

        #region Build Command

        public ICommand BuildSolutionCmd
        {
            get
            {
                if (this._buildSolutionCmd == null)
                {
                    this._buildSolutionCmd = new MyCommand(() =>
                    {
                        this.ProjectFactory.BuildSolution(false);
                    }, () =>
                    {
                        return true;
                    });
                }
                return _buildSolutionCmd;
            }
        }
        private ICommand _buildSolutionCmd;

        public ICommand RebuildSolutionCmd
        {
            get
            {
                if (this._rebuildSolutionCmd == null)
                {
                    this._rebuildSolutionCmd = new MyCommand(() =>
                    {
                        this.ProjectFactory.BuildSolution(true);
                    }, () =>
                    {
                        return true;
                    });
                }
                return _rebuildSolutionCmd;
            }
        }
        private ICommand _rebuildSolutionCmd;

        public ICommand BuildActiveProjectCmd
        {
            get
            {
                if (this._buildActiveProjectCmd == null)
                {
                    this._buildActiveProjectCmd = new MyCommand(() =>
                    {
                        this.ProjectFactory.BuildActiveProject(false);
                    }, () =>
                    {
                        return true;
                    });
                }
                return _buildActiveProjectCmd;
            }
        }
        private ICommand _buildActiveProjectCmd;

        public ICommand RebuildActiveProjectCmd
        {
            get
            {
                if (this._rebuildActiveProjectCmd == null)
                {
                    this._rebuildActiveProjectCmd = new MyCommand(() =>
                    {
                        this.ProjectFactory.BuildActiveProject(true);
                    }, () =>
                    {
                        return true;
                    });
                }
                return _rebuildActiveProjectCmd;
            }
        }
        private ICommand _rebuildActiveProjectCmd;

        public ICommand BuildStopCmd
        {
            get
            {
                if (this._buildStop == null)
                {
                    this._buildStop = new MyCommand(() =>
                    {
                        this.OnBuildStop(null, null);
                    }, () => true);
                }
                return this._buildStop;
            }
        }
        private ICommand _buildStop;

        #endregion

        public class MyCommand : ICommand
        {
            private Action _action;
            private Func<bool> _func;

            public MyCommand(Action action, Func<bool> func)
            {
                this._action = action;
                this._func = func;
            }

            public bool CanExecute(object parameter)
            {
                if (this._func != null)
                    return this._func();

                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                this._action();
            }
        }

    }
}
