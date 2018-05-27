/// <summary>
/// @file   TreeViewExtension.cs
///	@brief  TreeView 类的一些扩展函数。
/// @author	DothanTech 刘伟宏
/// 
/// Copyright(C) 2011~2014, DothanTech. All rights reserved.
/// </summary>

using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DothanTech.Helpers
{
    /// <summary>
    /// TreeView 类的一些扩展函数。
    /// </summary>
    public static class TreeViewExtension
    {
        /// <summary>
        /// 得到下拉框选中项目的 Tag 的字符串。
        /// </summary>
        public static object GetSelectedTag(this TreeView tree)
        {
            return (tree.SelectedItem as FrameworkElement).Tag;
        }

        /// <summary>
        /// 根据指定的 Tag 对象，选中 TreeView 中的项目。
        /// </summary>
        /// <returns>成功与否？</returns>
        public static bool SelectTag(this TreeView tree, object value)
        {
            TreeViewItem item = FindTag(tree, value);
            if (item == null) return false;

            item.IsSelected = true;
            return true;
        }

        /// <summary>
        /// 根据指定的 Tag 对象，找到 TreeView 中的项目。
        /// </summary>
        /// <returns>成功与否？</returns>
        public static TreeViewItem FindTag(this TreeView tree, object value)
        {
            return FindTag(tree.Items, value);
        }

        /// <summary>
        /// 根据指定的 Tag 对象，找到 TreeViewItem 中的项目。
        /// </summary>
        /// <returns>成功与否？</returns>
        public static TreeViewItem FindTag(this TreeViewItem item, object value)
        {
            return FindTag(item.Items, value);
        }

        static TreeViewItem FindTag(ItemCollection items, object value)
        {
            if (items != null && items.Count > 0)
            {
                // 遍历所有子节点
                foreach (object _item in items)
                {
                    // 和节点自己比较
                    TreeViewItem item = _item as TreeViewItem;
                    if (item != null && item.Tag == value)
                        return item;

                    // 递归子树
                    item = FindTag(item.Items, value);
                    if (item != null)
                        return item;
                }
            }
            return null;
        }

        /// <summary>
        /// 在 TreeViewItem 的子节点中，顺序排序方式插入一子节点。
        /// </summary>
        /// <param name="item">TreeViewItem 的子节点集合</param>
        /// <param name="child">需要插入的子节点</param>
        public static void SortAdd(this ItemCollection item, TreeViewItem child)
        {
            item.SortAdd(child, null);
        }

        /// <summary>
        /// 在 TreeViewItem 的子节点中，顺序排序方式插入一子节点。
        /// </summary>
        /// <param name="item">TreeViewItem 的子节点集合</param>
        /// <param name="child">需要插入的子节点</param>
        /// <param name="dirOrFile">用于判断节点是目录还是文件？目录类型的节点返回 true，会统一排序在子节点的头部。</param>
        public static void SortAdd(this ItemCollection item, TreeViewItem child, Func<TreeViewItem, bool> dirOrFile)
        {
            if (child == null) return;

            // 通过 DataContext 来控制显示，也用这个来进行排序插入
            string shownText = child.DataContext as string;
            if (string.IsNullOrEmpty(shownText))
            {
                item.Add(child);
                return;
            }

            // 先记录插入的节点是目录还是文件？
            bool childIsDir = (dirOrFile == null ? false : dirOrFile(child));

            // 用二分法查找插入位置
            int left = 0, right = item.Count - 1, mid;
            while (left <= right)
            {
                mid = left + (right - left) / 2;

                TreeViewItem comp = item[mid] as TreeViewItem;
                if (comp == null)
                {
                    right = mid - 1;
                }
                else if (!(comp.DataContext is string))
                {
                    right = mid - 1;
                }
                else
                {
                    // 目录还是文件？只有同样类型的才需要比较显示字符串
                    bool compIsDir = (dirOrFile == null ? false : dirOrFile(comp));
                    if (compIsDir == childIsDir)
                    {
                        if (string.Compare(shownText, comp.DataContext as string, true) >= 0)
                        {
                            left = mid + 1;
                        }
                        else
                        {
                            right = mid - 1;
                        }
                    }
                    else if (childIsDir)
                    {
                        right = mid - 1;
                    }
                    else
                    {
                        left = mid + 1;
                    }
                }
            }

            // left 就是插入点的序号
            item.Insert(left, child);
        }

        /// <summary>
        /// 克隆一个 TreeViewItem。
        /// </summary>
        public static TreeViewItem Clone(this TreeViewItem item)
        {
            item = ((DependencyObject)item).Clone() as TreeViewItem;
            if (item == null) return null;
            return item;
        }

        /// <summary>
        /// 克隆一个 TreeViewItem，同时设置其 DataContext。
        /// </summary>
        public static TreeViewItem Clone(this TreeViewItem item, object dataContext)
        {
            item = item.Clone();
            if (item == null) return null;
            item.DataContext = dataContext;
            return item;
        }

        /// <summary>
        /// 克隆一个 TreeViewItem，同时设置其 DataContext。
        /// </summary>
        public static TreeViewItem Clone(this TreeViewItem item, object dataContext, object tag)
        {
            item = item.Clone();
            if (item == null) return null;
            item.DataContext = dataContext;
            item.Tag = tag;
            return item;
        }

        /// <summary>
        /// 将一个 TreeViewItem，从 TreeView 中删除。
        /// </summary>
        /// <param name="item">需要删除的 TreeViewItem。</param>
        public static void Remove(this TreeViewItem item)
        {
            TreeViewItem parent = item.Parent as TreeViewItem;
            if (parent == null) return;

            parent.Items.Remove(item);
        }
    }
}
