/// <summary>
/// @file   ProjectManager.Files.cs
///	@brief  ViGET 工程数据管理器管理工程使用的文件列表的模块。
/// @author	DothanTech 刘伟宏
/// 
/// Copyright(C) 2011~2014, DothanTech. All rights reserved.
/// </summary>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows;
using System.ComponentModel;

using System.Diagnostics;
using System.Collections;
using System.Text.RegularExpressions;

namespace DothanTech.ViGET.Manager
{
    public partial class ProjectManager
    {
        #region 密码

        //public void DoSetPassword()
        //{
        //    IniFile iniFile = new IniFile(this.ProjectFile);
        //    ViObject objPassword = new ViNamedObject();
        //    objPassword.Password = iniFile.GetValueS("STATION", "MYVERSION").Trim();

        //    Dothan.Controls.SetPassword dlg = new Dothan.Controls.SetPassword
        //        (!objPassword.NeedOriginal ? (Func<string, bool>)null :
        //        (password) =>
        //        {
        //            return objPassword.CheckPassword(password);
        //        },
        //        (password) =>
        //        {
        //            objPassword.SetPassword(password);
        //            iniFile.SetValue("STATION", "MYVERSION", objPassword.Password ?? "");
        //        });
        //    new WindowInteropHelper(dlg) { Owner = HwndHelper.GetForegroundWindow() };
        //    dlg.ShowName = System.IO.Path.GetFileNameWithoutExtension(this.ProjectFile);
        //    dlg.ShowDialog();
        //}

        //public static new bool CheckPassword(string projectFile)
        //{
        //    if (string.IsNullOrEmpty(projectFile))
        //        return true;

        //    IniFile iniFile = new IniFile(projectFile);
        //    ViObject objPassword = new ViNamedObject();
        //    objPassword.Password = iniFile.GetValueS("STATION", "MYVERSION").Trim();
        //    if (objPassword.HasPassword)
        //    {
        //        bool checkOK = false;

        //        CheckPassword dig = new CheckPassword((pwd => objPassword.CheckPassword(pwd)), new Action(() => checkOK = true));
        //        new WindowInteropHelper(dig) { Owner = HwndHelper.GetForegroundWindow() };
        //        dig.ShowName = System.IO.Path.GetFileNameWithoutExtension(projectFile);
        //        dig.ShowDialog();

        //        return checkOK;
        //    }

        //    return true;
        //}

        #endregion

    }
}
