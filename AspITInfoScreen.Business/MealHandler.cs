using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspITInfoScreen.DAL.Entities;
using AspITInfoScreen.DAL;

namespace AspITInfoScreen.Business
{
    public class MealHandler : DBHandler
    {
        public List<Meal> GetMealsForLunchPlan(int id)
        {
            var mvls = Model.MealsVsLunchPlans.Where(mvl => mvl.LunchPlanId == id);
            List<Meal> meals = new List<Meal>();
            foreach (var mvl in mvls)
            {
                Meal meal = new Meal();
                meal = Model.Meals.Where(m => m.Id == mvl.MealId).FirstOrDefault();
                meals.Add(meal);
            }
            return meals;
        }
    }
}
