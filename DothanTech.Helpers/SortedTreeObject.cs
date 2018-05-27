/// <summary>
/// @file   SortedTreeObject.cs
///	@brief  ViGET 编辑器中所有具有子对象的对象的基类，子对象集合是按照对象名称“从小到大”存放的。
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

namespace DothanTech.Helpers
{
    /// <summary>
    /// ViGET 编辑器中所有具有子对象的对象的基类，子对象集合是按照对象名称“从小到大”存放的。
    /// </summary>
    public class SortedTreeObject : ViNamedObject, IViCollectionChanged
    {
        #region Life cycle

        /// <summary>
        /// 构建对象。
        /// </summary>
        public SortedTreeObject()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// 构建指定名称的对象。
        /// </summary>
        /// <param name="name">对象名称</param>
        public SortedTreeObject(string name)
            : base(name)
        {
            this.Owner = this;
            this.Children = new ViObservableDictionary<ViNamedObject>();
        }

        /// <summary>
        /// 释放与其它对象之间的弱引用。
        /// </summary>
        public override void Dispose()
        {
            if (this.Owner == this)
            {
                foreach (var v in this.Children)
                    v.Dispose();
            }

            this.Children.Clear();

            base.Dispose();
        }

        /// <summary>
        /// 子对象集合的元素的拥有者（会对子对象 SetParent(Owner);）
        /// </summary>
        public ViObject Owner { get; set; }

        #endregion

        #region Children

        /// <summary>
        /// 子节点集合。
        /// </summary>
        public ViObservableDictionary<ViNamedObject> Children { get; protected set; }

        /// <summary>
        /// 获取指定索引的子对象。
        /// </summary>
        public ViNamedObject this[int index]
        {
            get
            {
                return this.Children[index];
            }
        }

        /// <summary>
        /// 添加子节点。子节点会按照字母序（不区分大小写）添加到子节点集合中。
        /// 
        /// @note 是按照“从小到大”排序的方式加入到子对象集合中的。
        /// </summary>
        /// <param name="child">需要添加的子节点</param>
        public virtual void AddChild(ViNamedObject child)
        {
            if (this.Owner != null)
                child.SetParent(this.Owner);

            this.Children.SortedAdd(child);
        }

        /// <summary>
        /// 删除子节点。
        /// </summary>
        /// <param name="child">被删除的子节点</param>
        /// <returns>如果子节点不在本对象的子节点列表中，则失败</returns>
        public virtual bool DeleteChild(ViNamedObject child)
        {
            if (this.Owner != null)
            {
                if (child.GetParent() != this.Owner)
                    return false;

                child.SetParent(null);
            }

            return this.Children.Remove(child);
        }

        /// <summary>
        /// 删除所有子节点。
        /// </summary>
        public virtual void DeleteAll()
        {
            if (this.Owner != null)
            {
                foreach (ViNamedObject child in this.Children)
                    child.SetParent(null);
            }

            this.Children.Clear();
        }

        /// <summary>
        /// 对子节点，按照字母序（不区分大小写）进行排序。
        /// </summary>
        public virtual void SortChildren()
        {
            this.Children.Sort();
        }

        /// <summary>
        /// 找到指定名称的子节点（不递归查找）。
        /// </summary>
        /// <param name="name">子节点名称</param>
        /// <returns>指定名称的子节点</returns>
        public ViNamedObject ChildByName(string name)
        {
            return this.ChildByName(name, false);
        }

        /// <summary>
        /// 找到指定名称的子节点，递归或不递归方式。
        /// </summary>
        /// <param name="name">子节点名称</param>
        /// <param name="recursive">是否递归查找子节点的子节点？</param>
        /// <returns>指定名称的子节点</returns>
        public ViNamedObject ChildByName(string name, bool recursive)
        {
            int index = this.IndexOfChild(name);
            if (index < 0)
            {
                if (recursive)
                {
                    foreach (ViNamedObject child in this.Children)
                    {
                        SortedTreeObject treeobj = child as SortedTreeObject;
                        if (treeobj == null) continue;

                        ViNamedObject findobj = treeobj.ChildByName(name, recursive);
                        if (findobj != null)
                            return findobj;
                    }
                }
                return null;
            }

            return this.Children[index];
        }

        /// <summary>
        /// 找到指定路径名称的子节点，路径名称用 / 或 \ 进行分隔。
        /// </summary>
        /// <param name="path">指定的路径名称</param>
        /// <returns>指定路径名称的子节点</returns>
        public ViNamedObject ChildByPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            string[] parts = path.Split('/', PathSeperator);

            return this.ChildByPath(parts);
        }

        /// <summary>
        /// 找到指定路径名称的子节点，路径名称已经分割成字符串数组。
        /// </summary>
        /// <param name="path">分割后的路径字符串数组</param>
        /// <returns>指定路径名称的子节点</returns>
        public ViNamedObject ChildByPath(string[] path)
        {
            if (path == null || path.Count() <= 0)
                return null;

            return this.ChildByPath(path, 0);
        }

        /// <summary>
        /// 得到指定名称的子节点的集合索引，< 0 表示没有找到。
        /// </summary>
        /// <param name="name">子节点名称</param>
        /// <returns>子节点的集合索引，< 0 表示没有找到</returns>
        public virtual int IndexOfChild(string name, bool ignoreCase = true)
        {
            return this.Children.BinarySearch(new ViNamedObject(name));
        }

        /// <summary>
        /// 返回指定对象在子节点集合中的索引，< 0 表示没有找到。
        /// </summary>
        /// <param name="child">指定的子节点</param>
        /// <returns>子节点的集合索引，< 0 表示没有找到</returns>
        public virtual int IndexOfChild(ViNamedObject child)
        {
            return this.Children.IndexOf(child);
        }

        protected ViNamedObject ChildByPath(string[] path, int start)
        {
            ViNamedObject child = this.ChildByName(path[start], false);
            if (child == null)
                return null;

            if (start + 1 >= path.Count())
                return child;

            SortedTreeObject treeobj = child as SortedTreeObject;
            if (treeobj == null)
                return null;

            return treeobj.ChildByPath(path, start + 1);
        }

        #endregion

        #region Collection Changed

        /// <summary>
        /// 集合元素发生变化时的通知事件。
        /// </summary>
        public event ViDataChangedEventHandler CollectionChanged
        {
            add
            {
                this.Children.CollectionChanged += value;
            }
            remove
            {
                this.Children.CollectionChanged -= value;
            }
        }

        /// <summary>
        /// 集合修改的修改类型。可以为 ViChangeType 中的常量定义。
        /// </summary>
        public virtual uint ChangeType
        {
            get
            {
                return this.Children.ChangeType;
            }
            set
            {
                this.Children.ChangeType = value;
            }
        }

        //
        // Summary:
        //     Inserts an element into the System.Collections.ObjectModel.Collection<T>
        //     at the specified index.
        //
        // Parameters:
        //   index:
        //     The zero-based index at which item should be inserted.
        //
        //   item:
        //     The object to insert. The value can be null for reference types.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is less than zero.  -or- index is greater than System.Collections.ObjectModel.Collection<T>.Count.
        public void _InsertAt(int index, object item)
        {
            this.Children._InsertAt(index, item);
        }

        //
        // Summary:
        //     Removes the element at the specified index of the System.Collections.ObjectModel.Collection<T>.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the element to remove.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is less than zero.  -or- index is equal to or greater than System.Collections.ObjectModel.Collection<T>.Count.
        public void _RemoveAt(int index)
        {
            this.Children._RemoveAt(index);
        }

        //
        // Summary:
        //     Replaces the element at the specified index.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the element to replace.
        //
        //   item:
        //     The new value for the element at the specified index.
        public void _SetItem(int index, object item)
        {
            this.Children._SetItem(index, item);
        }

        #endregion

        #region Dependency Property Changed

        /// <summary>
        /// 对象发生变化时的通知事件。
        /// </summary>
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

        #endregion
    }

    /// <summary>
    /// 父对象类型已经确定的 SortedTreeObject 类。
    /// </summary>
    /// <typeparam name="PT">父对象类型</typeparam>
    public class SortedTreeObject<PT> : SortedTreeObject
        where PT : ViObject
    {
        #region Life cycle

        /// <summary>
        /// 构建对象。
        /// </summary>
        public SortedTreeObject()
            : base()
        {
        }

        /// <summary>
        /// 构建指定名称的对象。
        /// </summary>
        /// <param name="name">对象名称</param>
        public SortedTreeObject(string name)
            : base(name)
        {
        }

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
