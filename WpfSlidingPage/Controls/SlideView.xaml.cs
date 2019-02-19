using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfSlidingPage.Controls
{
    public partial class SlideView : UserControl, INotifyPropertyChanged
    {
        public List<ISlidingPage> slidingPages;
        private bool isTransitionOn;
        public event PropertyChangedEventHandler PropertyChanged;
        public int PageCount { get; set; }
        //public int PageIndex { get; set; }
        public ISlidingPage CurrentPage { get; set; }

        public int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageIndexProperty =
            DependencyProperty.Register("PageIndex", typeof(int), typeof(SlideView), new PropertyMetadata(0));

        public bool IsTransitionOn
        {
            get { return isTransitionOn; }
            set
            {
                isTransitionOn = value;
                NotifyPropertyChange("IsTransitionOn");
            }
        }

        public SlideView()
        {
            InitializeComponent();
            this.DataContext = this;
            slidingPages = new List<ISlidingPage>();
            //SetValue(ItemSourceProperty, new ObservableCollection<SlidingItem>());
        }

        public void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int ItemsPerPage
        {
            get { return (int)GetValue(ItemsPerPageProperty); }
            set { SetValue(ItemsPerPageProperty, value); }
        }

        public static readonly DependencyProperty ItemsPerPageProperty =
            DependencyProperty.Register("ItemsPerPage", typeof(int), typeof(SlideView), new PropertyMetadata());

        public IEnumerable<SlidingItem> ItemSource
        {
            get { return (IEnumerable<SlidingItem>)GetValue(ItemSourceProperty); }
            set
            {
                SetValue(ItemSourceProperty, value);
                NotifyPropertyChange("ItemSource");
            }
        }

        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(IEnumerable<SlidingItem>), typeof(SlideView), 
                new FrameworkPropertyMetadata(new PropertyChangedCallback(ItemSourceChanged), new CoerceValueCallback(ItemSourceValueChange)));// new PropertyMetadata(null, new PropertyChangedCallback(ItemSourceChanged)));

        private static object ItemSourceValueChange(DependencyObject d, object baseValue)
        {
            if(((SlideView)d).ItemSource != null) 
                ((SlideView)d).ItemSourceValueChange((IEnumerable<SlidingItem>)baseValue);
            return baseValue;
        }

        private void ItemSourceValueChange(IEnumerable<SlidingItem> items)
        {
            if (items != null)
            {
                decimal pageFraction = (decimal)items.Count() / ItemsPerPage;
                int pageCount = (int)Math.Ceiling(pageFraction);

                if (PageCount <= 0 && pageCount != PageCount)
                {
                    PageCount = pageCount;
                    for (int i = 1; i <= PageCount; i++)
                    {
                        var pageItems = items.Skip((i - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
                        slidingPages.Add(new SlidingPage
                        {
                            Index = i - 1,
                            Items = pageItems
                        });
                    }
                    CurrentPage = slidingPages[PageIndex];
                }
                else if(pageCount > PageCount)
                {
                    PageCount = pageCount;
                    slidingPages.Add(new SlidingPage
                    {
                        Index = PageCount - 1,
                        Items = items.Skip((PageCount - 1) * ItemsPerPage).Take(ItemsPerPage).ToList()
                    });
                    HandleButtonVisibility();
                }
                else
                {
                    PageCount = pageCount;
                    bool isRemoved = false;
                    foreach (var page in slidingPages)
                    {
                        var removed = page.Items.Except(items.Select(x => x)).FirstOrDefault();
                        if(removed != null)
                        {
                            isRemoved = true;
                            slidingPages.Clear();
                            
                            for (int i = 1; i <= PageCount; i++)
                            {
                                var pageItems = items.Skip((i - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
                                slidingPages.Add(new SlidingPage
                                {
                                    Index = i - 1,
                                    Items = pageItems
                                });
                            }
                            PageIndex = PageIndex >= PageCount ? PageIndex - 1 : PageIndex;
                            CurrentPage = slidingPages[PageIndex];
                            HandleButtonVisibility();
                            Transitor.RefreshPage(CurrentPage);
                            break;
                        }
                    }

                    if (!isRemoved)
                    {
                        var slidingPage = slidingPages[PageCount - 1];
                        slidingPage.Items = items.Skip((PageCount - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
                        CurrentPage = slidingPages[PageIndex];
                    }
                }
            }
        }

        private static void ItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var lb = (SlideView)d;
            var oldItem = (IEnumerable<SlidingItem>)e.OldValue;
            var newItem = (IEnumerable<SlidingItem>)e.NewValue;
            lb.ItemSourceChanged(oldItem, newItem);
        }

        private void ItemSourceChanged(IEnumerable<SlidingItem> oldItem, IEnumerable<SlidingItem> newItem)
        {
            //if (newItem != null && newItem.Count() > 0)
            //{
            //    decimal v = (decimal)newItem.Count() / ItemsPerPage;
            //    PageCount = (int)Math.Ceiling(v);

            //    if (slidingPages == null) slidingPages = new List<ISlidingPage>();
            //    else slidingPages.Clear();

            //    for (int i = 1; i <= PageCount; i++)
            //    {
            //        var items = ItemSource.Skip((i - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();
            //        slidingPages.Add(new SlidingPage
            //        {
            //            Index = i - 1,
            //            Items = items
            //        });
            //    }
            //    CurrentPage = slidingPages[0];
            //}
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //You can arrange your elements according to size.
            var width = e.NewSize.Width;
            var height = e.NewSize.Height;

            int hCount = (int)width / 300;
            int vCount = (int)height / 150;

            //Uncomment to calculate Items per page by size.
            //ItemsPerPage = hCount * vCount;

            //if (ItemSource != null)
            //{
            //    decimal pageFraction = (decimal)ItemSource.Count() / ItemsPerPage;
            //    PageCount = (int)Math.Ceiling(pageFraction);
            //}
            //ItemSourceValueChange(ItemSource);
            Debug.WriteLine($"Height: {height}\nWidth: {width}\nhCount: {hCount}\nvCount: {vCount}");
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsTransitionOn) return;
            IsTransitionOn = true;

            PageIndex += 1;
            HandleButtonVisibility();
            this.CurrentPage = this.slidingPages[PageIndex];
            ChangePage(p => Transitor.SlideLeft(p));
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            //Skip Prev Transition
            if (IsTransitionOn) return;
            IsTransitionOn = true;

            PageIndex -= 1;
            HandleButtonVisibility();
            this.CurrentPage = this.slidingPages[PageIndex];
            ChangePage(p => Transitor.SlideRight(p));
        }

        private void HandleButtonVisibility()
        {
            //Middle Page Selected from Multiple Pages 
            if (PageIndex > 0 && PageCount > 1)
            {
                PrevButton.IsEnabled = true;
                NextButton.IsEnabled = true;
            }
            //Single Page
            if (PageIndex == 0 && PageCount == 1)
            {
                PrevButton.IsEnabled = false;
                NextButton.IsEnabled = false;
            }
            //First Page Selected from Multiple Pages 
            if (PageIndex == 0 && PageCount > 1)
            {
                PrevButton.IsEnabled = false;
                NextButton.IsEnabled = true;
            }
            //Last Page Selected from Multiple Pages 
            if (PageCount > 1 && PageCount == (PageIndex + 1))
            {
                PrevButton.IsEnabled = true;
                NextButton.IsEnabled = false;
            }
            

            //if (PageCount == 1)
            //{
            //    PrevButton.IsEnabled = false;
            //    NextButton.IsEnabled = false;
            //}
            //if ((PageIndex + 1) == PageCount)
            //{
            //    NextButton.IsEnabled = false;
            //}
            //if (PageIndex == 0)
            //{
            //    PrevButton.IsEnabled = false;
            //}
            //if (PageIndex > 0 && (PageIndex + 1) < PageCount)
            //{
            //    PrevButton.IsEnabled = true;
            //    NextButton.IsEnabled = true;
            //}
            //if (PageIndex == 0 && PageCount > 1)
            //{
            //    NextButton.IsEnabled = true;
            //}
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var width = this.ActualWidth;
            var height = this.ActualHeight;

            int hCount = (int)width / 300;
            int vCount = (int)height / 150;

            ItemsPerPage = hCount * vCount;

            ItemSourceValueChange(ItemSource);

            HandleButtonVisibility();
            ChangePage(p => Transitor.SlideRight(p));
        }

        void ChangePage(Action<object> execute)
        {
            ISlidingPage p = this.CurrentPage;
            try
            {
                execute(slidingPages[p.Index]);
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
        }
    }

    public class SlidingItem : INotifyPropertyChanged
    {
        private int index;
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                NotifyPropertyChange("Index");
            }
        }

        private string shipment;
        public string Shipment
        {
            get { return shipment; }
            set { shipment = value;
                NotifyPropertyChange("Shipment");
            }
        }

        private int count;
        public int Count
        {
            get { return count; }
            set { count = value;
                NotifyPropertyChange("Count");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
