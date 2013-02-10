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
        }

        #endregion

        #region Methods

        /// <summary>
        /// This is the a common click handler for the buttons on the Flyout.  You would replace this with your own handler
        ///     if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void FlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as Button;
            if (b != null)
            {
                // rootPage.NotifyUser("You selected the " + b.Content + " button", NotifyType.StatusMessage);
            }
        }

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

        #endregion
    }
}