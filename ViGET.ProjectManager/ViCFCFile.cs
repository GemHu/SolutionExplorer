using DothanTech.ViGET.ViCommand;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DothanTech.ViGET.Manager
{
    public class ViCFCFile : ViFileInfo<ViFolderInfo>
    {
        public ViCFCFile(String file) : base(file)
        {
        }

        #region Name & Path

        public override string Extension
        {
            get
            {
                return PcsFileInfo.Extension.CFCProgram;
            }
        }
        #endregion

        #region Command Execute

        public override void CanExecuteCommand(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (e.Command == ViCommands.Link)
            {
                if (this.Parent is ViCPUInfo && !this.Linked)
                {
                    e.CanExecute = true;
                }
                return;
            }
            else if (e.Command == ViCommands.Unlink)
            {
                if (this.Parent is ViCPUInfo && this.Linked)
                {
                    e.CanExecute = true;
                }
                return;
            }
            else if (e.Command == ViCommands.Rename)
            {
                e.CanExecute = true;
                return;
            }

            base.CanExecuteCommand(sender, e);
        }

        public override void ExecutedCommand(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.Command == ViCommands.Link)
            {
                this.Linked = true;
                ViCPUInfo cpu = this.Parent as ViCPUInfo;
                cpu.TaskManager.LinkTask(this.FullName);
            }
            else if (e.Command == ViCommands.Unlink)
            {
                this.Linked = false;
                ViCPUInfo cpu = this.Parent as ViCPUInfo;
                cpu.TaskManager.UnlinkTask(this.FullName);
            }

            base.ExecutedCommand(sender, e);
        }

        #endregion

        #region 复制、粘贴、剪切、选择等常用函数

        public override bool CanCopy
        {
            get { return true; }
        }

        public override bool CanCut
        {
            get { return true; }
        }

        public override bool CanPaste
        {
            get { return this.Parent.CanPaste; }
        }

        public override void DoPaste()
        {
            this.Parent.DoPaste();
        }

        #endregion

        public override void DoubleClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.TheFactory != null)
            {
                this.TheFactory.OpenDocument(this.FullName);
            }
        }
    }
}
