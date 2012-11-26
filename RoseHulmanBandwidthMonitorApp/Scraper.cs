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
    }

    public class Scraper
    {
        private static MainPage _page;
        private const int BUFFER_SIZE = 1024;

        public static void Scrape(MainPage page)
        {
            _page = page;

            var web = new HtmlWeb();
            web.LoadCompleted += ParseBandwidthDocument;
            var settings = IsolatedStorageSettings.ApplicationSettings;
            web.LoadAsync("http://netreg.rose-hulman.edu/tools/networkUsage.pl",
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

            _page.UpdateUi(new BandwidthResults()
                                  {
                                      BandwidthClass = resultsList.ElementAt(0).InnerText,
                                      PolicyReceived = resultsList.ElementAt(1).InnerText,
                                      PolicySent = resultsList.ElementAt(2).InnerText,
                                      ActualReceived = resultsList.ElementAt(3).InnerText,
                                      ActualSent = resultsList.ElementAt(4).InnerText
                                  });

        }
    }
}