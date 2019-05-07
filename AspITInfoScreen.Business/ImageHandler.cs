using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspITInfoScreen.Business
{
    public class ImageHandler
    {
        private const string weatherURL = "http://servlet.dmi.dk/byvejr/servlet/byvejr_dag1?by=2630&mode=short";
        private string garfieldURL = "https://d1ejxu6vysztl5.cloudfront.net/comics/garfield/";


        public ImageHandler()
        {

        }
        /// <summary>
        /// Retrieves a web address for the current weather
        /// </summary>
        /// <returns>Uri</returns>
        public Uri GetWeather()
        {
            return new Uri(weatherURL);
        }
        /// <summary>
        /// Retrives a garfield comic of the day
        /// </summary>
        /// <param name="deductDays">Used to find previous comic</param>
        /// <returns>Uri</returns>
        public Uri GetComic(int deductDays = 0)
        {
            return new Uri(garfieldURL + CalendarHandler.GetStringDate("yyyy") + "/" + CalendarHandler.GetDate().AddDays(-deductDays).ToString("yyyy-MM-dd") + ".gif");
        }
    }
}
