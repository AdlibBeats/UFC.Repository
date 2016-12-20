using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using UFC.UI;

namespace UFC.Pages
{
    public partial class Page3 : Page, IFloppyPage
    {
        public event FloppyPageNavigateEventHandler Navigate;
        public event FloppyPageGoBackEventHandler GoBack;
        public IFloppyPages IFloppyPages { get; set; }
        public Page3() : this(null) { }
        public Page3(object dataContext)
        {
            InitializeComponent();
            if (dataContext != null)
                this.DataContext = dataContext;
            else
                this.DataContext = this;
            Title = "Третья страница";
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Navigate != null)
            {
                Task selected = Task.Run(() =>
                {
                    this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
                    {
                        Thread.Sleep(100);
                        switch (listBox.SelectedIndex)
                        {
                            case 0: Navigate(new Page2(DataContext), new FloppyPageEventArgs()); break;
                            case 1: Navigate(new Page4(DataContext), new FloppyPageEventArgs()); break;
                            case 2: GoBack(new FloppyPageEventArgs()); break;
                            default: break;
                        }
                        listBox.SelectedIndex = -1;
                    });
                });
            }
        }
    }
}
