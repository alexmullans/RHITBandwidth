namespace RoseHulmanBandwidthMonitorApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using HtmlAgilityPack;

    using W8RHITBandwidth;

    using Windows.Foundation.Collections;
    using Windows.Storage;

    public struct BandwidthResults
    {
        #region Public Properties

        public string ActualReceived { get; internal set; }

        public string ActualSent { get; internal set; }

        public string BandwidthClass { get; internal set; }

        public string PolicyReceived { get; internal set; }

        public string PolicySent { get; internal set; }

        #endregion

        #region Public Methods and Operators

        public static BandwidthResults RetrieveFromIsolatedStorage()
        {
            IPropertySet settings = ApplicationData.Current.RoamingSettings.Values;
            var toReturn = new BandwidthResults
                               {
                                   BandwidthClass = (string)settings["BandwidthClass"],
                                   PolicyReceived = (string)settings["PolicyRecieved"],
                                   PolicySent = (string)settings["PolicySent"],
                                   ActualReceived = (string)settings["ActualReceived"],
                                   ActualSent = (string)settings["ActualSent"]
                               };
            return toReturn;
        }

        public void SaveToIsolatedStorage()
        {
            var settings = ApplicationData.Current.RoamingSettings.Values;
            settings["BandwidthClass"] = this.BandwidthClass;
            settings["PolicyRecieved"] = this.PolicyReceived;
            settings["PolicySent"] = this.PolicySent;
            settings["ActualReceived"] = this.ActualReceived;
            settings["ActualSent"] = this.ActualSent;
        }

        #endregion
    }

    public class Scraper
    {
        #region Static Fields

        private static MainPage page;

        #endregion

        #region Public Methods and Operators

        public static async void Scrape(object page)
        {
            Scraper.page = (MainPage)page;

            var web = new HtmlWeb();

            var settings = ApplicationData.Current.RoamingSettings.Values;
            var siteToLoad = (string)settings["user"] == "testuser"
                                    ? "http://alexmullans.com/bandwidth.html"
                                    : "http://netreg.rose-hulman.edu/tools/networkUsage.pl";
            var doc =
                await
                web.LoadFromWebAsync(
                    siteToLoad, new UTF8Encoding(), (string)settings["user"], (string)settings["pass"], "rose-hulman");
            ParseBandwidthDocument(doc);
        }

        #endregion

        #region Methods

        private static void ParseBandwidthDocument(HtmlDocument doc)
        {
            // if (e.Error is WebException)
            // {
            // page.ReportCredentialsError();
            // return;
            // }
            // if (e.Error != null) return;
            // var doc = e.Document;
            var summaryTable = from desc in doc.DocumentNode.Descendants()
                                                 where desc.Name == "td" && desc.InnerText == "Bandwidth Class"
                                                 select desc.ParentNode.ParentNode;

            var resultsList = summaryTable.ElementAt(0).Elements("tr").ElementAt(1).Elements("td");
            var htmlNodes = resultsList as HtmlNode[] ?? resultsList.ToArray();
            var results = new BandwidthResults
                              {
                                  BandwidthClass = htmlNodes.ElementAt(0).InnerText, 
                                  PolicyReceived = htmlNodes.ElementAt(1).InnerText, 
                                  PolicySent = htmlNodes.ElementAt(2).InnerText, 
                                  ActualReceived = htmlNodes.ElementAt(3).InnerText, 
                                  ActualSent = htmlNodes.ElementAt(4).InnerText
                              };
            page.UpdateUi(results, true);
            results.SaveToIsolatedStorage();
        }

        #endregion
    }
}