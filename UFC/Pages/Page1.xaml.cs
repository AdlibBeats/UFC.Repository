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
using UFC.UI;

namespace UFC.Pages
{
    public partial class Page1 : Page, IFloppyPage
    {
        public event FloppyPageNavigateEventHandler Navigate;
        public event FloppyPageGoBackEventHandler GoBack;
        public IFloppyPages IFloppyPages { get; set; }
        public Page1() : this(null) { }
        public Page1(object dataContext)
        {
            InitializeComponent();
            if (dataContext != null)
                this.DataContext = dataContext;
            else
                this.DataContext = this;
            Title = "Первая страница";
        }

        private void NavigateTo_MainPage(object sender, RoutedEventArgs e)
        {
            if (Navigate != null)
                Navigate(new MainPage(DataContext), new FloppyPageEventArgs());
        }

        private void NavigateTo_Page2(object sender, RoutedEventArgs e)
        {
            if (Navigate != null)
                Navigate(new Page2(DataContext), new FloppyPageEventArgs());
        }

        private void NavigateTo_Page3(object sender, RoutedEventArgs e)
        {
            if (Navigate != null)
                Navigate(new Page3(DataContext), new FloppyPageEventArgs());
        }

        private void Button_GoBack(object sender, RoutedEventArgs e)
        {
            if (GoBack != null)
                GoBack(new FloppyPageEventArgs());
        }
    }
}
