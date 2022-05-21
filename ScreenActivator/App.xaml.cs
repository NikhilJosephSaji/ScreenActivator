using ScreenActivator.Buisness;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Msg = CustomMessageBox;

namespace ScreenActivator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex _mutex;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        App()
        {
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            // Try to grab mutex
            bool createdNew;
            _mutex = new Mutex(true, "ScreenActivator", out createdNew);

            // InitializeLog
            Logger.Log.Logger.InitializeLog();

            if (!createdNew)
            {
                Sound.Warning();
                Msg.CustomMessageBox.Show("The Application is Already running on ur Machine");
                // Bring other instance to front and exit.
                Process current = Process.GetCurrentProcess();
                foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        SetForegroundWindow(process.MainWindowHandle);
                        break;
                    }
                }
                Application.Current.Shutdown();
            }
            else
            {
                // Add Event handler to exit event.
                Exit += CloseMutexHandler;
            }
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Msg.CustomMessageBox.Show(e.Exception.Message);
        }

        protected virtual void CloseMutexHandler(object sender, EventArgs e)
        {
            _mutex?.Close();
        }
    }
}
