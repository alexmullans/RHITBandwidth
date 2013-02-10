// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BandwidthMeter.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   The bandwidth meter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace W8RHITBandwidth
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// The bandwidth meter.
    /// </summary>
    public partial class BandwidthMeter : UserControl
    {
        #region Static Fields

        /// <summary>
        /// The low threshold mb property.
        /// </summary>
        public static readonly DependencyProperty LowThresholdMbProperty = DependencyProperty.Register(
            "LowThresholdMb", typeof(int), typeof(BandwidthMeter), new PropertyMetadata(default(int)));

        /// <summary>
        /// The med threshold mb property.
        /// </summary>
        public static readonly DependencyProperty MedThresholdMbProperty = DependencyProperty.Register(
            "MidThresholdMb", typeof(int), typeof(BandwidthMeter), new PropertyMetadata(default(int)));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BandwidthMeter"/> class.
        /// </summary>
        public BandwidthMeter()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the low threshold mb.
        /// </summary>
        public int LowThresholdMb
        {
            get
            {
                return (int)GetValue(LowThresholdMbProperty);
            }

            set
            {
                RedTextBlock.Text = value + " MB";
                SetValue(LowThresholdMbProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mid threshold mb.
        /// </summary>
        public int MidThresholdMb
        {
            get
            {
                return (int)GetValue(MedThresholdMbProperty);
            }

            set
            {
                YellowTextBlock.Text = value + " MB";
                SetValue(MedThresholdMbProperty, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The update border.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="gridHeight">
        /// The grid height.
        /// </param>
        public void UpdateBorder(double value, double gridHeight)
        {
            var sb = (Storyboard)Resources["ShowUsageStoryboard"];
            var fractionOfMaxUsageShown = value / ((2 * LowThresholdMb) - MidThresholdMb);
            var heightFromFraction = fractionOfMaxUsageShown * gridHeight;
            var to = heightFromFraction - 7.5; // compensate for border
            var an = (DoubleAnimation)sb.Children[0];
            an.To = to > 40 ? to : 40;
            sb.Begin();
        }

        /// <summary>
        /// The update text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        public void UpdateText(string text)
        {
            UsageTextBlock.Text = text;
        }

        #endregion
    }
}