using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace AspITInfoScreen.Business
{
    public class CalendarHandler
    {
        private Calendar calendar;
        private readonly CalendarWeekRule CWR;
        private readonly DayOfWeek DOW;

        public CalendarHandler()
        {
            CultureInfo myCI = new CultureInfo("da-DK");
            calendar = myCI.Calendar;
            CWR = myCI.DateTimeFormat.CalendarWeekRule;
            DOW = myCI.DateTimeFormat.FirstDayOfWeek;
        }
        /// <summary>
        /// Return an int for the week number, based on the defined rules in the constructor
        /// </summary>
        /// <returns>int</returns>
        public int GetWeekNumber()
        {
            return calendar.GetWeekOfYear(DateTime.Now, CWR, DOW);
        }
        /// <summary>
        /// A method to read that gives DateTime.Now with the specified format
        /// </summary>
        /// <param name="format">etc. Hour:Minutes:Seconds --> "hh:mm:ss"</param>
        /// <returns></returns>
        public string GetStringDate(string format)
        {
            return DateTime.Now.ToString(format);
        }

        public DateTime GetDate()
        {
            return DateTime.Now;
        }
    }
}
