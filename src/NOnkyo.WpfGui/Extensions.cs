#region License
/*Copyright (c) 2013, Karl Sparwald
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that 
the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following 
disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the 
following disclaimer in the documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS
OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, 
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
OF THE POSSIBILITY OF SUCH DAMAGE.*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using NOnkyo.ISCP;
using MahApps.Metro.Controls;
using System.Reflection;

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

    public static class AssemblyExtensions
    {
        public static string Copyright(this Assembly value)
        {
            return ((AssemblyCopyrightAttribute)value.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true)[0]).Copyright;
        }

        public static string Product(this Assembly value)
        {
            return ((AssemblyProductAttribute)value.GetCustomAttributes(typeof(AssemblyProductAttribute), true)[0]).Product;
        }

        public static string Version(this Assembly value)
        {
            return value.GetName().Version.ToString();
        }
    }
}
