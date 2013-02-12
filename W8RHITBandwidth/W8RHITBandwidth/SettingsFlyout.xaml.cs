// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="SettingsFlyout.xaml.cs">
//   
// </copyright>
// <summary>
//   An empty page that can be used on its own or navigated to within a Frame.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace ApplicationSettings
{
    using W8RHITBandwidth.Common;

    using Windows.Storage;
    using Windows.UI.ApplicationSettings;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Media.Animation;

    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsFlyout : LayoutAwarePage
    {
        // The guidelines recommend using 100px offset for the content animation.
        #region Constants

        /// <summary>
        /// The content animation offset.
        /// </summary>
        private const int ContentAnimationOffset = 100;

        #endregion

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        // MainPage rootPage = MainPage.Current;
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsFlyout"/> class.
        /// </summary>
        public SettingsFlyout()
        {
            InitializeComponent();
            FlyoutContent.Transitions = new TransitionCollection();
            FlyoutContent.Transitions.Add(
                new EntranceThemeTransition
                    {
                        FromHorizontalOffset =
                            (SettingsPane.Edge == SettingsEdgeLocation.Right)
                                ? ContentAnimationOffset
                                : (ContentAnimationOffset * -1)
                    });

            var settings = ApplicationData.Current.RoamingSettings.Values;

            if (settings.ContainsKey("user")) UsernameTextBox.Text = (string)settings["user"];
            if (settings.ContainsKey("pass")) PasswordBox.Password = (string)settings["pass"];

            midThreshold = (int)settings["MidThreshold"];
            lowThreshold = (int)settings["LowThreshold"];
            MidRateTextBox.Text = ((int)settings["MidRate"]).ToString();
            LowRateTextBox.Text = ((int)settings["LowRate"]).ToString();
            HighestPercentDiscountTextBox.Text = ((int)settings["PctDiscount"]).ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// This is the click handler for the back button on the Flyout.
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void MySettingsBackClicked(object sender, RoutedEventArgs e)
        {
            // First close our Flyout.
            var parent = Parent as Popup;
            if (parent != null)
            {
                parent.IsOpen = false;
            }

            // If the app is not snapped, then the back button shows the Settings pane again.
            if (ApplicationView.Value != ApplicationViewState.Snapped)
            {
                SettingsPane.Show();
            }
        }

        private int lowThreshold;
        public int LowThreshold
        {
            get
            {
                return lowThreshold;
            }
            set
            {
                lowThreshold = value;

            }
        }

        private int midThreshold;
        public int MidThreshold
        {
            get
            {
                return midThreshold;
            }
            set
            {
                midThreshold = value;

            }
        }

        #endregion
    }
}