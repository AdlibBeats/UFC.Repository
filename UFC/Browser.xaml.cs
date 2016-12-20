using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UFC.Pages;

namespace UFC
{
    public partial class Browser : Window
    {
        public Browser()
        {
            InitializeComponent();
        }

        private void floppyPages_Navigated(object sender, RoutedEventArgs e)
        {
            if (floppyPages.CanGoBack)
                BackButton.Visibility = Visibility.Visible;
        }

        private void floppyPages_WentBack(object sender, RoutedEventArgs e)
        {
            if (!floppyPages.CanGoBack)
                BackButton.Visibility = Visibility.Hidden;
        }

        private void Button_GoBack(object sender, RoutedEventArgs e)
        {
            floppyPages.GoBack();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            floppyPages.FirstPage = new MainPage();
        }
    }
}
