using Logger;
using ScreenActivator.Buisness;
using System.Windows;
using System.Windows.Input;
using Msg = CustomMessageBox;

namespace ScreenActivator.View
{
    /// <summary>
    /// Interaction logic for RecordAreaWindow.xaml
    /// </summary>
    public partial class RecordAreaWindow : Window
    {
        private ScreenActivatorHelper _helper;
        private MainWindow _win;
        public RecordAreaWindow(ScreenActivatorHelper helper, MainWindow win)
        {
            _helper = helper;
            _win = win;
            InitializeComponent();
            ConfirmSize.Visibility = Visibility.Collapsed;
            ConfirmSizeBorder.Visibility = Visibility.Collapsed;
            _win.Logger?.Log.LogInfo(LogLevel.SummaryInfo, "RecordAreaWindow Opened");
            _win.Speech?.Speak("RecordAreaWindow Opened");
        }

        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            _win.Sound?.ClickSound();
            _win.Logger?.Log.LogInfo(LogLevel.SummaryInfo, "Full Screen Button Clicked");
            _win.Speech?.Speak("Full Screen Button Clicked");
            _helper.Width = 0;
            _helper.Height = 0;
            _helper.Top = 0;
            _helper.Left = 0;
            this.Close();
        }

        private void ChooseSizeButton_Click(object sender, RoutedEventArgs e)
        {
            _win.Sound?.ClickSound();
            _win.Logger?.Log.LogInfo(LogLevel.SummaryInfo, "Choose Size Button Clicked");
            _win.Speech?.Speak("Choose Size Button Clicked");
            ConfirmSize.Visibility = Visibility.Visible;
            ConfirmSizeBorder.Visibility = Visibility.Visible;
            ChooseSizeButton.Visibility = Visibility.Collapsed;
            ChooseSizeButtonBorder.Visibility = Visibility.Collapsed;
        }

        private void ConfirmSize_Click(object sender, RoutedEventArgs e)
        {
            _win.Sound?.ClickSound();
            _win.Logger?.Log.LogInfo(LogLevel.SummaryInfo, "Confirm Size Clicked");
            _win.Speech?.Speak("Confirm Size Clicked");
            var result = Msg.CustomMessageBox.Show("Are You Continue with this Size", "Information", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                _helper.Width = (int)this.Width;
                _helper.Height = (int)this.Height;
                _helper.Top = (int)this.Top;
                _helper.Left = (int)this.Left;
                _win.Sound?.ClickSound();
                _win.Logger?.Log.LogInfo(LogLevel.SummaryInfo, "Size Choosed " + "Height : " + _helper.Height + " Width : " + _helper.Width  +" Top : " + _helper.Top + " Left : " + _helper.Left);
                _win.Speech?.Speak("Size Choosed");
                this.Close();
            }
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}
