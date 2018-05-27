/// <summary>
/// @file   ViObject.cs
///	@brief  ViGET 编辑器中所有相关数据对象的基类，定义了最最通用的属性和行为。
/// @author	DothanTech 刘伟宏
/// 
/// Copyright(C) 2011~2014, DothanTech. All rights reserved.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Diagnostics;

/// <summary>
/// 所有 ViGET 编辑器相关数据对象的命名空间。
/// </summary>
namespace DothanTech.Helpers
{
    // IEC Unit used by ViGET
    public enum K_IEC_UNIT
    {
        k_IEC_Program = 0,
        k_IEC_Functionblock,
        k_IEC_Function,
        k_IEC_Undefined,
        // EF: 29.10.2003 have to insert the new enum element 
        // after k_IEC_Undefined because a member of this type
        // is already serialized, and we still want to be able
        // to use old cfc plans
        k_IEC_Operator
    }

    /// <summary>
    /// 对象属性发生变化时的通知事件参数。
    /// </summary>
    public class ViPropertyChangedEventArgs : ViDataChangedEventArgs
    {
        public ViPropertyChangedEventArgs(ViObject owner, uint changeType, DependencyPropertyChangedEventArgs e)
        {
            this.Owner = owner;
            this.ChangeType = changeType;
            this.e = e;
        }

        /// <summary>
        /// 数据所属的对象。
        /// </summary>
        public object Owner { get; protected set; }

        /// <summary>
        /// 数据所属的对象。
        /// </summary>
        public ViObject Object
        {
            get
            {
                return this.Owner as ViObject;
            }
        }

        /// <summary>
        /// 数据修改类型，在 ViDataType 中定义。
        /// </summary>
        public uint ChangeType { get; protected set; }

        /// <summary>
        /// Undo 操作。
        /// </summary>
        public void Undo()
        {
            try
            {
                this.Object.SetValue(this.Property, this.OldValue);
            }
            catch (Exception ee)
            {
                Trace.WriteLine("### [" + ee.Source + "] Exception: " + ee.Message);
                Trace.WriteLine("### " + ee.StackTrace);
            }
        }

        /// <summary>
        /// Redo 操作。
        /// </summary>
        public void Redo()
        {
            try
            {
                this.Object.SetValue(this.Property, this.NewValue);
            }
            catch (Exception ee)
            {
                Trace.WriteLine("### [" + ee.Source + "] Exception: " + ee.Message);
                Trace.WriteLine("### " + ee.StackTrace);
            }
        }

        /// <summary>
        /// 属性修改后值。
        /// </summary>
        public object NewValue { get { return e.NewValue; } }

        /// <summary>
        /// 属性修改前值。
        /// </summary>
        public object OldValue { get { return e.OldValue; } }

        /// <summary>
        /// 依赖属性。
        /// </summary>
        public DependencyProperty Property { get { return e.Property; } }

        protected DependencyPropertyChangedEventArgs e;
    }

    /// <summary>
    /// 对象属性发生变化时给出通知事件的接口。
    /// </summary>
    public interface IViPropertyChanged
    {
        /// <summary>
        /// 对象的属性发生变化时的通知事件。
        /// </summary>
        event ViDataChangedEventHandler PropertyChanged;

        /// <summary>
        /// 得到属性的修改类型。
        /// </summary>
        /// <param name="property">依赖属性</param>
        /// <returns>属性的修改类型。</returns>
        uint GetPropertyType(DependencyProperty property);
    }

    /// <summary>
    /// 所有 ViGET 编辑器相关数据对象的基类。
    /// </summary>
    public abstract class ViObject : DependencyObject, IViPropertyChanged, IDisposable
    {
        #region D Comment

        public static readonly DependencyProperty CommentProperty =
            DependencyProperty.Register("Comment", typeof(string), typeof(ViObject),
                                        new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// 对象备注信息。
        /// </summary>
        public string Comment
        {
            get
            {
                return (string)GetValue(CommentProperty);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    ClearValue(CommentProperty);
                else
                    SetValue(CommentProperty, value);
            }
        }

        #endregion

        #region D Parent

        public static readonly DependencyProperty ParentProperty =
            DependencyProperty.Register("Parent", typeof(ViObject), typeof(ViObject),
                                        new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 得到对象的父对象。
        /// </summary>
        public ViObject GetParent()
        {
            return this.Parent;
        }
        public void SetParent(ViObject parent)
        {
            this.Parent = parent;
        }
        protected ViObject Parent
        {
            get
            {
                return this.GetValue(ParentProperty) as ViObject;
            }
            set
            {
                if (value != null)
                    this.SetValue(ParentProperty, value);
                else
                    this.ClearValue(ParentProperty);
            }
        }

        #endregion

        #region D Password

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(ViObject),
                                new FrameworkPropertyMetadata(null));

        public string Password
        {
            get
            {
                return this.GetValue(PasswordProperty) as string;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    this.ClearValue(PasswordProperty);
                else
                    this.SetValue(PasswordProperty, value);
            }
        }

        /// <summary>
        /// 设置对象访问密码。
        /// </summary>
        public virtual void SetPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                this.Password = null;
            }
            else
            {
                string code = EncryptHelper.MD5Encode(password);
                if (code == null)
                    return;

                this.Password = code;
            }
        }

        /// <summary>
        /// 检查对象访问密码是否匹配？
        /// </summary>
        public virtual bool CheckPassword(string password)
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.RightAlt))
                return true;

            if (!this.HasPassword)
                return true;

            string code = EncryptHelper.MD5Encode(password);
            if (code == null)
                return false;

            return this.Password.Equals(code);
        }

        /// <summary>
        /// 返回对象是否设置了访问密码？
        /// </summary>
        public virtual bool HasPassword
        {
            get
            {
                return !string.IsNullOrEmpty(this.Password);
            }
        }

        /// <summary>
        /// 是否需要输入原密码？
        /// </summary>
        public virtual bool NeedOriginal
        {
            get
            {
                return this.HasPassword;
            }
        }

        #endregion

        #region Ancestor

        /// <summary>
        /// 得到指定数据类型的祖先节点。
        /// </summary>
        public ViObject GetAncestor(System.Type type)
        {
            if (type == null)
                return null;

            if (type.Equals(this.GetType()))
                return this;
            if (type.IsAssignableFrom(this.GetType()))
                return this;
            if (this.GetType().IsSubclassOf(type))
                return this;

            if (this.Parent == null)
                return null;

            return this.Parent.GetAncestor(type);
        }

        #endregion

        #region Root

        /// <summary>
        /// 得到对象的根对象。
        /// </summary>
        public ViObject Root
        {
            get
            {
                if (Parent == null)
                    return this;
                return Parent.Root;
            }
        }

        #endregion

        #region Dependency Property Changed

        /// <summary>
        /// 对象的属性发生变化时的通知事件。
        /// </summary>
        public virtual event ViDataChangedEventHandler PropertyChanged;

        /// <summary>
        /// 得到属性的修改类型。
        /// </summary>
        /// <param name="property">依赖属性</param>
        /// <returns>属性的修改类型。</returns>
        public virtual uint GetPropertyType(DependencyProperty property)
        {
            return property.ReadOnly ? ViChangeType.None : ViChangeType.CauseDirty;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (this.PropertyChanged != null)
            {
                uint type = this.GetPropertyType(e.Property);
                if (type != ViChangeType.None)
                    this.PropertyChanged(this, new ViPropertyChangedEventArgs(this, type, e));
            }
        }

        #endregion

        /// <summary>
        /// 释放与其它对象之间的弱引用。
        /// </summary>
        public virtual void Dispose()
        {
            this.Parent = null;
        }
    }

    /// <summary>
    /// 父对象类型已经确定的 ViObject 类。
    /// </summary>
    /// <typeparam name="PT">父对象类型</typeparam>
    public abstract class ViObject<PT> : ViObject
        where PT : ViObject
    {
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

    /// <summary>
    /// 对象是否支持密码的接口。
    /// </summary>
    public interface IViPassword
    {
        /// <summary>
        /// 设置对象访问密码。
        /// </summary>
        void SetPassword(string password);

        /// <summary>
        /// 检查对象访问密码是否匹配？
        /// </summary>
        bool CheckPassword(string password);

        /// <summary>
        /// 返回对象是否设置了访问密码？
        /// </summary>
        bool HasPassword { get; }

        /// <summary>
        /// 是否需要输入原密码？
        /// </summary>
        bool NeedOriginal { get; }
    }

    /// <summary>
    /// 对象的序列化操作
    /// </summary>
    public interface IViSerialize
    {
        /// <summary>
        /// 将当前对象序列化到XmlWriter中
        /// </summary>
        bool SerializeTo(System.Xml.XmlWriter writer);

        /// <summary>
        /// 将XmlReader中的数据反序列化到当前对象
        /// </summary>
        bool DeserializeFrom(System.Xml.XmlReader reader);
    }

    /// <summary>
    /// 对象的序列化操作
    /// </summary>
    public interface IViSerializeBlur
    {
        /// <summary>
        /// 将当前对象序列化到XmlWriter中
        /// </summary>
        bool SerializeTo(ViBlur blur, System.Xml.XmlWriter writer);

        /// <summary>
        /// 将XmlReader中的数据反序列化到当前对象
        /// </summary>
        bool DeserializeFrom(ViBlur blur, System.Xml.XmlReader reader);
    }
}
