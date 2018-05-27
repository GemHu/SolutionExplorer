/// <summary>
/// @file   UcSolutionExplorer.xaml.cs
///	@brief  Solution Explorer 操作界面；
/// @author	DothanTech 胡殿兴
/// 
/// Copyright(C) 2011~2018, DothanTech. All rights reserved.
/// </summary>

using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using DothanTech.ViGET.ViCommand;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using DothanTech.ViGET.ViService;
using DothanTech.ViGET.Manager;

namespace DothanTech.ViGET.SolutionExplorer
{
    [Export]
    public partial class UcSolutionExplorer : UserControl, IPartImportsSatisfiedNotification
    {
        public UcSolutionExplorer()
        {
            InitializeComponent();
            // 初始化相关命令；
            this.initCommands();
        }

        public void OnImportsSatisfied()
        {
            this.DataContext = this.Factory;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            ViFileInfo target = tb.Tag as ViFileInfo;
            if (target == null)
                return;

            target.Remane(target.Name, tb.Text);
            target.IsEditMode = false;
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            ViFileInfo tag = tb.Tag as ViFileInfo;
            if (tag == null)
                return;

            if (e.Key == Key.Escape)
            {
                tag.IsEditMode = false;
            }
            else if (e.Key == Key.Enter)
            {
                tag.Remane(tag.Name, tb.Text);
                tag.IsEditMode = false;
            }
        }
    }

    public class IsActiveStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
