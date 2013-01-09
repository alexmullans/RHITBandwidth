using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RoseHulmanBandwidthMonitorApp.Resources;

namespace RoseHulmanBandwidthMonitorApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        public ImageBrush PanoramaBackgroundImage
        {
            get
            {
                var lightThemeEnabled = (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"] ==
                                        Visibility.Visible;
                var url = lightThemeEnabled ? "/Assets/background_light.jpg" : "/Assets/background.jpg";
                return new ImageBrush() { ImageSource = new BitmapImage(new Uri(url, UriKind.Relative)) };
            }
        }

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPageLoaded;
        }

        private void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("BandwidthClass"))
                UpdateUi(BandwidthResults.RetrieveFromIsolatedStorage(), false);
            PolicyDown.LowThresholdMb = (int)settings["LowThreshold"];
            PolicyDown.MidThresholdMb = (int)settings["MidThreshold"];
            PolicyUp.LowThresholdMb = (int)settings["LowThreshold"];
            PolicyUp.MidThresholdMb = (int)settings["MidThreshold"];

            var lowThresholdMultipliedByDiscount = (int)settings["LowThreshold"] * Math.Pow(1 - ((int)settings["PctDiscount"]) / 100.0, -1);
            var midThresholdMultipliedByDiscount = (int)settings["MidThreshold"] * Math.Pow(1 - ((int)settings["PctDiscount"]) / 100.0, -1);

            ActualDown.LowThresholdMb = (int)lowThresholdMultipliedByDiscount;
            ActualDown.MidThresholdMb = (int)midThresholdMultipliedByDiscount;
            ActualUp.LowThresholdMb = (int)lowThresholdMultipliedByDiscount;
            ActualUp.MidThresholdMb = (int)midThresholdMultipliedByDiscount;

            var indicator = new ProgressIndicator();
            indicator.IsVisible = true;
            indicator.Text = "Updating usage...";
            indicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator = indicator;

            new Thread(Scraper.Scrape).Start(this);
        }

        private void StopProgressIndicator()
        {
            SystemTray.ProgressIndicator.IsVisible = false;
        }

        public void UpdateUi(BandwidthResults bandwidthResults, bool fromNetwork)
        {
            HideDemoModeIndicator();
            foreach (var control in
                       new Dictionary<BandwidthMeter, String> { 
            { PolicyDown, bandwidthResults.PolicyReceived },
            { PolicyUp, bandwidthResults.PolicySent }, 
            { ActualDown, bandwidthResults.ActualReceived },
            { ActualUp, bandwidthResults.ActualSent } })
            {
                control.Key.UpdateBorder(GetBandwidthNumberFromString(control.Value), PolicyDown.ActualHeight);
                control.Key.UsageTextBlock.Text =
                    control.Value;
            }

            var tileData = new FlipTileData()
                               {
                                   BackContent = GetBandwidthStringForTile(bandwidthResults),
                                   Title = "Bandwidth Monitor"
                               };
            var primaryTile = ShellTile.ActiveTiles.First();
            primaryTile.Update(tileData);
            if (fromNetwork)
                StopProgressIndicator();
            if ((string)IsolatedStorageSettings.ApplicationSettings["user"] == "testuser")
                ShowDemoModeIndicator();
        }

        private void ShowDemoModeIndicator()
        {
            DemoText.Visibility = Visibility.Visible;
        }

        private void HideDemoModeIndicator()
        {
            DemoText.Visibility = Visibility.Collapsed;
        }

        private static String GetBandwidthStringForTile(BandwidthResults results)
        {
            var received = Convert.ToInt32(GetBandwidthNumberFromString(results.PolicyReceived)) + " MB";
            var sent = Convert.ToInt32(GetBandwidthNumberFromString(results.PolicySent)) + " MB";
            return results.BandwidthClass + "\r\nD: " + received + "\r\nU: " + sent;
        }

        private static double GetBandwidthNumberFromString(String str)
        {
            return Double.Parse(str.Split(' ')[0]);
        }


        internal void ReportCredentialsError()
        {
            Dispatcher.BeginInvoke(() =>
                                       {
                                           MessageBox.Show(
                                               "The credentials you entered don't seem to be working, or we can't find the bandwidth tool right now.");
                                           NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
                                       });
        }

        private void SettingsButtonClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }
    }
}