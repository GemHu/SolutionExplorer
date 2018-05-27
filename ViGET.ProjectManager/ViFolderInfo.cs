using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Diagnostics;
using DothanTech.Helpers;
using DothanTech.ViGET.ViCommand;
using DothanTech.ViGET.ViService;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;

namespace DothanTech.ViGET.Manager
{
    public interface IViFolderInfo : IViFileInfo
    {

    }

    /// <summary>
    /// TreeNode相关基类，从该类集成过来的子类通常会在SolutionExplorer中显示对应的节点，并且该节点下面还会有其他的子节点；
    /// </summary>
    public class ViFolderInfo : ViFileInfo, IViFolderInfo
    {
        #region life cycle

        public ViFolderInfo(String file)
            : base(file)
        {
            this.Name = Path.GetFileNameWithoutExtension(file);
            this.Children = new ViObservableCollection<ViFileNode>();
            this.PropertyChanged += this.OnChildPropertyChanged;
        }

        #endregion

        #region File & Path

        /// <summary>
        /// FolderInfo对应Solution中的一个容器节点；
        /// FolderPath这对应于一个本地文件夹路径；
        /// </summary>
        public String FolderPath
        {
            get
            {
                if (File.Exists(this.FullName))
                    return Path.GetDirectoryName(this.FullName);

                ViFolderInfo parent = this.Parent as ViFolderInfo;
                if (parent != null)
                    return Path.Combine(parent.FolderPath, this.Name);

                return this.Name;
            }
        }

        /// <summary>
        /// 获取给定的文件相对于当前文件夹的相对路径；
        /// </summary>
        /// <param name="file">目标文件</param>
        /// <returns>相对路径</returns>
        public String GetRelativePath(String file)
        {
            if (String.IsNullOrEmpty(file) || String.IsNullOrEmpty(this.FolderPath))
                return file;
            if (!file.StartsWith(this.FolderPath))
                return file;

            return file.Substring(this.FolderPath.Length).Trim(Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// 获取全路径；
        /// </summary>
        public String GetFullPath(String relativePath)
        {
            if (String.IsNullOrEmpty(relativePath))
                return relativePath;

            relativePath = relativePath.Trim(Path.DirectorySeparatorChar);
            return Path.Combine(this.FolderPath, relativePath);
        }
        
        /// <summary>
        /// 文件名非法校验；
        /// </summary>
        /// <param name="fileName">待检测文件名；</param>
        /// <returns>true：表示文件名可用，false：表示文件名无效；</returns>
        public override bool FileNameCanUsed(string fileName, string type, Collection<ViFileInfo> exepts = null)
        {
            return this.FileNameCanUsed(fileName, type, true, exepts);
        }
        /// <summary>
        /// 文件名非法校验；
        /// </summary>
        /// <param name="fileName">待检测文件名；</param>
        /// <returns>true：表示文件名可用，false：表示文件名无效；</returns>
        public bool FileNameCanUsed(string fileName, string type, bool deepCheck, Collection<ViFileInfo> exepts = null)
        {
            if (!base.FileNameCanUsed(fileName, type, exepts))
                return false;

            if (deepCheck)
            {
                foreach (ViFileInfo item in this.Children)
                {
                    if (!item.FileNameCanUsed(fileName, type, exepts))
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region Children

        public virtual ViFileInfo CreateFile(String fileType, String filePath, String fileName)
        {
            return FileCreateFactory.CreateFile(filePath, fileType);
        }

        /// <summary>
        /// 根据绝对路径查找目标文件；
        /// </summary>
        /// <param name="fullPath">目标文件绝对路径；</param>
        /// <returns></returns>
        public override ViFileInfo GetFileInfoByFullPath(string fullPath)
        {
            if (String.IsNullOrEmpty(fullPath))
                return null;
            if (fullPath.TrimEnd('\\', '/').Equals(this.FolderPath.TrimEnd('\\', '/'), StringComparison.OrdinalIgnoreCase))
                return this;

            foreach (ViFileInfo item in this.Children)
            {
                ViFileInfo target = item.GetFileInfoByFullPath(fullPath);
                if (target != null)
                    return target;
            }

            return null;
        }

        #endregion

        #region 文件的复制、剪切、粘贴、选择等操作

        /// <summary>
        /// 判断给定的文件是否是当前文件夹所支持的文件；
        /// </summary>
        /// <param name="file">目标文件</param>
        /// <returns>支持与否</returns>
        public virtual bool IsSupportedFile(ViFileInfo file)
        {
            if (file is ViCFCFile)
                return true;

            return false;
        }

        public override bool CanPaste
        {
            get
            {
                List<ViFileInfo> list = this.TheSolution.ClipBoardFiles;
                foreach (ViFileInfo item in list)
                {
                    // 判断要粘贴的文件是否是当前文件夹所支持的文件；
                    if (!this.IsSupportedFile(item))
                        return false;
                }

                return true;
            }
        }

        public override void DoPaste()
        {
            foreach (ViFileInfo item in this.TheSolution.ClipBoardFiles)
            {
                if (item.GetParent() != this)
                {
                    // 只用系统中的文件才可以执行剪切操作
                    if (this.AddExistingItem(item.FullName, item.Type))
                    {
                        if (item.IsCutting)
                        {
                            item.DoDelete(false);
                        }
                    }
                }
                else
                {
                    // 从当前文件夹中剪切文件，然后粘贴到当前文件夹中，没有任何意义；
                    if (item.IsCutting)
                    {
                        item.IsCutting = false;
                        return;
                    }

                    // 目标文件在当前文件夹下，则文件名进行递增处理
                    String newFile = FileName.IncreateFileName(item.FullName, (file, name) => 
                    {
                        return this.CurrProject.FileNameCanUsed(name, item.Type);
                    }, "{0} - Copy ({1}){2}");
                    if (FileName.CopyFile(item.FullName, newFile))
                        this.AddExistingItem(newFile, item.Type);
                }
            }
        }

        public void ModeFileInfo(ViFileInfo file)
        {
            if (file == null)
                return;
            // 如果文件所属项目为空，则文件处于游离状态，暂时不做处理
            if (this.CurrProject == null || file.CurrProject == null)
                return;

            if (this.CurrProject == file.CurrProject)
            {
                // 同一个项目下的文件
                // 如果目标文件已经打开，则需要关闭已打开的文件；
                //IViDocManager docManager = file.TheFactory.DocManager;
                //if (docManager != null && docManager.IsFileOpened(file.FullName))
                //{
                //    if (!docManager.CloseDocument(file.FullName))
                //        return;
                //}
                if (this.AddExistingItem(file.FullName, file.Type))
                {
                    file.DoDelete(false);
                }
            }
            else
            {
                // 不同项目下的文件，执行复制操作；
                this.AddExistingItem(file.FullName, file.Type);
            }
        }

        #endregion

        #region Event

        private void OnChildPropertyChanged(object sender, ViDataChangedEventArgs e)
        {
            if ((e.ChangeType != ViChangeType.None) &&
                (e.ChangeType & ViChangeType.NotCauseCache) == ViChangeType.None)
            {
                this.IsDirty = true;
            }
        }

        #endregion

        #region Command Handle

        public override void CanExecuteCommand(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (e.Command == ViCommands.OpenLocalFolder ||
                e.Command == ViCommands.AddNewItem)
            {
                e.CanExecute = true;
                return;
            }
            else if (e.Command == ViCommands.Build ||
                e.Command == ViCommands.Rebuild ||
                e.Command == ViCommands.Clean)
            {
                //e.CanExecute = this.CanBuild();
            }

            base.CanExecuteCommand(sender, e);
        }

        public override void ExecutedCommand(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.Command == ViCommands.OpenLocalFolder)
            {
                if (Directory.Exists(this.FolderPath))
                    System.Diagnostics.Process.Start(this.FolderPath);
                return;
            }
            else if (e.Command == ViCommands.AddNewItem)
            {
                this.AddNewItem();
                return;
            }
            else if (e.Command == ViCommands.AddExistingItem)
            {
                this.AddExistingItem();
                return;
            }
            else if (e.Command == ViCommands.Build)
            {
                //this.Build(false);
            }
            else if (e.Command == ViCommands.Rebuild)
            {
                //this.Build(true);
            }
            else if (e.Command == ViCommands.Clean)
            {
                //this.BuildClean();
            }

            base.ExecutedCommand(sender, e);
        }

        #endregion

        /// <summary>
        /// 添加新项；
        /// </summary>
        protected void AddNewItem()
        {
            if (this.TheFactory == null || this.TheFactory.TempWizard == null)
                return;
            if (!Directory.Exists(this.FolderPath))
                Directory.CreateDirectory(this.FolderPath);

            this.TheFactory.TempWizard.RunCreateFileWizard(this.FolderPath, this.Type, (filePath, name, type) =>
            {
                ViFileInfo child = this.CreateFile(type, filePath, name);
                this.AddChild(child);
                // 如果child为一个目录，则需要创建对应的本地文件夹
                if (child is ViFolderInfo)
                {
                    ViFolderInfo folder = child as ViFolderInfo;
                    if (!Directory.Exists(folder.FolderPath))
                        Directory.CreateDirectory(folder.FolderPath);
                }
            }, (fileName, type) => {
                return this.CurrProject.FileNameCanUsed(fileName, type);
            });
        }

        /// <summary>
        /// 添加本地已存在的项；
        /// </summary>
        protected void AddExistingItem()
        {
            // 打开文件选择对话框；
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = this.GetDefaultExt();
            dialog.Filter = this.GetFileFilter();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == true)
            {
                this.AddExistingItem(dialog.FileNames);
            }
        }

        protected void AddExistingItem(String[] files)
        {
            if (files == null)
                return;

            foreach (var item in files)
            {
                if (!this.AddExistingItem(item, String.Empty))
                    return;
            }
        }

        protected bool AddExistingItem(String file, String type)
        {
            if (!File.Exists(file))
            {
                MessageBox.Show(String.Format("File {0} is not exist!", file), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            // 如果当前文件夹不存在，进行文件复制的时候就会出问题；
            if (!Directory.Exists(this.FolderPath))
                Directory.CreateDirectory(this.FolderPath);

            if (String.IsNullOrEmpty(type))
            {
                PcsFileInfo fileInfo = PcsFileInfo.GetPcsFileInfo(file);
                type = fileInfo == null ? String.Empty : fileInfo.type;
            }

            // 如果新旧文件为同一个文件，并且目标文件已经存在，则退出；
            String fileName = Path.GetFileName(file);
            String newFile = Path.Combine(this.FolderPath, fileName);
            // 如果项目中不存在该文件名
            if (!this.FileNameCanUsed(newFile, type, false))
            {
                // 文件名冲突；
                MessageBox.Show(String.Format("FileName {0} is existing in project!", fileName), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (file.Equals(newFile, StringComparison.OrdinalIgnoreCase))
            {
                // 目标文件与newFile为同一个文件，则直接导入；
                return this.AddExistingItem(FileCreateFactory.CreateFile(newFile, type));
            }
            else if (File.Exists(newFile))
            {
                // 目标文件与的newFile不为同一个文件，单newFile已经存在；
                MessageBox.Show(String.Format("File {0} is existing in {1}!", newFile, this.FolderPath), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // 复制文件
            if (FileName.CopyFile(file, newFile))
                return this.AddExistingItem(FileCreateFactory.CreateFile(newFile, String.Empty));

            return false;
        }

        private bool AddExistingItem(ViFileInfo file)
        {
            if (file == null)
                return false;

            this.AddChild(file);
            file.Linked = true;
            file.IsSelected = true;
            return true;
        }

        protected virtual String GetDefaultExt()
        {
            return PcsFileInfo.Extension.CFCProgram;
        }

        protected virtual String GetFileFilter()
        {
            return "All Files(*.*)|*.*" +
                    "|CFC File(*.cfc)|*.cfc";
        }

        #region 常用命令处理函数

        #endregion

        #region Build Manager

        //protected bool CanBuild()
        //{
        //    if (this.TheFactory == null || this.TheFactory.BuildManager == null)
        //        return false;

        //    return (this is IViBuildGroup) || (this is IViBuildItem);
        //}

        //protected void Build(bool rebuild)
        //{
        //    if (this is IViBuildGroup)
        //        this.TheFactory.BuildManager.Build(this as IViBuildGroup, rebuild);
        //    else if (this is IViBuildItem)
        //        this.TheFactory.BuildManager.Build(this as IViBuildItem, rebuild);
        //}

        //protected void BuildClean()
        //{
        //    if (this is IViBuildGroup)
        //        this.TheFactory.BuildManager.Clean(this as IViBuildGroup);
        //    else if (this is IViBuildItem)
        //        this.TheFactory.BuildManager.Clean(this as IViBuildItem);
        //}

        #endregion
    }
    public class ViFolderInfo<PT> : ViFolderInfo where PT : ViFolderInfo
    {
        #region Life Cycle

        public ViFolderInfo(String file)
            : base(file)
        {

        }

        #endregion

        #region Parent

        /// <summary>
        /// 得到父对象。
        /// </summary>
        /// <returns>父对象</returns>
        public new PT GetParent()
        {
            return this.Parent;
        }

        /// <summary>
        /// 得到父对象。
        /// </summary>
        public new PT Parent
        {
            get
            {
                return base.GetParent() as PT;
            }
            set
            {
                base.SetParent(value);
            }
        }

        #endregion
    }

}
