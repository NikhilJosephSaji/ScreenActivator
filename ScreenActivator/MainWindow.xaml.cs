using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ScreenActivator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer MOuseTimer = new DispatcherTimer();
        DispatcherTimer IdleTimerFinder = new DispatcherTimer();
        DispatcherTimer KeyPressTimer = new DispatcherTimer();
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        private bool Speaker = false;
        private bool Mic = false;
        private int[] ss = new int[61];
        private int radius = 140, pos = 0, WIDTH = 600, HEIGHT = 600, cx, cy;
        private ViewModel vm;
        private WindowState m_storedWindowState = WindowState.Normal;
        private bool KeyBoardRunning = false;
        private bool ScreenRunning = false;
        private bool MouseRunning = false;
        public MainWindow()
        {
            InitializeComponent();
            cx = WIDTH / 2;
            cy = HEIGHT / 2;
            vm = new ViewModel(this);
            Initialize();
        }

        private void Initialize()
        {
            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "The app has been minimised.";
            m_notifyIcon.BalloonTipTitle = "Screen Activator";
            m_notifyIcon.Text = "Screen Activator";
            m_notifyIcon.Icon = new System.Drawing.Icon("ScreenIco.ico");
            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);
            MOuseTimer.Interval = new TimeSpan(0, 0, 5);
            MOuseTimer.Tick += Changemousepointer;
            KeyPressTimer.Interval = new TimeSpan(0, 0, 10);
            KeyPressTimer.Tick += KeyPressTimer_Tick;
            SpecialFunction.Background = (LinearGradientBrush)this.Resources["normal"];
            this.DataContext = vm;
            for (int i = 0; i <= 60; i++)
                ss[i] = i;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            KeepMonitorActive();
            Screen.Background = Brushes.LightBlue;
            Mute.Background = Brushes.LightBlue;
            MuteMicrophone.Background = Brushes.LightBlue;
            ScreenRunning = true;
            Speaker = true;
            Mic = true;
            this.MouseLeftButtonDown += delegate
            {
                try { DragMove(); } catch { }
            };
        }

        public void KeepMonitorActive()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS); //Do not Go To Sleep
        }

        private void KeyPressTimer_Tick(object sender, EventArgs e)
        {
            keybd_event(VK_CAPSLOCK, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            keybd_event(VK_CAPSLOCK, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP,
                (UIntPtr)0);
        }

        private void Changemousepointer(object sender, EventArgs e)
        {          
            var handCoord = msCoord(ss[pos], radius);
            SetCursorPos(handCoord[0], handCoord[1]);
            pos++;
            if (pos == 60)
                pos = 1;
        }

        private int[] msCoord(int val, int hlen)
        {
            int[] coord = new int[2];
            val *= 6;   //each minute and second make 6 degree

            if (val >= 0 && val <= 180)
            {
                coord[0] = cx + (int)(hlen * Math.Sin(Math.PI * val / 180));
                coord[1] = cy - (int)(hlen * Math.Cos(Math.PI * val / 180));
            }
            else
            {
                coord[0] = cx - (int)(hlen * -Math.Sin(Math.PI * val / 180));
                coord[1] = cy - (int)(hlen * Math.Cos(Math.PI * val / 180));
            }
            return coord;
        }

        void m_notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = m_storedWindowState;
        }

        void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            CheckTrayIcon();
        }

        void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        void ShowTrayIcon(bool show)
        {
            if (m_notifyIcon != null)
                m_notifyIcon.Visible = show;
        }
        public void RestoreMonitorSettings()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS); //Restore Previous Settings, ie, Go To Sleep Again
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            if (ScreenRunning)
                RestoreMonitorSettings();
            if (MouseRunning)
                MOuseTimer.Stop();
            if (KeyBoardRunning)
                KeyPressTimer.Stop();
        }

        void OnClose(object sender, CancelEventArgs args)
        {
            m_notifyIcon.Dispose();
            m_notifyIcon = null;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                if (m_notifyIcon != null)
                    m_notifyIcon.ShowBalloonTip(2000);
            }
            else
                m_storedWindowState = WindowState;
        }

        #region Click Handlers
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MOuse_Click(object sender, RoutedEventArgs e)
        {
            if (MouseRunning)
            {
                MOuseTimer.Stop();
                MouseRunning = false;
                MOuse.Background = Brushes.White;
            }
            else
            {
                MOuseTimer.Start();
                MouseRunning = true;
                MOuse.Background = Brushes.LightBlue;
            }
        }

        private async void SpecialFunction_Click(object sender, RoutedEventArgs e)
        {
            vm.ProcessHandleStop(false);
            ChangeBackgroundOfSpecialCase(false);
            await ChangetheSizeofUIandVisibility(false);
        }
        private void KeyBoard_Click(object sender, RoutedEventArgs e)
        {
            if (KeyBoardRunning)
            {
                KeyPressTimer.Stop(); KeyBoardRunning = false; KeyBoard.Background = Brushes.White;
            }
            else { KeyPressTimer.Start(); KeyBoardRunning = true; KeyBoard.Background = Brushes.LightBlue; }

        }

        private void Screen_Click(object sender, RoutedEventArgs e)
        {
            if (ScreenRunning)
            {
                RestoreMonitorSettings();
                Screen.Background = Brushes.White;
                ScreenRunning = false;
            }
            else
            {
                KeepMonitorActive();
                Screen.Background = Brushes.LightBlue;
                ScreenRunning = true;
            }
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            SetMicAndSpeaker("Speakers");
        }

        private void MuteMicrophone_Click(object sender, RoutedEventArgs e)
        {
            SetMicAndSpeaker("Microphone");
        }

        #endregion 

        private void SetMicAndSpeaker(string sysdevice)
        {
            using (var enumerator = new NAudio.CoreAudioApi.MMDeviceEnumerator())
            {
                foreach (var device in enumerator.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.All, NAudio.CoreAudioApi.DeviceState.Active))
                {
                    if (device.AudioEndpointVolume?.HardwareSupport.HasFlag(NAudio.CoreAudioApi.EEndpointHardwareSupport.Mute) == true)
                    {
                        if (device.FriendlyName.Contains(sysdevice))
                        {
                            if (sysdevice == "Speakers")
                            {
                                if (Speaker)
                                { device.AudioEndpointVolume.Mute = true; Speaker = false; Mute.Background = Brushes.White; }
                                else
                                { device.AudioEndpointVolume.Mute = false; Speaker = true; Mute.Background = Brushes.LightBlue; }
                            }
                            else if (sysdevice == "Microphone")
                            {
                                if (Mic)
                                { device.AudioEndpointVolume.Mute = true; Mic = false; MuteMicrophone.Background = Brushes.White; }
                                else
                                { device.AudioEndpointVolume.Mute = false; Mic = true; MuteMicrophone.Background = Brushes.LightBlue; }
                            }
                        }
                    }
                }
            }
        }

        public void ChangeBackgroundOfSpecialCase(bool isclicked)
        {
            if (isclicked)
                SpecialFunction.Background = (LinearGradientBrush)this.Resources["Clicked"];
            else
                SpecialFunction.Background = (LinearGradientBrush)this.Resources["normal"];
        }

        public async Task ChangetheSizeofUIandVisibility(bool isneeded)
        {
            await Task.Run(() => { Thread.Sleep(3000); });
            if (isneeded)
            { await HeightIncreaser(true); SpecialFunctionPanel.Visibility = Visibility.Visible; }
            else
            { await HeightIncreaser(false); SpecialFunctionPanel.Visibility = Visibility.Collapsed; }
        }

        private async Task HeightIncreaser(bool tomax)
        {
            int min = 95;
            int max = 145;
            if (tomax)
            {
                this.Height = min;
                while (this.Height <= max)
                {
                    this.Height = this.Height + 1;
                    await Task.Run(() => { Thread.Sleep(50); });
                }
            }
            else
            {
                this.Height = max;
                while (this.Height >= min)
                {
                    this.Height = this.Height - 1;
                    await Task.Run(() => { Thread.Sleep(50); });
                }
            }
        }

        #region KeyBoard Settings
        public const byte VK_NUMLOCK = 0x90;
        public const byte VK_CAPSLOCK = 0x14;

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        const int KEYEVENTF_EXTENDEDKEY = 0x1;
        const int KEYEVENTF_KEYUP = 0x2;
        #endregion

        #region Mouse Settings
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        #endregion

        #region Screen Settings
        [FlagsAttribute()]
        public enum EXECUTION_STATE : uint //Determine Monitor State
        {
            ES_AWAYMODE_REQUIRED = 0x40,
            ES_CONTINUOUS = 0x80000000u,
            ES_DISPLAY_REQUIRED = 0x2,
            ES_SYSTEM_REQUIRED = 0x1
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }

        //Enables an application to inform the system that it is in use, thereby preventing the system from entering sleep or turning off the display while the application is running.
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        //This function queries or sets system-wide parameters, and updates the user profile during the process.
        [DllImport("user32", EntryPoint = "SystemParametersInfo", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const Int32 SPI_SETSCREENSAVETIMEOUT = 15;
        #endregion
    }
}
