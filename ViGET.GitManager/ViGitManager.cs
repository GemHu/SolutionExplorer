using DothanTech.ViGET.ViService;
using GitSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DothanTech.ViGET.GitManager
{
    /// <summary>
    /// GitSharp有点坑，git初始化，需要借助于 GitSharp.Core.Repository类来完成；
    /// 但是git的其他管理，需要借助于 GitSharp.Repository类来完成；
    /// </summary>
    [Export(typeof(IGitManager))]
    public class ViGitManager : IGitManager
    {
        #region 相关属性

        private String solutionPath;

        public Repository TheRepository { get; private set; }

        public bool HasCreated
        {
            get
            {
                return this.TheRepository != null;
            }
        }

        #endregion

        #region Life Cycle

        private void Reset()
        {
            this.TheRepository = null;
            this.solutionPath = String.Empty;
        }

        /// <summary>
        /// 初始化 Git管理器；
        /// </summary>
        /// <param name="location"></param>
        public void Init(String location)
        {
            // 由于GitManager是一个全局的属性，所以在初始化的时候需要清除旧的数据；
            this.Reset();
            if (!Directory.Exists(location))
                return;

            this.solutionPath = location;
            String url = Repository.FindRepository(location);
            if (!Repository.IsValid(url))
                return;

            this.TheRepository = new Repository(url);
        }

        /// <summary>
        /// 如果指定的仓库不存在，则创建 新的Git仓库；
        /// </summary>
        /// <param name="folder">需要创建Git仓库的文件夹</param>
        /// <returns>成功与否？</returns>
        public bool Create(String[] ignores = null)
        {
            if (!Directory.Exists(this.solutionPath))
                return false;
            if (this.TheRepository != null)
                return true;

            Repository repo = Repository.Init(this.solutionPath);
            if (repo == null)
                return false;

            this.TheRepository = repo;
            // 创建ignore文件；

            String fullPath = Path.GetDirectoryName(this.TheRepository.Directory);
            var ignoreFile = Path.Combine(fullPath, ".gitignore");
            if (File.Exists(ignoreFile))
                return true;

            if (ignores == null)
                ignores = new String[0];
            File.WriteAllLines(ignoreFile, ignores);

            return true;
        }

        #endregion

        /// <summary>
        /// 查看有改动的文件；
        /// </summary>
        public void Diff(String path = null)
        {
            DiffDialog.ShowModelDialog(this.TheRepository, path);
        }

        /// <summary>
        /// 显示代码提交对话框；
        /// </summary>
        public void Commit(String path = null)
        {
            CommitDialog.ShowModelDialog(this.TheRepository, path);
        }

        /// <summary>
        /// 查看提交记录对话框；
        /// </summary>
        public void ShowLog(String path = null)
        {
            ShowLogDialog.ShowModelDialog(this.TheRepository);
        }
    }
}
