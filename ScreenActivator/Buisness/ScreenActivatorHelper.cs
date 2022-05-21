using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ScreenActivator.Buisness
{
    public class ScreenActivatorHelper
    {
        private MainWindow _win;
        public ScreenActivatorHelper(MainWindow win)
        {
            _win = win;
        }
        public void DisableEnableMicButton(bool isNeeded)
        {
            if (isNeeded)
            {
                _win.MuteMicrophone.Background = Brushes.SlateBlue;
                _win.MuteMicrophone.Click -= new RoutedEventHandler(_win.MuteMicrophone_Click);
            }
            else
            {
                _win.MuteMicrophone.Background = Brushes.LightBlue;
                _win.MuteMicrophone.Click += new RoutedEventHandler(_win.MuteMicrophone_Click);
            }
        }

        public void DisableEnableSpeaker(bool isNeeded)
        {
            if (isNeeded)
            {
                _win.SpeakerBtn.Background = Brushes.SlateBlue;
                _win.SpeakerBtn.Click -= new RoutedEventHandler(_win.SpeakerBtn_Click);
            }
            else
            {
                _win.SpeakerBtn.Background = Brushes.LightBlue;
                _win.SpeakerBtn.Click += new RoutedEventHandler(_win.SpeakerBtn_Click);
            }
        }

        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MOVE = 0xF010;

        public IntPtr WndProcNoMove(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            switch (msg)
            {
                case WM_SYSCOMMAND:
                    int command = wParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                    {
                        handled = true;
                    }
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        public void RestartMyApplication()
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Process.Start(path + "\\Restarter.exe");
            Environment.Exit(0);
        }
    }
}
