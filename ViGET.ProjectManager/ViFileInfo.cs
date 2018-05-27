using DothanTech.Helpers;
using DothanTech.ViGET.Manager.Properties;
using DothanTech.ViGET.ViCommand;
using DothanTech.ViGET.ViService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace DothanTech.ViGET.Manager
{
    public interface IViFileInfo
    {
        /// <summary>
        /// 本地文件全路径；
        /// </summary>
        String FullName { get; }

        ///// <summary>
        ///// 通过递归查找方式，获取当前文件的相对路径；该路径是相对于Project的一个路径；
        ///// </summary>
        //String ViPath { get; }

        /// <summary>
        /// 当前节点所属项目；
        /// </summary>
        ProjectManager CurrProject { get; }

        /// <summary>
        /// 当前节点所属Solution；
        /// </summary>
        SolutionManager TheSolution { get; }

        /// <summary>
        /// 当前项目所属根节点；
        /// </summary>
        IProjectFactory TheFactory { get; }

        String Key { get; }
    }

    public class ViFileInfo : ViFileNode, IViFileInfo
    {
        #region Life Cycle

        public ViFileInfo(String file) : base(file)
        {
            // 先判断下给定的文件名称是不是文件全路径，如果不是，就需要加上项目路径；
        }

        #endregion
    
        #region IViNode & File & Path

        /// <summary>
        /// 当前文件所对应的扩展名，如果有的话；
        /// </summary>
        public virtual String Extension
        {
            get { return String.Empty; }
        }

        public virtual ProjectManager CurrProject
        {
            get { return this.GetAncestor(typeof(ProjectManager)) as ProjectManager; }
        }

        public virtual SolutionManager TheSolution
        {
            get { return this.GetAncestor(typeof(SolutionManager)) as SolutionManager; }
        }

        public IProjectFactory TheFactory
        {
            get { return this.GetAncestor(typeof(IProjectFactory)) as IProjectFactory; }
        }

        public override String Key
        {
            get 
            {
                if (!String.IsNullOrEmpty(this.FullName))
                    return PCSFile.GetFileKey(this.FullName);

                return base.Key;
            }
        }

        public new String Type
        {
            get { return base.Type as String; }
            set { base.Type = value; }
        }

        /// <summary>
        /// 文件名非法校验；
        /// </summary>
        /// <param name="fileName">待检测文件名；</param>
        /// <returns>true：表示文件名可用，false：表示文件名无效；</returns>
        public virtual bool FileNameCanUsed(String fileName, String type, Collection<ViFileInfo> exepts = null)
        {
            // 目标名称为空，则不可用；
            if (String.IsNullOrEmpty(fileName))
                return false;
            // 如果当前名称为空，无法比较，放行；
            if (String.IsNullOrEmpty(this.Name))
                return true;
            // 
            if (exepts != null && exepts.Contains(this))
                return true;

            // 类型判断，如果类型不一致，则放行；
            //if (String.IsNullOrEmpty(type) && !String.IsNullOrEmpty(this.Type))
            //    return true;
            if (!String.IsNullOrEmpty(type))
            {
                if (String.IsNullOrEmpty(this.Type))
                    return true;
                if (!type.Equals(this.Type, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            // 如果类型一致，则比较文件名，如果文件名冲突，则表示给定的文件名不可用；
            if (fileName.Equals(this.Name, StringComparison.OrdinalIgnoreCase))
                return false;
            if (Path.GetFileNameWithoutExtension(fileName).Equals(Path.GetFileNameWithoutExtension(this.Name), StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }

        #endregion

        #region Global Property

        public IGitManager GitManager
        {
            get
            {
                return this.TheFactory == null ? null : this.TheFactory.GitManager;
            }
        }

        #endregion

        #region 相关事件及处理函数

        public virtual void DoubleClick(MouseButtonEventArgs e)
        {
        }

        public virtual void CanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Delete)
            {
                e.CanExecute = this.CanDelete;
                return;
            }
            else if (e.Command == ApplicationCommands.Copy)
            {
                e.CanExecute = this.CanCopy;
                return;
            }
            else if (e.Command == ApplicationCommands.Cut)
            {
                e.CanExecute = this.CanCut;
                return;
            }
            else if (e.Command == ApplicationCommands.Paste)
            {
                e.CanExecute = this.CanPaste;
                return;
            }

            e.CanExecute = false;
        }

        public virtual void ExecutedCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Delete)
            {
                this.DoDelete(true);
            }
            else if (e.Command == ViCommands.Rename)
            {
                // 打开重命名编辑框；
                this.IsEditMode = true;
            }
            else if (e.Command == ApplicationCommands.Copy)
            {
                this.DoCopy();
                return;
            }
            else if (e.Command == ApplicationCommands.Cut)
            {
                this.DoCut();
                return;
            }
        }

        #endregion

        #region D Linked Property

        public static readonly DependencyProperty LinkedProperty =
            DependencyProperty.Register("Linked", typeof(bool), typeof(ViFileInfo),
                                        new PropertyMetadata(true));

        public bool Linked
        {
            get { return (bool)GetValue(LinkedProperty); }
            set { SetValue(LinkedProperty, value); }
        }

        #endregion

        #region IsCutting Property

        public static readonly DependencyProperty IsCuttingProperty =
            DependencyProperty.Register("IsCutting", typeof(bool), typeof(ViFileInfo),
                                        new PropertyMetadata(false));

        /// <summary>
        /// 设置剪切状态，Cut与Copy的区别就是粘贴完之后是否需要删除目标文件；
        /// </summary>
        public bool IsCutting
        {
            get { return (bool)GetValue(IsCuttingProperty); }
            set { SetValue(IsCuttingProperty, value); }
        }

        #endregion

        #region File & Children

        /// <summary>
        /// 查找制定名称文件；
        /// </summary>
        /// <param name="name">需要查找的目标文件名称</param>
        /// <returns></returns>
        public virtual ViFileInfo GetFileInfo(String name, ViFileInfo exept = null)
        {
            if (String.IsNullOrEmpty(name) || exept == this)
                return null;

            if (Path.GetFileNameWithoutExtension(name).Equals(Path.GetFileNameWithoutExtension(this.Name), StringComparison.OrdinalIgnoreCase))
                return this;

            foreach (var item in this.Children)
            {
                var target = item is ViFileInfo ? (item as ViFileInfo).GetFileInfo(name) : null;
                if (target != null)
                    return target;
            }

            return null;
        }

        public virtual ViFileInfo GetFileInfoByFullPath(String fullPath)
        {
            if (String.IsNullOrEmpty(fullPath))
                return null;

            if (fullPath.Equals(this.FullName, StringComparison.OrdinalIgnoreCase))
                return this;

            return null;
        }

        #endregion

        #region 文件的复制、剪切、粘贴、重命名等操作

        public virtual bool CanCopy { get { return false;} }

        public virtual bool CanCut { get {return false;} }

        public virtual bool CanPaste { get { return false; } }

        public virtual void DoCopy()
        {
            if (!File.Exists(this.FullName))
                return;

            // 执行复制操作
            this.TheSolution.ClipBoardFiles = new List<ViFileInfo>() { this};
        }

        public virtual void DoCut()
        {
            // 限制性复制操作，然后在修改下剪切的状态；
            this.DoCopy();
            this.IsCutting = true;
        }

        public virtual void DoPaste() { }

        public virtual bool Remane(String oldName, String newName)
        {
            // 如果newName为空，则返回；
            if (String.IsNullOrEmpty(newName))
                return true;
            // 如果文件名未改变，则返回；
            if (!String.IsNullOrEmpty(this.Extension) && !newName.EndsWith(this.Extension, StringComparison.OrdinalIgnoreCase))
                newName = newName + this.Extension;

            if (Path.GetFileNameWithoutExtension(newName).Equals(Path.GetFileNameWithoutExtension(oldName)))
                return true;
            // 如果文件名被占用，则提示用于
            if (!this.CurrProject.FileNameCanUsed(newName, this.Type))
            {
                MessageBox.Show(String.Format("FileName \"{0}\" is already used!", newName), "ViGET", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // 修改相关数据；
            String newFile = Path.Combine(Path.GetDirectoryName(this.FullName), newName);
            if (File.Exists(newFile))
            {
                MessageBox.Show(String.Format("File \"{0}\" is already existing!", newFile), "ViGET", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            // 本地文件重命名
            if (!FileName.MoveFile(this.FullName, newFile))
                return false;

            //
            this.FullName = newFile;

            return true;
        }

        public bool CanDelete
        {
            get
            {
                if (this.Parent == null || !(this.Parent is ViFolderInfo))
                    return false;

                return true;
            }
        }

        public bool DoDelete(bool alert)
        {
            if (!this.CanDelete)
                return false;

            if (alert)
            {
                if (MessageBox.Show(Resource.DeleteItemMsg, Resource.DefaultDlgTitle, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk) != MessageBoxResult.OK)
                    return false;
            }

            if (!(this.Parent as ViFolderInfo).RemoveChild(this))
                return false;
            // 如果文件已经存在，则将文件删除到回收站；
            if (File.Exists(this.FullName))
            {
                return FileName.DeleteToRecycleBin(this.FullName);
            }

            return false;
        }

	    #endregion
    }

    public class ViFileInfo<PT> : ViFileInfo where PT : ViFolderInfo
    {
        #region Life Cycle

        public ViFileInfo(String file)
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
