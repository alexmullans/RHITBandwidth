using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
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
                                                                  UpdateBorder(control.Key.UsageBorder,
                                                                               GetBandwidthNumberFromString(
                                                                                   control.Value));
                                                                  control.Key.UsageTextBlock.Text =
                                                                      control.Value;
                                                              }
                                                          });
        }

        private static double GetBandwidthNumberFromString(String str)
        {
            return Double.Parse(str.Split(' ')[0]);
        }

        private void UpdateBorder(Border border, double policyReceived)
        {
            border.Visibility = Visibility.Visible;
            border.Height = policyReceived / 5000 * PolicyUsageGrid.ActualHeight;
        }

        internal void ReportCredentialsError()
        {
            MessageBox.Show(
                "The credentials you entered don't seem to be working, or we can't find the bandwidth tool right now.");
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }
    }
}