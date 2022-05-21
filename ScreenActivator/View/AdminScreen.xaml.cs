﻿using Logger;
using ScreenActivator.Buisness;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
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
            _win.Logger?.Log.LogInfo(LogLevel.SummaryInfo, "AdminScreen FeedBack Link Clicked");
            var url = "http://screenactivator.com/";
            Process.Start("chrome.exe", url);
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
            _win.GetXml();
            _win.ApplySettings();
        }

        private void AdminScreen_Loaded(object sender, RoutedEventArgs e)
        {
            _win.Logger?.Log.LogInfo(LogLevel.SummaryInfo, "AdminSCreen Window Loaded");
            xml = new XmlHelper();
            DisableMicroPhone.IsChecked = xml.ConvertXmlStringToBool(xml.Xml.Element("DisableMicroPhone").Value);
            DisableSpeaker.IsChecked = xml.ConvertXmlStringToBool(xml.Xml.Element("DisableSpeaker").Value);
            EnableSound.IsChecked = xml.ConvertXmlStringToBool(xml.Xml.Element("EnableSound").Value);
            EnableMinimize.IsChecked = xml.ConvertXmlStringToBool(xml.Xml.Element("EnableMinimize").Value);
            EnableScreenDrag.IsChecked = xml.ConvertXmlStringToBool(xml.Xml.Element("EnableScreenDrag").Value);
            EnableSpeech.IsChecked = xml.ConvertXmlStringToBool(xml.Xml.Element("EnableSpeech").Value);
            EnableLog.IsChecked = xml.ConvertXmlStringToBool(xml.Xml.Element("EnableLog").Value);
            _win.Speech?.Speak("Admin Window Opened");
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