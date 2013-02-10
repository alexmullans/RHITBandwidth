// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   A basic page that provides characteristics common to most applications.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace W8RHITBandwidth
{
    using System;
    using System.Collections.Generic;

    using RoseHulmanBandwidthMonitorApp;

    using W8RHITBandwidth.Common;

    using Windows.Foundation.Collections;
    using Windows.Storage;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;

    /// <summary>
    ///     A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : LayoutAwarePage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPageLoaded;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The update ui.
        /// </summary>
        /// <param name="bandwidthResults">
        /// The bandwidth results.
        /// </param>
        /// <param name="fromNetwork">
        /// The from network.
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
        /// The report credentials error.
        /// </summary>
        internal void ReportCredentialsError()
        {
            new MessageDialog(
                "The credentials you entered don't seem to be working, or we can't find the bandwidth tool right now.")
                .ShowAsync();
        }

        /// <summary>
        /// The get bandwidth number from string.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private static double GetBandwidthNumberFromString(string str)
        {
            return double.Parse(str.Split(' ')[0]);
        }

        /// <summary>
        /// The main page loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            IPropertySet settings = ApplicationData.Current.RoamingSettings.Values;
            PolicyDown.LowThresholdMb = (int)settings["LowThreshold"];
            PolicyDown.MidThresholdMb = (int)settings["MidThreshold"];
            PolicyUp.LowThresholdMb = (int)settings["LowThreshold"];
            PolicyUp.MidThresholdMb = (int)settings["MidThreshold"];

            double lowThresholdMultipliedByDiscount = (int)settings["LowThreshold"]
                                                      * Math.Pow(1 - (((int)settings["PctDiscount"]) / 100.0), -1);
            double midThresholdMultipliedByDiscount = (int)settings["MidThreshold"]
                                                      * Math.Pow(1 - (((int)settings["PctDiscount"]) / 100.0), -1);

            ActualDown.LowThresholdMb = (int)lowThresholdMultipliedByDiscount;
            ActualDown.MidThresholdMb = (int)midThresholdMultipliedByDiscount;
            ActualUp.LowThresholdMb = (int)lowThresholdMultipliedByDiscount;
            ActualUp.MidThresholdMb = (int)midThresholdMultipliedByDiscount;

            if (settings.ContainsKey("BandwidthClass"))
            {
                UpdateUi(BandwidthResults.RetrieveFromIsolatedStorage(), false);
            }

            BandwidthResults results = await Scraper.Scrape();
            UpdateUi(results, true);
        }

        #endregion
    }
}