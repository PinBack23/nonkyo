using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using MahApps.Metro.Controls;

namespace NOnkyo.WpfGui
{
    public static class WindowsExtensions
    {
        public static bool? ShowDialog(this Window value, Window owner)
        {
            value.Owner = owner;
            return value.ShowDialog();
        }

        public static void Show(this Window value, Window owner)
        {
            value.Owner = owner;
            value.Show();
        }
    }
}
