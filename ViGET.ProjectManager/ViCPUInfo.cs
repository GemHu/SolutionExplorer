/// <summary>
/// @file   CPUInfoManager.cs
///	@brief  ViGET 工程数据管理器管理 CPU 信息的模块。
/// @author	DothanTech 刘伟宏
/// 
/// Copyright(C) 2011~2014, DothanTech. All rights reserved.
/// </summary>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Markup;

namespace DothanTech.ViGET.Manager
{
    public partial class ViCPUInfo : ViFolderInfo<ProjectManager>
    {
        #region Life Cycle

        /// <summary>
        /// 构建对象。
        /// </summary>
        public ViCPUInfo(String name)
            : base(String.Empty)
        {
            this.Name = name.ToUpper();
        }

        #endregion

        #region IsActive Property

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(ViCPUInfo),
                                        new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsActiveChanged)));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ViCPUInfo currCpu = (ViCPUInfo)d;
            SolutionManager solution = currCpu.TheSolution;
            if (solution == null)
                return;

            if ((bool?)e.NewValue == true)
            {
                solution.LoopCPUs((cpu) =>
                {
                    if (cpu != currCpu && cpu.IsActive)
                    {
                        cpu.IsActive = false;
                    }
                    return true;
                });
                //
                ProjectManager currProject = currCpu.CurrProject;
                currProject.IsActive = true;
                //
                foreach (var item in solution.Children)
                {
                    if (item != currProject && item is ProjectManager)
                    {
                        ProjectManager projectItem = item as ProjectManager;
                        if (projectItem.IsActive)
                            projectItem.IsActive = false;
                    }
                }
            }
        }

        #endregion

        #region CPU相关其他属性
        
        /// <summary>
        /// 当前CPU时候使用了共享内存变量；
        /// </summary>
        public bool HasShmVars { get; set; }

        public String TcpPort { get; set; }

        public String TcpIp { get; set; }

        #endregion

        #region 序列化

        public override bool LoadElement(XmlElement element)
        {
            if (element == null || !Constants.TAG.CPU.Equals(element.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            bool bValue;
            int iValue;
            this.Name = element.GetAttribute(Constants.Attribute.Name);
            String sActive = element.GetAttribute(Constants.Attribute.IsActive);
            this.IsActive = Boolean.TryParse(sActive, out bValue) && bValue;

            foreach (XmlNode groupElement in element.ChildNodes)
            {
                // Settings
                if (Constants.TAG.Settings.Equals(groupElement.Name, StringComparison.OrdinalIgnoreCase))
                {
                    this.Type = (groupElement as XmlElement).GetAttribute(Constants.Attribute.HwType);
                    String hasShm = (groupElement as XmlElement).GetAttribute(Constants.Attribute.HasShmVars);
                    if (Boolean.TryParse(hasShm, out bValue))
                        this.HasShmVars = bValue;
                    foreach (XmlNode itemElement in groupElement.ChildNodes)
                    {
                        // connection
                        if (Constants.TAG.Connection.Equals(itemElement.Name))
                        {
                            this.TcpPort = (itemElement as XmlElement).GetAttribute(Constants.Attribute.TcpPort);
                            this.TcpIp = (itemElement as XmlElement).GetAttribute(Constants.Attribute.TcpIp);
                        }
                    }
                } // Tasks
                else if (Constants.TAG.Tasks.Equals(groupElement.Name, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (XmlNode itemElement in groupElement.ChildNodes)
                    {
                        // Task
                        if (Constants.TAG.Task.Equals(itemElement.Name))
                        {
                            String name = (itemElement as XmlElement).GetAttribute(Constants.Attribute.Name);
                            String type = (itemElement as XmlElement).GetAttribute(Constants.Attribute.Type);
                            String priority = (itemElement as XmlElement).GetAttribute(Constants.Attribute.Priority);
                            ViTaskInfo task = this.TaskManager.CreateTaskInfo(type, name, Int32.TryParse(priority, out iValue) ? iValue : 0);

                            if (task != null)
                                task.LoadElement(itemElement as XmlElement);
                        }
                    }
                } // Files
                else if (Constants.TAG.Files.Equals(groupElement.Name, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (XmlNode fileElement in groupElement.ChildNodes)
                    {
                        var fileType = (fileElement as XmlElement).GetAttribute(Constants.Attribute.FileType);
                        var filePath = (fileElement as XmlElement).GetAttribute(Constants.Attribute.FilePath);
                        var fileLinked = (fileElement as XmlElement).GetAttribute(Constants.Attribute.Linked);

                        // filePath是相对路径，需要转换为绝对路径
                        filePath = this.CurrProject.GetFullPath(filePath);
                        ViFileInfo fileInfo = this.CreateFile(fileType, filePath, String.Empty);
                        if (fileInfo != null)
                        {
                            if (Boolean.TryParse(fileLinked, out bValue))
                                fileInfo.Linked = bValue;
                            this.AddChild(fileInfo);
                        }
                    }
                }
            }

            return true;
        }

        public override bool SaveElement(XmlElement parent, XmlDocument doc)
        {
            XmlElement element = doc.CreateElement(Constants.TAG.CPU);
            element.SetAttribute(Constants.Attribute.Name, this.Name);
            element.SetAttribute(Constants.Attribute.IsActive, this.IsActive.ToString());
            parent.AppendChild(element);

            // Settings
            XmlElement groupElement = doc.CreateElement(Constants.TAG.Settings);
            element.AppendChild(groupElement);
            groupElement.SetAttribute(Constants.Attribute.HwType, this.Type);
            groupElement.SetAttribute(Constants.Attribute.HasShmVars, this.HasShmVars.ToString());

            XmlElement itemElement = doc.CreateElement(Constants.TAG.Connection);
            groupElement.AppendChild(itemElement);
            itemElement.SetAttribute(Constants.Attribute.TcpPort, this.TcpPort);
            itemElement.SetAttribute(Constants.Attribute.TcpIp, this.TcpIp);

            // Tasks
            groupElement = doc.CreateElement(Constants.TAG.Tasks);
            groupElement.SetAttribute(Constants.Attribute.Cyclic, this.TaskManager.CyclicTasks.Count().ToString());
            groupElement.SetAttribute(Constants.Attribute.Interrupt, this.TaskManager.InterruptTasks.Count().ToString());
            element.AppendChild(groupElement);

            foreach (var item in this.TaskManager.Tasks)
            {
                item.SaveElement(groupElement, doc);
            }

            // Files
            groupElement = doc.CreateElement(Constants.TAG.Files);
            element.AppendChild(groupElement);
            groupElement.SetAttribute(Constants.Attribute.FileNumber, this.Children.Count.ToString());
            foreach (ViFileInfo item in this.Children)
            {
                itemElement = doc.CreateElement(Constants.TAG.File);
                groupElement.AppendChild(itemElement);

                itemElement.SetAttribute(Constants.Attribute.FileType, item.Type);
                itemElement.SetAttribute(Constants.Attribute.FilePath, this.CurrProject.GetRelativePath(item.FullName));
                itemElement.SetAttribute(Constants.Attribute.Linked, item.Linked.ToString());
            }

            return base.SaveElement(parent, doc);
        }

        #endregion
    }
}
