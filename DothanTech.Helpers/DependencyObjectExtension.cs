/// <summary>
/// @file   DependencyObjectExtension.cs
///	@brief  DependencyObject 类的扩展函数。
/// @author	DothanTech 刘伟宏
/// 
/// Copyright(C) 2011~2014, DothanTech. All rights reserved.
/// </summary>

using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Windows.Markup;

namespace DothanTech.Helpers
{
    /// <summary>
    /// DependencyObject 类的扩展函数。
    /// </summary>
    public static class DependencyObjectExtension
    {
        /// <summary>
        /// 对两依赖属性对象进行 XAML 级别的比较。
        /// </summary>
        /// <returns>相等与否？</returns>
        public static bool XamlEquals(this DependencyObject This, DependencyObject compareTo)
        {
            if (compareTo == null) return false;
            return (XamlWriter.Save(This) == XamlWriter.Save(compareTo));
        }

        /// <summary>
        /// 根据指定的依赖对象，克隆一个一样的对象。
        /// </summary>
        public static DependencyObject Clone(this DependencyObject This)
        {
            try
            {
                string strContent = XamlWriter.Save(This);
                return XamlReader.Parse(strContent) as DependencyObject;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
