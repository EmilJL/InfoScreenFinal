using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspITInfoScreen.DAL;

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

    }
}
