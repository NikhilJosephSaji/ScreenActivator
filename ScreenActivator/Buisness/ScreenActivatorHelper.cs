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
        private bool speakerEvent;
        private bool micEvent;

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
                micEvent = false;
            }
            else
            {
                _win.MuteMicrophone.Background = Brushes.LightBlue;
                if (!micEvent)
                    _win.MuteMicrophone.Click += new RoutedEventHandler(_win.MuteMicrophone_Click);
                micEvent = true;
            }
        }

        public void ApplyScreenSettings()
        {
            if (_win.ScreenGlobal.EnableScreenRecord)
            {
                _win.Width = 340;
                _win.Record.Visibility = Visibility.Visible;
            }
            else
            {
                _win.Width = 295;
                _win.Record.Visibility = Visibility.Collapsed;
            }
        }

        public void ApplyLogSettings()
        {
            if (_win.ScreenGlobal.EnableLog)
                _win.Logger = new Logging();
            else
                _win.Logger = null;
        }

        public void ApplySpeechSettings()
        {
            if (_win.ScreenGlobal.EnableSpeech)
                _win.Speech = new Speech();
            else
                _win.Speech = null;
        }

        public void ApplySoundSettings()
        {
            if (_win.ScreenGlobal.EnableSound)
                _win.Sound = new Sound();
            else
                _win.Sound = null;
        }

        public void DisableEnableSpeaker(bool isNeeded)
        {
            if (isNeeded)
            {
                _win.SpeakerBtn.Background = Brushes.SlateBlue;
                _win.SpeakerBtn.Click -= new RoutedEventHandler(_win.SpeakerBtn_Click);
                speakerEvent = false;
            }
            else
            {
                _win.SpeakerBtn.Background = Brushes.LightBlue;
                if (!speakerEvent)
                    _win.SpeakerBtn.Click += new RoutedEventHandler(_win.SpeakerBtn_Click);
                speakerEvent = true;
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
