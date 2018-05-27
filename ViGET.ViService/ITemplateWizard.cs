using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DothanTech.ViGET.ViService
{
    public interface ITemplateWizard
    {
        /// <summary>
        /// 运行新建项目向导；
        /// </summary>
        /// <param name="callback">
        /// Action参数：
        /// 参数1：项目文件全路径；
        /// 参数2：项目名称；
        /// 参数3：项目类型；
        /// 参数4：Solution名称；
        /// 参数5：Solution路径；
        /// </param>
        void RunNewProjectWizard(Action<String, String, String> callback);
        
        /// <summary>
        /// 运行在指定Solution上添加新项目向导；
        /// </summary>
        /// <param name="location">需要添加项目的Solution路径；</param>
        /// <param name="callback">
        /// 回调参数：
        /// 参数1：项目文件全路径；
        /// 参数2：项目类型；
        /// </param>
        void RunAddNewProjectWizard(String location, Action<String, String, String> callback);
        
        /// <summary>
        /// 运行新建文件向导；
        /// </summary>
        /// <param name="location">新建文件的目标路径；</param>
        /// <param name="containerType">需要添加项所述类型</param>
        /// <param name="callback">添加文件结果回调函数
        /// 回调参数：
        /// 参数1：添加后的文件全路径；
        /// 参数2：添加的文件模板类型；
        /// </param>
        /// <param name="invalidCheck">名称合法性检测回调函数；
        /// 参数1：待检测文件名；
        /// 参数2：返回值，合法与否；
        /// </param>
        void RunCreateFileWizard(String location, String containerType, Action<String, String, String> callback, Func<String, String, bool> invalidCheck = null);
    }
}
