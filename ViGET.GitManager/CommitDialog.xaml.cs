using DothanTech.Helpers;
using GitSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DothanTech.ViGET.GitManager
{
    /// <summary>
    /// Interaction logic for CommitDialog.xaml
    /// </summary>
    public partial class CommitDialog : Window
    {
        public CommitDialog()
        {
            InitializeComponent();
        }

        public static void ShowModelDialog(Repository repository, String rootPath = null)
        {
            if (repository == null)
                return;

            try
            {
                CommitDialog dlg = new CommitDialog();
                var model = new FileManagerViewModel(repository, rootPath);
                model.Owner = dlg;
                dlg.DataContext = model;
                dlg.ShowDialog();
            }
            catch (Exception ee)
            {
                Trace.WriteLine("### [" + ee.Source + "] Exception: " + ee.Message);
                Trace.WriteLine("### " + ee.StackTrace);
            }
        }
    }
}
