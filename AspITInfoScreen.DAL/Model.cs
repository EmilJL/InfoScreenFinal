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
            IpAddresses = model.IpAddresses;
        }
        public Model(ObservableCollection<LunchPlan> lunchPlans, ObservableCollection<Message> messages, ObservableCollection<Meal> meals, ObservableCollection<MealsVsLunchPlans> mealsVsLunchPlans, ObservableCollection<IpAddress> ipAddresses)
        {
            LunchPlans = lunchPlans;
            Messages = messages;
            Meals = meals;
            MealsVsLunchPlans = mealsVsLunchPlans;
            IpAddresses = ipAddresses;
        }
        public ObservableCollection<Meal> Meals { get; private set; }
        public ObservableCollection<LunchPlan> LunchPlans { get; private set; }
        public ObservableCollection<Message> Messages { get; private set; }
        public ObservableCollection<MealsVsLunchPlans> MealsVsLunchPlans { get; private set; }
        public ObservableCollection<IpAddress> IpAddresses { get; private set; }
    }
}
