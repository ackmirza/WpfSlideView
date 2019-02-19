using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSlidingPage.Controls
{
    public interface ISlidingPage
    {
        string Name { get; }
        int Index { get; }

        List<SlidingItem> Items { get; set; }

        void RefreshPage();
    }
}
