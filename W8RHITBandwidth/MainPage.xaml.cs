// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="">
// </copyright>
// <summary>
//   A basic page that provides characteristics common to most applications.
//   Code from the ApplicationSettings MSDN example on dev.windows.com is used in this page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace W8RHITBandwidth
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RoseHulmanBandwidthMonitorApp;

    using W8RHITBandwidth.Common;

    using Windows.Foundation;
    using Windows.Foundation.Collections;
    using Windows.Storage;
    using Windows.UI.ApplicationSettings;
    using Windows.UI.Core;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : LayoutAwarePage
    {
        #region Fields

        private bool isEventRegistered;

        private Popup settingsPopup;

        private double settingsWidth = 346;

        private Rect windowBounds;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPageLoaded;

            windowBounds = Window.Current.Bounds;

            // Added to listen for events when the window size is updated.
            Window.Current.SizeChanged += OnWindowSizeChanged;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Updates each <see cref="BandwidthMeter" /> with the appropriate result text.
        /// </summary>
        /// <param name="bandwidthResults">
        ///     The bandwidth results.
        /// </param>
        /// <param name="fromNetwork">
        ///     Whether the update is from storage or the network scraper.
        /// </param>
        public void UpdateUi(BandwidthResults bandwidthResults, bool fromNetwork)
        {
            foreach (var control in
                new Dictionary<BandwidthMeter, string>
                    {
                        { PolicyDown, bandwidthResults.PolicyReceived }, 
                        { PolicyUp, bandwidthResults.PolicySent }, 
                        { ActualDown, bandwidthResults.ActualReceived }, 
                        { ActualUp, bandwidthResults.ActualSent }
                    })
            {
                control.Key.UpdateBorder(GetBandwidthNumberFromString(control.Value), PolicyDown.ActualHeight);
                control.Key.UpdateText(control.Value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The report credentials error.
        /// </summary>
        internal void ReportCredentialsError()
        {
            new MessageDialog(
                "The credentials you entered don't seem to be working, or we can't find the bandwidth tool right now.")
                .ShowAsync();
        }

        /// <summary>
        ///     Invoked when the navigation is about to change to a different page. You can use this function for cleanup.
        /// </summary>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // Added to make sure the event handler for CommandsRequested is cleaned up before other scenarios.
            if (isEventRegistered)
            {
                SettingsPane.GetForCurrentView().CommandsRequested -= OnCommandsRequested;
                isEventRegistered = false;
            }

            // Unregister the event that listens for events when the window size is updated.
            Window.Current.SizeChanged -= OnWindowSizeChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!isEventRegistered)
            {
                // Listening for this event lets the app initialize the settings commands and pause its UI until the user closes the pane.
                // To ensure your settings are available at all times in your app, place your CommandsRequested handler in the overridden
                // OnWindowCreated of App.xaml.cs
                SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
                isEventRegistered = true;
            }
        }

        /// <summary>
        ///     The get bandwidth number from string.
        /// </summary>
        /// <param name="str">
        ///     The str.
        /// </param>
        /// <returns>
        ///     The <see cref="double" />.
        /// </returns>
        private static double GetBandwidthNumberFromString(string str)
        {
            return double.Parse(str.Split(' ')[0]);
        }

        /// <summary>
        ///     The main page loaded.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private async void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            var settings = ApplicationData.Current.RoamingSettings.Values;
            PolicyDown.LowThresholdMb = (int)settings["LowThreshold"];
            PolicyDown.MidThresholdMb = (int)settings["MidThreshold"];
            PolicyUp.LowThresholdMb = (int)settings["LowThreshold"];
            PolicyUp.MidThresholdMb = (int)settings["MidThreshold"];

            var lowThresholdMultipliedByDiscount = (int)settings["LowThreshold"]
                                                      * Math.Pow(1 - (((int)settings["PctDiscount"]) / 100.0), -1);
            var midThresholdMultipliedByDiscount = (int)settings["MidThreshold"]
                                                      * Math.Pow(1 - (((int)settings["PctDiscount"]) / 100.0), -1);

            ActualDown.LowThresholdMb = (int)lowThresholdMultipliedByDiscount;
            ActualDown.MidThresholdMb = (int)midThresholdMultipliedByDiscount;
            ActualUp.LowThresholdMb = (int)lowThresholdMultipliedByDiscount;
            ActualUp.MidThresholdMb = (int)midThresholdMultipliedByDiscount;

            if (settings.ContainsKey("BandwidthClass"))
            {
                UpdateUi(BandwidthResults.RetrieveFromIsolatedStorage(), false);
            }

            if (!settings.ContainsKey("user"))
            {
                OnSettingsCommand(null);
            }
            else
            {
                await Scrape();
            }
        }

        private async Task Scrape()
        {
            var results = await Scraper.Scrape();
            UpdateUi(results, true);
        }

        /// <summary>
        ///     When the Popup closes we no longer need to monitor activation changes.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private async void OnPopupClosed(object sender, object e)
        {
            Window.Current.Activated -= OnWindowActivated;
            await Scrape();
        }

        /// <summary>
        ///     We use the window's activated event to force closing the Popup since a user maybe interacted with
        ///     something that didn't normally trigger an obvious dismiss.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void OnWindowActivated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
            {
                settingsPopup.IsOpen = false;
            }
        }

        /// <summary>
        ///     Invoked when the window size is updated.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            windowBounds = Window.Current.Bounds;
        }

        /// <summary>
        ///     This event is generated when the user opens the settings pane. During this event, append your
        ///     SettingsCommand objects to the available ApplicationCommands vector to make them available to the
        ///     SettingsPange UI.
        /// </summary>
        /// <param name="settingsPane">Instance that triggered the event.</param>
        /// <param name="eventArgs">Event data describing the conditions that led to the event.</param>
        private void OnCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs eventArgs)
        {
            UICommandInvokedHandler handler = OnSettingsCommand;

            var generalCommand = new SettingsCommand("SettingsId", "Bandwidth Settings", handler);
            eventArgs.Request.ApplicationCommands.Add(generalCommand);
        }

        // This is the container that will hold our custom content.

        /// <summary>
        ///     This the event handler for the "Defaults" button added to the settings charm. This method
        ///     is responsible for creating the Popup window will use as the container for our settings Flyout.
        ///     The reason we use a Popup is that it gives us the "light dismiss" behavior that when a user clicks away
        ///     from our custom UI it just dismisses.  This is a principle in the Settings experience and you see the
        ///     same behavior in other experiences like AppBar.
        /// </summary>
        /// <param name="command"></param>
        private void OnSettingsCommand(IUICommand command)
        {
            // Create a Popup window which will contain our flyout.
            settingsPopup = new Popup();
            settingsPopup.Closed += OnPopupClosed;
            Window.Current.Activated += OnWindowActivated;
            settingsPopup.IsLightDismissEnabled = true;
            settingsPopup.Width = settingsWidth;
            settingsPopup.Height = windowBounds.Height;

            // Add the proper animation for the panel.
            settingsPopup.ChildTransitions = new TransitionCollection();
            settingsPopup.ChildTransitions.Add(
                new PaneThemeTransition
                    {
                        Edge =
                            (SettingsPane.Edge == SettingsEdgeLocation.Right)
                                ? EdgeTransitionLocation.Right
                                : EdgeTransitionLocation.Left
                    });

            // Create a SettingsFlyout the same dimenssions as the Popup.
            var mypane = new SettingsFlyout();
            mypane.Width = settingsWidth;
            mypane.Height = windowBounds.Height;

            // Place the SettingsFlyout inside our Popup window.
            settingsPopup.Child = mypane;

            // Let's define the location of our Popup.
            settingsPopup.SetValue(
                Canvas.LeftProperty,
                SettingsPane.Edge == SettingsEdgeLocation.Right ? (windowBounds.Width - settingsWidth) : 0);
            settingsPopup.SetValue(Canvas.TopProperty, 0);
            settingsPopup.IsOpen = true;
        }

        #endregion
    }
}