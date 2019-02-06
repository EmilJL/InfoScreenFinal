using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspITInfoScreen.DAL.Entities
{
    public class ViewAdminMessageJoin
    {
        private string username;
        private string header;
        private string text;
        private DateTime date;

        public ViewAdminMessageJoin()
        {
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public string Header
        {
            get { return header; }
            set { header = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

    }
}
