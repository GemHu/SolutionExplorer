using DothanTech.Helpers;
using GitSharp;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DothanTech.ViGET.GitManager
{
    public class FileDiffItem : DiffItemBase, IComparable
    {
        #region Life Cycle

        public FileDiffItem(PathStatus model)
        {
            this.Model = model;
        }

        public PathStatus Model
        {
            get { return _model; }
            set
            {
                _model = value;
                this.RaisePropertyChanged("Model");
                // 
                if (value != null)
                {
                    this.Path = value.Path;
                    var root = System.IO.Path.GetDirectoryName(value.Repository.Directory);
                    this.FullPath = System.IO.Path.Combine(root, value.Path);
                    this.Status = value.WorkingPathStatus.ToString();
                }
                else
                {
                    this.Path = String.Empty;
                    this.FullPath = String.Empty;
                    this.Status = String.Empty;
                }
            }
        }
        private PathStatus _model;

        #endregion

        /// <summary>
        /// 文件选择状态；
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                this.RaisePropertyChanged("IsSelected");
            }
        }
        private bool _isSelected;

        public int CompareTo(object obj)
        {
            FileDiffItem target = obj as FileDiffItem;
            if (this.Model.WorkingPathStatus == target.Model.WorkingPathStatus)
                return this.Path.CompareTo(target.Path);

            if (this.Model.WorkingPathStatus == WorkingPathStatus.Modified)
                return -1;
            if (target.Model.WorkingPathStatus == WorkingPathStatus.Modified)
                return 1;
            if (this.Model.WorkingPathStatus == WorkingPathStatus.Missing)
                return -1;
            if (target.Model.WorkingPathStatus == WorkingPathStatus.Missing)
                return 1;
            if (this.Model.WorkingPathStatus == WorkingPathStatus.Untracked)
                return -1;
            if (target.Model.WorkingPathStatus == WorkingPathStatus.Untracked)
                return 1;

            return this.Path.CompareTo(target.Path);
        }
    }
}
