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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using NOnkyo.ISCP;
using LightCore;
using LightCore.Lifecycle;

namespace NOnkyo.WpfGui
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {

        #region Logging

        private static Lazy<NLog.Logger> moLazyLogger = new Lazy<NLog.Logger>(() => NLog.LogManager.GetCurrentClassLogger());

        private static NLog.Logger Logger
        {
            get
            {
                return moLazyLogger.Value;
            }
        }

        #endregion

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            if (e.Args != null && e.Args.Length > 0 && e.Args[0] == "FAKE")
                RegisterFake();
            else
                Register();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception loExp = e.ExceptionObject as Exception;
            Logger.Log(NLog.LogLevel.Fatal, "Unhandled Exception detected", loExp);
            MessageBox.Show("{0}{1}Shutdown Application!".FormatWith(loExp.Message, Environment.NewLine), "Unhandled Exception detected");
            //Application.Current.Shutdown();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Log(NLog.LogLevel.Fatal, "Unhandled Exception detected", e.Exception);
            MessageBox.Show("{0}{1}Shutdown Application!".FormatWith(e.Exception.Message, Environment.NewLine), "Unhandled Exception detected");
            e.Handled = false;
            e.Dispatcher.InvokeShutdown();
        }

        public static IContainer Container { get; set; }

        private void Register()
        {
            var loBuilder = new ContainerBuilder();

            //Init DeviceSearch
            loBuilder.Register<IDeviceSearch, DeviceSearch>();

            //Init Connection
            loBuilder.Register<IConnection, Connection>();

            Container = loBuilder.Build();
        }

        private void RegisterFake()
        {
            var loBuilder = new ContainerBuilder();

            //Init DeviceSearch
            loBuilder.Register<IDeviceSearch, Fake.DeviceSearch>();

            //Init Connection
            loBuilder.Register<IConnection, Fake.Connection>();

            Container = loBuilder.Build();
        }

    }
}
