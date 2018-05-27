/// <summary>
/// @file   ViewHelper.cs
///	@brief  提供 Control 操作的辅助函数。
/// @author	DothanTech 刘伟宏
/// 
/// Copyright(C) 2011~2014, DothanTech. All rights reserved.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace DothanTech.Helpers
{
	/// <summary>
	/// 提供 Control 操作的辅助函数。
	/// </summary>
    public class ViewHelper
    {
		/// <summary>
		/// 通过给定的依赖对象，在控件树中向上查找指定类型的控件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
        public static T FindVisualParent<T>(DependencyObject obj) where T : class
        {
            while (obj != null)
            {
                if (obj is T)
                    return obj as T;

                obj = VisualTreeHelper.GetParent(obj);
            }

            return null;
        }

		/// <summary>
		/// 在给定的空间中，查找给定坐标点的给定类型的控件
		/// </summary>
        public static T HitTestView<T>(Visual visual, Point pos) where T : class
        {
            HitTestResult result = VisualTreeHelper.HitTest(visual, pos);
            if (result == null)
                return null;
            DependencyObject obj = result.VisualHit;
            
			return FindVisualParent<T>(obj);
        }
    }
}
