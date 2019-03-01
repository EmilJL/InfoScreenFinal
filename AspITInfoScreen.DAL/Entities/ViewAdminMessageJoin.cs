using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspITInfoScreen.DAL.Entities
{
    public class ViewAdminMessageJoin : Message
    {
        private string username;

        public ViewAdminMessageJoin() : base()
        {
        }
        /*
         * Unessesary with inheritance
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
        */
        public string Username
        {
            get { return username; }
            set {
                if (DataValidation.String(value))
                    username = value;
                else
                    throw new ArgumentNullException($"username cannot be null, whitespace or empty; is {value}");
            }
        }
    }
}
