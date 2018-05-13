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
using NOnkyo.ISCP;
using NOnkyo.WpfGui.ViewModels.Commands;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Net;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;

namespace NOnkyo.WpfGui.ViewModels
{
    public class AutoDetectViewModel : ViewModelBase
    {
        #region Attributes

        private Device moSelectedDevice = null;
        private ObservableCollection<Device> moDeviceList = new ObservableCollection<Device>();
        private bool mbProgressBarVisibility = false;
        private bool mbShowDeviceList = false;
        private bool mbShowIpConfig = false;
        private string msIPPort = Properties.Settings.Default.DevicePort.ToString(System.Globalization.CultureInfo.InvariantCulture);
        private RelayCommand moSearchCommand;
        private RelayCommand moSelectDeviceCommand;
        private RelayCommand moCloseCommand;

        #endregion

        #region Constructor / Destructor

        public AutoDetectViewModel()
        {
            this.ErrorList.Add(this.GetPropertyNameFromExpression(() => this.IPPort), string.Empty);
        }

        #endregion

        #region Public Methods / Properties

        public Device SelectedDevice
        {
            get { return this.moSelectedDevice; }
            set
            {
                if (this.moSelectedDevice != value)
                {
                    this.moSelectedDevice = value;
                    this.OnPropertyChanged(() => this.SelectedDevice);
                }
                this.moSelectDeviceCommand.RaiseCanExecuteChanged();
            }
        }

        public bool ProgressBarVisibility
        {
            get { return this.mbProgressBarVisibility; }
            set
            {
                if (this.mbProgressBarVisibility != value)
                {
                    this.mbProgressBarVisibility = value;
                    this.OnPropertyChanged(() => this.ProgressBarVisibility);
                }
            }
        }

        public bool ShowDeviceList
        {
            get { return this.mbShowDeviceList; }
            set
            {
                if (this.mbShowDeviceList != value)
                {
                    this.mbShowDeviceList = value;
                    this.OnPropertyChanged(() => this.ShowDeviceList);
                }
            }
        }

        public bool ShowIpConfig
        {
            get { return this.mbShowIpConfig; }
            set
            {
                if (this.mbShowIpConfig != value)
                {
                    this.mbShowIpConfig = value;
                    this.OnPropertyChanged(() => this.ShowIpConfig);
                }
            }
        }

        public string IPPort
        {
            get { return this.msIPPort; }
            set
            {
                if (this.msIPPort != value)
                {
                    this.msIPPort = value;
                    this.ValidateIPPort();
                    this.OnPropertyChanged(() => this.IPPort);
                }
            }
        }


        public ObservableCollection<Device> DeviceList
        {
            get { return this.moDeviceList; }
            set
            {
                if (this.moDeviceList != value)
                {
                    this.moDeviceList = value;
                    this.OnPropertyChanged(() => this.DeviceList);
                }
            }
        }

        #endregion

        #region Commands

        public ICommand SearchCommand
        {
            get
            {
                if (this.moSearchCommand == null)
                    this.moSearchCommand = new RelayCommand(param => this.SearchAction(),
                        param => this.CanSearch);
                return this.moSearchCommand;
            }
        }

        public ICommand SelectDeviceCommand
        {
            get
            {
                if (this.moSelectDeviceCommand == null)
                    this.moSelectDeviceCommand = new RelayCommand(param => this.IsDialogClose = true,
                        param => this.moSelectedDevice != null);
                return this.moSelectDeviceCommand;
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                if (this.moCloseCommand == null)
                    this.moCloseCommand = new RelayCommand(param =>
                    {
                        this.SelectedDevice = null;
                        this.IsDialogClose = true;
                    });
                return this.moCloseCommand;
            }
        }
        #endregion

        #region Public Methods / Properties

        public void WindowLoaded()
        {
            this.SearchCommand.Execute(null);


            //NOnkyo.ISCP.Properties.Settings.Default.Reset();
            //NOnkyo.ISCP.Properties.Settings.Default.Save();
            //Properties.Settings.Default.DevicePort = 12;
            //Properties.Settings.Default.Save();
            //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //config.AppSettings.Settings["oldPlace"].Value = "3";
            //config.Save(ConfigurationSaveMode.Modified);
            //ConfigurationManager.RefreshSection("appSettings");

        }

        #endregion

        #region Validation

        private void ValidateIPPort()
        {
            string lsErrorMessage = string.Empty;
            int lnDummy = -1;

            if (this.msIPPort.IsEmpty() || !Int32.TryParse(this.msIPPort, out lnDummy))
                lsErrorMessage = "Please insert a valid portnumber";

            if (lnDummy < 1)
                lsErrorMessage = "Please insert a valid portnumber";

            this.ErrorList[this.GetPropertyNameFromExpression(() => this.IPPort)] = lsErrorMessage;
            this.moSearchCommand.RaiseCanExecuteChanged();
        }

        #endregion

        private void SearchAction()
        {
            this.PrepareSearch();

            Task.Factory.StartNew(() =>
                {
                    this.SearchDevices();
                }).
                ContinueWith((t) =>
                {
                    this.SearchCompleted();
                }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private void PrepareSearch()
        {
            this.ProgressBarVisibility = true;
            this.ShowIpConfig = false;
            this.ShowDeviceList = false;
            this.SelectedDevice = null;
            this.moSearchCommand.RaiseCanExecuteChanged();
            this.DeviceList = new ObservableCollection<Device>();
        }

        private void SearchCompleted()
        {
            this.ProgressBarVisibility = false;
            this.moSearchCommand.RaiseCanExecuteChanged();
            this.OnPropertyChanged(() => this.DeviceList);

            if (this.moDeviceList.Count > 0)
                this.SelectedDevice = this.moDeviceList[0];

            if (this.moDeviceList.Count == 1)
                this.moSelectDeviceCommand.Execute(null);

            this.ShowDeviceList = this.moDeviceList.Count > 0;
            this.ShowIpConfig = this.moDeviceList.Count == 0;
        }


        private bool CanSearch
        {
            get
            {
                return !this.mbProgressBarVisibility &&
                    this.ErrorList[this.GetPropertyNameFromExpression(() => this.IPPort)].IsEmpty();
            }
        }

        private void SearchDevices()
        {
            List<Device> loDeviceList = null;
            var loDeviceSearch = ContainerAccessor.Container.Resolve<IDeviceSearch>();
            loDeviceSearch.Timeout = TimeSpan.FromMilliseconds(Properties.Settings.Default.TimeoutDeviceSearch);

            //With fixed IP-Address
            if (Properties.Settings.Default.UsedFixedIP)
            {
                try
                {
                    IPAddress loDeviceIP;

                    if (IPAddress.TryParse(Properties.Settings.Default.DeviceIP, out loDeviceIP))
                    {
                        for (int i = 0; i < Properties.Settings.Default.DeviceSearchLoop; i++)
                        {
                            loDeviceList = loDeviceSearch.Discover(loDeviceIP, int.Parse(this.msIPPort, System.Globalization.CultureInfo.InvariantCulture));
                            if (loDeviceList.Count > 0)
                            {
                                this.moDeviceList = new ObservableCollection<Device>(loDeviceList);
                                break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Properties.Settings.Default.UsedFixedIP = false;
                    Properties.Settings.Default.Save();
                    throw;
                }
            }

            //Not Found or not fixed IP-Address
            if (this.moDeviceList == null || this.moDeviceList.Count == 0)
            {
                for (int i = 0; i < Properties.Settings.Default.DeviceSearchLoop; i++)
                {
                    loDeviceList = loDeviceSearch.Discover(int.Parse(this.msIPPort, System.Globalization.CultureInfo.InvariantCulture));
                    if (loDeviceList.Count > 0)
                    {
                        this.moDeviceList = new ObservableCollection<Device>(loDeviceList);
                        break;
                    }
                }
            }
        }
    }
}
