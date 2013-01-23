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
            Logger.LogException(NLog.LogLevel.Fatal, "Unhandled Exception detected", loExp);
            MessageBox.Show("{0}{1}Shutdown Application!".FormatWith(loExp.Message, Environment.NewLine), "Unhandled Exception detected");
            //Application.Current.Shutdown();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.LogException(NLog.LogLevel.Fatal, "Unhandled Exception detected", e.Exception);
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
