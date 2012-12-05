using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using HtmlAgilityPack;

namespace RoseHulmanBandwidthMonitorApp
{
    public struct BandwidthResults
    {
        public String BandwidthClass { get; internal set; }
        public String PolicyReceived { get; internal set; }
        public String PolicySent { get; internal set; }
        public String ActualReceived { get; internal set; }
        public String ActualSent { get; internal set; }
        public void SaveToIsolatedStorage()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings["BandwidthClass"] = BandwidthClass;
            settings["PolicyRecieved"] = PolicyReceived;
            settings["PolicySent"] = PolicySent;
            settings["ActualReceived"] = ActualReceived;
            settings["ActualSent"] = ActualSent;
        }
        public static BandwidthResults RetrieveFromIsolatedStorage()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            var toReturn = new BandwidthResults
                               {
                                   BandwidthClass = (String) settings["BandwidthClass"],
                                   PolicyReceived = (String) settings["PolicyRecieved"],
                                   PolicySent = (String) settings["PolicySent"],
                                   ActualReceived = (String) settings["ActualReceived"],
                                   ActualSent = (String) settings["ActualSent"]
                               };
            return toReturn;
        }
    }

    public class Scraper
    {
        private static MainPage _page;

        public static void Scrape(object page)
        {
            _page = (MainPage)page;

            var web = new HtmlWeb();
            web.LoadCompleted += ParseBandwidthDocument;
            var settings = IsolatedStorageSettings.ApplicationSettings;
            var siteToLoad = (String)settings["user"] == "testuser" ?
                                    "http://alexmullans.com/bandwidth.html" :
                                    "http://netreg.rose-hulman.edu/tools/networkUsage.pl";
            web.LoadAsync(siteToLoad,
                new UTF8Encoding(),
                (String)settings["user"],
                (String)settings["pass"],
                "rose-hulman");
        }

        private static void ParseBandwidthDocument(object sender, HtmlDocumentLoadCompleted e)
        {
            if (e.Error is WebException)
            {
                _page.ReportCredentialsError();
                return;
            }
            if (e.Error != null) return;
            var doc = e.Document;
            var summaryTable = from desc in doc.DocumentNode.Descendants()
                               where desc.Name == "td" &&
                                     desc.InnerText == "Bandwidth Class"
                               select desc.ParentNode.ParentNode;

            var resultsList = summaryTable.ElementAt(0).Elements("tr").ElementAt(1).Elements("td");
            var htmlNodes = resultsList as HtmlNode[] ?? resultsList.ToArray();
            var results = new BandwidthResults()
                              {
                                  BandwidthClass = htmlNodes.ElementAt(0).InnerText,
                                  PolicyReceived = htmlNodes.ElementAt(1).InnerText,
                                  PolicySent = htmlNodes.ElementAt(2).InnerText,
                                  ActualReceived = htmlNodes.ElementAt(3).InnerText,
                                  ActualSent = htmlNodes.ElementAt(4).InnerText
                              };
            Deployment.Current.Dispatcher.BeginInvoke(() => _page.UpdateUi(results, true));
            results.SaveToIsolatedStorage();
        }
    }
}