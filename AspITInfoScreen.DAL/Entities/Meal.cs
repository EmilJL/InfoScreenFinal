using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspITInfoScreen.DAL.Entities
{
    public class Meal
    {
        public int Id { get; set; }
        private int timesChosen;
        private string description;
        public Meal()
        {

        }
        public Meal(string description, int timesChosen = 0)
        {
            Description = description;
            TimesChosen = timesChosen;
        }
        public int TimesChosen
        {
            get { return timesChosen; }
            set {
                if (DataValidation.NaturalNumber(value))
                    timesChosen = value;
                else
                    throw new ArgumentOutOfRangeException($"A meal's timesChosen must be a natural number; is {value}");
            }
        }
        public string Description
        {
            get { return description; }
            set {
                if (DataValidation.String(value))
                    description = value;
                else
                {
                    throw new ArgumentNullException($"description cannot be null, whitespace or empty; is {value}");
                }
            }
        }
    }
}
