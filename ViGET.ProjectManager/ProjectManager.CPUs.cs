/// <summary>
/// @file   ProjectManager.CPUs.cs
///	@brief  ViGET 工程数据管理器管理 CPU 列表的模块。
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
using System.Collections;
using DothanTech.ViGET.ViService;

namespace DothanTech.ViGET.Manager
{
    public partial class ProjectManager
    {
        public List<ViCPUInfo> CPUs
        {
            get
            {
                return this.Children.Where(item => item is ViCPUInfo).Select(item => item as ViCPUInfo).ToList();
            }
        }

        /// <summary>
        /// 工程的 CPU 对象列表。
        /// </summary>
        public Dictionary<string, ViCPUInfo> DCPUs
        {
            get
            {
                return this.Children.Where(item => item is ViCPUInfo).ToDictionary(
                    item => (item as ViCPUInfo).Key,
                    item => item as ViCPUInfo);
            }
        }

        /// <summary>
        /// 当前激活的 CPU 对象。
        /// </summary>
        public ViCPUInfo ActiveCPU
        {
            get
            {
                foreach (ViCPUInfo item in this.Children)
                {
                    if (item.IsActive)
                        return item;
                }

                return null;
            }
        }

        #region D IsActive Property

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(ProjectManager),
                                        new PropertyMetadata(false));

        public bool IsActive
        {
            get 
            {
                return this.ActiveCPU != null;
            }
            set 
            { 
                SetValue(IsActiveProperty, value); 
            }
        }

        #endregion


        public void UpdateActiveCPU()
        {
            if (this.IsActive)
                return;

            this.LoopChild<ViCPUInfo>(cpu =>
            {
                cpu.IsActive = true;
                return false;
            });
        }

        /// <summary>
        /// 向工程中添加 CPU。
        /// </summary>
        /// <param name="makefile">CPU 的 Makefile 全路径名称。</param>
        /// <returns>成功与否？</returns>
        public bool AddCPU(ViCPUInfo cpu)
        {
            if (cpu == null)
                return false;

            base.AddChild(cpu);

            return true;
        }

        public override void AddChild(ViFileNode child)
        {
            if (child is ViCPUInfo)
            {
                this.AddCPU(child as ViCPUInfo);
            }
            else
            {
                base.AddChild(child);
            }
        }

        #region create/delete/rename CPU

        public override ViFileInfo CreateFile(string fileType, string filePath, String fileName)
        {
            return FileCreateFactory.CreateFile(fileName, fileType);
        }

        private void createCPU(string cpuName, string hardware)
        {
        }

        private void renameCPU(string oldName, string newName)
        {
        }

        private void deleteCPU(string cpuName)
        {
        }

        #endregion
    }
}
