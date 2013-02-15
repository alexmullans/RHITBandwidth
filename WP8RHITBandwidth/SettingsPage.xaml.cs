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
    using System.Collections;
    using System.ComponentModel;
    using System.Windows.Media;

    public partial class SettingsPage : PhoneApplicationPage, INotifyDataErrorInfo
    {
        Dictionary<string, string> errorList = new Dictionary<string, string>();

        private int lowThreshold;
        public int LowThreshold
        {
            get
            {
                return lowThreshold;
            }
            set
            {
                errorList.Clear();
                var val = Int32.Parse(value.ToString());
                if (val < midThreshold) errorList.Add("LowThreshold", "lowest must be greater than middle");
                lowThreshold = val;
                ChangeErrors();
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
                errorList.Clear();
                var val = Int32.Parse(value.ToString());
                if (val > lowThreshold) errorList.Add("MidThreshold", "middle must be less than lowest");
                midThreshold = val;
                ChangeErrors();
            }
        }

        private void ChangeErrors()
        {
            if (this.ErrorsChanged != null)
            {
                this.ErrorsChanged(this, new DataErrorsChangedEventArgs("LowThreshold"));
                this.ErrorsChanged(this, new DataErrorsChangedEventArgs("MidThreshold"));
            }
        }

        public SettingsPage()
        {
            InitializeComponent();
            var settings = IsolatedStorageSettings.ApplicationSettings;

            if (settings.Contains("user")) UsernameTextBox.Text = (string)settings["user"];
            if (settings.Contains("pass")) PasswordBox.Password = (string)settings["pass"];

            midThreshold = ((int)settings["MidThreshold"]);
            lowThreshold = ((int)settings["LowThreshold"]);
            MidRateTextBox.Text = ((int)settings["MidRate"]).ToString();
            LowRateTextBox.Text = ((int)settings["LowRate"]).ToString();
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
            NavigationService.RemoveBackEntry();
            NavigationService.RemoveBackEntry();
        }

        private void LimitsGridBindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            var source = e.OriginalSource as Control;
            var background = new SolidColorBrush(Colors.White);
            var error = string.Empty;
            if (e.Action == ValidationErrorEventAction.Added)
            {
                background = new SolidColorBrush(Colors.Red);
                error = e.Error.ErrorContent.ToString();
            }
            source.Background = background;
            if (source == LowThresholdTextBox) LowErrorTextBlock.Text = error;
            if (source == MidThresholdTextBox) MidErrorTextBlock.Text = error;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = !HasErrors;
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public IEnumerable GetErrors(string propertyName)
        {
            if (!errorList.ContainsKey(propertyName)) return string.Empty;
            var value = errorList[propertyName];
            return new[] { value };
        }

        public bool HasErrors
        {
            get
            {
                return errorList.Any();
            }
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var binding = textBox.GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();
        }
    }
}