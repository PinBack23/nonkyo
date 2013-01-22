using System;
using System.Windows;

namespace NOnkyo.WpfGui.AttachedProperty
{
    public static class CloseDialog
    {
        public static readonly DependencyProperty DialogResultProperty = 
            DependencyProperty.RegisterAttached("DialogResult", typeof(Boolean?), typeof(CloseDialog),new PropertyMetadata(DialogResultChanged));

        private static void DialogResultChanged(DependencyObject poDependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var loWindow = poDependencyObject as Window;
            Boolean lbIsModal = System.Windows.Interop.ComponentDispatcher.IsThreadModal;
            if (loWindow != null)
                if (lbIsModal)
                    loWindow.DialogResult = e.NewValue as Boolean?;
                else
                    loWindow.Close();
        }

        public static void SetDialogResult(Window poTarget, Boolean? pbDialogResult)
        {
            poTarget.SetValue(DialogResultProperty, pbDialogResult);
        }
    }
}