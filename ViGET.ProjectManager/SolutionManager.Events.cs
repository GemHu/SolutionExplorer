using DothanTech.Helpers;
using DothanTech.ViGET.ViCommand;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DothanTech.ViGET.Manager
{
    public partial class SolutionManager
    {
        #region 用于监控需要持久化的数据；

        /// <summary>
        /// 用于监控需要持久化的数据是否发生变化；
        /// </summary>
        private void OnChildPropertyChanged(object sender, ViDataChangedEventArgs e)
        {
            if ((e.ChangeType & ViChangeType.CauseDirty) != ViChangeType.None)
            {
                this.IsDirty = true;
                // 项目个数发生了变化，则需要实时更新显示信息；
                this.UpdateShownName();
            }
        }

        #endregion

        #region Command Execute

        public override void CanExecuteCommand(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (e.Command == ViCommands.Build ||
                e.Command == ViCommands.Rebuild ||
                e.Command == ViCommands.Clean)
            {
                if (this.TheFactory != null)
                {
                    e.CanExecute = true;
                    return;
                }
            }
            else if (e.Command == ViCommands.GitCreate)
            {
                e.CanExecute = this.CanInitGit;
                return;
            }
            else if (e.Command == ViCommands.GitDiff ||
                e.Command == ViCommands.GitCommit ||
                e.Command == ViCommands.GitShowLog)
            {
                e.CanExecute = (this.GitManager != null && this.GitManager.HasCreated);
                return;
            }

            base.CanExecuteCommand(sender, e);
        }

        public override void ExecutedCommand(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.Command == ViCommands.Build ||
                e.Command == ViCommands.Rebuild ||
                e.Command == ViCommands.Clean)
            {
                bool? rebuild = null;
                if (e.Command == ViCommands.Rebuild)
                    rebuild = true;
                else if (e.Command == ViCommands.Build)
                    rebuild = false;
                this.TheFactory.BuildSolution(rebuild);
                return;
            }
            else if (e.Command == ViCommands.GitCreate)
            {
                this.InitGit();
            }
            else if (e.Command == ViCommands.GitDiff)
            {
                this.GitManager.Diff();
            }
            else if (e.Command == ViCommands.GitCommit)
            {
                this.GitManager.Commit();
            }
            else if (e.Command == ViCommands.GitShowLog)
            {
                this.GitManager.ShowLog();
            }

            base.ExecutedCommand(sender, e);
        }

        #endregion

        #region Git相关命令处理函数

        private bool CanInitGit
        {
            get
            {
                if (this.GitManager == null)
                    return false;

                return !this.GitManager.HasCreated;
            }
        }

        private void InitGit()
        {
            String[] lines = { "[Dd]ebug*/", "[Rr]elease*/" };
            this.GitManager.Create(lines);
        }

        #endregion

        #region 常用命令处理函数

        /// <summary>
        /// 剪切板中的文件；
        /// </summary>
        public List<ViFileInfo> ClipBoardFiles
        {
            get
            {
                List<ViFileInfo> items = new List<ViFileInfo>();
                StringCollection files = Clipboard.GetFileDropList();
                if (files != null && files.Count > 0)
                {
                    foreach (String item in files)
                    {
                        ViFileInfo target = this.GetFileInfoByFullPath(item);
                        if (target != null)
                            items.Add(target);
                    }
                }

                return items;
            }
            set
            {
                if (value == null || value.Count <= 0)
                    return;
                // 重置之前已经剪切过的文件
                foreach (ViFileInfo item in this.ClipBoardFiles)
                {
                    item.IsCutting = false;
                }
                // 
                StringCollection items = new StringCollection();
                foreach (ViFileInfo item in value)
                {
                    items.Add(item.FullName);
                }
                Clipboard.SetFileDropList(items);
            }
        }

        #endregion
    }
}
