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
            set {
                if (DataValidation.WeekDay(value))
                    weekday = value;
                else
                    throw new ArgumentException($"weekday must be a day of the week; is {value}");
            }
        }

        public string Meal
        {
            get { return meal; }
            set {
                if (DataValidation.String(value))
                    meal = value;
                else
                {
                    throw new ArgumentNullException($"Meal description 'meal' cannot be null, whitespace or empty; is {value}");
                }

            }
        }
    }
}
