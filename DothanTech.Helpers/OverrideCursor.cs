/// <summary>
/// @file   OverrideCursor.cs
///	@brief  提供程序全局鼠标对象控制的功能，比方说显示沙漏鼠标、自动恢复鼠标状态。
/// @author	DothanTech 刘伟宏
/// 
/// Copyright(C) 2011~2014, DothanTech. All rights reserved.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;

namespace DothanTech.Helpers
{
    /// <summary>
    /// 提供程序全局鼠标对象控制的功能。
    /// </summary>
    public class OverrideCursor : IDisposable
    {
        static protected OverrideCursor s_Instance = null;
        static protected Cursor s_SavedCursor = null;

        /// <summary>
        /// 设置程序全局鼠标样式，同时会保存设置之前的鼠标样式，以便于恢复鼠标状态。
        /// </summary>
        /// <param name="newCursor">新鼠标样式。</param>
        public OverrideCursor(Cursor newCursor)
        {
            if (s_Instance == null)
            {
                s_Instance = this;
                s_SavedCursor = Mouse.OverrideCursor;
                Mouse.OverrideCursor = newCursor;
            }
        }

        public void Dispose()
        {
            if (this == s_Instance)
            {
                Mouse.OverrideCursor = s_SavedCursor;
                s_Instance = null;
                s_SavedCursor = null;
            }
        }
    }

    /// <summary>
    /// 提供显示沙漏鼠标、自动恢复鼠标状态功能。
    /// </summary>
    public class OverrideBusyCursor : OverrideCursor
    {
        public OverrideBusyCursor() : base(Cursors.Wait) { }

        public OverrideBusyCursor(bool autoDispose)
            : this()
        {
            if (autoDispose && this == s_Instance)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                    {
                        this.Dispose();
                    }), DispatcherPriority.ApplicationIdle);
            }
        }
    }
}
