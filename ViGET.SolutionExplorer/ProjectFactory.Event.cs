using DothanTech.ViGET.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DothanTech.ViGET.SolutionExplorer
{
    public partial class ProjectFactory
    {
        #region Events

        #region SolutionChanged Event

        public class SolutionChangedEventArgs
        {
            public SolutionChangedEventArgs(SolutionManager newSolution, SolutionManager oldSolution)
            {
                this.NewSolution = newSolution;
                this.OldSolution = oldSolution;
            }

            public SolutionManager OldSolution;
            public SolutionManager NewSolution;
        }

        public delegate void SolutionChangedHandler(object sender, SolutionChangedEventArgs e);

        public event SolutionChangedHandler SolutionChanged;

        private void RaiseSolutionChanged(SolutionManager newSolution, SolutionManager oldSolution)
        {
            if (this.SolutionChanged == null)
                return;

            SolutionChanged.Invoke(this, new SolutionChangedEventArgs(newSolution, oldSolution));
        }

        #endregion

        #endregion

        #region Commands

        #endregion

    }
}
