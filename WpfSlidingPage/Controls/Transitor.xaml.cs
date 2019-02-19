using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WpfSlidingPage.Controls
{
    public partial class Transitor : UserControl
    {
        ContentControl currentPresenter;
        ContentControl newPresenter;
        Storyboard slideLeft;
        Storyboard slideRight;
        Storyboard refresh;

        public bool IsTransitionOn
        {
            get { return (bool)GetValue(IsTransitionOnProperty); }
            set { SetValue(IsTransitionOnProperty, value); }
        }

        public static readonly DependencyProperty IsTransitionOnProperty =
            DependencyProperty.Register("IsTransitionOn", typeof(bool), typeof(Transitor), new PropertyMetadata(false));

        public Transitor()
        {
            InitializeComponent();
            slideLeft = Resources["SlideLeft"] as Storyboard;
            slideRight = Resources["SlideRight"] as Storyboard;
            refresh = Resources["Refresh"] as Storyboard;
            currentPresenter = Presenter2;
        }

        public void SlideLeft(object newPage)
        {
            if (currentPresenter.Content == null)
            {
                //On start of application, currentPresenter content will be null, so no need to slide.
                currentPresenter.Content = newPage;
                return;
            }

            if (currentPresenter == Presenter1)
            {
                newPresenter = Presenter2;
            }
            else
            {
                newPresenter = Presenter1;
            }

            Storyboard.SetTarget(slideLeft.Children[0], currentPresenter);
            Storyboard.SetTarget(slideLeft.Children[1], newPresenter);
            newPresenter.Visibility = System.Windows.Visibility.Visible;
            newPresenter.Content = newPage;


            slideLeft.Begin();
        }

        public void SlideRight(object newPage)
        {
            if (currentPresenter.Content == null)
            {
                //On start of application, currentPresenter content will be null, so no need to slide.
                currentPresenter.Content = newPage;
                return;
            }

            if (currentPresenter == Presenter1)
            {
                newPresenter = Presenter2;
            }
            else
            {
                newPresenter = Presenter1;
            }

            Storyboard.SetTarget(slideRight.Children[1], currentPresenter);
            Storyboard.SetTarget(slideRight.Children[0], newPresenter);
            newPresenter.Visibility = System.Windows.Visibility.Visible;
            newPresenter.Content = newPage;


            slideRight.Begin();
        }

        public void RefreshPage(object page)
        {
            Storyboard.SetTarget(refresh.Children[0], currentPresenter);
            currentPresenter.Content = page;
            refresh.Begin();
        }

        private void Refresh_Completed(object sender, EventArgs e)
        {
            //isRefresh++;
            //RefreshPage(currentPresenter.Content);
        }

        private void SlideLeft_Completed(object sender, EventArgs e)
        {
            if (currentPresenter == Presenter2)
                Panel.SetZIndex(Presenter1, 1);
            else
                Panel.SetZIndex(Presenter1, 0);


            (currentPresenter.RenderTransform as TranslateTransform).X = 0;
            currentPresenter.Visibility = Visibility.Collapsed;

            //Change the current presenter to new page which can be used in next transit operation.
            currentPresenter = newPresenter;

            IsTransitionOn = false;
        }

        private void SlideRight_Completed(object sender, EventArgs e)
        {
            if (currentPresenter == Presenter2)
                Panel.SetZIndex(Presenter1, 0);
            else
                Panel.SetZIndex(Presenter1, 1);


            (currentPresenter.RenderTransform as TranslateTransform).X = 0;
            currentPresenter.Visibility = System.Windows.Visibility.Collapsed;

            //Change the current presenter to new page which can be used in next transit operation.
            currentPresenter = newPresenter;

            IsTransitionOn = false;
        }


    }
}
