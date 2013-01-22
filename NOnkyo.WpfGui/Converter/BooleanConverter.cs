using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace NOnkyo.WpfGui.Converter
{
    public class BooleanConverter<T> : IValueConverter
    {
        public BooleanConverter(T poTrueValue, T poFalseValue)
        {
            this.TrueValue = poTrueValue;
            this.FalseValue = poFalseValue;
        }

        public T TrueValue { get; set; }
        public T FalseValue { get; set; }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && ((bool)value) ? this.TrueValue : this.FalseValue;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is T && EqualityComparer<T>.Default.Equals((T)value, this.TrueValue);
        }
    }
}
