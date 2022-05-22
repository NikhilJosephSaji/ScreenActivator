using Logger;
using ScreenActivator.Buisness;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using Msg = CustomMessageBox;

namespace ScreenActivator
{
    /// <summary>
    /// Interaction logic for AdminScreen.xaml
    /// </summary>
    public partial class AdminScreen : Window
    {
        private MainWindow _win;
        private XmlHelper xml;
        private bool _drag_Value;
        public AdminScreen(MainWindow win)
        {
            _win = win;
            InitializeComponent();
            _win.Logger?.Log.LogInfo(LogLevel.SummaryInfo, "AdminScreen initialized");
        }

        private void closebtn_Click(object sender, RoutedEventArgs e)
        {
            _win.Logger?.Log.LogInfo(LogLevel.SummaryInfo, "AdminScreen Close Button Clicked");
            _win.Speech?.Speak("Admin window Closed");
            _win.Sound?.ClickSound();
            _win.AdminScreenCount = 0;
            this.Close();
            _win.Show();
        }

        private void FeedBack_Click(object sender, RoutedEventArgs e)
        {
            _win.Sound?.ClickSound();
            _win.Logger?.Log.LogInfo(LogLevel.SummaryInfo, "AdminScreen FeedBack Link Clicked");
            var url = "https://screenactivator.com/";
            try
            {
                Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", url);
            }
            catch
            {
                try { Process.Start(@"C:\Program Files\Internet Explorer\iexplore.exe", url); }
                catch
                {
                    Process.Start(@"C:\Program Files(x86)\Internet Explorer\iexplore.exe", url);
                }
            }
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            _win.Logger?.Log.LogInfo(LogLevel.SummaryInfo, "AdminSCreen Save Button Clicked");
            _win.Sound?.ClickSound();
            _win.Speech?.Speak("Save Button Clicked");
            xml.Xml.Element(Encryption.StringToHex("DisableMicroPhone")).Value = SetValuetoXml(DisableMicroPhone.IsChecked.ToString());
            xml.Xml.Element(Encryption.StringToHex("DisableSpeaker")).Value = SetValuetoXml(DisableSpeaker.IsChecked.ToString());
            xml.Xml.Element(Encryption.StringToHex("EnableSound")).Value = SetValuetoXml(EnableSound.IsChecked.ToString());
            xml.Xml.Element(Encryption.StringToHex("EnableMinimize")).Value = SetValuetoXml(EnableMinimize.IsChecked.ToString());
            xml.Xml.Element(Encryption.StringToHex("EnableScreenDrag")).Value = SetValuetoXml(EnableScreenDrag.IsChecked.ToString());
            xml.Xml.Element(Encryption.StringToHex("EnableSpeech")).Value = SetValuetoXml(EnableSpeech.IsChecked.ToString());
            xml.Xml.Element(Encryption.StringToHex("EnableLog")).Value = SetValuetoXml(EnableLog.IsChecked.ToString());
            _drag_Value = xml.XmlStringToBool(new XmlHelper().Xml.Element(Encryption.StringToHex("EnableScreenDrag")).Value);
            if (xml.SaveXml() == 1)
            {
                var msg = "Settings Saved Sucessfully !";
                if (_win.Speech != null)
                    await Task.Run(() => Thread.Sleep(2000));
                _win.Speech?.Speak(msg);
                _win.Sound?.ExclamationSound();
                Msg.CustomMessageBox.Show(msg);
            }
            _win.GetXml();
            _win.ApplySettings();
            if (!_drag_Value)
                if (EnableScreenDrag.IsChecked.Value)
                {
                    _win.Sound?.ExclamationSound();
                    var msg = "Restart is Required as the Screen Drag is Enabled. Do you want to Restart.";
                    _win.Speech?.Speak(msg);
                    if (Msg.CustomMessageBox.Show(msg, "Warning", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        new ScreenActivatorHelper(_win).RestartMyApplication();
                }
        }

        private void AdminScreen_Loaded(object sender, RoutedEventArgs e)
        {
            _win.Logger?.Log.LogInfo(LogLevel.SummaryInfo, "AdminSCreen Window Loaded");
            GetXmlData();
            _win.Speech?.Speak("Admin Window Opened");
        }

        private void GetXmlData()
        {
            xml = new XmlHelper();
            DisableMicroPhone.IsChecked = xml.XmlStringToBool(xml.Xml.Element(Encryption.StringToHex("DisableMicroPhone")).Value);
            DisableSpeaker.IsChecked = xml.XmlStringToBool(xml.Xml.Element(Encryption.StringToHex("DisableSpeaker")).Value);
            EnableSound.IsChecked = xml.XmlStringToBool(xml.Xml.Element(Encryption.StringToHex("EnableSound")).Value);
            EnableMinimize.IsChecked = xml.XmlStringToBool(xml.Xml.Element(Encryption.StringToHex("EnableMinimize")).Value);
            EnableScreenDrag.IsChecked = xml.XmlStringToBool(xml.Xml.Element(Encryption.StringToHex("EnableScreenDrag")).Value);
            EnableSpeech.IsChecked = xml.XmlStringToBool(xml.Xml.Element(Encryption.StringToHex("EnableSpeech")).Value);
            EnableLog.IsChecked = xml.XmlStringToBool(xml.Xml.Element(Encryption.StringToHex("EnableLog")).Value);
        }

        private void EnableSoundAndSpeech_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = (ToggleButton)sender;
            if (tb.IsChecked.Value)
            {
                if (tb.Name == EnableSound.Name)
                {
                    if (EnableSpeech.IsChecked.Value)
                        EnableSpeech.IsChecked = false;
                }
                else
                {
                    if (EnableSound.IsChecked.Value)
                        EnableSound.IsChecked = false;
                }
            }
        }

        private string SetValuetoXml(string str)
        {
            Random _rdm = new Random();
            var val = Guid.NewGuid().ToString() + "-" + _rdm.Next(0000, 9999) + str;
            return Encryption.Encrypt(val);
        }
    }
}
