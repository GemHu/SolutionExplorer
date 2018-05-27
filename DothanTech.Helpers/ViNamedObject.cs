/// <summary>
/// @file   ViNamedObject.cs
///	@brief  ViGET 编辑器中有名称的数据对象的基类。
/// @author	DothanTech 刘伟宏
/// 
/// Copyright(C) 2011~2014, DothanTech. All rights reserved.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;

namespace DothanTech.Helpers
{
    /// <summary>
    /// 有名称的 ViGET 数据对象。
    /// </summary>
    public class ViNamedObject : ViObject, IComparable<ViNamedObject>, IComparable
    {
        #region Life cycle

        /// <summary>
        /// 构建对象。
        /// </summary>
        public ViNamedObject()
        {
        }

        /// <summary>
        /// 构建指定名称的对象。
        /// </summary>
        /// <param name="name">对象名称</param>
        public ViNamedObject(string name)
            : this()
        {
            this.Name = name;
        }

        #endregion

        #region D Name

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(ViNamedObject),
                                        new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// 对象名称。
        /// </summary>
        public virtual string Name
        {
            get
            {
                return (string)GetValue(NameProperty);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    ClearValue(NameProperty);
                else
                    SetValue(NameProperty, value);
            }
        }

        #endregion

        #region D Type Property

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(object), typeof(ViNamedObject),
                                        new PropertyMetadata(null));

        public object Type
        {
            get { return (object)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        #endregion

        #region Key

        /// <summary>
        /// 对象关键字。
        /// </summary>
        public virtual string Key
        {
            get
            {
                string Name = this.Name;
                return (Name == null ? null : Name.ToUpper());
            }
        }

        #endregion

        #region Path

        /// <summary>
        /// 路径分隔符。
        /// </summary>
        public const char PathSeperator = '\\';

        /// <summary>
        /// 得到对象的路径，以路径分隔符分隔。
        /// </summary>
        /// <returns>对象路径</returns>
        public virtual string GetPath()
        {
            return this.GetPath(null);
        }

        /// <summary>
        /// 得到对象的路径，以路径分隔符分隔。
        /// </summary>
        /// <param name="stopObject">
        /// 搜索到指定的祖先对象，就停止路径的搜索。路径中不包括 stopObject 的信息。
        /// 搜索过程中如果碰到非 ViNamedObject 类型的对象，或者对象的名称为空时，
        /// 也会主动停止路径的递归。
        /// </param>
        /// <returns>对象路径</returns>
        public virtual string GetPath(ViObject stopObject)
        {
            // 从自己开始
            ViNamedObject obj = this;
            string path = obj.Name;
            while (true)
            {
                // 已经到了指定的祖先对象？
                if (obj.Parent == stopObject)
                    break;

                // 必须要是有名字的对象
                obj = obj.Parent as ViNamedObject;
                if (obj == null)
                    break;
                if (string.IsNullOrEmpty(obj.Name))
                    break;

                // 形成路径名称
                path = obj.Name + PathSeperator + path;
            }

            return path;
        }

        #endregion

        #region IComparable

        /// <summary>
        /// 与另一个 ViNamedObject 对象进行比较。对象数组排序时使用。
        /// </summary>
        /// <param name="other">另一个对象</param>
        /// <returns> > 0, == 0, < 0 </returns>
        public virtual int CompareTo(ViNamedObject other)
        {
            if (other == null)
                return 1;
            return string.Compare(this.Name, other.Name, true);
        }

        /// <summary>
        /// 与另一个对象进行比较，会被转换为 ViNamedObject 对象之后再进行比较。对象数组排序时使用。
        /// </summary>
        /// <param name="other">另一个对象</param>
        /// <returns> > 0, == 0, < 0 </returns>
        public virtual int CompareTo(object other)
        {
            return this.CompareTo(other as ViNamedObject);
        }

        /// <summary>
        /// 判断两对象是否相同？
        /// </summary>
        public virtual bool Equals(ViNamedObject other)
        {
            if (other == null)
                return false;
            return CompareTo(other) == 0;
        }

        /// <summary>
        /// 判断两对象是否相同？
        /// </summary>
        public virtual bool Equals(object other)
        {
            if (other == null)
                return false;
            return CompareTo(other) == 0;
        }

        #endregion
    }

    /// <summary>
    /// 父对象类型已经确定的 ViNamedObject 类。
    /// </summary>
    /// <typeparam name="PT">父对象类型</typeparam>
    public class ViNamedObject<PT> : ViNamedObject
        where PT : ViObject
    {
        #region Life cycle

        /// <summary>
        /// 构建对象。
        /// </summary>
        public ViNamedObject()
            : base()
        {
        }

        /// <summary>
        /// 构建指定名称的对象。
        /// </summary>
        /// <param name="name">对象名称</param>
        public ViNamedObject(string name)
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
