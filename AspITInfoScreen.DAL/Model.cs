using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspITInfoScreen.DAL.Entities;


namespace AspITInfoScreen.DAL
{
    public class Model
    {
        DbAccess dbAccess = new DbAccess();
        public Model()
        {
            dbAccess = new DbAccess();
            Model model = dbAccess.GetDataAndCreateModel();
            Meals = model.Meals;
            LunchPlans = model.LunchPlans;
            Messages = model.Messages;
            MealsVsLunchPlans = model.MealsVsLunchPlans;
        }
        public Model(ObservableCollection<LunchPlan> lunchPlans, ObservableCollection<Message> messages, ObservableCollection<Meal> meals, ObservableCollection<MealsVsLunchPlans> mealsVsLunchPlans)
        {
            LunchPlans = lunchPlans;
            Messages = messages;
            Meals = meals;
            MealsVsLunchPlans = mealsVsLunchPlans;
        }
        // PRIVATE SETTERS?
        public ObservableCollection<Meal> Meals { get; set; }
        public ObservableCollection<LunchPlan> LunchPlans { get; set; }
        public ObservableCollection<Message> Messages { get; set; }
        public ObservableCollection<MealsVsLunchPlans> MealsVsLunchPlans { get; set; }
    }
}
