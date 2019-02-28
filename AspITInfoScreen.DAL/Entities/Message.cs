using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspITInfoScreen.DAL.Entities
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
                if (DataValidation.Date(value))
                    date = value;
                else
                    throw new ArgumentOutOfRangeException($"date of the message cannnot be from the future; is {value.ToLongDateString()}. Compared to local system time");
            }
        }
        public string Text
        {
            get { return text; }
            set
            {
                if (DataValidation.String(value))
                    text = value;
                else
                    throw new ArgumentException("text cannot be null, whitespace or empty");
            }
        }
        public string Header
        {
            get { return header; }
            set
            {
                if (DataValidation.String(value))
                    header = value;
                else
                    throw new ArgumentException("header cannot be null, whitespace or empty.");
            }
        }
    }
}
