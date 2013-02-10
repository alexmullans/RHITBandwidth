// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scraper.cs" company="">
//   
// </copyright>
// <summary>
//   The bandwidth results.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RoseHulmanBandwidthMonitorApp
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using HtmlAgilityPack;

    using Windows.Foundation.Collections;
    using Windows.Storage;

    /// <summary>
    /// The bandwidth results.
    /// </summary>
    public struct BandwidthResults
    {
        #region Public Properties

        /// <summary>
        /// Gets the actual received.
        /// </summary>
        public string ActualReceived { get; internal set; }

        /// <summary>
        /// Gets the actual sent.
        /// </summary>
        public string ActualSent { get; internal set; }

        /// <summary>
        /// Gets the bandwidth class.
        /// </summary>
        public string BandwidthClass { get; internal set; }

        /// <summary>
        /// Gets the policy received.
        /// </summary>
        public string PolicyReceived { get; internal set; }

        /// <summary>
        /// Gets the policy sent.
        /// </summary>
        public string PolicySent { get; internal set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The retrieve from isolated storage.
        /// </summary>
        /// <returns>
        /// The <see cref="BandwidthResults"/>.
        /// </returns>
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

        /// <summary>
        /// The save to isolated storage.
        /// </summary>
        public void SaveToIsolatedStorage()
        {
            IPropertySet settings = ApplicationData.Current.RoamingSettings.Values;
            settings["BandwidthClass"] = BandwidthClass;
            settings["PolicyRecieved"] = PolicyReceived;
            settings["PolicySent"] = PolicySent;
            settings["ActualReceived"] = ActualReceived;
            settings["ActualSent"] = ActualSent;
        }

        #endregion
    }

    /// <summary>
    /// The scraper.
    /// </summary>
    public class Scraper
    {
        #region Public Methods and Operators

        /// <summary>
        /// The scrape.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static async Task<BandwidthResults> Scrape()
        {
            var web = new HtmlWeb();

            IPropertySet settings = ApplicationData.Current.RoamingSettings.Values;
            string siteToLoad = (string)settings["user"] == "testuser"
                                    ? "http://alexmullans.com/bandwidth.html"
                                    : "http://netreg.rose-hulman.edu/tools/networkUsage.pl";
            HtmlDocument doc =
                await
                web.LoadFromWebAsync(
                    siteToLoad, new UTF8Encoding(), (string)settings["user"], (string)settings["pass"], "rose-hulman");
            return ParseBandwidthDocument(doc);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The parse bandwidth document.
        /// </summary>
        /// <param name="doc">
        /// The doc.
        /// </param>
        /// <returns>
        /// The <see cref="BandwidthResults"/>.
        /// </returns>
        private static BandwidthResults ParseBandwidthDocument(HtmlDocument doc)
        {
            // if (e.Error is WebException)
            // {
            // page.ReportCredentialsError();
            // return;
            // }
            // if (e.Error != null) return;
            // var doc = e.Document;
            IEnumerable<HtmlNode> summaryTable = from desc in doc.DocumentNode.Descendants()
                                                 where desc.Name == "td" && desc.InnerText == "Bandwidth Class"
                                                 select desc.ParentNode.ParentNode;

            IEnumerable<HtmlNode> resultsList = summaryTable.ElementAt(0).Elements("tr").ElementAt(1).Elements("td");
            HtmlNode[] htmlNodes = resultsList as HtmlNode[] ?? resultsList.ToArray();
            var results = new BandwidthResults
                              {
                                  BandwidthClass = htmlNodes.ElementAt(0).InnerText, 
                                  PolicyReceived = htmlNodes.ElementAt(1).InnerText, 
                                  PolicySent = htmlNodes.ElementAt(2).InnerText, 
                                  ActualReceived = htmlNodes.ElementAt(3).InnerText, 
                                  ActualSent = htmlNodes.ElementAt(4).InnerText
                              };
            results.SaveToIsolatedStorage();
            return results;
        }

        #endregion
    }
}