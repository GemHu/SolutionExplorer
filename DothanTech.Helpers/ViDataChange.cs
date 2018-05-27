/// <summary>
/// @file   ViDataChange.cs
///	@brief  ViGET 编辑器数据修改相关的类和操作。
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
    /// 用来定义属性对修改的影响类型的常量。基类使用最高字节，派生类可以使用剩下的
    /// 三个字节，用以区分属性修改的影响，但是最高字节必须或成对应的类型值。
    /// </summary>
    public static class ViChangeType
    {
        public const uint None = 0x00000000;            ///< 不需要关心的属性修改
        public const uint CauseDirty = 0x01000000;      ///< 需要关心的属性修改，同时会引起文档的修改
        public const uint NotCauseDirty = 0x02000000;   ///< 需要关心的属性修改，同时不会引起文档的修改
        public const uint NotCauseCache = 0x04000000;   ///< 不需要关心的属性修改，同时不会引起文档的修改，但是需要刷新界面
    }

    /// <summary>
    /// 数据发生变化的通知事件参数。
    /// </summary>
    public interface ViDataChangedEventArgs
    {
        /// <summary>
        /// 数据所属的对象。
        /// </summary>
        object Owner { get; }

        /// <summary>
        /// 数据修改类型，在 ViChangeType 中定义。
        /// </summary>
        uint ChangeType { get; }

        /// <summary>
        /// 依赖属性。
        /// </summary>
        DependencyProperty Property { get; }

        /// <summary>
        /// 属性修改后值。
        /// </summary>
        object NewValue { get; }

        /// <summary>
        /// Undo 操作。
        /// </summary>
        void Undo();

        /// <summary>
        /// Redo 操作。
        /// </summary>
        void Redo();
    }

    /// <summary>
    /// 对象属性发生变化时的通知委托。
    /// </summary>
    public delegate void ViDataChangedEventHandler(object sender, ViDataChangedEventArgs e);
}
