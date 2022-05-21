using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            _win.speech?.Speak("Authenticate Window Opened");
        }

        private bool valid()
        {
            if (ValidateUsserName.Text == null || ValidateUsserName.Text == string.Empty)
            {
                ErrorMsg.Visibility = Visibility.Visible;
                ErrorMsg.Content = "Please Enter UsserName";
                _win.speech?.Speak(ErrorMsg.Content.ToString());
                return false;
            }

            if (validatePassword.Password == null || validatePassword.Password == string.Empty)
            {
                ErrorMsg.Visibility = Visibility.Visible;
                ErrorMsg.Content = "Please Enter Password";
                _win.speech?.Speak(ErrorMsg.Content.ToString());
                return false;
            }

            return true;
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            _win.sound?.ClickSound();            
            if (!valid())
                return;
            if (ValidateUsserName.Text == "admin" && validatePassword.Password == "admin")
            {
                _win.speech?.Speak("Validate User");
                Thread.Sleep(1000);
                this.Close();
                new AdminScreen(_win).Show();
            }
            else
            {
                ErrorMsg.Visibility = Visibility.Visible;
                ErrorMsg.Content = "Authentication Failed";
                _win.speech?.Speak(ErrorMsg.Content.ToString());
            }
        }

        private void ValidateCloseButton_Click(object sender, RoutedEventArgs e)
        {
            _win.sound?.ClickSound();
            _win.speech?.Speak("Authenticate Window Closed");
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
