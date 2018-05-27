using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace DothanTech.ViGET.TemplateWizard
{
    public class TemplateManager : ProjectTypeModel
    {
        public TemplateManager()
            : base("Templates", null)
        {
            this.IsExpanded = true;
        }

        public override void AddChild(string childName, List<VSTemplate> templates)
        {
            base.AddChild(childName, templates);
            if (this.Children.Count == 1)
            {
                (this.Children[0] as ProjectTypeModel).IsSelected = true;
            }
        }
    }
}
