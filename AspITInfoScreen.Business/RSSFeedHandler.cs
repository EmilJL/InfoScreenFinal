using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;
using AspITInfoScreen.DAL.Entities;

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
                Client.SetRequestHeader("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
                Feed = await Client.RetrieveFeedAsync(uri);

                string title = feed.Title.Text;

                foreach (SyndicationItem item in Feed.Items)
                {
                    TV2NewsItem newsItem = new TV2NewsItem();
                    newsItem.Title = item.Title == null ? "Ingen titel" : item.Title.Text;
                    newsItem.PubDate = item.PublishedDate.DateTime;
                    newsItem.Description = item.Content == null ? "Ingen indehold" : item.Content.Text.FirstOrDefault().ToString();
                    newsItem.Author = item.Authors == null ? "Ingen forfatter" : item.Authors.FirstOrDefault().Name.ToString();
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
