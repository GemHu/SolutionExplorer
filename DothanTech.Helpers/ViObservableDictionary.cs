/// <summary>
/// @file   ViObservableDictionary.cs
///	@brief  ViGET 中使用的 ObservableCollection 集合对象，支持集合元素变化通知事
///	        件，同时支持集合中元素依赖属性的变化通知事件。
///	        相对于 ViObservableCollection，内部使用了字典来提高 Contains 的效率。
/// @author	DothanTech 刘伟宏
/// 
/// Copyright(C) 2011~2014, DothanTech. All rights reserved.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace DothanTech.Helpers
{
    /// <summary>
    /// ViGET 中使用的 ObservableCollection 集合对象，支持集合元素变化通知事
    /// 件，同时支持集合中元素依赖属性的变化通知事件。
    ///	相对于 ViObservableCollection，内部使用了字典来提高 Contains 的效率。
    /// </summary>
    public class ViObservableDictionary<T> : ViObservableCollection<T>
    {
        protected Dictionary<T, byte> dicItems = new Dictionary<T, byte>();

        /// <summary>
        /// 构造对象。
        /// </summary>
        public ViObservableDictionary()
        {
        }

        /// <summary>
        /// 构造对象。
        /// </summary>
        public ViObservableDictionary(uint collChangeType)
            : base(collChangeType)
        {
        }

        /// <summary>
        /// 使用内部字典来实现 Contains 函数，提高效率。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public new bool Contains(T item)
        {
            return this.dicItems.ContainsKey(item);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        foreach (T v in e.OldItems)
                            this.dicItems.Remove(v);
                    }
                    if (e.NewItems != null)
                    {
                        foreach (T v in e.NewItems)
                            this.dicItems[v] = 1;
                    }
                    break;
            }
        }
    }
}
