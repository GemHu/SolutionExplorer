/// <summary>
/// @file   MappedTreeObject.cs
///	@brief  ViGET 编辑器中所有具有子对象的对象的基类，子对象集合没有进行排序。但是对有名称的子对象，加上了字典的索引，以加快按照名字来查找的效率。
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
    /// ViGET 编辑器中所有具有子对象的对象的基类，子对象集合没有进行排序。但是对有名称的子对象，加上了字典的索引，以加快按照名字来查找的效率。
    /// </summary>
    public class MappedTreeObject : UnsortTreeObject
    {
        #region Life cycle

        /// <summary>
        /// 构建对象。
        /// </summary>
        public MappedTreeObject()
        {
            this.Children.CollectionChanged += Children_CollectionChanged;
        }

        /// <summary>
        /// 构建指定名称的对象。
        /// </summary>
        /// <param name="name">对象名称</param>
        public MappedTreeObject(string name)
            : base(name)
        {
            this.Children.CollectionChanged += Children_CollectionChanged;
        }

        /// <summary>
        /// 释放与其它对象之间的弱引用。
        /// </summary>
        public override void Dispose()
        {
            this.Children.CollectionChanged -= Children_CollectionChanged;
            this.ChildMap = null;

            base.Dispose();
        }

        #endregion

        #region Children Map

        protected Dictionary<string, ViNamedObject> ChildMap;

        void Children_CollectionChanged(object sender, ViDataChangedEventArgs e)
        {
            // 仅仅时删除 Map 信息，后面自动重新生成
            this.ChildMap = null;
        }

        /// <summary>
        /// 找到指定名称的子节点（不递归查找）。
        /// </summary>
        /// <param name="name">子节点名称</param>
        /// <returns>指定名称的子节点</returns>
        public new ViNamedObject ChildByName(string name)
        {
            if (this.ChildMap == null)
            {
                if (this.Children.Count <= 0)
                    return null;

                this.ChildMap = new Dictionary<string, ViNamedObject>(this.Children.Count);
                foreach (ViNamedObject child in this.Children)
                {
                    if (!string.IsNullOrEmpty(child.Name))
                        this.ChildMap[child.Name.ToUpper()] = child;
                }
            }

            name = name.ToUpper();
            if (this.ChildMap.ContainsKey(name))
                return this.ChildMap[name];

            return null;
        }

        #endregion
    }

    /// <summary>
    /// 父对象类型已经确定的 MappedTreeObject 类。
    /// </summary>
    /// <typeparam name="PT">父对象类型</typeparam>
    public class MappedTreeObject<PT> : MappedTreeObject
        where PT : ViObject
    {
        #region Life cycle

        /// <summary>
        /// 构建对象。
        /// </summary>
        public MappedTreeObject()
            : base()
        {
        }

        /// <summary>
        /// 构建指定名称的对象。
        /// </summary>
        /// <param name="name">对象名称</param>
        public MappedTreeObject(string name)
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
