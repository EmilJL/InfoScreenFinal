using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspITInfoScreen.DAL
{
    public class LunchPlan
    {
        public int Id { get; set; }
        private int week;
        public LunchPlan()
        {

        }
        public LunchPlan(int week)
        {
            Week = week;
        }
        public int Week
        {
            get { return week; }
            set { week = value; }
        }
    }
}
