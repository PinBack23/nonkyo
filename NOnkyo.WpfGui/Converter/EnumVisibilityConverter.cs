using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace NOnkyo.WpfGui.Converter
{
    public class EnumVisibilityConverter : IValueConverter
    {

        #region IValueConverter Member

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string psEnumString = value.ToString();
            
            if (parameter.ToString() == value.ToString())
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
