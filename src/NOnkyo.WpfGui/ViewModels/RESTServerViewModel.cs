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
    public class RESTServerViewModel : ViewModelBase, IDisposable
    {
        #region Static
        private static ManualResetEventSlim StopServerEvent = new ManualResetEventSlim();
        private static bool mbRESTApiStarted = false;
        private static void StartRESTApi(Object poObject)
        {
            mbRESTApiStarted = true;
#if DEBUG
            using (WebApp.Start<Startup>(url: "http://localhost:9876/"))
            //using (WebApp.Start<Startup>(url: "http://*:9876/"))
#else
            using (WebApp.Start<Startup>(url: "http://*:9876/"))
#endif
            {
                StopServerEvent.Wait();
            }
            mbRESTApiStarted = false;
        }

        #endregion

        #region Attribute

        #endregion

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
            StopServerEvent.Reset();
            ThreadPool.QueueUserWorkItem(new WaitCallback(StartRESTApi));
            Thread.Sleep(400);
            this.moStopServerCommand.RaiseCanExecuteChanged();
            this.moStartServerCommand.RaiseCanExecuteChanged();
        }

        private bool CanStartServer()
        {
            return !mbRESTApiStarted;
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
            StopServerEvent.Set();
            Thread.Sleep(400);
            this.moStopServerCommand.RaiseCanExecuteChanged();
            this.moStartServerCommand.RaiseCanExecuteChanged();
        }

        private bool CanStopServer()
        {
            return mbRESTApiStarted;
        }

        #endregion

        #endregion

        #region Public Methods / Properties

        #endregion

        #region IDisposable

        // Track whether Dispose has been called.
        private bool mbDisposed = false;

        /// <summary>
        /// Implement IDisposable
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing">true, to dispose managed ad unmanaged resources, false to dispose unmanaged resources only</param>
        protected virtual void Dispose(bool disposing)
        {
            // Note that this is not thread safe.
            // Another thread could start disposing the object
            // after the managed resources are disposed,
            // but before the disposed flag is set to true.
            // If thread safety is necessary, it must be
            // implemented by the client.

            // Check to see if Dispose has already been called.
            if (!this.mbDisposed)
            {
                try
                {
                    // If disposing equals true, dispose all managed 
                    // and unmanaged resources.
                    if (disposing)
                    {
                        // Dispose managed resources. HERE ->
                        if (!StopServerEvent.IsSet)
                            this.StopServer();
                        //Release ComObjects
                        //System.Runtime.InteropServices.Marshal.ReleaseComObject(ComObj);
                    }
                    // Release unmanaged resources. If disposing is false, 
                    // only the following code is executed. HERE ->
                }
                catch (Exception)
                {
                    this.mbDisposed = false;
                    throw;
                }
                this.mbDisposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method 
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~RESTServerViewModel()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            this.Dispose(false);
        }
        #endregion IDisposable

    }
}
