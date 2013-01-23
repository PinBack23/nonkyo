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
using System.Windows.Media.Animation;
using System.IO;

namespace NOnkyo.WpfGui.Views
{
    /// <summary>
    /// Interaktionslogik für RemoteView.xaml
    /// </summary>
    public partial class RemoteView
    {
        #region Constructor / Destructor

        public RemoteView()
        {
            InitializeComponent();
            this.Model.SelectDevice += new EventHandler<SelectDeviceEventArgs>(Model_SelectDevice);
            this.Model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Model_PropertyChanged);
            this.Model.CloseInputSelector += new EventHandler(Model_CloseInputSelector);
            this.Model.KeyboardInput += new EventHandler<KeyboardInputEventArgs>(Model_KeyboardInput);
            this.Loaded += new RoutedEventHandler(RemoteView_Loaded);
        }

        #endregion

        #region Public Methods / Properties

        public RemoteViewModel Model
        {
            get { return this.DataContext as RemoteViewModel; }
        }

        #endregion

        #region EventHandler

        private void Model_SelectDevice(object sender, SelectDeviceEventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Hidden;
            var loView = new AutoDetectView();
            if (loView.ShowDialog(this) == true)
            {
                e.Device = loView.Model.SelectedDevice;
            }
            this.Visibility = System.Windows.Visibility.Visible;
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == this.Model.GetPropertyNameFromExpression(() => this.Model.MuteStatus))
                {
                    if (!this.Model.MuteStatus.GetValueOrDefault())
                        this.bruMute.Visual = this.FindResource("appbar_sound_0") as Visual;
                    else
                        this.bruMute.Visual = this.FindResource("appbar_sound_mute") as Visual;
                }
                else if (e.PropertyName == this.Model.GetPropertyNameFromExpression(() => this.Model.AlbumImage))
                {
                    if (this.Model.AlbumImage == null)
                        this.imgAlbum.Visibility = System.Windows.Visibility.Collapsed;
                    else
                    {
                        BitmapImage loImage = new BitmapImage();
                        loImage.BeginInit();

                        //using (MemoryStream loStream = new MemoryStream(this.moImageBytes.ToArray()))
                        //{
                        //    this.Album = Image.FromStream(loStream);
                        //    this.IsReady = true;
                        //}

                        MemoryStream loStream = new MemoryStream(this.Model.AlbumImage);
                        loImage.StreamSource = loStream;

                        loImage.EndInit();
                        this.imgAlbum.Source = loImage;
                        (this.FindResource("AlbumImageStoryboard") as Storyboard).Begin(this);
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        private void Model_KeyboardInput(object sender, KeyboardInputEventArgs e)
        {
            try
            {
                if (!e.CloseInputView)
                {
                    InputView loView = new InputView();
                    loView.Top = this.Top + 30;
                    loView.Left = this.Left + this.Width / 2 - loView.Width / 2;
                    loView.Init(e.Title);
                    loView.ShowDialog(this);
                    e.Input = loView.Input;
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }            
        }

        private void RemoteView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Model.WindowLoaded();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (this.Model != null)
                this.Model.Dispose();
        }

        private void Selectors_Click(object sender, RoutedEventArgs e)
        {
            this.Flyouts[0].IsOpen = !this.Flyouts[0].IsOpen;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Model.SelectNetItemCommand.Execute(null);
        }

        private void Model_CloseInputSelector(object sender, EventArgs e)
        {
            this.Flyouts[0].IsOpen = false;
        }

        #endregion
    }
}
