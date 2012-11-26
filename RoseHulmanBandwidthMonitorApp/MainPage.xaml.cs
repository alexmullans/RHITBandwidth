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
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
        public void UpdateUi(BandwidthResults bandwidthResults)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
                                                          {
                                                              UpdateBorder(DownloadUsageBorder,
                                                                  GetBandwidthNumberFromString(bandwidthResults.PolicyReceived));
                                                              UpdateBorder(UploadUsageBorder,
                                                                  GetBandwidthNumberFromString(bandwidthResults.PolicySent));
                                                              DownloadUsageTextBlock.Text =
                                                                  bandwidthResults.PolicyReceived;
                                                              UploadUsageTextBlock.Text = bandwidthResults.PolicySent;
                                                          });
        }

        private static double GetBandwidthNumberFromString(String str)
        {
            return Double.Parse(str.Split(' ')[0]);
        }

        private void UpdateBorder(Border border, double policyReceived)
        {
            border.Visibility = Visibility.Visible;
            border.Height = policyReceived / 5000 * DownloadUsageGrid.ActualHeight;
        }

        internal void ReportCredentialsError()
        {
            MessageBox.Show(
                "The credentials you entered don't seem to be working, or we can't find the bandwidth tool right now.");
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }
    }
}