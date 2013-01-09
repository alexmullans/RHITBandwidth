using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace RoseHulmanBandwidthMonitorApp
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            var settings = IsolatedStorageSettings.ApplicationSettings;

            if (settings.Contains("user")) UsernameTextBox.Text = (string)settings["user"];
            if (settings.Contains("pass")) PasswordBox.Password = (string)settings["pass"];

            MidThresholdTextBox.Text = ((int)settings["MidThreshold"]).ToString();
            LowThresholdTextBox.Text = ((int)settings["LowThreshold"]).ToString();
            MidRateTextBox.Text= ((int)settings["MidRate"]).ToString();
            LowRateTextBox.Text= ((int)settings["LowRate"]).ToString();
            HighestPercentDiscountTextBox.Text = ((int)settings["PctDiscount"]).ToString();
        }

        private void SaveClick(object sender, EventArgs e)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("user"))
                settings["user"] = UsernameTextBox.Text;
            else
                settings.Add("user", UsernameTextBox.Text);
            if (settings.Contains("pass"))
                settings["pass"] = PasswordBox.Password;
            else settings.Add("pass", PasswordBox.Password);

            settings["MidThreshold"] = int.Parse(MidThresholdTextBox.Text);
            settings["LowThreshold"] = int.Parse(LowThresholdTextBox.Text);
            settings["MidRate"] = int.Parse(MidRateTextBox.Text);
            settings["LowRate"] = int.Parse(LowRateTextBox.Text);
            settings["PctDiscount"] = int.Parse(HighestPercentDiscountTextBox.Text);
            
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}