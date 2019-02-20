using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;
using AspITInfoScreen.DAL.Entities;
using System.Xml;
using System.Net;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AspITInfoScreen.Business
{
    public class RSSFeedHandler
    {
        private SyndicationClient client;
        private SyndicationFeed feed;
        private Uri uri = null;
        private List<TV2NewsItem> newsList;

        public RSSFeedHandler(string url)
        {
            try
            {
                uri = new Uri(url);
                client = new SyndicationClient();
                newsList = new List<TV2NewsItem>();
                GetFeed();
            }
            catch (Exception err)
            {
                throw;
            }
        }

        public List<TV2NewsItem> NewsList
        {
            get { return newsList; }
        }

        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        public SyndicationFeed Feed
        {
            get { return feed; }
            set { feed = value; }
        }

        public SyndicationClient Client
        {
            get { return client; }
            set { client = value; }
        }

        public async void GetFeed()
        {
            try
            {
                XmlDocument rssXmlDoc = new XmlDocument();

                //Load the RSS file from the RSS URL
                rssXmlDoc.Load(uri.ToString());

                //Parse the Items in the RSS file
                XmlNodeList rssNodes = rssXmlDoc.SelectNodes("rss/channel/item");

                StringBuilder rssContent = new StringBuilder();

                //Iterates through the Items in the RSS file
                foreach (XmlNode rssNode in rssNodes)
                {
                    TV2NewsItem newsItem = new TV2NewsItem();

                    //Title
                    XmlNode rssSubNode = rssNode.SelectSingleNode("title");
                    newsItem.Title = rssSubNode != null ? rssSubNode.InnerText : "Ingen titel";

                    //Datetime
                    rssSubNode = rssNode.SelectSingleNode("pubDate");
                    DateTime articleDate = Convert.ToDateTime(rssSubNode.InnerText);
                    newsItem.PubDate = articleDate;

                    //Content
                    rssSubNode = rssNode.SelectSingleNode("description");
                    //Regex for html tag removal
                    Regex rgxHTML = new Regex("<[^>]*>"); //Additional patterns |\r?\n|\r
                    string content = rgxHTML.Replace(rssSubNode.InnerText, "");
                    //WebUtility to insert correct characters instead of &#160; or %nbsp; and the like
                    content = WebUtility.HtmlDecode(content);

                    newsItem.Description = content;

                    //Author
                    rssSubNode = rssNode.SelectSingleNode("author");
                    newsItem.Author = rssSubNode != null ? rssSubNode.InnerText : "Ingen forfatter";

                    newsList.Add(newsItem);
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message, err.GetType());
            }
        }
    }
}
