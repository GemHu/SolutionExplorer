/// <summary>
/// @file   SolutionManager.cs
///	@brief  ViGET 工程组管理器。
/// @author	DothanTech 刘伟宏
/// 
/// Copyright(C) 2011~2014, DothanTech. All rights reserved.
/// </summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Windows;

namespace DothanTech.ViGET.Manager
{
    public partial class SolutionManager : ViFolderInfo
    {
        #region Solution File & Path

        public static String Extention
        {
            get { return Constants.Extention.Solution; }
        }

        /// <summary>
        /// 判断给定的文件是不是solution文件；
        /// </summary>
        public static bool IsSolutionFile(String file)
        {
            if (String.IsNullOrEmpty(file))
                return false;

            return file.EndsWith(Constants.Extention.Solution, StringComparison.OrdinalIgnoreCase);
        }

        #region ShownName Property

        public static readonly DependencyProperty ShownNameProperty =
            DependencyProperty.Register("ShownName", typeof(String), typeof(SolutionManager),
                                        new PropertyMetadata(String.Empty));

        public String ShownName
        {
            get { return (String)GetValue(ShownNameProperty); }
            set { SetValue(ShownNameProperty, value); }
        }

        private void UpdateShownName()
        {
            this.ShownName = String.Format("Solution '{0}' ({1} projects)", this.Name, this.Children.Count);
        }

        #endregion
        
        #endregion

        #region Life Circle

        private SolutionManager(String fileName) 
            : base (fileName)
        {
            // 监控待序列化的数据是否发生了变化；
            this.PropertyChanged += this.OnChildPropertyChanged;
        }

        /// <summary>
        /// 解析Solution文件；
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static SolutionManager OpenSolution(String fileName)
        {
            if (String.IsNullOrEmpty(fileName))
                return null;
            if (!File.Exists(fileName))
                return null;

            SolutionManager sln = new SolutionManager(fileName);
            sln.Load();
            sln.UpdateActiveCpu();
            return sln;
        }

        /// <summary>
        /// 关闭解决方案。
        /// </summary>
        public void CloseSolution(bool save = true)
        {
            // 关闭之前需要检测下是否有需要保存的数据，如果有，则保存；
            this.Save();
            if (this.Projects.Count <= 0)
                return;

            // RemoveProject的时候会自动保存sln文件，或者更新ActiveCPU等信息，所以在此调用该函数；
            //foreach (ProjectManager item in this.Projects)
            //{
            //    this.RemoveProject(item);
            //}
        }

        #endregion

        #region 文件序列化

        protected override bool LoadDocument(XmlDocument doc)
        {
            XmlElement root = doc.DocumentElement;
            if (root == null || !Constants.TAG.Soluton.Equals(root.Name))
                return false;

            this.Type = root.GetAttribute(Constants.Attribute.Type);
            String version = root.GetAttribute(Constants.Attribute.Version);
            Dictionary<String, String> dProjectFile = new Dictionary<string, string>();

            foreach (XmlElement group in root.ChildNodes)
            {
                if (Constants.TAG.Projects.Equals(group.Name))
                {
                    // ProjectGroup
                    foreach (XmlElement itemElement in group.ChildNodes)
                    {
                        // 根据项目相关信息，创建项目实例；
                        if (Constants.TAG.Project.Equals(itemElement.Name))
                        {
                            String include = itemElement.GetAttribute(Constants.Attribute.FilePath);
                            String projFile = this.GetFullPath(include);
                            if (File.Exists(projFile))
                            {
                                ProjectManager project = new ProjectManager(projFile);
                                this.AddChild(project);
                                project.Load();
                            }
                        }
                    }
                }
                else if (Constants.TAG.Global.Equals(group.Name))
                {
                    // Global
                }
            }

            this.UpdateActiveCpu();

            return true;
        }

        public override bool Save()
        {
            // 保存前需要先检测并保存下项目相关文件；
            foreach (var item in this.Projects)
            {
                item.Save();
            }

            return base.Save();
        }

        protected override bool SaveDocument(XmlDocument doc)
        {
            //
            if (!this.IsDirty)
                return true;

            XmlElement root = doc.CreateElement(Constants.TAG.Soluton);
            doc.AppendChild(root);
            root.SetAttribute(Constants.Attribute.Version, "1.0");

            // Projects
            XmlElement projectGroup = doc.CreateElement(Constants.TAG.Projects);
            root.AppendChild(projectGroup);

            foreach (ProjectManager item in this.Projects)
            {
                XmlElement itemElement = doc.CreateElement(Constants.TAG.Project);
                projectGroup.AppendChild(itemElement);

                itemElement.SetAttribute(Constants.Attribute.FilePath, this.GetRelativePath(item.ProjectFile));
            }

            return true;
        }

        #endregion
    }
}
