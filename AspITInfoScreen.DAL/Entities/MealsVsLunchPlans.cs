using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspITInfoScreen.DAL.Entities
{
    public class MealsVsLunchPlans
    {
        public int Id { get; set; }

        private int lunchPlanId;
        private int mealId;
        private string weekday;

        public MealsVsLunchPlans()
        {

        }
        public MealsVsLunchPlans(int lunchPlanId, int mealId, string weekday)
        {
            LunchPlanId = lunchPlanId;
            MealId = mealId;
            Weekday = weekday;
        }

        public string Weekday
        {
            get { return weekday; }
            set {
                if (DataValidation.WeekDay(value))
                    weekday = value;
                else
                    throw new ArgumentNullException($"weekday must be a day of the week; is {value}");
            }
        }
        public int MealId
        {
            get { return mealId; }
            set {
                if (DataValidation.WholeNumber(value))
                    mealId = value;
                else
                    throw new ArgumentOutOfRangeException($"mealId must be a whole number; is {value}");
            }
        }
        public int LunchPlanId
        {
            get { return lunchPlanId; }
            set {
                if (DataValidation.WholeNumber(value))
                    lunchPlanId = value;
                else
                    throw new ArgumentOutOfRangeException($"lunchPlanId must be a whole number; is {value}");
            }
        }
    }
}
