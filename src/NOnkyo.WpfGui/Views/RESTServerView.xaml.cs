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

    }
}
