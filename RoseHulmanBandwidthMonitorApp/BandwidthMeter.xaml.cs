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
            Loaded += this.BandwidthMeterLoaded;
        }

        void BandwidthMeterLoaded(object sender, RoutedEventArgs e)
        {
            RedTextBlock.Text = LowThresholdMb + " MB";
            YellowTextBlock.Text = this.MidThresholdMb + " MB";
        }

        public static readonly DependencyProperty LowThresholdMbProperty =
            DependencyProperty.Register("LowThresholdMb", typeof(Int32), typeof(BandwidthMeter), new PropertyMetadata(default(Int32)));

        public Int32 LowThresholdMb
        {
            get { return (Int32)GetValue(LowThresholdMbProperty); }
            set { SetValue(LowThresholdMbProperty, value); }
        }

        public static readonly DependencyProperty MedThresholdMbProperty =
            DependencyProperty.Register("MidThresholdMb", typeof(Int32), typeof(BandwidthMeter), new PropertyMetadata(default(Int32)));

        public Int32 MidThresholdMb
        {
            get { return (Int32)GetValue(MedThresholdMbProperty); }
            set { SetValue(MedThresholdMbProperty, value); }
        }

        public void UpdateBorder(double value, double gridHeight)
        {
            var sb = (Storyboard)Resources["ShowUsageStoryboard"];
            var fractionOfMaxUsageShown = (value / (2 * this.LowThresholdMb - this.MidThresholdMb));
            var heightFromFraction = fractionOfMaxUsageShown *gridHeight;
            var to = heightFromFraction - 7.5; // compensate for border
            ((DoubleAnimation)sb.Children[0]).To = to > 40 ? to : 40;
            sb.Begin();
        }
    }
}
