using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspITInfoScreen.DAL;
using AspITInfoScreen.DAL.Entities;

namespace AspITInfoScreen.Business
{
    public class LunchPlanHandler : DBHandler
    {
        /// <summary>
        /// Returns a lunchplan from the database with the given id
        /// </summary>
        /// <param name="id">Id to search for in the database</param>
        /// <returns></returns>
        public LunchPlan GetLunchPlan(int id)
        {
            LunchPlan lunchPlan = Model.LunchPlans.Where(m => m.Id == id).FirstOrDefault();
            return lunchPlan;
        }
        /// <summary>
        /// Returns a lunchplan from the database for the given week
        /// </summary>
        /// <param name="week"></param>
        /// <returns></returns>
        public LunchPlan GetLunchPlanForWeek(int week)
        {
            LunchPlan lunchPlan = Model.LunchPlans.Where(l => l.Week == week).FirstOrDefault();
            return lunchPlan;
        }
        /// <summary>
        /// Returns meals for the provided week
        /// </summary>
        /// <param name="week">int</param>
        /// <returns></returns>
        public List<ViewMealsVsLunchPlansJoin> GetMealsForWeek(int week)
        {
            LunchPlan lp = GetLunchPlanForWeek(week);
            List<MealsVsLunchPlans> mealsForWeek = Model.MealsVsLunchPlans.Where(mvsl => mvsl.LunchPlanId == lp.Id).ToList();
            List<ViewMealsVsLunchPlansJoin> result = new List<ViewMealsVsLunchPlansJoin>();
            foreach (MealsVsLunchPlans mvsl in mealsForWeek)
            {
                ViewMealsVsLunchPlansJoin meal = new ViewMealsVsLunchPlansJoin
                {
                    Meal = Model.Meals.Where(m => m.Id == mvsl.MealId).FirstOrDefault().Description,
                    Weekday = mvsl.Weekday
                };
                result.Add(meal);
            }

            return result;
        }
    }
}
