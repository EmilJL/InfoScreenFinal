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
        private CalendarWeekRule CWR;
        private DayOfWeek DOW;

        public CalendarHandler()
        {
            CultureInfo myCI = new CultureInfo("da-DK");
            calendar = myCI.Calendar;
            CalendarWeekRule CWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek DOW = myCI.DateTimeFormat.FirstDayOfWeek;
        }

        public int GetWeekNumber()
        {
            return calendar.GetWeekOfYear(DateTime.Now, CWR, DOW);
        }

        public string GetStringDate(string format)
        {
            return DateTime.Now.ToString(format);
        }
    }
}
