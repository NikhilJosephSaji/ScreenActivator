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
    /// Interaction logic for AdminScreen.xaml
    /// </summary>
    public partial class AdminScreen : Window
    {
        private MainWindow _win;
        public AdminScreen(MainWindow win)
        {
            _win = win;
            InitializeComponent();
        }

        private void closebtn_Click(object sender, RoutedEventArgs e)
        {
            _win.AdminScreenCount = 0;
            this.Close();
        }
    }
}
