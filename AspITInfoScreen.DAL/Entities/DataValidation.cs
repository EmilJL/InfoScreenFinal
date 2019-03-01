using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspITInfoScreen.DAL.Entities
{
    public static class DataValidation
    {
        public static void SaveError(string errorMessage)
        {
            using (StreamReader streamReader = new StreamReader("../../InfoScreen-ErrorLog.txt"))
            {
                string log = streamReader.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(log))
                {
                    errorMessage = log + "\n\n" + errorMessage;
                }
            }

            using (StreamWriter streamWriter = File.CreateText("../../InfoScreen-ErrorLog.txt"))
            {
                streamWriter.Write(errorMessage);
            }
        }
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
            if (!string.IsNullOrWhiteSpace(str))
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
                if (day == "Monday" || day == "Tuesday" || day == "Wednesday" || day == "Thursday" || day == "Friday")
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
