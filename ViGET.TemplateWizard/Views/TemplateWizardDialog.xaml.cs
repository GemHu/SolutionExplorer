using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Configuration;
using System.IO;
using DothanTech.Helpers;
using DothanTech.ViGET.ViCommand;

namespace DothanTech.ViGET.TemplateWizard
{
    internal enum WizardType
    {
        NewProject,         ///< 新建项目，未指定Solution相关信息；
        AddNewProject,      ///< 添加新项目到指定的Solution；
        NewFile,            ///< 添加ProjectItem；
        //AddNewFile,         ///< 添加指定的ProjectItem；
        None                ///< 无效的向导；
    }

    /// <summary>
    /// Interaction logic for ProjectWizard.xaml
    /// </summary>
    public partial class TemplateWizardDialog : Window
    {
        public TemplateWizardDialog()
        {
            InitializeComponent();
            this.DataContext = this;

            //
            this.ProjectTypes = new ViObservableCollection<ProjectTypeModel>();
            this.Location = this.DefaultLocation;
            this.TemplateManager = new TemplateManager();
        }

        public new static void ShowDialog(String location, String type, Action<String, String, String> callback, Func<String, String, bool> invalidCheck)
        {
            new TemplateWizardDialog().ShowWizardDialog(location, type, callback, invalidCheck);
        }

        /// <summary>
        /// 显示添加项目向导；
        /// </summary>
        /// <param name="location"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        public void ShowWizardDialog(String location, String type, Action<String, String, String> callback, Func<String, String, bool> invalidCheck)
        {
            this.Callback = callback;
            this.InvalidCheck = invalidCheck;

            if (Directory.Exists(location))
                this.Location = location;

            // 未指定Type，表示添加项目；
            if (String.IsNullOrEmpty(type))
            {
                Dictionary<String, List<VSTemplate>> dTemplates = TemplateFactory.sGetProjectTemplates();
                if (Directory.Exists(location))
                {
                    // 在现有Solution中添加新项目；
                    this.WizardType = ViGET.TemplateWizard.WizardType.AddNewProject;
                }
                else
                {
                    // 创建新项目；
                    this.WizardType = ViGET.TemplateWizard.WizardType.NewProject;
                    this.ShowLocation = true;
                    this.ShowSolutionName = true;
                }
                //
                foreach (String key in dTemplates.Keys)
                {
                    List<VSTemplate> list = dTemplates[key];
                    this.TemplateManager.AddChild(key + " Project", list);
                }
            }
            else
            {
                // 添加新项到制定位置
                this.WizardType = ViGET.TemplateWizard.WizardType.NewFile;
                List<VSTemplate> tempList = TemplateFactory.sGetProjectItemTemplates(type);

                List<VSTemplate> templates = TemplateFactory.sGetProjectItemTemplates(type);
                this.TemplateManager.AddChild(type + " Items", templates);
            }
            
            this.ShowDialog();
        }

        #region TemplateList Property

        public static readonly DependencyProperty TemplateListProperty =
            DependencyProperty.Register("TemplateList", typeof(ViObservableCollection<TemplateItemModel>), typeof(TemplateWizardDialog),
                                        new PropertyMetadata(null));

        public ViObservableCollection<TemplateItemModel> TemplateList
        {
            get { return (ViObservableCollection<TemplateItemModel>)GetValue(TemplateListProperty); }
            set { SetValue(TemplateListProperty, value); }
        }

        #endregion


        #region 需要运行的想到类型；

        private WizardType WizardType
        {
            get { return _wizardType; }
            set
            {
                this._wizardType = value;
                switch (value)
                {
                    case WizardType.NewProject:
                        this.ShowLocation = true;
                        this.ShowSolutionName = true;
                        break;
                    case WizardType.AddNewProject:
                        break;
                    default:
                    case WizardType.NewFile:
                        this.OKText = "Add";
                        break;
                }
            }
        }
        private WizardType _wizardType;

        #endregion

        #region Callback Property

        /// <summary>
        /// 新建 ProjectItem完毕后的回调函数；
        /// </summary>
        /// <param>参数1：添加后的文件全路径；</param>
        /// <param>参数2：添加的文件名称；</param>
        /// <param>参数3：添加的文件模板类型；</param>
        public Action<String, String, String> Callback;
        /// <summary>
        /// 文件名非法检验检测回调函数；
        /// <param>参数1：待校验文件名；</param>
        /// <param>参数2：待校验文件类型；</param>
        /// <param>参数3：返回值，成功与否；true：表示给定的文件名合法可用，false：表示给的的文件名不可用；</param>
        /// </summary>
        public Func<String, String, bool> InvalidCheck;

        #endregion

        #region Property

        #region SolutionName Property

        public static readonly DependencyProperty SolutionNameProperty =
            DependencyProperty.Register("SolutionName", typeof(String), typeof(TemplateWizardDialog),
                                        new PropertyMetadata(String.Empty));

        public String SolutionName
        {
            get { return (String)GetValue(SolutionNameProperty); }
            set { SetValue(SolutionNameProperty, value); }
        }

        #endregion

        #region ProjectName Property

        public static readonly DependencyProperty ProjectNameProperty =
            DependencyProperty.Register("ProjectName", typeof(String), typeof(TemplateWizardDialog),
                                        new PropertyMetadata(String.Empty));

        public String ProjectName
        {
            get { return (String)GetValue(ProjectNameProperty); }
            set
            {
                SetValue(ProjectNameProperty, value);
                // 保持SolutionName与ProjectName一致；
                if (!String.IsNullOrEmpty(value))
                {
                    this.SolutionName = value;
                }
            }
        }

        #endregion

        #region OKText Property 提交按钮显示的内容

        public static readonly DependencyProperty OKTextProperty =
            DependencyProperty.Register("OKText", typeof(String), typeof(TemplateWizardDialog),
                                        new PropertyMetadata("OK"));

        public String OKText
        {
            get { return (String)GetValue(OKTextProperty); }
            set { SetValue(OKTextProperty, value); }
        }

        #endregion

        #region Location Property

        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register("Location", typeof(String), typeof(TemplateWizardDialog),
                                        new PropertyMetadata(String.Empty));

        public String Location
        {
            get { return (String)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }

        /// <summary>
        /// 获取或者修改默认项目位置；
        /// </summary>
        public String DefaultLocation
        {
            get
            {
                String defaultLocation = DzAppDataConfig.sGetValue(DzAppDataConfig.LAST_LOCATION);
                if (String.IsNullOrEmpty(defaultLocation))
                    defaultLocation = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ViGET\\Projects");

                return defaultLocation;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;
                //if (value.Equals(this.DefaultLocation, StringComparison.OrdinalIgnoreCase))
                //    return;
                if (!Directory.Exists(value))
                    return;

                DzAppDataConfig.sSetValue(DzAppDataConfig.LAST_LOCATION, value);
            }
        }

        #endregion

        #region ShowSolutionName Property 用于控制是否显示Solution输入对话框

        public static readonly DependencyProperty ShowSolutionNameProperty =
            DependencyProperty.Register("ShowSolutionName", typeof(bool), typeof(TemplateWizardDialog),
                                        new PropertyMetadata(false));

        public bool ShowSolutionName
        {
            get { return (bool)GetValue(ShowSolutionNameProperty); }
            set { SetValue(ShowSolutionNameProperty, value); }
        }

        #endregion

        #region ShowLocation Property 用于控制是否显示Location输入对话框

        public static readonly DependencyProperty ShowLocationProperty =
            DependencyProperty.Register("ShowLocation", typeof(bool), typeof(TemplateWizardDialog),
                                        new PropertyMetadata(false));

        public bool ShowLocation
        {
            get { return (bool)GetValue(ShowLocationProperty); }
            set { SetValue(ShowLocationProperty, value); }
        }

        #endregion


        #region CurrTemplate Property

        public static readonly DependencyProperty CurrTemplateProperty =
            DependencyProperty.Register("CurrTemplate", typeof(TemplateItemModel), typeof(TemplateWizardDialog),
                                        new PropertyMetadata(null));

        public TemplateItemModel CurrTemplate
        {
            get { return (TemplateItemModel)GetValue(CurrTemplateProperty); }
            set
            {
                if (value == GetValue(CurrTemplateProperty))
                    return;

                SetValue(CurrTemplateProperty, value);
                // 获取默认项目名称
                this.ProjectName = this.GetValidName(value != null ? value.Template : null);
            }
        }

        private String GetValidName(VSTemplate template)
        {
            String name = template == null ? String.Empty : template.TemplateData.DefaultName;
            if (String.IsNullOrEmpty(name))
                return name;

            String defaultExt = System.IO.Path.GetExtension(name);
            String nameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(name);

            String folderName = System.IO.Path.Combine(this.Location, nameWithoutExt);
            String fileName = folderName + defaultExt;
            
            for (int i = 1; true; i++)
            {
                String path = folderName + i;
                fileName = path + defaultExt;
                if (this.InvalidCheck == null){
                    if (!Directory.Exists(path) && !File.Exists(fileName))
                        return nameWithoutExt + i + defaultExt;
                } else {
                    if (this.InvalidCheck(nameWithoutExt + i, template.TemplateData.Type))
                        return nameWithoutExt + i + defaultExt;
                }
            }
        }

        #endregion

        #region TemplateManager

        public ViObservableCollection<ProjectTypeModel> ProjectTypes { get; protected set; }

        #endregion

        #region TemplateManager Property

        public static readonly DependencyProperty TemplateManagerProperty =
            DependencyProperty.Register("TemplateManager", typeof(TemplateManager), typeof(TemplateWizardDialog),
                                        new PropertyMetadata(null));

        public TemplateManager TemplateManager
        {
            get { return (TemplateManager)GetValue(TemplateManagerProperty); }
            set
            {
                TemplateManager oldValue = (TemplateManager)GetValue(TemplateManagerProperty);
                if (oldValue == value)
                    return;

                SetValue(TemplateManagerProperty, value);
                this.ProjectTypes.Add(value);
            }
        }

        #endregion

        #endregion

        #region Command

        private RelayCommand _browserCommand;
        public RelayCommand BrowserCommand
        {
            get
            {
                if (this._browserCommand == null)
                {
                    this._browserCommand = new RelayCommand(() =>
                    {
                        FolderBrowserDialog dialog = new FolderBrowserDialog();
                        dialog.SelectedPath = this.Location;
                        DialogResult result = dialog.ShowDialog();
                        if (result == System.Windows.Forms.DialogResult.OK)
                        {
                            this.Location = dialog.SelectedPath;
                        }
                    });
                }

                return this._browserCommand;
            }
        }

        private RelayCommand _commitCommand;
        public RelayCommand CommitCommand
        {
            get
            {
                if (this._commitCommand == null)
                {
                    this._commitCommand = new RelayCommand(() =>
                    {
                        this.Close();

                        TemplateItemModel template = this.CurrTemplate;
                        switch (this.WizardType)
                        {
                            case WizardType.NewProject:
                                // 1、更新默认项目位置；
                                this.DefaultLocation = this.Location;
                                this.NewProject(this.Location, this.SolutionName, this.ProjectName, template.Template);
                                break;
                            case WizardType.AddNewProject:
                                this.AddNewProject(this.Location, this.ProjectName, template.Template);
                                break;
                            case WizardType.NewFile:
                                this.AddNewFile(this.Location, this.ProjectName, template.Template);
                                break;
                        }
                    });
                }

                return this._commitCommand;
            }
        }

        private void NewProject(String solutionLocation, String solutionName, String projectName, VSTemplate template)
        {
            if (template == null || this.Callback == null)
                return;

            //
            String solutionFolder = System.IO.Path.Combine(this.Location, this.SolutionName);
            if (Directory.Exists(solutionFolder))
            {
                // Solution文件夹已经存在；
                String message = String.Format(Constants.Message.ProjectIsExist, this.Location);
                System.Windows.MessageBox.Show(message, Constants.Message.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Directory.CreateDirectory(solutionFolder);
            //
            String projectFolder = System.IO.Path.Combine(solutionFolder, ProjectName);
            String projectFile = template.Copy(projectFolder, projectName);
            //this(projectFile, projectName, template.TemplateData.Type, solutionName, solutionFolder);
            this.Callback(projectFile, projectName, template.TemplateData.Type);
        }

        private void AddNewProject(String location, String projectName, VSTemplate template)
        {
            if (template == null || this.Callback == null)
                return;

            String projectFolder = System.IO.Path.Combine(location, ProjectName);
            //
            if (Directory.Exists(projectFolder))
            {
                String message = String.Format(Constants.Message.ProjectIsExist, this.ProjectName);
                System.Windows.MessageBox.Show(message, Constants.Message.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // 3、复制相关模板文件；
            String projectFile = template.Copy(projectFolder, this.ProjectName);
            // 3、调用回调函数；
            this.Callback(projectFile, projectName, template.TemplateData.Type);
        }

        private void AddNewFile(String location, String fileName, VSTemplate template)
        {
            if (template == null || this.Callback == null)
                return;
            if (String.IsNullOrEmpty(location) || String.IsNullOrEmpty(fileName))
                return;

            fileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
            String file = template.GetProjectFile(location, fileName);
            if (File.Exists(file) || (this.InvalidCheck != null && !this.InvalidCheck(fileName, template.TemplateData.Type)))
            {
                String msg = String.Format(Constants.Message.FileIsExist, this.ProjectName);
                System.Windows.MessageBox.Show(msg, Constants.Message.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // 1、复制相关文件；
            template.Copy(this.Location, fileName);
            // 2、
            this.Callback(file, fileName, template.TemplateData.Type);
        }

        private RelayCommand _cancelCommand;
        public RelayCommand CancelCommand
        {
            get
            {
                if (this._cancelCommand == null)
                {
                    this._cancelCommand = new RelayCommand(() =>
                    {
                        this.Close();
                    });
                }

                return this._cancelCommand;
            }
        }

        private RelayCommand _projectTypeChangedCommand;
        public RelayCommand ProjectTypeChangedCommand
        {
            get
            {
                if (this._projectTypeChangedCommand == null)
                {
                    this._projectTypeChangedCommand = new RelayCommand(() =>
                    {

                    });
                }

                return this._projectTypeChangedCommand;
            }
        }

        #endregion

        private void OnSelectedProjectTypeChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ProjectTypeModel selectedItem = this.treeProjectType.SelectedItem as ProjectTypeModel;
            if (selectedItem != null)
            {
                this.TemplateList = selectedItem.TemplateList;
                if (this.TemplateList.Count > 0)
                    this.TemplateList[0].IsSelected = true;
            }
        }

        private void OnSelectedTemplateChanged(object sender, SelectionChangedEventArgs e)
        {
            this.CurrTemplate = this.listTemplateList.SelectedItem as TemplateItemModel;
        }
    }
}
