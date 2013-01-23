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

namespace NOnkyo.WpfGui.Views
{
    /// <summary>
    /// Interaktionslogik für InputView.xaml
    /// </summary>
    public partial class InputView
    {
        #region Constructor / Destructor

        public InputView()
        {
            InitializeComponent();
            this.Input = string.Empty;
            this.txtInput.Focus();
            this.txtInput.KeyDown += new KeyEventHandler(txtInput_KeyDown);
        }

        #endregion

        #region Public Methods / Properties

        public string Input { get; private set; }
        public void Init(string psTitle)
        {
            this.lblInput.Content = psTitle;
        }

        #endregion

        #region EventHandler

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                this.Input = this.txtInput.Text.Trim();
                this.Close();
            }
        }

        #endregion

    }
}
