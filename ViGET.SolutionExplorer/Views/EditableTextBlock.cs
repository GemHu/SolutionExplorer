using DothanTech.ViGET.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DothanTech.ViGET.SolutionExplorer
{
    public class EditableTextBlock : UserControl
    {
        #region IsEditMode Property

        public static readonly DependencyProperty IsEditModeProperty =
            DependencyProperty.Register("IsEditMode", typeof(bool), typeof(EditableTextBlock),
                                        new PropertyMetadata(false));

        public bool IsEditMode
        {
            get { return (bool)GetValue(IsEditModeProperty); }
            set { SetValue(IsEditModeProperty, value); }
        }

        #endregion
    }
}
