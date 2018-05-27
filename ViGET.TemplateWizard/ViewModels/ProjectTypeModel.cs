using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using DothanTech.Helpers;

namespace DothanTech.ViGET.TemplateWizard
{
    public class ProjectTypeModel : UnsortTreeObject
    {
        public List<VSTemplate> Templates { get; set; }
        public ProjectTypeModel(String name, List<VSTemplate> templates)
        {
            this.Name = name;
            this.Templates = templates;
            this.TemplateList = new ViObservableCollection<TemplateItemModel>();

            this.Refresh();
        }

        public void Refresh()
        {
            if (this.Templates != null)
            {
                foreach (var item in this.Templates)
                {
                    this.TemplateList.SortedAdd(new TemplateItemModel(item));
                    if (this.TemplateList.Count == 1)
                        this.TemplateList[0].IsSelected = true;
                }
            }
        }

        #region Children属性

        /// <summary>
        /// 获取指定名称的子节点；
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ProjectTypeModel GetChild(String name)
        {
            if (String.IsNullOrEmpty(name))
                return null;

            foreach (ProjectTypeModel item in this.Children)
            {
                if (name.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                    return item;
            }

            return null;
        }

        public virtual void AddChild(String childName, List<VSTemplate> templates)
        {
            this.AddChild(new ProjectTypeModel(childName, templates));
        }

        #endregion

        #region TemplateList属性

        public ViObservableCollection<TemplateItemModel> TemplateList { get; protected set; }

        public void AddTemplateModel(TemplateItemModel model)
        {
            if (model == null)
                return;

            model.SetParent(this);
            this.TemplateList.SortedAdd<TemplateItemModel>(model);
        }

        public override event ViDataChangedEventHandler PropertyChanged
        {
            add
            {
                base.PropertyChanged += value;
                this.TemplateList.PropertyChanged += value;
            }
            remove
            {
                this.TemplateList.PropertyChanged -= value;
                base.PropertyChanged -= value;
            }
        }

        #endregion

        #region IsExpanded Property

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ProjectTypeModel),
                                        new PropertyMetadata(false));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        #endregion

        #region IsSelected Property

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ProjectTypeModel),
                                        new PropertyMetadata(false));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        #endregion

    }
}
