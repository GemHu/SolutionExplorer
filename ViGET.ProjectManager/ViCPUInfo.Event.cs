using DothanTech.ViGET.ViCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DothanTech.ViGET.Manager
{
    public partial class ViCPUInfo
    {
        #region Command

        public override void CanExecuteCommand(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (e.Command == ViCommands.AddExistingItem)
            {
                e.CanExecute = true;
                return;
            }
            else if (e.Command == ViCommands.Build ||
                e.Command == ViCommands.Rebuild ||
                e.Command == ViCommands.Clean)
            {
                this.CurrProject.CanExecuteCommand(sender, e);
                return;
            }
            else if (e.Command == ViCommands.IsActive)
            {
                e.CanExecute = !this.IsActive;
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
                this.CurrProject.ExecutedCommand(sender, e);
                return;
            }
            else if (e.Command == ViCommands.IsActive)
            {
                this.IsActive = true;
                return;
            }

            base.ExecutedCommand(sender, e);
        }

        #endregion

        #region 相关事件处理函数

        #endregion
    }
}
