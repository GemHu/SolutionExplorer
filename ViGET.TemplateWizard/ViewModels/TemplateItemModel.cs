using DothanTech.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DothanTech.ViGET.TemplateWizard
{
    public class TemplateItemModel : ViObject, IComparable
    {
        public TemplateItemModel(VSTemplate template)
        {
            this.Template = template;
            this.Refresh();
        }

        public void Refresh()
        {
            if (this.Template == null || this.Template.TemplateData == null)
                return;

            TemplateData data = this.Template.TemplateData;
            this.TemplateName = data.Name;
            this.TemplateType = data.Type;
            this.Description = data.Description;
            this.DefaultName = data.DefaultName;
            if (!String.IsNullOrEmpty(data.Icon))
            {
                this.Icon = Path.Combine(this.Template.TemplateFolder, data.Icon);
            }
        }

        public VSTemplate Template { get; protected set; }

        #region TemplateName Property

        public static readonly DependencyProperty TemplateNameProperty =
            DependencyProperty.Register("TemplateName", typeof(String), typeof(TemplateItemModel),
                                        new PropertyMetadata(String.Empty));

        public String TemplateName
        {
            get { return (String)GetValue(TemplateNameProperty); }
            set { SetValue(TemplateNameProperty, value); }
        }

        #endregion

        #region DefaultName Property

        public static readonly DependencyProperty DefaultNameProperty =
            DependencyProperty.Register("DefaultName", typeof(String), typeof(TemplateItemModel),
                                        new PropertyMetadata(String.Empty));

        public String DefaultName
        {
            get { return (String)GetValue(DefaultNameProperty); }
            set { SetValue(DefaultNameProperty, value); }
        }

        #endregion

        #region TemplateType Property

        public static readonly DependencyProperty TemplateTypeProperty =
            DependencyProperty.Register("TemplateType", typeof(String), typeof(TemplateItemModel),
                                        new PropertyMetadata(String.Empty));

        public String TemplateType
        {
            get { return (String)GetValue(TemplateTypeProperty); }
            set { SetValue(TemplateTypeProperty, value); }
        }

        #endregion

        #region Description Property

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(String), typeof(TemplateItemModel),
                                        new PropertyMetadata(String.Empty));

        public String Description
        {
            get { return (String)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        #endregion
        
        #region Icon Property

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(String), typeof(TemplateItemModel),
                                        new PropertyMetadata(String.Empty));

        public String Icon
        {
            get { return (String)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        #endregion

        #region IsSelected Property

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(TemplateItemModel),
                                        new PropertyMetadata(false));
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        #endregion

        public int CompareTo(object obj)
        {
            if (obj == null || !(obj is TemplateItemModel))
                return -1;

            TemplateItemModel target = obj as TemplateItemModel;
            if (target.Template == null || target.Template.TemplateData == null)
                return -1;
            if (this.Template == null || this.Template.TemplateData == null)
                return 1;
            if (this.Template.TemplateData.SortOrder < target.Template.TemplateData.SortOrder)
                return -1;
            else if (this.Template.TemplateData.SortOrder > target.Template.TemplateData.SortOrder)
                return 1;
            else
                return 0;
        }
    }
}
