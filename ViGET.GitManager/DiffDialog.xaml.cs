using DothanTech.Helpers;
using GitSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DothanTech.ViGET.GitManager
{
    /// <summary>
    /// Interaction logic for DiffDialog.xaml
    /// </summary>
    public partial class DiffDialog : Window
    {
        public DiffDialog()
        {
            InitializeComponent();
        }

        public static void ShowModelDialog(Repository repository, String rootPath = null)
        {
            if (repository == null)
                return;

            try
            {
                DiffDialog dlg = new DiffDialog();
                FileManagerViewModel model = new FileManagerViewModel(repository, rootPath);
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

        #region OnClick

        private void OnStage(object sender, RoutedEventArgs e)
        {
            //var status_paths = m_list.SelectedItems.OfType<PathStatus>();
            //if (status_paths == null)
            //    return;
            //try
            //{
            //    Repository.Index.Stage(status_paths.Select(sp => sp.Path).ToArray());
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //Reload();
        }

        private void OnUnstage(object sender, RoutedEventArgs e)
        {
            //var status_paths = m_list.SelectedItems.OfType<PathStatus>();
            //if (status_paths == null)
            //    return;
            //try
            //{
            //    Repository.Index.Unstage(status_paths.Select(sp => sp.Path).ToArray());
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //Reload();
        }

        private void OnCheckout(object sender, RoutedEventArgs e)
        {
            //var status_paths = m_list.SelectedItems.OfType<PathStatus>();
            //if (status_paths == null)
            //    return;
            //try
            //{
            //    Repository.Head.CurrentCommit.Checkout(status_paths.Select(sp => sp.Path).ToArray());
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //Reload();
        }

        private void OnCheckoutIndex(object sender, RoutedEventArgs e)
        {
            //var status_paths = m_list.SelectedItems.OfType<PathStatus>();
            //if (status_paths == null)
            //    return; try
            //{
            //    Repository.Index.Checkout(status_paths.Select(sp => sp.Path).ToArray());
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //Reload();
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            //var status_paths = m_list.SelectedItems.OfType<PathStatus>();
            //if (status_paths == null)
            //    return;
            //try
            //{
            //    Repository.Index.Delete(status_paths.Select(sp => sp.Path).ToArray());
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //Reload();
        }

        private void OnRemove(object sender, RoutedEventArgs e)
        {
            //var status_paths = m_list.SelectedItems.OfType<PathStatus>();
            //if (status_paths == null)
            //    return;
            //try
            //{
            //    Repository.Index.Remove(status_paths.Select(sp => sp.Path).ToArray());
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //Reload();
        }

        private void OnCommitIndex(object sender, RoutedEventArgs e)
        {
            StartCommitDialog();
        }

        public void StartCommitDialog()
        {
            //var dlg = new CommitDialog { Repository = Repository };
            //dlg.Init(_status_paths.Where(p => p.IndexPathStatus != IndexPathStatus.Unchanged));
            //dlg.ShowDialog();
            //Update(Repository);
        }
        #endregion
    }
}
