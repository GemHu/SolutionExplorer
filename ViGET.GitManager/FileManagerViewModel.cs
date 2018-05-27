using DothanTech.Helpers;
using GitSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace DothanTech.ViGET.GitManager
{
    public class FileManagerViewModel : ViewModelBase
    {
        #region Life Cycle

        public FileManagerViewModel(Repository repository, String folder = null)
        {
            this.TheRepository = repository;
            this.CurrFolder = folder;
            this.PathStatusItems = new ObservableCollection<FileDiffItem>();
            this.Reload();
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 某些情况下禁止界面关闭；
            if (this.isChecking)
            {
                e.Cancel = true;
                return;
            }
        }

        private void Reload()
        {
            this.PathStatusItems.Clear();
            this.isChecking = true;
            // 更新ShowCommitCmd使能状态；
            CommandManager.InvalidateRequerySuggested();
            // 异步加载
            ThreadPool.QueueUserWorkItem(o =>
            {
                var options = new RepositoryStatusOptions()
                {
                    ForceContentCheck = false,
                    PerPathNotificationCallback = OnUpdateStatus
                };
                new RepositoryStatus(this.TheRepository, options);
                //this.TheRepository.GetDirectoryStatus(this.CurrFolder, true, options);
                // 检测完毕；
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.OnUpdateStatusFinish();
                }));
            });
        }

        /// <summary>
        /// 检测到文件；
        /// </summary>
        /// <param name="status"></param>
        private void OnUpdateStatus(PathStatus status)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                PathStatusItems.SortedAdd(new FileDiffItem(status));
            }));
        }

        /// <summary>
        /// diff检测完毕；
        /// </summary>
        private void OnUpdateStatusFinish()
        {
            this.isChecking = false;
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion

        #region Model

        private bool isChecking = false;
        /// <summary>
        /// Git仓库
        /// </summary>
        public Repository TheRepository { get; protected set; }

        /// <summary>
        /// 待管理文件夹路径；
        /// </summary>
        public String CurrFolder
        {
            get { return String.IsNullOrEmpty(this._currFolder) ? String.Empty : this._currFolder; }
            set { this._currFolder = value; }
        }
        private String _currFolder;

        public Window Owner
        {
            get { return this._owner; }
            set
            {
                if (this._owner != null)
                    this._owner.Closing -= OnWindowClosing;
                if (value != null)
                    value.Closing += OnWindowClosing;
                this._owner = value;
            }
        }

        private Window _owner;

        #endregion

        #region ViewModel Propertys

        public ObservableCollection<FileDiffItem> PathStatusItems
        {
            get { return _pathStatusItem; }
            set
            {
                _pathStatusItem = value;
                this.RaisePropertyChanged("PathStatusItems");
            }
        }
        private ObservableCollection<FileDiffItem> _pathStatusItem;

        public String CommitMessage
        {
            get { return _commitMessage; }
            set
            {
                _commitMessage = value;
                this.RaisePropertyChanged("CommitMessage");
            }
        }
        private String _commitMessage;

        #endregion

        #region Commands

        /// <summary>
        /// 提交相关代码到本地仓库；
        /// </summary>
        public RelayCommand CommitCmd
        {
            get
            {
                if (this._commitCmd == null)
                {
                    this._commitCmd = new RelayCommand(() =>
                    {
                        this.TheRepository.Commit(this.CommitMessage);
                        //this.TheRepository.Commit(this.CommitMessage, this.PathStatusItems.Where(item => item.IsSelected).Select(item => item.FullPath).ToArray());
                        this.CloseDialog();
                    }, () =>
                    {
                        if (String.IsNullOrEmpty(this.CommitMessage))
                            return false;
                        if (this.PathStatusItems.Where(item => item.IsSelected).FirstOrDefault() == null)
                            return false;

                        return true;
                    });
                }
                return this._commitCmd;
            }
        }
        private RelayCommand _commitCmd;

        public RelayCommand ShowCommitCmd
        {
            get
            {
                if (this._showCommitCmd == null)
                {
                    this._showCommitCmd = new RelayCommand(() =>
                    {
                        CommitDialog.ShowModelDialog(this.TheRepository, this.CurrFolder);
                        this.Reload();
                    }, () =>
                    {
                        return !this.isChecking;
                    });
                }
                return this._showCommitCmd;
            }
        }
        private RelayCommand _showCommitCmd;

        /// <summary>
        /// 关闭Diff、Commit等相关对话框；
        /// </summary>
        public RelayCommand CancelCmd
        {
            get
            {
                if (this._cancelCmd == null)
                {
                    this._cancelCmd = new RelayCommand(() =>
                    {
                        this.CloseDialog();
                    }, () =>
                    {
                        return true;
                    });
                }
                return _cancelCmd;
            }
        }
        private RelayCommand _cancelCmd;

        private void CloseDialog()
        {
            if (this.Owner != null)
                this.Owner.Close();
        }

        /// <summary>
        /// 更新相关文件状态命令；
        /// </summary>
        public RelayCommand RefreshCmd
        {
            get
            {
                if (this._refreshCmd == null)
                {
                    this._refreshCmd = new RelayCommand(() =>
                    {
                        this.Reload();
                    }, () =>
                    {
                        return true;
                    });
                }

                return _refreshCmd;
            }
        }
        private RelayCommand _refreshCmd;

        public bool IsSelectAll
        {
            get { return _isSelectAll; }
            set 
            {
                _isSelectAll = value;
                this.RaisePropertyChanged("IsSelectAll");
                this.SelectAll(!value);
            }
        }
        private bool _isSelectAll;


        private void SelectAll(bool deselect = false, IEnumerable<FileDiffItem> exepts = null)
        {
            foreach (var item in this.PathStatusItems)
            {
                // 排除项
                if (exepts != null && exepts.Contains(item))
                {
                    item.IsSelected = deselect;
                }
                else
                {
                    item.IsSelected = !deselect;
                }
            }
        }

        #endregion

        public static IValueConverter Model2ColorConverter
        {
            get
            {
                if (_status2ColorConverter == null)
                {
                    _status2ColorConverter = new GenericConverter()
                    {
                        ConvertFunc = (args, type, parameter) =>
                        {
                            PathStatus model = args as PathStatus;
                            if (model == null)
                                return Brushes.Black;

                            if (model.IndexPathStatus == IndexPathStatus.MergeConflict)
                                return Brushes.Red;
                            if (model.IndexPathStatus == IndexPathStatus.Added || model.IndexPathStatus == IndexPathStatus.Staged)
                                return Brushes.Chartreuse;
                            if (model.IndexPathStatus == IndexPathStatus.Removed || model.WorkingPathStatus == WorkingPathStatus.Missing)
                                return Brushes.Red;
                            if (model.WorkingPathStatus == WorkingPathStatus.Modified)
                                return Brushes.RoyalBlue;

                            return Brushes.Black;
                        }
                    };
                }

                return _status2ColorConverter;
            }
        }
        private static IValueConverter _status2ColorConverter;

    }
}
