using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
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
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Scraper.Scrape(this);
        }

        public void UpdateUi(BandwidthResults bandwidthResults)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                          {
                                                              foreach (
                                                                  var control in
                                                                      new Dictionary<BandwidthMeter, String>()
                                                                      {{PolicyDown, bandwidthResults.PolicyReceived}, 
                                                                      {PolicyUp, bandwidthResults.PolicySent},
                                                                      {ActualDown, bandwidthResults.ActualReceived},
                                                                      {ActualUp, bandwidthResults.ActualSent}})
                                                              {
                                                                  control.Key.UpdateBorder(GetBandwidthNumberFromString(
                                                                                   control.Value), PolicyUsageGrid.ActualHeight);
                                                                  control.Key.UsageTextBlock.Text =
                                                                      control.Value;
                                                              }
                                                          });


            var tileData = new FlipTileData()
            {
                BackContent = GetBandwidthStringForTile(bandwidthResults),
                Title = "Bandwidth Monitor",
                Count = Convert.ToInt32(GetBandwidthNumberFromString(bandwidthResults.PolicyReceived)/1000)
            };
            var primaryTile = ShellTile.ActiveTiles.First();
            primaryTile.Update(tileData);
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
            MessageBox.Show(
                "The credentials you entered don't seem to be working, or we can't find the bandwidth tool right now.");
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

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
    }
}