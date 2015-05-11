using NOnkyo.WpfGui.ViewModels;
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
using NOnkyo.ISCP;
using System.Diagnostics;

namespace NOnkyo.WpfGui.Views
{
    /// <summary>
    /// Interaktionslogik für RESTServerView.xaml
    /// </summary>
    public partial class RESTServerView
    {
        public RESTServerView()
        {
            InitializeComponent();
        }

        #region Public Methods / Properties

        public RESTServerViewModel Model
        {
            get { return this.DataContext as RESTServerViewModel; }
        }

        #endregion

        #region EventHandler

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                if (e.Uri.AbsoluteUri.IsNotEmpty())
                {
                    Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                }
                e.Handled = true;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        #endregion
    }
}
