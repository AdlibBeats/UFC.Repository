using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UFC.UI
{
    #region DefaultPage

    /// <summary>
    /// Страница по умолчанию.
    /// </summary>
    internal class DefaultPage : IFloppyPage
    {
        public event FloppyPageNavigateEventHandler Navigate;
        public event FloppyPageGoBackEventHandler GoBack;
        public IFloppyPages IFloppyPages { get; set; }
        public string Title { get; set; }
        public DefaultPage()
        {
            Title = "Страница по умолчанию";
        }
    }
    #endregion

    #region IFloppyPage

    public delegate void FloppyPageNavigateEventHandler(IFloppyPage page, FloppyPageEventArgs e);
    public delegate void FloppyPageGoBackEventHandler(FloppyPageEventArgs e);
    public class FloppyPageEventArgs : EventArgs
    {
        public FloppyPageEventArgs() { }
    }

    public interface IFloppyPage
    {
        event FloppyPageNavigateEventHandler Navigate;
        event FloppyPageGoBackEventHandler GoBack;
        IFloppyPages IFloppyPages { get; set; }
        string Title { get; set; }
    }
    #endregion

    #region IFloppyPages
    public interface IFloppyPages
    {
        IFloppyPage FirstPage { get; set; }
        IFloppyPage CurrentPage { get; set; }
        int JournalCount { get; set; }
        void Navigate(IFloppyPage page);
        bool GoBack();
        bool CanGoBack { get; set; }
    }
    #endregion
}

namespace UFC.UI.Controls
{
    #region FloppyPages
    public class FloppyPages : Control, IFloppyPages, INotifyPropertyChanged
    {
        #region Private Members

        private bool GridNumber = false;
        private bool IsDoneAnimation = true;
        private bool IsDoneInitialization = false;
        private List<IFloppyPage> journal = new List<IFloppyPage>();

        private Frame frame1 = null;
        private Frame frame2 = null;
        private Grid mainGrid = null;
        private Grid grid1 = null;
        private Grid grid2 = null;

        private BeginStoryboard animation1 = null;
        private BeginStoryboard animation2 = null;
        private BeginStoryboard animation3 = null;
        private BeginStoryboard animation4 = null;

        #endregion

        #region Constructors
        static FloppyPages()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FloppyPages), new FrameworkPropertyMetadata(typeof(FloppyPages)));

            FloppyPages.NavigatedRoutedEvent = EventManager.RegisterRoutedEvent("Navigated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FloppyPages));
            FloppyPages.WentBackRoutedEvent = EventManager.RegisterRoutedEvent("WentBack", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FloppyPages));
        }

        public FloppyPages() { }

        #endregion

        #region Public Dependency Properties
        
        public static readonly DependencyProperty FirstPageProperty =
            DependencyProperty.RegisterAttached("FirstPage", typeof(IFloppyPage), typeof(FloppyPages));

        #endregion

        #region Public Properties
        public IFloppyPage FirstPage
        {
            get { return (IFloppyPage)GetValue(FirstPageProperty); }
            set
            {
                SetValue(FirstPageProperty, value);
                OnFirstPage(FirstPage);
                OnPropertyChanged("FirstPage");
            }
        }

        #endregion

        #region Public RoutedEvents

        public static readonly RoutedEvent NavigatedRoutedEvent;
        public static readonly RoutedEvent WentBackRoutedEvent;

        #endregion

        #region Public Events

        public event RoutedEventHandler Navigated
        {
            add { base.AddHandler(FloppyPages.NavigatedRoutedEvent, value); }
            remove { base.RemoveHandler(FloppyPages.NavigatedRoutedEvent, value); }
        }

        public event RoutedEventHandler WentBack
        {
            add { base.AddHandler(FloppyPages.WentBackRoutedEvent, value); }
            remove { base.RemoveHandler(FloppyPages.WentBackRoutedEvent, value); }
        }

        #endregion

        #region Public Members
        public IFloppyPage CurrentPage
        {
            get
            {
                if (journal.Count > 0)
                    return journal[journal.Count - 1];
                else
                    return null;
            }
            set { /*Использую Binding*/ }
        }

        public int JournalCount
        {
            get
            {
                return journal.Count;
            }
            set { /*Использую Binding*/ }
        }

        public void Navigate(IFloppyPage page)
        {
            Start_Navigate(page);
        }

        public bool GoBack()
        {
            return Start_GoBack();
        }

        public bool CanGoBack
        {
            get
            {
                if (journal.Count > 1)
                    return true;
                else
                    return false;
            }
            set { /*Использую Binding*/ }
        }

        #endregion

        #region Private OnFirstPage
        private void OnFirstPage(IFloppyPage page)
        {
            if (page != null && IsDoneInitialization)
            {
                if (GridNumber)
                    frame1.Navigate(page);
                else
                    frame2.Navigate(page);

                page.Navigate += Page_Navigate;
                page.GoBack += Page_GoBack;
                journal.Clear();
                journal.Add(page);
                
                OnPropertyChanged("JournalCount");
                OnPropertyChanged("CanGoBack");
                OnPropertyChanged("CurrentPage");
            }
        }

        #endregion

        #region Public OnApplyTemplate
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mainGrid = GetTemplateChild("mainGrid") as Grid;

            grid1 = GetTemplateChild("grid1") as Grid;
            if (grid1 != null)
                grid1.Margin = new Thickness(0);

            grid2 = GetTemplateChild("grid2") as Grid;
            if (grid2 != null)
                grid2.Margin = new Thickness(this.ActualWidth, 0, (-1 * this.ActualWidth), 0);

            frame1 = GetTemplateChild("frame1") as Frame;
            frame2 = GetTemplateChild("frame2") as Frame;

            animation1 = mainGrid.Resources["grid1Animation"] as BeginStoryboard;
            animation2 = mainGrid.Resources["grid2Animation"] as BeginStoryboard;
            animation3 = mainGrid.Resources["grid3Animation"] as BeginStoryboard;
            animation4 = mainGrid.Resources["grid4Animation"] as BeginStoryboard;

            if (animation1 != null)
                if (animation1.Storyboard != null)
                    animation1.Storyboard.Completed += NewGridMargin_Completed;
            if (animation2 != null)
                if (animation2.Storyboard != null)
                    animation2.Storyboard.Completed += NewGridMargin_Completed;
            if (animation3 != null)
                if (animation3.Storyboard != null)
                    animation3.Storyboard.Completed += OldGridMargin_Completed;
            if (animation4 != null)
                if (animation4.Storyboard != null)
                    animation4.Storyboard.Completed += OldGridMargin_Completed;

            if (mainGrid != null)
            {
                mainGrid.SizeChanged += (sender, e) =>
                {
                    Application.Current.Resources["Dynamic.ThicknessAnimation.Margin"] =
                    new Thickness(this.ActualWidth, 0, -1 * this.ActualWidth, 0);
                };
            }
            IsDoneInitialization = true;
            FirstPage = new DefaultPage();
        }

        #endregion

        #region Private Events
        private void Page_Navigate(IFloppyPage page, FloppyPageEventArgs e)
        {
            Start_Navigate(page);
        }

        private void Page_GoBack(FloppyPageEventArgs e)
        {
            Start_GoBack();
        }

        private void NewGridMargin_Completed(object sender, EventArgs e)
        {
            Set_NewMargin();
        }

        private void OldGridMargin_Completed(object sender, EventArgs e)
        {
            Set_OldMargin();
        }

        #endregion

        #region Private Navigate
        private void Start_Navigate(IFloppyPage page)
        {
            if (page != null && IsDoneAnimation)
            {
                IsDoneAnimation = false;
                GridNumber = !GridNumber;
                page.Navigate += Page_Navigate;
                page.GoBack += Page_GoBack;

                if (!GridNumber)
                {
                    animation1.Storyboard.Stop();
                    frame2.Navigate(page);
                    Panel.SetZIndex(grid1, 0);
                    Panel.SetZIndex(grid2, 1);
                    grid2.Visibility = Visibility.Visible;
                    animation2.Storyboard.Begin();
                }
                else
                {
                    animation2.Storyboard.Stop();
                    frame1.Navigate(page);
                    Panel.SetZIndex(grid2, 0);
                    Panel.SetZIndex(grid1, 1);
                    grid1.Visibility = Visibility.Visible;
                    animation1.Storyboard.Begin();
                }
                journal.Add(page);

                OnPropertyChanged("JournalCount");
                OnPropertyChanged("CurrentPage");
                OnPropertyChanged("CanGoBack");

                base.RaiseEvent(new RoutedEventArgs(FloppyPages.NavigatedRoutedEvent, this));
            }
        }
        private void Set_NewMargin()
        {
            if (!GridNumber)
            {
                grid2.Margin = new Thickness(0);
                grid1.Margin = new Thickness(this.ActualWidth, 0, (-1 * this.ActualWidth), 0);
                grid1.Visibility = Visibility.Hidden;
            }
            else
            {
                grid1.Margin = new Thickness(0);
                grid2.Margin = new Thickness(this.ActualWidth, 0, (-1 * this.ActualWidth), 0);
                grid2.Visibility = Visibility.Hidden;
            }
            IsDoneAnimation = true;
        }

        #endregion

        #region Private GoBack
        private bool Start_GoBack()
        {
            if (journal.Count > 1 && IsDoneAnimation)
            {
                IsDoneAnimation = false;
                GridNumber = !GridNumber;
                journal[journal.Count - 1].Navigate -= Page_Navigate;
                journal[journal.Count - 1].GoBack -= Page_GoBack;

                grid1.Visibility = Visibility.Visible;
                grid2.Visibility = Visibility.Visible;

                if (!GridNumber)
                {
                    animation4.Storyboard.Stop();
                    grid2.Margin = new Thickness(0);
                    frame2.Navigate(journal[journal.Count - 2]);
                    animation3.Storyboard.Begin();
                }
                else
                {
                    animation3.Storyboard.Stop();
                    grid1.Margin = new Thickness(0);
                    frame1.Navigate(journal[journal.Count - 2]);
                    animation4.Storyboard.Begin();
                }
                journal.Remove(journal[journal.Count - 1]);

                OnPropertyChanged("JournalCount");
                OnPropertyChanged("CurrentPage");
                OnPropertyChanged("CanGoBack");

                base.RaiseEvent(new RoutedEventArgs(FloppyPages.WentBackRoutedEvent, this));

                return true;
            }
            else
                return false;
        }
        private void Set_OldMargin()
        {
            if (!GridNumber)
            {
                Panel.SetZIndex(grid1, 0);
                Panel.SetZIndex(grid2, 1);
                grid1.Margin = new Thickness(this.ActualWidth, 0, (-1 * this.ActualWidth), 0);
                grid1.Visibility = Visibility.Hidden;
            }
            else
            {
                Panel.SetZIndex(grid1, 1);
                Panel.SetZIndex(grid2, 0);
                grid2.Margin = new Thickness(this.ActualWidth, 0, (-1 * this.ActualWidth), 0);
                grid2.Visibility = Visibility.Hidden;
            }
            IsDoneAnimation = true;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
    #endregion
}
