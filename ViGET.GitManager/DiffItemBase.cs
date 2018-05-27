using DothanTech.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DothanTech.ViGET.GitManager
{
    public class DiffItemBase : ViewModelBase
    {
        #region Property

        public String Path
        {
            get { return _path; }
            set
            {
                _path = value;
                this.RaisePropertyChanged("Path");
                this.Extension = String.IsNullOrEmpty(value) ? String.Empty : System.IO.Path.GetExtension(value);
            }
        }
        private String _path;

        public String FullPath
        {
            get 
            {
                if (String.IsNullOrEmpty(this._fullPath))
                    return this.Path;

                return _fullPath; 
            }
            set
            {
                _fullPath = value;
                this.RaisePropertyChanged("FullPath");
                this.Image = FileIcon.GetImageSource(value, true);
            }
        }
        private String _fullPath;

        public String Extension
        {
            get 
            {
                if (String.IsNullOrEmpty(this._extension))
                    return System.IO.Path.GetExtension(this.Path);

                return _extension; 
            }
            set
            {
                _extension = value;
                this.RaisePropertyChanged("Extension");
            }
        }
        private String _extension;

        public String Status
        {
            get { return _status; }
            set
            {
                _status = value;
                this.RaisePropertyChanged("Status");
            }
        }
        private String _status;

        public int Add
        {
            get { return _add; }
            set
            {
                _add = value;
                this.RaisePropertyChanged("Add");
            }
        }
        private int _add;

        public int Delete
        {
            get { return _delete; }
            set { 
                _delete = value;
                this.RaisePropertyChanged("Delete");
            }
        }
        private int _delete;

        public ImageSource Image
        {
            get 
            {
                if (this._image == null)
                    return FileIcon.GetImageSource(this.FullPath, true);

                return this._image; 
            }
            set
            {
                this._image = value;
                this.RaisePropertyChanged("Image");
            }
        }
        private ImageSource _image;

        #endregion
    }
}
