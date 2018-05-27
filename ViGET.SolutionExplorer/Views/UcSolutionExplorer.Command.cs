using DothanTech.Helpers;
using DothanTech.ViGET.Manager;
using DothanTech.ViGET.ViCommand;
using DothanTech.ViGET.ViService;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DothanTech.ViGET.SolutionExplorer
{
    public partial class UcSolutionExplorer
    {
        private void initCommands()
        {
            //-----------------------Project相关命令--------------------//
            this.CommandBindings.Add(new CommandBinding(ViCommands.AddNewProject, this.AddNewProject_Executed, this.AddNewProject_CanExecute));
            this.CommandBindings.Add(new CommandBinding(ViCommands.AddExistingProject, this.AddExistingProject_Executed, this.AddExistingProject_CanExecute));
            this.CommandBindings.Add(new CommandBinding(ViCommands.OpenLocalFolder, this.ExecutedCommand, this.CanExecuteCommand));

            //--------------------- Project Item 相关命令--------------------//
            this.CommandBindings.Add(new CommandBinding(ViCommands.AddNewItem, this.ExecutedCommand, this.CanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(ViCommands.AddExistingItem, this.ExecutedCommand, this.CanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(ViCommands.IsActive, this.ExecutedCommand, this.CanExecuteCommand));

            // ------------------- File 相关命令 -----------------------------//
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, this.ExecutedCommand, this.CanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, this.ExecutedCommand, this.CanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, this.ExecutedCommand, this.CanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, this.ExecutedCommand, this.CanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(ViCommands.Rename, this.ExecutedCommand, this.CanExecuteCommand));
            //this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, this.ExecutedCommand, this.CanExecuteCommand));
            //this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Properties, this.ExecutedCommand, this.CanExecuteCommand));
            //this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, this.ExecutedCommand, this.CanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(ViCommands.Link, this.ExecutedCommand, this.CanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(ViCommands.Unlink, this.ExecutedCommand, this.CanExecuteCommand));

            this.CommandBindings.Add(new CommandBinding(ViCommands.Build, this.ExecutedCommand, this.CanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(ViCommands.Rebuild, this.ExecutedCommand, this.CanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(ViCommands.Clean, this.ExecutedCommand, this.CanExecuteCommand));
            //this.CommandBindings.Add(new CommandBinding(ViCommands.BatchBuild, this.ExecutedCommand, this.CanExecuteCommand));
            //this.CommandBindings.Add(new CommandBinding(ViCommands.Git, this.ExecutedCommand, this.CanExecuteCommand));
            //this.CommandBindings.Add(new CommandBinding(ViCommands.Online, this.ExecutedCommand, this.CanExecuteCommand));

            // ----------------------- Git 相关命令 -----------------------//
            this.CommandBindings.Add(new CommandBinding(ViCommands.GitCreate, this.ExecutedCommand, this.CanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(ViCommands.GitCommit, this.ExecutedCommand, this.CanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(ViCommands.GitDiff, this.ExecutedCommand, this.CanExecuteCommand));
            this.CommandBindings.Add(new CommandBinding(ViCommands.GitShowLog, this.ExecutedCommand, this.CanExecuteCommand));

        }

        [Import(typeof(IProjectFactory))]
        public ProjectFactory Factory { get; set; }

        #region 通用命令执行函数

        private void CanExecuteCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            // 1、通过右键菜单来获取命令来源
            if (e.Parameter is ViFileInfo)
            {
                (e.Parameter as ViFileInfo).CanExecuteCommand(sender, e);
            }
            else
            {
                // 获取UserControl的全局命令；
                if (e.Command == ApplicationCommands.SelectAll)
                {
                    e.CanExecute = this.Factory.TheSolution != null;
                }
                else if (e.Command == ApplicationCommands.Copy)
                {
                    e.CanExecute = this.CanCopy;
                }
                else if (e.Command == ApplicationCommands.Cut)
                {
                    e.CanExecute = this.CanCut;
                }
                else if (e.Command == ApplicationCommands.Paste)
                {
                    e.CanExecute = this.CanPaste;
                }
            }
        }

        private void ExecutedCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is ViFileInfo)
            {
                (e.Parameter as ViFileInfo).ExecutedCommand(sender, e);
            }
            else
            {
                if (e.Command == ApplicationCommands.SelectAll)
                {
                    this.Factory.TheSolution.SelectAll();
                }
                else if (e.Command == ApplicationCommands.Copy)
                {
                    this.DoCopy();
                }
                else if (e.Command == ApplicationCommands.Cut)
                {
                    this.DoCut();
                }
                else if (e.Command == ApplicationCommands.Paste)
                {
                    this.DoPaste();
                }
            }
        }

        #endregion

        #region AddNewProject Command

        private void AddNewProject_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AddNewProject_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Factory.AddNewProject();
        }

        #endregion

        #region AddExistingProject Command

        private void AddExistingProject_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AddExistingProject_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Factory.AddExistingProject();
        }

        #endregion

        #region Copy

        private bool CanCopy
        {
            get
            {
                List<ViFileInfo> list = this.SelectedItems;
                if (list.Count <= 0)
                    return false;

                foreach (ViFileInfo item in list)
                {
                    if (!(item is ViCFCFile))
                        return false;
                }
                return true;
            }
        }

        private bool CanCut
        {
            get
            {
                List<ViFileInfo> list = this.SelectedItems;
                if (list.Count <= 0)
                    return false;

                foreach (ViFileInfo item in list)
                {
                    if (!(item is ViCFCFile))
                        return false;
                }
                return true;
            }
        }

        private void DoCut()
        {
            List<ViFileInfo> list = new List<ViFileInfo>();
            this.Factory.TheSolution.GetSelectedItems(ref list);
            if (list.Count <= 0)
                return;
            //
            foreach (ViFileInfo item in list)
            {
                item.IsCutting = true;
            }

            this.Factory.TheSolution.ClipBoardFiles = list;
        }

        private void DoCopy()
        {
            List<ViFileInfo> list = new List<ViFileInfo>();
            this.Factory.TheSolution.GetSelectedItems(ref list);
            if (list.Count <= 0)
                return;

            this.Factory.TheSolution.ClipBoardFiles = list;
        }

        private bool CanPaste
        {
            get
            {
                if (this.Factory.TheSolution == null)
                    return false;
                // 1、查看剪切板中有没有数据；
                List<ViFileInfo> sourceList = this.Factory.TheSolution.ClipBoardFiles;
                if (sourceList.Count <= 0)
                    return false;
                // 2、如果当前选中的文件只有一个，则判断当前文件是否可以执行粘贴命令；
                List<ViFileInfo> selectedItems = this.SelectedItems;
                if (selectedItems.Count <= 0)
                    return false;
                if (selectedItems.Count == 1)
                    return selectedItems[0].CanPaste;
                // 3、如果当前选中的文件有多个，则判断这多个文件是否在同一个文件夹下；
                // 3.1、如果不在同一个文件夹下，则没法执行粘贴操作；
                ViFileInfo parent = selectedItems[0].GetParent() as ViFileInfo;
                if (parent == null)
                    return false;
                foreach (ViFileInfo item in selectedItems)
                {
                    if (item.GetParent() != parent)
                        return false;
                }
                // 3.2、如果在，则判断这些文件的父文件夹是否可以执行粘贴命令；
                return parent.CanPaste;
            }
        }

        private void DoPaste()
        {
            List<ViFileInfo> selectItems = this.SelectedItems;
            ViFileInfo target = selectItems.Count == 1 ? selectItems[0] : selectItems[0].GetParent() as ViFileInfo;
            if (target != null)
                target.DoPaste();
        }

        #endregion
    }
}
