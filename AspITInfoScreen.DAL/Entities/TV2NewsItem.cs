using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspITInfoScreen.DAL.Entities
{
    public class TV2NewsItem
    {
        private string title;
        private DateTime pubDate;
        private string description;
        private string author;

        public TV2NewsItem()
        {

        }

        public string Author
        {
            get { return author; }
            set { author = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public DateTime PubDate
        {
            get { return pubDate; }
            set { pubDate = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

    }
}
