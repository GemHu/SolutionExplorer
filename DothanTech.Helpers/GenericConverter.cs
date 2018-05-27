using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DothanTech.Helpers
{
    public class GenericConverter : IValueConverter
    {
        public Func<object, Type, object, object> ConvertFunc { get; set; }
        public Func<object, Type, object, object> ConvertBackFunc { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertFunc(value, targetType, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ConvertBackFunc == null)
                return null;

            return ConvertBackFunc(value, targetType, parameter);
        }

        #endregion
    }
}
