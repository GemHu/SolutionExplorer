/// <summary>
/// @file   UnsortTreeObject.cs
///	@brief  ViGET 编辑器中所有具有子对象的对象的基类，子对象集合是按照加入集合的顺序存放的。
/// @author	DothanTech 刘伟宏
/// 
/// Copyright(C) 2011~2014, DothanTech. All rights reserved.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DothanTech.Helpers
{
    /// <summary>
    /// ViGET 编辑器中所有具有子对象的对象的基类，子对象集合是按照加入集合的顺序存放的。
    /// </summary>
    public class UnsortTreeObject : SortedTreeObject
    {
        #region Life cycle

        /// <summary>
        /// 构建对象。
        /// </summary>
        public UnsortTreeObject()
        {
        }

        /// <summary>
        /// 构建指定名称的对象。
        /// </summary>
        /// <param name="name">对象名称</param>
        public UnsortTreeObject(string name)
            : base(name)
        {
        }

        #endregion

        #region Children

        /// <summary>
        /// 添加子节点到子节点集合最后。
        /// </summary>
        /// <param name="child">需要添加的子节点</param>
        public override void AddChild(ViNamedObject child)
        {
            this.Children.Add(child);

            // 注意：SetParent 不能放到 Add(child)前面，因为此时功能块的 PropertyChanged有可能为null，所以功能块在设置Parent的时候，
            //有可能没法通知UndoEngine来进行记录属性的修改。
            if (this.Owner != null)
                child.SetParent(this);
        }

        /// <summary>
        /// 在指定的索引位置，插入子节点。
        /// </summary>
        /// <param name="index">索引位置，从 0 开始</param>
        /// <param name="child">要插入的子节点</param>
        public virtual void InsertChild(int index, ViNamedObject child)
        {
            if (this.Owner != null)
                child.SetParent(this);

            this.Children.Insert(index, child);
        }

        /// <summary>
        /// 将子节点移动到新的索引位置。
        /// </summary>
        /// <param name="newIndex">新的索引位置</param>
        /// <param name="child">要移动位置的子节点</param>
        /// <returns>成功与否？</returns>
        public virtual bool MoveChild(int newIndex, ViNamedObject child)
        {
            if (this.Owner != null)
            {
                if (child.GetParent() != this)
                    return false;
            }

            int oldIndex = this.IndexOfChild(child);
            if (oldIndex == newIndex)
                return true;
            if (oldIndex > newIndex)
                ++newIndex;

            this.Children.RemoveAt(oldIndex);
            this.InsertChild(newIndex, child);

            return true;
        }

        /// <summary>
        /// 得到指定名称的子节点的集合索引，< 0 表示没有找到
        /// </summary>
        /// <param name="name">子节点名称</param>
        /// <returns>子节点的集合索引，< 0 表示没有找到</returns>
        public override int IndexOfChild(string name, bool ignoreCase = true)
        {
            int index = 0;
            foreach (var v in this.Children)
            {
                if (string.Compare(v.Name, name, ignoreCase) == 0)
                    return index;
                ++index;
            }
            return -1;
        }

        #endregion
    }

    /// <summary>
    /// 父对象类型已经确定的 UnsortTreeObject 类。
    /// </summary>
    /// <typeparam name="PT">父对象类型</typeparam>
    public class UnsortTreeObject<PT> : UnsortTreeObject
        where PT : ViObject
    {
        #region Life cycle

        /// <summary>
        /// 构建对象。
        /// </summary>
        public UnsortTreeObject()
            : base()
        {
        }

        /// <summary>
        /// 构建指定名称的对象。
        /// </summary>
        /// <param name="name">对象名称</param>
        public UnsortTreeObject(string name)
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
            set
            {
                base.SetParent(value);
            }
        }

        #endregion
    }
}
