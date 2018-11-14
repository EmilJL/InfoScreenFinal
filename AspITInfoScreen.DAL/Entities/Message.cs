using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspITInfoScreen.DAL
{
    public class Message
    {
        public int Id {get; set;}
        public int AdminId { get; set; }
        private DateTime date;
        private string text;
        private string header;

        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
            }
        }
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
            }
        }
        public string Header
        {
            get { return header; }
            set
            {
                header = value;

            }
        }
    }
}
