/// <summary>
/// @file   ViObservableCollection.cs
///	@brief  ViGET 中使用的 ObservableCollection 集合对象，支持集合元素变化
///	        通知事件，同时支持集合中元素依赖属性的变化通知事件。
/// @author	DothanTech 刘伟宏
/// 
/// Copyright(C) 2011~2014, DothanTech. All rights reserved.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace DothanTech.Helpers
{
    /// <summary>
    /// 集合内容发生变化的通知事件参数。
    /// </summary>
    public class ViCollectionChangedEventArgs : ViDataChangedEventArgs
    {
        /// <summary>
        /// 标记集合内容发生变化的假的依赖属性。
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register("Collection", typeof(object), typeof(ViObject),
                                        new FrameworkPropertyMetadata(null));

        public ViCollectionChangedEventArgs(IViCollectionChanged owner, NotifyCollectionChangedEventArgs e)
        {
            this.Owner = owner;
            this.e = e;
        }

        /// <summary>
        /// 数据所属的对象。
        /// </summary>
        public object Owner { get; protected set; }

        /// <summary>
        /// 数据所属的对象。
        /// </summary>
        public IViCollectionChanged Collection
        {
            get
            {
                return this.Owner as IViCollectionChanged;
            }
        }

        /// <summary>
        /// 数据修改类型，在 ViChangeType 中定义。
        /// </summary>
        public uint ChangeType
        {
            get
            {
                return this.Collection.ChangeType;
            }
        }

        /// <summary>
        /// 依赖属性（假的）。
        /// </summary>
        public DependencyProperty Property
        {
            get
            {
                return CollectionProperty;
            }
        }

        /// <summary>
        /// 属性修改后值（假的）。
        /// </summary>
        public object NewValue
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Undo 操作。
        /// </summary>
        public void Undo()
        {
            try
            {
                IViCollectionChanged Collection = this.Collection;
                switch (this.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (this.NewItems != null)
                        {
                            int count = this.NewItems.Count;
                            for (int i = 0; i < count; ++i)
                                Collection._RemoveAt(this.NewStartingIndex + i);
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (this.OldItems != null)
                        {
                            int count = this.OldItems.Count;
                            for (int i = 0; i < count; ++i)
                                Collection._InsertAt(this.OldStartingIndex + i, this.OldItems[i]);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        if (this.OldItems != null)
                        {
                            int count = this.OldItems.Count;
                            for (int i = 0; i < count; ++i)
                                Collection._SetItem(this.OldStartingIndex + i, this.OldItems[i]);
                        }
                        break;
                }
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
                switch (this.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        if (this.NewItems != null)
                        {
                            int count = this.NewItems.Count;
                            for (int i = 0; i < count; ++i)
                                Collection._InsertAt(this.NewStartingIndex + i, this.NewItems[i]);
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        if (this.OldItems != null)
                        {
                            int count = this.OldItems.Count;
                            for (int i = 0; i < count; ++i)
                                Collection._RemoveAt(this.OldStartingIndex + i);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        if (this.NewItems != null)
                        {
                            int count = this.NewItems.Count;
                            for (int i = 0; i < count; ++i)
                                Collection._SetItem(this.NewStartingIndex + i, this.NewItems[i]);
                        }
                        break;
                }
            }
            catch (Exception ee)
            {
                Trace.WriteLine("### [" + ee.Source + "] Exception: " + ee.Message);
                Trace.WriteLine("### " + ee.StackTrace);
            }
        }

        // Summary:
        //     Gets the action that caused the event.
        //
        // Returns:
        //     A System.Collections.Specialized.NotifyCollectionChangedAction value that
        //     describes the action that caused the event.
        public NotifyCollectionChangedAction Action
        {
            get
            {
                return this.e.Action;
            }
        }

        //
        // Summary:
        //     Gets the list of new items involved in the change.
        //
        // Returns:
        //     The list of new items involved in the change.
        public IList NewItems
        {
            get
            {
                return this.e.NewItems;
            }
        }

        //
        // Summary:
        //     Gets the index at which the change occurred.
        //
        // Returns:
        //     The zero-based index at which the change occurred.
        public int NewStartingIndex
        {
            get
            {
                return this.e.NewStartingIndex;
            }
        }

        //
        // Summary:
        //     Gets the list of items affected by a System.Collections.Specialized.NotifyCollectionChangedAction.Replace,
        //     Remove, or Move action.
        //
        // Returns:
        //     The list of items affected by a System.Collections.Specialized.NotifyCollectionChangedAction.Replace,
        //     Remove, or Move action.
        public IList OldItems
        {
            get
            {
                return this.e.OldItems;
            }
        }

        //
        // Summary:
        //     Gets the index at which a System.Collections.Specialized.NotifyCollectionChangedAction.Move,
        //     Remove, ore Replace action occurred.
        //
        // Returns:
        //     The zero-based index at which a System.Collections.Specialized.NotifyCollectionChangedAction.Move,
        //     Remove, or Replace action occurred.
        public int OldStartingIndex
        {
            get
            {
                return this.e.OldStartingIndex;
            }
        }

        protected NotifyCollectionChangedEventArgs e;
    }

    /// <summary>
    /// 集合元素发生变化时，给出通知事件的接口定义。
    /// </summary>
    public interface IViCollectionChanged
    {
        /// <summary>
        /// 集合元素发生变化时的通知事件。
        /// </summary>
        event ViDataChangedEventHandler CollectionChanged;

        /// <summary>
        /// 集合修改的修改类型。可以为 ViChangeType 中的常量定义。
        /// </summary>
        uint ChangeType { get; }

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
        void _InsertAt(int index, object item);

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
        void _RemoveAt(int index);

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
        void _SetItem(int index, object item);
    }

    /// <summary>
    /// ViGET 中使用的 ObservableCollection 集合对象，支持集合元素变化通知
    /// 事件，同时支持集合中元素依赖属性的变化通知事件。
    /// </summary>
    public class ViObservableCollection<T> : ObservableCollection<T>, IViCollectionChanged, IViPropertyChanged
    {
        /// <summary>
        /// 集合修改的修改类型。可以为 ViChangeType 中的常量定义。
        /// </summary>
        public uint ChangeType { get; set; }

        /// <summary>
        /// 构造对象。
        /// </summary>
        public ViObservableCollection()
        {
            this.ChangeType = ViChangeType.CauseDirty;
        }

        /// <summary>
        /// 构造对象。
        /// </summary>
        public ViObservableCollection(uint collChangeType)
            : this()
        {
            this.ChangeType = collChangeType;
        }

        /// <summary>
        /// 集合元素发生变化时的通知事件。
        /// </summary>
        public new event ViDataChangedEventHandler CollectionChanged;

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
            if (item is T)
                base.Insert(index, (T)item);
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
            base.RemoveAt(index);
        }

        /// <summary>
        /// 清空所有数据，Clear()方法的替代函数。
        /// 当直接进行Clear操作的时候，有时候会弹出一些未知的异常，譬如：[ PresentationFramework] Exception: Range actions are not supported.
        /// </summary>
        public void RemoveAll()
        {
            while (this.Count > 0)
            {
                this.RemoveAt(this.Count - 1);
            }
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
            if (item is T)
                base.SetItem(index, (T)item);
        }

        /// <summary>
        /// 清空集合所有元素。系统自带的 Clear() 函数发出的 NotifyCollectionChangedAction.Reset
        /// 通知事件中，其 OldItems 并没有带上当前集合元素的信息，而这个信息非常重要。
        /// 因此此处只能重新实现 Clear() 函数，模拟正确的 NotifyCollectionChangedAction.Reset 事件。
        /// </summary>
        protected override void ClearItems()
        {
            if (this.Count > 0)
            {
                // 不能用 this，必须重新克隆一个 List
                T[] items = this.ToArray<T>();
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs
                    (NotifyCollectionChangedAction.Remove, items, 0);
                this.OnCollectionChanged(e);
            }

            base.ClearItems();
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (this.CollectionChanged != null)
            {
                this.CollectionChanged.Invoke(this, new ViCollectionChangedEventArgs(this, e));
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        if (this.ChildPropertyChanged != null)
                            RemovePropertyChanged(e.OldItems);
                    }
                    if (e.NewItems != null)
                    {
                        if (this.ChildPropertyChanged != null)
                            AddPropertyChanged(e.NewItems);
                    }
                    break;
            }
        }

        /// <summary>
        /// 对象的属性发生变化时的通知事件。
        /// </summary>
        public new event ViDataChangedEventHandler PropertyChanged
        {
            add
            {
                if (this.ChildPropertyChanged == null)
                    AddPropertyChanged(this);
                //
                this.CollectionChanged += value;
                this.ChildPropertyChanged += value;
            }
            remove
            {
                this.CollectionChanged -= value;
                this.ChildPropertyChanged -= value;
                //
                if (this.ChildPropertyChanged == null)
                    RemovePropertyChanged(this);
            }
        }
        protected event ViDataChangedEventHandler ChildPropertyChanged;

        /// <summary>
        /// 得到属性的修改类型。
        /// </summary>
        /// <param name="property">依赖属性</param>
        /// <returns>属性的修改类型。</returns>
        public virtual uint GetPropertyType(DependencyProperty property)
        {
            return property.ReadOnly ? ViChangeType.None : ViChangeType.CauseDirty;
        }

        protected void AddPropertyChanged(IList items)
        {
            foreach (var obj in items)
            {
                if (obj is IViPropertyChanged)
                {
                    (obj as IViPropertyChanged).PropertyChanged += child_PropertyChanged;
                }
                else if (obj is IViCollectionChanged)
                {
                    (obj as IViCollectionChanged).CollectionChanged += child_CollectionChanged;
                }
            }
        }

        protected void RemovePropertyChanged(IList items)
        {
            foreach (var obj in items)
            {
                if (obj is IViPropertyChanged)
                {
                    (obj as IViPropertyChanged).PropertyChanged -= child_PropertyChanged;
                }
                else if (obj is IViCollectionChanged)
                {
                    (obj as IViCollectionChanged).CollectionChanged -= child_CollectionChanged;
                }
            }
        }

        void child_CollectionChanged(object sender, ViDataChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged.Invoke(sender, e);
            }
        }

        void child_PropertyChanged(object sender, ViDataChangedEventArgs e)
        {
            if (this.ChildPropertyChanged != null)
            {
                this.ChildPropertyChanged(sender, e);
            }
        }
    }
}
