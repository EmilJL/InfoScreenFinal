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
        public LunchPlan GetLunchPlan(int id)
        {
            LunchPlan lunchPlan = Model.LunchPlans.Where(m => m.Id == id).FirstOrDefault();
            return lunchPlan;
        }
        public LunchPlan GetLunchPlanForWeek(int week)
        {
            LunchPlan lunchPlan = Model.LunchPlans.Where(l => l.Week == week).FirstOrDefault();
            return lunchPlan;
        }
        public List<ViewMealsVsLunchPlansJoin> GetMealsForWeek(int week)
        {
            var mealsForWeek = Model.MealsVsLunchPlans.Where(mvsl => mvsl.LunchPlanId == GetLunchPlanForWeek(week).Id);
            List<ViewMealsVsLunchPlansJoin> result = new List<ViewMealsVsLunchPlansJoin>();
            foreach (MealsVsLunchPlans mvsl in mealsForWeek)
            {
                ViewMealsVsLunchPlansJoin meal = new ViewMealsVsLunchPlansJoin();
                meal.Meal = Model.Meals.Where(m => m.Id == mvsl.MealId).FirstOrDefault().Description;
                meal.Weekday = mvsl.Weekday;
                result.Add(meal);
            }
            return result;
        }
    }
}
