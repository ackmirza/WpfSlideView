using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfSlidingPage.Controls
{
    public partial class SlidingPage : UserControl, ISlidingPage, INotifyPropertyChanged
    {
        public SlidingPage()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private List<SlidingItem> items;
        public List<SlidingItem> Items
        {
            get { return items; }
            set
            {
                items = value;
                NotifyPropertyChanged("Items");
            }
        }

        public int Index { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RefreshPage()
        {
            //var itms = Items;
            //Items.Clear();
            //ItemsListBox.Refresh();
        }
    }
}
