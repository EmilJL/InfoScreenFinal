using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspITInfoScreen.DAL.Entities
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
            set {
                if (DataValidation.WeekOfYear(value))
                    week = value;
                else
                    throw new ArgumentOutOfRangeException($"Week must be greater than 0 and equal to or less than 52; is {value}");
            }
        }
    }
}
