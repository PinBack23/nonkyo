using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace NOnkyo.WpfGui.Converter
{
    public class MuteConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Member

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            FrameworkElement loTargetObject = values[0] as FrameworkElement;
            VisualBrush loBrush = values[1] as VisualBrush;
            Rectangle loRec = values[2] as Rectangle;
            if (loTargetObject == null)
                return null;


            object lo = null;

            
            if ((bool)values[3])
                lo = loTargetObject.TryFindResource("appbar_sound_0");
            else
                lo = loTargetObject.TryFindResource("appbar_sound_mute");
            loBrush.Visual = lo as Visual;
            
            return lo;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
