using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace RoseHulmanBandwidthMonitorApp
{
    public partial class BandwidthMeter : UserControl
    {
        public BandwidthMeter()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty RedTextBlockProperty =
            DependencyProperty.Register("RedTextBlock", typeof(String), typeof(BandwidthMeter), new PropertyMetadata(default(String)));

        public String RedTextBlock
        {
            get { return (String)GetValue(RedTextBlockProperty); }
            set { SetValue(RedTextBlockProperty, value); }
        }

        public static readonly DependencyProperty YellowTextBlockProperty =
            DependencyProperty.Register("YellowTextBlock", typeof(String), typeof(BandwidthMeter), new PropertyMetadata(default(String)));

        public String YellowTextBlock
        {
            get { return (String)GetValue(YellowTextBlockProperty); }
            set { SetValue(YellowTextBlockProperty, value); }
        }

        public static readonly DependencyProperty GreenTextBlockProperty =
            DependencyProperty.Register("GreenTextBlock", typeof(String), typeof(BandwidthMeter), new PropertyMetadata(default(String)));

        public String GreenTextBlock
        {
            get { return (String)GetValue(GreenTextBlockProperty); }
            set { SetValue(GreenTextBlockProperty, value); }
        }

        public static readonly DependencyProperty CapacityMbProperty =
            DependencyProperty.Register("CapacityMb", typeof(Int32), typeof(BandwidthMeter), new PropertyMetadata(default(Int32)));

        public Int32 CapacityMb
        {
            get { return (Int32)GetValue(CapacityMbProperty); }
            set { SetValue(CapacityMbProperty, value); }
        }

        public void UpdateBorder(double value, double gridHeight)
        {
            var sb = (Storyboard)Resources["ShowUsageStoryboard"];
            var to = (value / CapacityMb) * gridHeight;
            ((DoubleAnimation)sb.Children[0]).To = to > 40 ? to : 40;
            sb.Begin();
        }
    }
}
