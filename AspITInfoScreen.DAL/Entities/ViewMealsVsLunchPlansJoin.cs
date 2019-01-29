using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspITInfoScreen.DAL.Entities
{
    public class ViewMealsVsLunchPlansJoin
    {
        private string weekday;
        private string meal;
        public ViewMealsVsLunchPlansJoin()
        {

        }

        public ViewMealsVsLunchPlansJoin(string weekday, string meal)
        {
            this.weekday = weekday;
            this.meal = meal;
        }

        public string Weekday
        {
            get { return weekday; }
            set { weekday = value; }
        }

        public string Meal
        {
            get { return meal; }
            set { meal = value; }
        }
    }
}
