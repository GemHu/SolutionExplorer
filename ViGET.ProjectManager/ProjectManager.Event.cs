using DothanTech.ViGET.ViCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DothanTech.ViGET.Manager
{
    public partial class ProjectManager
    {
        #region Event

        private void OnChildPropertyChanged()
        {

        }

        #endregion

        #region Command

        public override void CanExecuteCommand(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (e.Command == ViCommands.AddExistingItem)
            {
                e.CanExecute = true;
                return;
            }
            else if (e.Command == ApplicationCommands.Delete)
            {
                e.CanExecute = true;
                return;
            }
            else if (e.Command == ViCommands.Rename)
            {

            }
            else if (e.Command == ViCommands.Build || 
                e.Command == ViCommands.Rebuild || 
                e.Command == ViCommands.Clean)
            {
                if (this.TheFactory != null)
                {
                    e.CanExecute = true;
                    return;
                }
            }
            else if (e.Command == ViCommands.GitDiff)
            {
                if (this.TheSolution.GitManager != null)
                {
                    e.CanExecute = true;
                }
                return;
            }

            base.CanExecuteCommand(sender, e);
        }

        public override void ExecutedCommand(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.Command == ViCommands.AddExistingItem)
            {
                // 1、弹出文件选择对话框；

                // 2、判断目标文件在哪，如果不在当前目录下，则复制目标文件（复制文件待定，因为还需要考虑依赖项）；

                // 3、加载目标文件；
                base.AddExistingItem();
            }
            else if (e.Command == ApplicationCommands.Delete)
            {
                if (MessageBox.Show("确定要删除该项目吗？", "Delete Project", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.TheSolution.RemoveProject(this);
                    }));
                }
                return;
            }
            else if (e.Command == ViCommands.Rename)
            {
                String newName = e.Parameter as String;
                if (String.IsNullOrEmpty(newName))
                    return;

                //this.RenameProject(newName);
            }
            else if (e.Command == ViCommands.Build ||
                e.Command == ViCommands.Rebuild ||
                e.Command == ViCommands.Clean)
            {
                bool? rebuild = null;
                if (e.Command == ViCommands.Rebuild)
                    rebuild = true;
                else if (e.Command == ViCommands.Build)
                    rebuild = false;
                this.TheFactory.BuildProjects(new List<string>() { this.ProjectFile }, rebuild);
                return;
            }
            else if (e.Command == ViCommands.GitDiff)
            {
                this.TheSolution.GitManager.ShowLog(this.FolderPath);
            }

            base.ExecutedCommand(sender, e);
        }

        #endregion
    }
}
