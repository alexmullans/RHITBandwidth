namespace W8RHITBandwidth
{
    using System;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Animation;

    public partial class BandwidthMeter : UserControl
    {
        #region Static Fields

        public static readonly DependencyProperty LowThresholdMbProperty = DependencyProperty.Register(
            "LowThresholdMb", typeof(int), typeof(BandwidthMeter), new PropertyMetadata(default(int)));

        public static readonly DependencyProperty MedThresholdMbProperty = DependencyProperty.Register(
            "MidThresholdMb", typeof(int), typeof(BandwidthMeter), new PropertyMetadata(default(int)));

        #endregion

        #region Constructors and Destructors

        public BandwidthMeter()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Properties

        public int LowThresholdMb
        {
            get
            {
                return (int)this.GetValue(LowThresholdMbProperty);
            }

            set
            {
                RedTextBlock.Text = value + " MB";
                this.SetValue(LowThresholdMbProperty, value);
            }
        }

        public int MidThresholdMb
        {
            get
            {
                return (int)this.GetValue(MedThresholdMbProperty);
            }

            set
            {
                YellowTextBlock.Text = value + " MB";
                this.SetValue(MedThresholdMbProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void UpdateBorder(double value, double gridHeight)
        {
            var sb = (Storyboard)this.Resources["ShowUsageStoryboard"];
            var fractionOfMaxUsageShown = value / (2 * this.LowThresholdMb - this.MidThresholdMb);
            var heightFromFraction = fractionOfMaxUsageShown * gridHeight;
            var to = heightFromFraction - 7.5; // compensate for border
            var an = ((DoubleAnimation)sb.Children[0]);
            an.To = to > 40 ? to : 40;
            sb.Begin();
        }

        public void UpdateText(string text)
        {
            UsageTextBlock.Text = text;
        }

        #endregion
    }
}