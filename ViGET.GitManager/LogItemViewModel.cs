using DothanTech.Helpers;
using GitSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace DothanTech.ViGET.GitManager
{
    public class LogItemViewModel : GitModelBase
    {
        public LogItemViewModel()
        {
            this.ChangedItems = new ObservableCollection<ChangedItem>();
        }

        public Commit Model
        {
            get { return _model; }
            set 
            { 
                _model = value;
                // message
                this.CommitMessage = value.Message;
                // change items
                this.ChangedItems.Clear();
                if (value != null)
                {
                    var items = value.CompareAgainst(value.Parent);
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            this.ChangedItems.Add(new ChangedItem(item));
                        }
                    }
                }
            }
        }
        private Commit _model;

        public ObservableCollection<ChangedItem> ChangedItems
        {
            get { return _changedItem; }
            set 
            { 
                _changedItem = value;
                this.RaisePropertyChanged("ChangedItems");
            }
        }
        private ObservableCollection<ChangedItem> _changedItem;

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


        public static IValueConverter ChangeToColorConverter
        {
            get
            {
                if (_changeToColorConverter == null)
                {
                    _changeToColorConverter = new GenericConverter()
                    {
                        ConvertFunc = ((arg, t, parameter) =>
                        {
                            try
                            {
                                ChangeType c = (ChangeType)arg;
                                if (c == ChangeType.Added)
                                    return Brushes.DarkOrange;
                                else if (c == ChangeType.Deleted)
                                    return Brushes.Red;
                                else if (c == ChangeType.Renamed)
                                    return Brushes.Blue;
                                else
                                    return Brushes.DodgerBlue;
                            }
                            catch (Exception ee)
                            {
                                Trace.WriteLine("### [" + ee.Source + "] Exception: " + ee.Message);
                                Trace.WriteLine("### " + ee.StackTrace);
                                return Brushes.Black;
                            }
                        })
                    };
                }

                return _changeToColorConverter;
            }
        }
        private static IValueConverter _changeToColorConverter;

        public RelayCommand CloseCmd
        {
            get 
            {
                if (this._closeCmd == null)
                {
                    this._closeCmd = new RelayCommand(() =>
                    {
                        this.Owner.Close();
                    }, () => 
                    {
                        return this.Owner != null;
                    });
                }

                return _closeCmd; 
            }
        }
        private RelayCommand _closeCmd;

    }
}
