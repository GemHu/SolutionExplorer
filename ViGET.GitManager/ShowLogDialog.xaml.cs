using GitSharp;
using GitSharp.Core.RevPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ShowLogDialog.xaml
    /// </summary>
    public partial class ShowLogDialog : Window
    {
        private PlotRenderer m_plot_renderer;
        private Repository m_repo;
        private PlotWalk m_revwalk;
        private Selection<Commit> m_selection;

        private LogItemViewModel model;

        public ShowLogDialog()
        {
            InitializeComponent();
            model = new LogItemViewModel();
            model.Owner = this;

            this.DataContext = this.model;
            
            m_plot_renderer = new PlotRenderer();
            m_plot_renderer.Init(mHistoryRender);
            m_plot_renderer.CommitClicked += OnCommitClicked;
            m_plot_renderer.LabelClicked += OnLabelClicked;
            m_selection = Selection<Commit>.ExclusiveSelection(); 
            m_selection.OnSelect = OnSelect;
            m_selection.OnUnselect = OnUnselect;
        }

        public static void ShowModelDialog(Repository repo)
        {
            try
            {
                ShowLogDialog dlg = new ShowLogDialog();
                dlg.Update(repo);
                dlg.ShowDialog();
            }
            catch (Exception ee)
            {
                Trace.WriteLine("### [" + ee.Source + "] Exception: " + ee.Message);
                Trace.WriteLine("### " + ee.StackTrace);
            }
        }
        
        public void Update(Repository repo)
        {
            m_repo = repo;
            var list = new PlotCommitList();
            m_revwalk = new PlotWalk(repo);
            m_revwalk.markStart(((GitSharp.Core.Repository)repo).getAllRefsByPeeledObjectId().Keys.Select(id => m_revwalk.parseCommit(id)));
            list.Source(m_revwalk);
            list.fillTo(1000);
            m_plot_renderer.Update(list);
        }

        private void OnSelect(Commit c)
        {
            m_plot_renderer.Select(c.Hash);
        }

        private void OnUnselect(Commit c)
        {
            m_plot_renderer.Unselect(c.Hash);
        }

        private void OnCommitClicked(PlotCommit commit)
        {
            this.model.Model = new Commit(m_repo, commit.Name);
            m_selection.Update(this.model.Model);
        }

        private void OnLabelClicked(GitSharp.Core.Ref @ref)
        {
            
        }

    }
}
