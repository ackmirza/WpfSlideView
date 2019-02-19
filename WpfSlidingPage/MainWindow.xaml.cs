using System.Collections.ObjectModel;
using System.Windows;
using WpfSlidingPage.Controls;
using System.Linq;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfSlidingPage
{
    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate () { };


        public static void Refresh(this UIElement uiElement)

        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<SlidingItem> slidingItems;
        public ObservableCollection<SlidingItem> SlidingItems
        {
            get { return slidingItems; }
            set
            {
                slidingItems = value;
                NotifyPropertyChanged("SlidingItems");
            }
        }

        //ObservableCollection<SlidingItem> slidingItems = new ObservableCollection<SlidingItem>();
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            SlidingItems = new ObservableCollection<SlidingItem>();
            SlidingItems.CollectionChanged += SlidingItems_CollectionChanged;
        }

        private void SlidingItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("ItemSource");
        }

        private void LoadItems()
        {
            //var sItems = new ObservableCollection<SlidingItem>();
            for (int i = 0; i < 49; i++) SlidingItems.Add(new SlidingItem
            {
                Index = i,
                Shipment = "300245" + i,
                Count = i + 1
            });
            //SlidingItems = sItems;
            slideView.ItemSource = SlidingItems;
        }

        private void ListBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //txtCount.Text = listBox.ItemsPerPage.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBackgroundPrerequisistes();
            LoadItems();

        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            SlidingItems = (ObservableCollection<SlidingItem>)slideView.ItemSource;
            int index = 0;
            if (SlidingItems != null && SlidingItems.Count > 0) index = SlidingItems.Last().Index + 1;
            SlidingItems.Add(new SlidingItem
            {
                Index = index,
                Shipment = "300245" + index,
                Count = index + 1
            });
            slideView.ItemSource = SlidingItems;
        }

        private void RemoveButtonClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {

                int index = int.Parse(IndexTextBox.Text);
                if (SlidingItems != null && index > 0 && index <= SlidingItems.Count)
                {
                    SlidingItems = (ObservableCollection<SlidingItem>)slideView.ItemSource;
                    SlidingItems.RemoveAt(index - 1);
                    slideView.ItemSource = SlidingItems;

                    //slideView.Refresh();
                }

            });
        }

        ImageBrush _brush;
        private void LoadBackgroundPrerequisistes()
        {
            _brush = (ImageBrush)Resources["ImageBackground"];
        }

        private void SetBackgroundButtonClick(object sender, RoutedEventArgs e)
        {
            string item = ((System.Windows.Controls.ComboBoxItem)BackgroundCombo.SelectedItem).Content.ToString();
            _brush.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/WpfSlidingPage;component/Images/{ item }.png"));
        }
    }
}
