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

using Microsoft.Owin.Hosting;
using NOnkyo.ISCP;
using NOnkyo.WpfGui.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NOnkyo.WpfGui.ViewModels
{
    public class RESTServerViewModel : ViewModelBase
    {

        #region Attribute

        private bool mbStartOnlyLocal = Properties.Settings.Default.StartServerOnlyLocal;
        private string mnPort = Properties.Settings.Default.ServerPort;
        private bool mbStartServerOnStartup = Properties.Settings.Default.StartServerOnStartup;

        #endregion

        public RESTServerViewModel()
        {
            this.ErrorList.Add(this.GetPropertyNameFromExpression(() => this.Port), string.Empty);
        }

        #region Commands

        #region StartServer

        private RelayCommand moStartServerCommand;
        public ICommand StartServerCommand
        {
            get
            {
                if (this.moStartServerCommand == null)
                    this.moStartServerCommand = new RelayCommand(param => this.StartServer(),
                        param => this.CanStartServer());
                return this.moStartServerCommand;
            }
        }

        private void StartServer()
        {
            Web.RESTServer.Instance.StartServer(this.mbStartOnlyLocal, Int32.Parse(this.mnPort));
            this.OnPropertyChanged(() => this.ServerUrl);
            this.OnPropertyChanged(() => this.IsServerUrlSet);
            this.moStopServerCommand.RaiseCanExecuteChanged();
            this.moStartServerCommand.RaiseCanExecuteChanged();
        }

        private bool CanStartServer()
        {
            return this.ErrorList[this.GetPropertyNameFromExpression(() => this.Port)].IsEmpty() &&
                !Web.RESTServer.Instance.IsServerStarted;
        }

        #endregion

        #region StopServer

        private RelayCommand moStopServerCommand;
        public ICommand StopServerCommand
        {
            get
            {
                if (this.moStopServerCommand == null)
                    this.moStopServerCommand = new RelayCommand(param => this.StopServer(),
                        param => this.CanStopServer());
                return this.moStopServerCommand;
            }
        }

        private void StopServer()
        {
            Web.RESTServer.Instance.StopServer();
            this.OnPropertyChanged(() => this.ServerUrl);
            this.OnPropertyChanged(() => this.IsServerUrlSet);
            this.moStopServerCommand.RaiseCanExecuteChanged();
            this.moStartServerCommand.RaiseCanExecuteChanged();
        }

        private bool CanStopServer()
        {
            return Web.RESTServer.Instance.IsServerStarted;
        }

        #endregion

        #endregion

        #region Public Methods / Properties

        public bool StartOnlyLocal
        {
            get { return this.mbStartOnlyLocal; }
            set
            {
                if (this.mbStartOnlyLocal != value)
                {
                    this.mbStartOnlyLocal = value;
                    Properties.Settings.Default.StartServerOnlyLocal = value;
                    Properties.Settings.Default.Save();
                    this.OnPropertyChanged(() => this.StartOnlyLocal);
                }
            }
        }

        public bool StartServerOnStartup
        {
            get { return this.mbStartServerOnStartup; }
            set
            {
                if (this.mbStartServerOnStartup != value)
                {
                    this.mbStartServerOnStartup = value;
                    Properties.Settings.Default.StartServerOnStartup = value;
                    Properties.Settings.Default.Save();
                    this.OnPropertyChanged(() => this.StartServerOnStartup);
                }
            }
        }

        public string Port
        {
            get { return this.mnPort; }
            set
            {
                if (this.mnPort != value)
                {
                    this.mnPort = value;
                    this.ValidateIPPort();
                    if (this.ErrorList[this.GetPropertyNameFromExpression(() => this.Port)].IsEmpty())
                    {
                        Properties.Settings.Default.ServerPort = value;
                        Properties.Settings.Default.Save();
                    }
                    this.OnPropertyChanged(() => this.Port);
                }
            }
        }

        public string ServerUrl
        {
            get
            {
                return Web.RESTServer.Instance.CurrentServerUrl;
            }
        }

        public bool IsServerUrlSet
        {
            get
            {
                return Web.RESTServer.Instance.CurrentServerUrl.IsNotEmpty();
            }
        }
        #endregion

        #region Validation

        private void ValidateIPPort()
        {
            string lsErrorMessage = string.Empty;
            int lnDummy = -1;

            if (this.mnPort.IsEmpty() || !Int32.TryParse(this.mnPort, out lnDummy))
                lsErrorMessage = "Please insert a valid portnumber";

            if (lnDummy < 1)
                lsErrorMessage = "Please insert a valid portnumber";

            this.ErrorList[this.GetPropertyNameFromExpression(() => this.Port)] = lsErrorMessage;
            this.moStartServerCommand.RaiseCanExecuteChanged();
        }

        #endregion

    }
}
