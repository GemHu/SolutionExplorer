using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using DothanTech.Helpers;
namespace DothanTech.ViGET.Manager
{
    public partial class ViCPUInfo
    {
        #region Tasks

        private ViTaskManager _taskManager;
        public ViTaskManager TaskManager
        {
            get
            {
                if (this._taskManager == null)
                    this._taskManager = new ViTaskManager();

                return this._taskManager;
            }
        }

        #endregion

        #region Children

        public override ViFileInfo CreateFile(string fileType, string filePath, string fileName)
        {
            ViFileInfo child = base.CreateFile(fileType, filePath, fileName);
            if (child != null)
            {
                child.Linked = true;
            }

            return child;
        }

        public override void AddChild(ViFileNode child)
        {
            if (child == null)
                return;

            base.AddChild(child);

            // Link File
            ViFileInfo item = child as ViFileInfo;
            if (item.Linked)
                this.TaskManager.LinkTask(item.FullName);
        }

        public override bool RemoveChild(ViFileNode child)
        {
            ViFileInfo item = child as ViFileInfo;
            item.Linked = false;
            this.TaskManager.UnlinkTask(item.FullName);

            return base.RemoveChild(child);
        }

        #endregion
    }
}
