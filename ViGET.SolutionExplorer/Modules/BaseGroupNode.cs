using Dothan.ViObject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DothanTech.ViGET.SolutionExplorer.Modules
{
    class BaseGroupNode : BaseItemNode
    {
        public BaseGroupNode() { }

        public BaseGroupNode(String groupName) : base(groupName) { }

        #region 相关属性

        public virtual String ExpandedIcon
        {
            get { return this.Icon; }
        }

        #endregion
    }

    class BaseGroupNode<PT> : BaseGroupNode where PT : BaseGroupNode
    {
        #region constructor

        public BaseGroupNode() { }

        public BaseGroupNode(String itemName) : base(itemName) { }

        #endregion

        #region Parent

        /// <summary>
        /// 得到父对象。
        /// </summary>
        /// <returns>父对象</returns>
        public new PT GetParent()
        {
            return this.Parent;
        }

        /// <summary>
        /// 得到父对象。
        /// </summary>
        public new PT Parent
        {
            get
            {
                return base.GetParent() as PT;
            }
            protected set
            {
                base.SetParent(value);
            }
        }

        #endregion
    }

    class ProjectIconConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                BaseGroupNode node = values[0] as BaseGroupNode;
                String iconPath = (bool)values[1] ? node.ExpandedIcon : node.Icon;
                if (String.IsNullOrEmpty(iconPath))
                    return null;

                return new BitmapImage(new Uri(iconPath));
            }
            catch (Exception ee)
            {
                Trace.WriteLine("### [ " + ee.Source + "] Exception: " + ee.Message);
                Trace.WriteLine("### " + ee.StackTrace);
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
