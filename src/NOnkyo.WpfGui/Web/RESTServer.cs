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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NOnkyo.ISCP;

namespace NOnkyo.WpfGui.Web
{
    public class RESTServer
    {
        #region HilfsKlasse

        public class RESTServerParams
        {
            public bool StartOnlyLocal { get; set; }
            public string Root
            {
                get
                {
                    if (this.StartOnlyLocal)
                        return "localhost";
                    return "*";
                }
            }
            public int Port { get; set; }

            public override string ToString()
            {
                return "http://{0}:{1}/".FormatWith(this.Root, this.Port);
            }
        }

        #endregion

        #region Static

        private static ManualResetEventSlim StopServerEvent = new ManualResetEventSlim();
        private static bool mbRESTApiStarted = false;
        private static void StartRESTApi(Object poObject)
        {
            RESTServerParams loParams = poObject as RESTServerParams;
            mbRESTApiStarted = true;

            using (WebApp.Start<Startup>(url: loParams.ToString()))
            {
                StopServerEvent.Wait();
            }
            mbRESTApiStarted = false;
        }

        #endregion

        #region Event ServerStateChanged

        [NonSerialized()]
        private EventHandler EventServerStateChanged;
        public event EventHandler ServerStateChanged
        {
            add
            { this.EventServerStateChanged += value; }
            remove
            { this.EventServerStateChanged -= value; }
        }

        protected virtual void OnServerStateChanged()
        {
            EventHandler loHandler = this.EventServerStateChanged;
            if (loHandler != null)
                loHandler(this, EventArgs.Empty);
        }

        #endregion

        #region Attributes

        private int mnWaitTime = 400;

        #endregion

        #region Singleton

        /// <summary>
        /// Constructor legt Singleton-Instanz an und kann nur einmal aufgerufen werden.
        /// </summary>    
        protected RESTServer() { }

        /// <summary>
        /// Instance Property
        /// </summary>
        public static RESTServer Instance
        {
            get { return NestedSingleton.Instance; }
        }

        private class NestedSingleton
        {
            internal static readonly RESTServer Instance = new RESTServer();

            // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
            // NICHT ENTFERNEN AUCH WENN FXCOP WAS ANDERES SAGT
            static NestedSingleton() { }
        }

        #endregion Singleton

        #region Public Methods / Properties

        public string CurrentServerUrl
        {
            get;
            private set;
        }

        public bool IsServerStarted
        {
            get
            {
                return mbRESTApiStarted;
            }
        }

        public void StartServer(bool pbStartOnlyLocal, int pnPort)
        {
            if (!mbRESTApiStarted)
            {
                StopServerEvent.Reset();
                var loParameter = new RESTServerParams() { StartOnlyLocal = pbStartOnlyLocal, Port = pnPort };
                this.CurrentServerUrl = loParameter.ToString();
                ThreadPool.QueueUserWorkItem(new WaitCallback(StartRESTApi), loParameter);
                Thread.Sleep(this.mnWaitTime);
                this.OnServerStateChanged();
            }
        }

        public void StopServer()
        {
            if (!StopServerEvent.IsSet)
            {
                this.CurrentServerUrl = string.Empty;
                StopServerEvent.Set();
                Thread.Sleep(this.mnWaitTime);
                this.OnServerStateChanged();
            }
        }

        #endregion

    }
}
