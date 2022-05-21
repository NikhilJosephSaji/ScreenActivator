using Logger;
using ScreenActivator.Buisness;
using System.Diagnostics;
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

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            _win.Logger?.Log.LogInfo(LogLevel.SummaryInfo, "AdminSCreen Save Button Clicked");
            _win.Sound?.ClickSound();
            _win.Speech?.Speak("Save Button Clicked");
            xml.Xml.Element("DisableMicroPhone").Value = DisableMicroPhone.IsChecked.ToString();
            xml.Xml.Element("DisableSpeaker").Value = DisableSpeaker.IsChecked.ToString();
            xml.Xml.Element("EnableSound").Value = EnableSound.IsChecked.ToString();
            xml.Xml.Element("EnableMinimize").Value = EnableMinimize.IsChecked.ToString();
            xml.Xml.Element("EnableScreenDrag").Value = EnableScreenDrag.IsChecked.ToString();
            xml.Xml.Element("EnableSpeech").Value = EnableSpeech.IsChecked.ToString();
            xml.Xml.Element("EnableLog").Value = EnableLog.IsChecked.ToString();
            if (xml.SaveXml() == 1)
                Msg.CustomMessageBox.Show("Settings Saved Sucessfully !");
            _drag_Value = false;
            _win.GetXml();
            _win.ApplySettings();
            if (!_drag_Value)
                if (EnableScreenDrag.IsChecked.Value)
                    if (Msg.CustomMessageBox.Show("Restart is Required as Enable the Screen Drag. Do you want to Restart.", "Warning", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        new ScreenActivatorHelper(_win).RestartMyApplication();
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
            DisableMicroPhone.IsChecked = xml.ConvertXmlStringToBool(xml.Xml.Element("DisableMicroPhone").Value);
            DisableSpeaker.IsChecked = xml.ConvertXmlStringToBool(xml.Xml.Element("DisableSpeaker").Value);
            EnableSound.IsChecked = xml.ConvertXmlStringToBool(xml.Xml.Element("EnableSound").Value);
            EnableMinimize.IsChecked = xml.ConvertXmlStringToBool(xml.Xml.Element("EnableMinimize").Value);
            EnableScreenDrag.IsChecked = xml.ConvertXmlStringToBool(xml.Xml.Element("EnableScreenDrag").Value);
            EnableSpeech.IsChecked = xml.ConvertXmlStringToBool(xml.Xml.Element("EnableSpeech").Value);
            EnableLog.IsChecked = xml.ConvertXmlStringToBool(xml.Xml.Element("EnableLog").Value);
            _drag_Value = false;
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
    }
}
