using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dothan.ViObject;
using System.Windows.Media.Imaging;

namespace DothanTech.ViGET.SolutionExplorer.Modules
{
    internal class FolderNode : BaseGroupNode<ProjectNode>
    {
        public FolderNode(String folder) : base(folder)
        {

        }

        public override String Icon
        {
            get
            {
                return "pack://application:,,,/SolutionExplorer;Component/Images/NODE_FOLDER.png";
            }
        }

        public override string ExpandedIcon
        {
            get
            {
                return "pack://application:,,,/SolutionExplorer;Component/Images/NODE_FOLDER_OPEN.png";
            }
        }
    }
}
