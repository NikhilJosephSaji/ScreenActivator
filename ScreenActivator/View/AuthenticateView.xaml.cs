using Logger;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ScreenActivator
{
    /// <summary>
    /// Interaction logic for AuthenticateView.xaml
    /// </summary>
    public partial class AuthenticateView : Window
    {
        private MainWindow _win;
        public AuthenticateView(MainWindow win)
        {
            _win = win;
            InitializeComponent();
            ErrorMsg.Visibility = Visibility.Collapsed;
            _win.Speech?.Speak("Authenticate Window Opened");
            _win.Logger?.Log.LogInfo(LogLevel.SummaryInfo, "Authenticate Window Loaded");
        }

        private bool valid()
        {
            if (ValidateUsserName.Text == null || ValidateUsserName.Text == string.Empty)
            {
                ErrorMsg.Visibility = Visibility.Visible;
                ErrorMsg.Content = "Please Enter UserName";
                _win.Speech?.Speak(ErrorMsg.Content.ToString());
                return false;
            }

            if (validatePassword.Password == null || validatePassword.Password == string.Empty)
            {
                ErrorMsg.Visibility = Visibility.Visible;
                ErrorMsg.Content = "Please Enter Password";
                _win.Speech?.Speak(ErrorMsg.Content.ToString());
                return false;
            }

            return true;
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            _win.Logger?.Log.LogInfo(LogLevel.SummaryInfo, "Application Validate Button Clicked");
            _win.Sound?.ClickSound();            
            if (!valid())
                return;
            if (ValidateUsserName.Text == "admin" && validatePassword.Password == "admin")
            {
                _win.Speech?.Speak("Validate User");
                Thread.Sleep(1000);
                this.Close();
                new AdminScreen(_win).Show();
            }
            else
            {
                ErrorMsg.Visibility = Visibility.Visible;
                ErrorMsg.Content = "Authentication Failed";
                _win.Speech?.Speak(ErrorMsg.Content.ToString());
            }
        }

        private void ValidateCloseButton_Click(object sender, RoutedEventArgs e)
        {
            _win.Logger?.Log.LogInfo(LogLevel.SummaryInfo, "Application Authenticate Window Close Button Clicked");
            _win.Sound?.ClickSound();
            _win.Speech?.Speak("Authenticate Window Closed");
            _win.AdminScreenCount = 0;
            this.Close();
        }

        private void validate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Validate_Click(sender, null);
            }
        }
    }
}
