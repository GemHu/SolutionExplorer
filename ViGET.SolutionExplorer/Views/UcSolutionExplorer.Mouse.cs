using DothanTech.Helpers;
using DothanTech.ViGET.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DothanTech.ViGET.SolutionExplorer
{
    /// <summary>
    /// SolutionExplorer鼠标及事件相关属性及方法；
    /// </summary>
    public partial class UcSolutionExplorer
    {
        public List<ViFileInfo> SelectedItems
        {
            get
            {
                List<ViFileInfo> items = new List<ViFileInfo>();
                if (this.Factory.TheSolution == null)
                    return items;

                this.Factory.TheSolution.GetSelectedItems(ref items);
                return items;
            }
        }

        public ViFileInfo SelectedItem
        {
            get { return treeProjects.SelectedItem as ViFileInfo; }
        }

        #region 鼠标拖拽操作

        private Rect dragBoxFromMouseDown = Rect.Empty;
        private Size dragSize = new Size(3, 3);

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);

            Point pos = e.GetPosition(treeProjects);
            HitTestResult result = VisualTreeHelper.HitTest(treeProjects, pos);
            TreeViewItem item = null;
            if (result != null)
                item = ViewHelper.FindVisualParent<TreeViewItem>(result.VisualHit);

            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (item != null)
                    item.IsSelected = true;
            }

            if (e.ClickCount == 2)
            {
                if (item != null && item.DataContext is ViFileInfo)
                {
                    ViFileInfo node = item.DataContext as ViFileInfo;
                    node.DoubleClick(e);
                }

                return;
            }

            dragBoxFromMouseDown = new Rect(pos.X - dragSize.Width / 2, pos.Y - dragSize.Height / 2, dragSize.Width, dragSize.Height);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            if (!dragBoxFromMouseDown.IsEmpty && !dragBoxFromMouseDown.Contains(e.GetPosition(treeProjects)))
            {
                Point pos = e.GetPosition(treeProjects);
                HitTestResult result = VisualTreeHelper.HitTest(treeProjects, pos);
                if (result == null)
                    return;

                TreeViewItem item = ViewHelper.FindVisualParent<TreeViewItem>(result.VisualHit);
                if (item == null)
                    return;
                DragDrop.DoDragDrop(this, new DataObject(item), DragDropEffects.Move);
                dragBoxFromMouseDown = Rect.Empty;
            }
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            e.Handled = true;
            e.Effects = DragDropEffects.None;

            //获取当前位置的TreeViewItem。
            Point pos = e.GetPosition(this.treeProjects);
            HitTestResult result = VisualTreeHelper.HitTest(treeProjects, pos);
            if (result == null)
                return;
            TreeViewItem curItem = ViewHelper.FindVisualParent<TreeViewItem>(result.VisualHit);
            if (curItem == null)
                return;
            //获取拖动的源Item。
            TreeViewItem sourceItem = e.Data.GetData(typeof(TreeViewItem)) as TreeViewItem;
            if (sourceItem == null)
                return;

            // 暂时只支持CFC文件的拖拽操作
            if (!(sourceItem.DataContext is ViCFCFile))
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            e.Effects = DragDropEffects.Move;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            e.Handled = true;
            e.Effects = DragDropEffects.None;

            //获取当前位置的TreeViewItem。
            Point pos = e.GetPosition(this.treeProjects);
            HitTestResult result = VisualTreeHelper.HitTest(treeProjects, pos);
            if (result == null)
                return;
            TreeViewItem curItem = ViewHelper.FindVisualParent<TreeViewItem>(result.VisualHit);
            if (curItem == null)
                return;
            //获取源对象。
            TreeViewItem sourceItem = e.Data.GetData(typeof(TreeViewItem)) as TreeViewItem;
            if (sourceItem == null)
                return;
            //判断当前位置是否可以拖放目标对象，暂时只支持将CFC文件拖拽到CPU中去。
            if (sourceItem.DataContext is ViCPUInfo || sourceItem.DataContext is ViCFCFile)
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            e.Handled = true;

            //获取当前位置的TreeViewItem
            Point pos = e.GetPosition(this.treeProjects);
            HitTestResult result = VisualTreeHelper.HitTest(treeProjects, pos);
            if (result == null)
                return;
            TreeViewItem curItem = ViewHelper.FindVisualParent<TreeViewItem>(result.VisualHit);
            if (curItem == null)
                return;

            //获取目标对象。
            TreeViewItem sourceItem = e.Data.GetData(typeof(TreeViewItem)) as TreeViewItem;
            if (sourceItem == null)
                return;
            //判断及进行执行相关的拖放操作。
            ViFileInfo curFile = curItem.DataContext as ViFileInfo;
            ViFileInfo srcFile = sourceItem.DataContext as ViFileInfo;
            ViFolderInfo currFolder = curFile is ViFolderInfo ? curFile as ViFolderInfo : curFile.GetParent() as ViFolderInfo;
            if (currFolder == null)
                return;

            currFolder.ModeFileInfo(srcFile);
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);
            e.Handled = true;
        }

        #endregion
    }
}
