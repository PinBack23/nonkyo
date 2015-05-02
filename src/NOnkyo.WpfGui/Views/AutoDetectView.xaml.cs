using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NOnkyo.WpfGui.ViewModels;
using MahApps.Metro.Controls;

namespace NOnkyo.WpfGui.Views
{
    /// <summary>
    /// Interaktionslogik für AutoDetectView.xaml
    /// </summary>
    public partial class AutoDetectView
    {
        #region Constructor / Destructor

        public AutoDetectView()
        {
            InitializeComponent();
            this.Model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Model_PropertyChanged);
            this.Loaded += new RoutedEventHandler(AutoDetectView_Loaded);
        }
        
        #endregion

        #region Public Methods / Properties

        public AutoDetectViewModel Model
        {
            get { return this.DataContext as AutoDetectViewModel; }
        }

        #endregion

        #region EventHandler

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ProgressBarVisibility")
            {
                this.staPrgBar.Visibility = this.Model.ProgressBarVisibility ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                if (this.Model.ProgressBarVisibility)
                {
                    this.prgSearch.Visibility = System.Windows.Visibility.Hidden;
                    this.prgSearch.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void AutoDetectView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Model.WindowLoaded();
        }

        #endregion

    }
}
