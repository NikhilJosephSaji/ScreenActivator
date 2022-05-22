using Logger;
using ScreenActivator.Buisness;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Msg = CustomMessageBox;

namespace ScreenActivator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Variable

        private DispatcherTimer mouseTimer = new DispatcherTimer();
        private DispatcherTimer keyPressTimer = new DispatcherTimer();
        private System.Windows.Forms.NotifyIcon m_notifyIcon;
        private bool speaker = false;
        private bool mic = false;
        private int[] ss = new int[61];
        private int radius = 140, pos = 0, WIDTH = 600, HEIGHT = 600, cx, cy;
        private ViewModel vm;
        private WindowState m_storedWindowState = WindowState.Normal;
        private bool keyBoardRunning = false;
        private bool screenRunning = false;
        private bool mouseRunning = false;
        private int adminScreenCount = 0;
        private ScreenActivatorGlobal scActG;
        private ScreenActivatorHelper helper;
        private WindowInteropHelper help;
        private HwndSource source;

        #endregion

        #region PublicVariables
        public int AdminScreenCount { get { return adminScreenCount; } set { adminScreenCount = value; } }
        public Sound Sound;
        public Speech Speech;
        public Logging Logger;
        #endregion
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
            mouseTimer.Interval = new TimeSpan(0, 0, 5);
            mouseTimer.Tick += Changemousepointer;
            keyPressTimer.Interval = new TimeSpan(0, 0, 10);
            keyPressTimer.Tick += KeyPressTimer_Tick;
            SpecialFunction.Background = (LinearGradientBrush)this.Resources["normal"];
            this.DataContext = vm;
            for (int i = 0; i <= 60; i++)
                ss[i] = i;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            help = new WindowInteropHelper(this);
            source = HwndSource.FromHwnd(help.Handle);
            GetXml();
            ApplySettings();
            KeepMonitorActive();
            Screen.Background = Brushes.LightBlue;
            screenRunning = true;
            Logger?.Log.LogInfo(LogLevel.SummaryInfo, "ScreenActivator Loaded");
        }

        public void EnableDisableDrag(bool isNeeded)
        {
            if (!isNeeded)
            {
                source.AddHook(helper.WndProcNoMove);
            }
            else
            {
                source.RemoveHook(helper.WndProcNoMove);
            }
        }

        public void ApplySettings()
        {
            helper = new ScreenActivatorHelper(this);
            if (scActG.DisableMicroPhone)
                helper.DisableEnableMicButton(scActG.DisableMicroPhone);
            else
            {
                helper.DisableEnableMicButton(scActG.DisableMicroPhone);
                GetSpeakerandMicStatus(false);
            }

            if (scActG.DisableSpeaker)
                helper.DisableEnableSpeaker(scActG.DisableSpeaker);
            else
            {
                helper.DisableEnableSpeaker(scActG.DisableSpeaker);
                GetSpeakerandMicStatus(true);
            }

            if (scActG.EnableScreenDrag)
                EnableDisableDrag(true);
            else
                EnableDisableDrag(false);

            if (scActG.EnableSound)
                Sound = new Sound();
            else
                Sound = null;

            if (scActG.EnableSpeech)
                Speech = new Speech();
            else
                Speech = null;

            if (scActG.EnableLog)
                Logger = new Logging();
            else
                Logger = null;
            Logger?.Log.LogInfo(LogLevel.SummaryInfo, "ScreenActivator Settings Applied");
        }

        public void GetXml()
        {
            XmlHelper xml = new XmlHelper();
            scActG.DisableMicroPhone = xml.XmlStringToBool(xml.Xml.Element(Encryption.StringToHex("DisableMicroPhone")).Value);
            scActG.DisableSpeaker = xml.XmlStringToBool(xml.Xml.Element(Encryption.StringToHex("DisableSpeaker")).Value);
            scActG.EnableSound = xml.XmlStringToBool(xml.Xml.Element(Encryption.StringToHex("EnableSound")).Value);
            scActG.EnableMinimize = xml.XmlStringToBool(xml.Xml.Element(Encryption.StringToHex("EnableMinimize")).Value);
            scActG.EnableScreenDrag = xml.XmlStringToBool(xml.Xml.Element(Encryption.StringToHex("EnableScreenDrag")).Value);
            scActG.EnableSpeech = xml.XmlStringToBool(xml.Xml.Element(Encryption.StringToHex("EnableSpeech")).Value);
            scActG.EnableLog = xml.XmlStringToBool(xml.Xml.Element(Encryption.StringToHex("EnableLog")).Value);
            Logger?.Log.LogInfo(LogLevel.SummaryInfo, "Xml Data Loaded");
        }

        public void CallMouseClickHandler()
        {
            this.MOuse.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        public void CallKeyBoardClickHanlder()
        {
            this.KeyBoard.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        public void CallMicroPhoneClickHanlder()
        {
            this.MuteMicrophone.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        public void CallSpeakerClickHanlder()
        {
            this.SpeakerBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void GetSpeakerandMicStatus(bool _speaker)
        {
            if (_speaker)
            {
                if (!SetMicAndSpeaker("Speakers", true))
                {
                    SpeakerBtn.Background = Brushes.LightBlue;
                    speaker = true;
                }
                else
                {
                    speaker = false;
                    SpeakerBtn.Background = Brushes.White;
                }
            }
            else
            {
                if (!SetMicAndSpeaker("Microphone", true))
                {
                    MuteMicrophone.Background = Brushes.LightBlue;
                    mic = true;
                }
                else
                {
                    mic = false;
                    MuteMicrophone.Background = Brushes.White;
                }
            }
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
            if (screenRunning)
                RestoreMonitorSettings();
            if (mouseRunning)
                mouseTimer.Stop();
            if (keyBoardRunning)
                keyPressTimer.Stop();
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
                if (scActG.EnableMinimize)
                {
                    Hide();
                    if (m_notifyIcon != null)
                        m_notifyIcon.ShowBalloonTip(2000);
                }
            }
            else
                m_storedWindowState = WindowState;
        }

        #region Click Handlers
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Logger?.Log.LogInfo(LogLevel.SummaryInfo, "Application Minimize Button Clicked");
            Sound?.ClickSound();
            Speech?.Speak("Minimize Button Clicked");
            this.WindowState = WindowState.Minimized;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Logger?.Log.LogInfo(LogLevel.SummaryInfo, "ScreenActivator Exited");
            Sound?.ClickSound();
            Speech?.Speak("Application Closed");
            await Task.Run(() => Thread.Sleep(500));
            this.Close();
        }

        private void MOuse_Click(object sender, RoutedEventArgs e)
        {
            Logger?.Log.LogInfo(LogLevel.SummaryInfo, "Application Mouse Button Clicked");
            Sound?.ClickSound();
            Speech?.Speak("Mouse Button Clicked");
            if (mouseRunning)
            {
                mouseTimer.Stop();
                mouseRunning = false;
                MOuse.Background = Brushes.White;
            }
            else
            {
                mouseTimer.Start();
                mouseRunning = true;
                MOuse.Background = Brushes.LightBlue;
            }
        }

        private async void SpecialFunction_Click(object sender, RoutedEventArgs e)
        {
            Logger?.Log.LogInfo(LogLevel.SummaryInfo, "Application Danger Clicked");
            Sound?.ClickSound();
            Speech?.Speak("Special Button Clicked");
            vm.ProcessHandleStop(false);
            ChangeBackgroundOfSpecialCase(false);
            await ChangetheSizeofUIandVisibility(false);
        }
        private void KeyBoard_Click(object sender, RoutedEventArgs e)
        {
            Logger?.Log.LogInfo(LogLevel.SummaryInfo, "Application KeyBoard Button Clicked");
            Sound?.ClickSound();
            Speech?.Speak("Keyboard Button Clicked");
            if (keyBoardRunning)
            {
                keyPressTimer.Stop(); keyBoardRunning = false; KeyBoard.Background = Brushes.White;
            }
            else { keyPressTimer.Start(); keyBoardRunning = true; KeyBoard.Background = Brushes.LightBlue; }

        }

        private void Screen_Click(object sender, RoutedEventArgs e)
        {
            Logger?.Log.LogInfo(LogLevel.SummaryInfo, "Application Screen Button Clicked");
            Sound?.ClickSound();
            Speech?.Speak("Screen Button Clicked");
            if (screenRunning)
            {
                RestoreMonitorSettings();
                Screen.Background = Brushes.White;
                screenRunning = false;
            }
            else
            {
                KeepMonitorActive();
                Screen.Background = Brushes.LightBlue;
                screenRunning = true;
            }
        }

        public void SpeakerBtn_Click(object sender, RoutedEventArgs e)
        {
            Logger?.Log.LogInfo(LogLevel.SummaryInfo, "Application Speaker Button Clicked");
            Sound?.ClickSound();
            Speech?.Speak("Speaker Button Clicked");
            SetMicAndSpeaker("Speakers");
        }

        public void MuteMicrophone_Click(object sender, RoutedEventArgs e)
        {
            Logger?.Log.LogInfo(LogLevel.SummaryInfo, "Application Microphone Button Clicked");
            Sound?.ClickSound();
            Speech?.Speak("Microphone Button Clicked");
            SetMicAndSpeaker("Microphone");
        }

        private void VersionHyperlink_Click(object sender, RoutedEventArgs e)
        {
            adminScreenCount++;
            if (adminScreenCount == 6)
                if (!mouseRunning && !keyBoardRunning && !vm.SpecialFun)
                    new AuthenticateView(this).Show();
                else
                {
                    Msg.CustomMessageBox.Show("When Mouse or KeyBoard is Activated you can't get into Admin Screen");
                    adminScreenCount = 0;
                }
        }

        #endregion

        private bool SetMicAndSpeaker(string sysdevice, bool checkstatus = false)
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
                                if (checkstatus)
                                    return device.AudioEndpointVolume.Mute;
                                if (speaker)
                                { device.AudioEndpointVolume.Mute = true; speaker = false; SpeakerBtn.Background = Brushes.White; }
                                else
                                { device.AudioEndpointVolume.Mute = false; speaker = true; SpeakerBtn.Background = Brushes.LightBlue; }
                            }
                            else if (sysdevice == "Microphone")
                            {
                                if (checkstatus)
                                    return device.AudioEndpointVolume.Mute;
                                if (mic)
                                { device.AudioEndpointVolume.Mute = true; mic = false; MuteMicrophone.Background = Brushes.White; }
                                else
                                { device.AudioEndpointVolume.Mute = false; mic = true; MuteMicrophone.Background = Brushes.LightBlue; }
                            }
                        }
                    }
                }
            }
            return false;
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
            Sound?.DangerSound();
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