using DothanTech.Helpers;
using GitSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DothanTech.ViGET.GitManager
{
    public class ChangedItem : DiffItemBase
    {
        public ChangedItem(Change model)
        {
            this.Model = model;
        }

        public Change Model
        {
            get { return _model; }
            set 
            { 
                _model = value;
                //
                if (value != null)
                {
                    this.FullPath = value.Path;
                    this.Path = value.Path;
                    this.Type = value.ChangeType;
                }
                else
                {
                    this.Path = String.Empty;
                    this.FullPath = String.Empty;
                    this.Type = value.ChangeType;
                }
            }
        }
        private Change _model;

        public ChangeType Type
        {
            get { return _type; }
            set 
            {
                _type = value;
                this.RaisePropertyChanged("Type");
                //
                this.Status = value.ToString();
            }
        }
        private ChangeType _type;
    }
}
