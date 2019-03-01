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
            set {
                if (DataValidation.String(value))
                    author = value;
                else
                    throw new ArgumentNullException($"author cannot be null, whitespace or empty; is {value}");
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                if (DataValidation.String(value))
                    description = value;
                else
                    throw new ArgumentNullException($"description cannot be null, whitespace or empty; is {value}");
            }
        }

        public DateTime PubDate
        {
            get { return pubDate; }
            set
            {
                if (DataValidation.Date(value))
                    pubDate = value;
                else
                    throw new ArgumentNullException($"pubDate cannot be from the future; is {value.ToLongDateString()}");
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                if (DataValidation.String(value))
                    title = value;
                else
                    throw new ArgumentNullException($"title cannot be null, whitespace or empty; is {value}");
            }
        }
    }
}
