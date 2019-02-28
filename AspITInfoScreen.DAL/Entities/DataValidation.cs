using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspITInfoScreen.DAL.Entities
{
    public static class DataValidation
    {
        public static bool Date(DateTime date)
        {
            if (date.CompareTo(DateTime.Now) < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool String(string str)
        {
            if (!string.IsNullOrEmpty(str) || !string.IsNullOrWhiteSpace(str))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool WeekDay(string day)
        {
            if (String(day))
            {
                if (day == "monday" || day == "tuesday" || day == "wednesday" || day == "thursday" || day == "friday")
                {
                    return true;
                }
            }
            return false;
        }
        public static bool WholeNumber(int number)
        {
            if(number > 0)
            {
                return true;
            } else
            {
                return false;
            }
        }
        public static bool NaturalNumber(int number)
        {
            if (number >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool WeekOfYear(int week)
        {
            if (week > 0 && week <= 52)
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
