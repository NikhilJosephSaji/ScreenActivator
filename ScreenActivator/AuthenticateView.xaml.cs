using System;
using System.Collections.Generic;
using System.Text;
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
        }

        private bool valid()
        {
            if (ValidateUsserName.Text == null || ValidateUsserName.Text == string.Empty)
            {
                ErrorMsg.Visibility = Visibility.Visible;
                ErrorMsg.Content = "Please Enter UsserName";
                return false;
            }

            if (validatePassword.Password == null || validatePassword.Password == string.Empty)
            {
                ErrorMsg.Visibility = Visibility.Visible;
                ErrorMsg.Content = "Please Enter Password";
                return false;
            }

            return true;
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            if (!valid())
                return;
            if (ValidateUsserName.Text == "admin" && validatePassword.Password == "admin")
            {
                this.Close();
                new AdminScreen(_win).Show();
            }
            else
            {
                ErrorMsg.Visibility = Visibility.Visible;
                ErrorMsg.Content = "Authentication Failed";
            }
        }

        private void ValidateCloseButton_Click(object sender, RoutedEventArgs e)
        {
            _win.AdminScreenCount = 0;
            this.Close();
        }
    }
}
