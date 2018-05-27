using DothanTech.ViGET.ViService;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DothanTech.ViGET.TemplateWizard
{
    /// <summary>
    /// 项目向导入口类；
    /// 
    /// 可以通过该类弹出相关向导对话框，譬如：
    /// 新建项目；
    /// 添加新项目；
    /// 添加文件等；
    /// </summary>
    [Export(typeof(ITemplateWizard))]
    public class TemplateWizard : ITemplateWizard
    {
        /// <summary>
        /// 运行新建项目向导；
        /// </summary>
        /// <param name="callback">
        /// Action参数：
        /// 参数1：项目文件全路径；
        /// 参数3：项目类型；
        /// </param>
        public void RunNewProjectWizard(Action<String, String, String> callback)
        {
            try
            {
                TemplateWizardDialog.ShowDialog(null, null, callback, null);
            }
            catch (Exception ee)
            {
                Trace.WriteLine("### [" + ee.Source + "] Exception: " + ee.Message);
                Trace.WriteLine("### " + ee.StackTrace);
            }
        }

        /// <summary>
        /// 运行在指定Solution上添加新项目向导；
        /// </summary>
        /// <param name="location">需要添加项目的Solution路径；</param>
        /// <param name="callback">
        /// 回调参数：
        /// 参数1：项目文件全路径；
        /// 参数3：项目类型；
        /// </param>
        public void RunAddNewProjectWizard(String location, Action<String, String, String> callback)
        {
            if (!Directory.Exists(location))
                return;

            try
            {
                TemplateWizardDialog.ShowDialog(location, null, callback, null);
            }
            catch (Exception ee)
            {
                Trace.WriteLine("### [" + ee.Source + "] Exception: " + ee.Message);
                Trace.WriteLine("### " + ee.StackTrace);
            }
        }

        /// <summary>
        /// 运行新建文件向导；
        /// </summary>
        /// <param name="location">新建文件的目标路径；</param>
        /// <param name="callback">添加文件结果回调函数
        /// 回调参数：
        /// 参数1：添加后的文件全路径；
        /// 参数2：添加的文件名称；
        /// </param>
        public void RunCreateFileWizard(string location, string containerType, Action<string, string, String> callback, Func<string, String, bool> invalidCheck = null)
        {
            if (!Directory.Exists(location) || String.IsNullOrEmpty(containerType))
                return;

            try
            {
                TemplateWizardDialog.ShowDialog(location, containerType, callback, invalidCheck);
            }
            catch (Exception ee)
            {
                Trace.WriteLine("### [" + ee.Source + "] Exception: " + ee.Message);
                Trace.WriteLine("### " + ee.StackTrace);
            }
        }
    }
}
