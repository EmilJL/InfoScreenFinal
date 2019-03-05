using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace AspITInfoScreen.DAL.Entities
{
    public static class DataValidation
    {
        public async static void SaveError(string errorMessage)
        {
            string newCaseStart = $"\r\nDate: {DateTime.Now.ToShortDateString()}\r\n ";
            const string caseEnd = "\r\n- - -";
            Windows.Storage.StorageFolder storageFolder =
                Windows.Storage.ApplicationData.Current.LocalFolder;

            Windows.Storage.StorageFile errorLog;

            
            //Check for existing file
                if (File.Exists(storageFolder.Path + "/InfoScreen-ErrorLog.txt")){
                //Select file
                errorLog = await storageFolder.GetFileAsync("InfoScreen-ErrorLog.txt");
                //Add to file
                await FileIO.AppendTextAsync(errorLog, $"{newCaseStart} {errorMessage} {caseEnd}");
            } else
            {
                //Create or replace existing file.
                errorLog = await storageFolder.CreateFileAsync("InfoScreen-ErrorLog.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(errorLog, $"" + newCaseStart + errorMessage + caseEnd);
            }
        }

        public async static void ErrorLogClean(DateTime now)
        {
            const string caseStart = "Date: ";
            const string caseEnd = "- - -";
            List<List<string>> errors = new List<List<string>>();
            string line;
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile errorLog;
            //Checks for file
            if (File.Exists(storageFolder.Path + "/InfoScreen-ErrorLog.txt")) {
                errorLog = await storageFolder.GetFileAsync("InfoScreen-ErrorLog.txt");
                StreamReader file = new StreamReader(errorLog.Path);
                //If new line - do
                while((line = file.ReadLine()) != null)
                {
                    //If new case read else leave
                    if (line.Contains(caseStart))
                    {
                        string sDate = line.Remove(0, 6);
                        DateTime date = Convert.ToDateTime(sDate);
                        //If not expired - do
                        if ((now - date).TotalDays < 31)
                        {
                            List<string> error = new List<string>();
                            //Until case end - do
                            do
                            {
                                error.Add(line);
                            } while (!(line = file.ReadLine()).Contains(caseEnd));
                            error.Add(line);
                            errors.Add(error);
                        }
                    }
                }

                file.Close();

                TextWriter writer = new StreamWriter(errorLog.Path);

                foreach (List<string> error in errors)
                {
                    foreach (string set in error)
                    {
                        await writer.WriteLineAsync(set);
                    }
                }

                writer.Close();
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
