using Dothan.Helpers;
using Dothan.ViObject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DothanTech.ViGET.SolutionExplorer.Modules
{
    abstract class BaseItemNode : UnsortTreeObject
    {
        #region 初始化

        public BaseItemNode() { }

        public BaseItemNode(String itemName) : base(itemName) 
        {
            // 初始化节点的上下文菜单
            this.initContextMenu();
        }

        #endregion

        /// <summary>
        /// 根节点对象；
        /// </summary>
        public ProjectFactory TheFactory 
        {
            get { return this.Root as ProjectFactory; }
        }

        #region ContextMenu

        protected virtual void initContextMenu() 
        {
        }

        /**
         * 判断制定参数中相关命令是否可以执行；
         */
        public virtual void CanExecuteCommand(object sender, CanExecuteRoutedEventArgs e) { }

        /**
         * 执行相关命令；
         */
        public virtual void ExecuteCommand(object sender, ExecutedRoutedEventArgs e) { }

        protected MenuItem AddMenuItem(string name, ICommand command, string iconName)
        {
            return this.AddMenuItem(null, name, command, iconName);
        }

        protected MenuItem AddMenuItem(MenuItem menu, string menuName, ICommand command, string iconName)
        {
            TextBlock txtMenuItem = new TextBlock();
            txtMenuItem.Text = menuName;
            txtMenuItem.HorizontalAlignment = HorizontalAlignment.Left;
            MenuItem subMenu = new MenuItem();
            subMenu.Margin = new Thickness(5, 0, 0, 0);
            subMenu.Command = command;
            subMenu.Header = txtMenuItem;

            if (!String.IsNullOrEmpty(iconName))
            {
                try
                {
                    EnableImage img = new EnableImage();
                    img.Margin = new Thickness(-5, 0, 5, 0);
                    img.Width = 16;
                    img.Height = 16;
                    img.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/CFCObjects;component/Images/MenuImg/" + iconName, UriKind.Absolute));
                    subMenu.Icon = img;
                }
                catch (Exception) { }
            }

            // 将菜单添加到ContextMenu中
            if (menu != null)
            {
                menu.Items.Add(subMenu);
            }
            else
            {
                if (this.ContextMenu == null)
                    this.ContextMenu = new ContextMenu();
                this.ContextMenu.Items.Add(subMenu);
            }

            return subMenu;
        }

        protected void AddMenuSeparator()
        {
            this.AddMenuSeparator(null);
        }

        protected void AddMenuSeparator(MenuItem menu)
        {
            if (menu != null)
            {
                if (menu.Items.Count > 0)
                {
                    menu.Items.Add(new Separator());
                }
            }
            else
            {
                // 想上下文菜单中添加分割线
                if (this.ContextMenu != null && this.ContextMenu.Items.Count > 0){
                    this.ContextMenu.Items.Add(new Separator());
                }
            }
        }

        #endregion

        #region 属性

        /**
         * SolutionExplorer中显示的图标；
         */
        public virtual String Icon { get { return null; } }

        /**
         * 当前节点是否已经展开了；
         */
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set 
            {
                if (this.Children.Count <= 0)
                    ClearValue(IsExpandedProperty);
                else 
                    SetValue(IsExpandedProperty, value); 
            }
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(BaseGroupNode),
                                        new FrameworkPropertyMetadata(false));

        /**
         * 节点上下文菜单；
         */
        public ContextMenu ContextMenu
        {
            get { return (ContextMenu)GetValue(ContextMenuProperty); }
            set { SetValue(ContextMenuProperty, value); }
        }

        public static readonly DependencyProperty ContextMenuProperty =
            DependencyProperty.Register("ContextMenu", typeof(ContextMenu), typeof(BaseItemNode), 
                                        new FrameworkPropertyMetadata(null));

        #endregion
    }

    abstract class BaseItemNode<PT> : BaseItemNode where PT : BaseGroupNode
    {
        #region constructor

        public BaseItemNode() { }

        public BaseItemNode(String itemName) : base(itemName) { }

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

}
