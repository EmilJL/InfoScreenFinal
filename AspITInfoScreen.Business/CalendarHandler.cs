using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace AspITInfoScreen.Business
{
    public static class CalendarHandler
    {
        private static readonly CultureInfo myCI = new CultureInfo("da-DK");
        private static readonly Calendar calendar = myCI.Calendar;
        private static readonly CalendarWeekRule CWR = myCI.DateTimeFormat.CalendarWeekRule;
        private static readonly DayOfWeek DOW = myCI.DateTimeFormat.FirstDayOfWeek;
        /// <summary>
        /// Return an int for the week number, based on the defined rules in the constructor
        /// </summary>
        /// <returns>int</returns>
        public static int GetWeekNumber()
        {
            return calendar.GetWeekOfYear(DateTime.Now, CWR, DOW);
        }
        /// <summary>
        /// A method to read that gives DateTime.Now with the specified format
        /// </summary>
        /// <param name="format">etc. Hour:Minutes:Seconds --> "hh:mm:ss"</param>
        /// <returns></returns>
        public static string GetStringDate(string format)
        {
            return DateTime.Now.ToString(format);
        }
        /// <summary>
        /// Returns a DateTime.Now 
        /// </summary>
        /// <returns></returns>
        public static DateTime GetDate()
        {
            return DateTime.Now;
        }
    }
}
