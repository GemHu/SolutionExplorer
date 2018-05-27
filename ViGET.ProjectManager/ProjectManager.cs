/// <summary>
/// @file   ProjectManager.cs
///	@brief  ViGET 工程数据管理器。
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
using System.Windows.Interop;

using DothanTech.Helpers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DothanTech.ViGET.ViService;

namespace DothanTech.ViGET.Manager
{
    /// <summary>
    /// ViGET 工程数据管理器。
    /// </summary>
    public partial class ProjectManager : ViFolderInfo, IViProject, IDisposable
    {
        #region Project File & Path

        /// <summary>
        /// 工程文件全路径名称。
        /// </summary>
        public string ProjectFile
        {
            get { return this.FullName; }
        }

        /// <summary>
        /// 工程名称（不包括路径，不包括后缀名）。
        /// </summary>
        public string ProjectName 
        {
            get { return this.Name; } 
        }

        /// <summary>
        /// 工程文件所在的路径名称，以 \ 结尾。
        /// </summary>
        public string ProjectPath { get { return this.FolderPath; } }

        /// <summary>
        /// 工程的 ENV 目录。
        /// </summary>
        public string ProjectEnvPath
        {
            get
            {
                return this.ProjectPath + @"$ENV$\";
            }
        }

        /// <summary>
        /// 判断给定的文件名是不是项目文件；
        /// </summary>
        public static bool IsProjectFile(String file)
        {
            if (String.IsNullOrEmpty(file))
                return false;

            return file.EndsWith(Constants.Extention.Project, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Logger

        #endregion

        #region Life cycle

        public ProjectManager(String file)
            : base(file)
        {
        }

        public override void Dispose()
        {
            this.Close();

            base.Dispose();
        }

        /// <summary>
        /// 关闭项目的时候需要做一些收尾的工作；
        /// </summary>
        public void Close()
        {
            // 检查并保存相关配置信息
            this.Save();
        }

        #endregion

        #region ProjectFile 序列化

        protected override bool LoadDocument(XmlDocument doc)
        {
            XmlElement root = doc.DocumentElement;
            if (root == null || !Constants.TAG.Project.Equals(root.Name))
                return false;

            this.Type = root.GetAttribute(Constants.Attribute.Type);
            //String version = root.GetAttribute(Constants.Attribute.Version);
            foreach (XmlNode groupNode in root.ChildNodes)
            {
                // 读取Project本身相关信息；
                if (Constants.TAG.GlobalVariable.Equals(groupNode.Name))
                {
                }
                else if (Constants.TAG.CPUs.Equals(groupNode.Name))
                {
                    // 读取文件相关信息；
                    foreach (XmlNode itemElement in groupNode.ChildNodes)
                    {
                        if (itemElement.NodeType == XmlNodeType.Comment)
                            continue;
                        // CPU
                        if (Constants.TAG.CPU.Equals(itemElement.Name))
                        {
                            ViCPUInfo cpu = new ViCPUInfo(String.Empty);
                            this.AddChild(cpu);
                            cpu.LoadElement(itemElement as XmlElement);
                        }
                    }
                }
            }

            return true;
        }

        protected override bool SaveDocument(XmlDocument doc)
        {
            XmlElement rootElement = doc.CreateElement(Constants.TAG.Project);
            doc.AppendChild(rootElement);
            // 特性本地名称不能为空白，否则会抛异常；
            //rootElement.SetAttribute(Constants.Attribute.EncyptCode, "");
            //rootElement.SetAttribute(Constants.Attribute.CreateDate, "");
            rootElement.SetAttribute(Constants.Attribute.LastModifyDate, new DateTime().ToString(Constants.Other.DateTimeFormat));
            rootElement.SetAttribute(Constants.Attribute.Version, "1.0");
            rootElement.SetAttribute(Constants.Attribute.Type, this.Type);

            // GlobalVariable
            XmlElement groupElement = doc.CreateElement(Constants.TAG.GlobalVariable);
            rootElement.AppendChild(groupElement);
            groupElement.SetAttribute(Constants.Attribute.FilePath, "");

            // CPUS
            groupElement = doc.CreateElement(Constants.TAG.CPUs);
            rootElement.AppendChild(groupElement);
            foreach (ViCPUInfo item in this.CPUs)
            {
                item.SaveElement(groupElement, doc);
            }

            return true;
        }

        #endregion

    }
}
