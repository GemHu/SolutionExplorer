using DothanTech.Helpers;
using DothanTech.ViGET.ViService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace DothanTech.ViGET.Manager
{
    public class ViFileNode : ViNamedObject
    {
        #region Life Cycle

        public ViFileNode(String fullName)
        {
            this.FullName = fullName;
            this.Children = new ViObservableCollection<ViFileNode>();
        }

        #endregion

        #region D FullName Property

        public static readonly DependencyProperty FullNameProperty =
            DependencyProperty.Register("FullName", typeof(String), typeof(ViFileNode),
                                        new PropertyMetadata(String.Empty));

        /// <summary>
        /// 本地文件全路径；
        /// </summary>
        public String FullName
        {
            get { return (String)GetValue(FullNameProperty); }
            set 
            {
                SetValue(FullNameProperty, value);
                this.Name = Path.GetFileName(value);
            }
        }

        #endregion

        #region IsSelected Property

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ViFileNode),
                                        new PropertyMetadata(false));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        #endregion

        #region IsEditMode Property

        public static readonly DependencyProperty IsEditModeProperty =
            DependencyProperty.Register("IsEditMode", typeof(bool), typeof(ViFileNode),
                                        new PropertyMetadata(false));

        public bool IsEditMode
        {
            get { return (bool)GetValue(IsEditModeProperty); }
            set { SetValue(IsEditModeProperty, value); }
        }

        #endregion

        #region DirectoryPath

        /// <summary>
        /// 当前文件所在文件夹路径全路径；
        /// </summary>
        public virtual String ParentPath
        {
            get 
            {
                if (File.Exists(this.FullName))
                    return Path.GetDirectoryName(this.FullName);

                ViFolderInfo parent = this.Parent as ViFolderInfo;
                if (parent != null)
                {
                    //return parent.FullPath;
                }

                return String.Empty;
            }
        }

        #endregion

        #region 文件选择操作

        public virtual void GetSelectedItems(ref List<ViFileInfo> selectedItems)
        {
            if (selectedItems == null)
                selectedItems = new List<ViFileInfo>();

            if (this.IsSelected && this is ViFileInfo)
                selectedItems.Add(this as ViFileInfo);

            foreach (ViFileNode item in this.Children)
            {
                item.GetSelectedItems(ref selectedItems);
            }
        }

        /// <summary>
        /// 选择所有子对象；
        /// </summary>
        public virtual void SelectAll()
        {
            this.IsSelected = true;
            foreach (ViFileNode item in this.Children)
            {
                item.SelectAll();
            }
        }

        #endregion

        #region 数据持久化

        public bool IsDirty { get; set; }

        /// <summary>
        /// 用于判断是否增在加载文件；
        /// </summary>
        public bool IsLoading { get; protected set; }
        /// <summary>
        /// 用于判断是否增在保存文件操作；
        /// </summary>
        public bool IsSaveing { get; protected set; }

        /// <summary>
        /// 加载本地数据；
        /// </summary>
        public virtual bool Load()
        {
            return this.LoadFile(this.FullName);
        }

        /// <summary>
        /// 从制定的文件中加载配置信息；
        /// </summary>
        public virtual bool LoadFile(String file)
        {
            if (!System.IO.File.Exists(file))
                return false;

            try
            {
                XmlDocument doc = new XmlDocument();
                this.IsLoading = true;
                doc.Load(file);
                if (!this.LoadDocument(doc))
                    return false;

                this.IsDirty = false;
                return true;
            }
            catch (Exception ee)
            {
                Trace.WriteLine("### [" + ee.Source + "] Exception: " + ee.Message);
                Trace.WriteLine("### " + ee.StackTrace);
                return false;
            }
            finally
            {
                this.IsLoading = false;
            }
        }

        /// <summary>
        /// 从制定的XML文件中读取相关配置信息；
        /// </summary>
        protected virtual bool LoadDocument(XmlDocument doc)
        {
            return true;
        }

        public virtual bool LoadElement(XmlElement element)
        {
            return true;
        }

        /// <summary>
        /// 保存本地数据；
        /// </summary>
        public virtual bool Save()
        {
            return this.SaveFile(this.FullName);
        }

        /// <summary>
        /// 保存相关数据到制定的文件；
        /// </summary>
        public virtual bool SaveFile(String file)
        {
            if (!File.Exists(file))
                return false;
            if (!this.IsDirty)
                return true;

            try
            {
                XmlDocument doc = new XmlDocument();
                XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.AppendChild(decl);
                this.IsSaveing = true;
                if (!this.SaveDocument(doc))
                    return false;

                doc.Save(file);
                this.IsDirty = false;
                return true;
            }
            catch (Exception ee)
            {
                Trace.WriteLine("### [" + ee.Source + "] Exception: " + ee.Message);
                Trace.WriteLine("### " + ee.StackTrace);

                return false;
            }
            finally
            {
                this.IsSaveing = false;
            }
        }

        /// <summary>
        /// 保存相关数据到指定的XML文件中；
        /// </summary>
        protected virtual bool SaveDocument(XmlDocument doc)
        {
            return true;
        }

        /// <summary>
        /// 保存相关配置信息到给定的XmlElement;
        /// </summary>
        public virtual bool SaveElement(XmlElement parent, XmlDocument doc)
        {
            return true;
        }

        #endregion

        #region Children

        public ViObservableCollection<ViFileNode> Children { get; protected set; }

        public override event ViDataChangedEventHandler PropertyChanged
        {
            add
            {
                base.PropertyChanged += value;
                this.Children.PropertyChanged += value;
            }
            remove
            {
                this.Children.PropertyChanged -= value;
                base.PropertyChanged -= value;
            }
        }

        /// <summary>
        /// 遍历制定型号的子类型；
        /// </summary>
        /// <typeparam name="CT"></typeparam>
        /// <param name="func"></param>
        public void LoopChild<CT>(Func<CT, bool> func) where CT : ViNamedObject
        {
            foreach (ViNamedObject item in this.Children)
            {
                if (item is CT)
                {
                    if (!func(item as CT))
                        break;
                }
            }
        }

        public ViNamedObject GetChild(String key)
        {
            if (String.IsNullOrEmpty(key))
                return null;

            foreach (var item in this.Children)
            {
                if (item is IViFileInfo)
                {
                    IViFileInfo subNode = item as IViFileInfo;
                    if (key.Equals(subNode.Key, StringComparison.OrdinalIgnoreCase))
                        return item;
                }
            }

            return null;
        }
        public virtual void AddChild(ViFileNode child)
        {
            if (child == null)
                return;

            this.Children.Add(child);
            child.SetParent(this);
        }
        public virtual bool RemoveChild(ViFileNode child)
        {
            if (child == null)
                return false;

            if (child.GetParent() == this)
            {
                child.SetParent(null);
                this.Children.Remove(child);

                return true;
            }

            return false;
        }

        #endregion
    }
}
