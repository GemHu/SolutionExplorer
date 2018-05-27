using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DothanTech.ViGET.ViService
{
    public interface IGitManager
    {
        /// <summary>
        /// 初始化Git管理器，在初始化的时候需要制定项目所在位置；
        /// </summary>
        /// <param name="location"></param>
        void Init(String location);

        /// <summary>
        /// 是否已经初始化Git仓库；
        /// </summary>
        bool HasCreated { get; }

        /// <summary>
        /// 如果未创建Git仓库，则创建Git仓库；
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        bool Create(String[] ignores = null);

        /// <summary>
        /// 执行git diff命令；
        /// </summary>
        void Diff(String path = null);

        /// <summary>
        /// 提交代码；
        /// </summary>
        void Commit(String path = null);

        /// <summary>
        /// 查看git提交记录
        /// </summary>
        void ShowLog(String path = null);
    }
}
