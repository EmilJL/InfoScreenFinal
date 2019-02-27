using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspITInfoScreen.Business
{
    public class ClockHandler
    {
        //Degrees
        private const int A = 30;
        private const int B = 60;
        private const int C = 90;

        //Radians
        private readonly double Arad;
        private readonly double Brad;
        private readonly double Crad;

        private int diameter;

        private readonly double txtDistMod;
        private readonly double txtSizeMod;

        private readonly double borderThicknessMod;

        private readonly double armSLengthMod;
        private readonly double armMLengthMod;
        private readonly double armHLengthMod;

        private readonly double armSHeightMod;
        private readonly double armMHeightMod;
        private readonly double armHHeightMod;
        /// <summary>
        /// Constructor for clock leave blank for default values - give new values in decimal percentages (1 = 100%)
        /// </summary>
        /// <param name="txtDistMod">Distance from center to text</param>
        /// <param name="txtSizeMod">Size of text relative to clock diameter</param>
        /// <param name="borderThicknessMod">Border thickness relative to clock diameter</param>
        /// <param name="armSLengthMod">Second arm's lenght relative to clock diameter</param>
        /// <param name="armMLengthMod">Minute arm's lenght relative to clock diameter</param>
        /// <param name="armHLengthMod">Hour arm's lenght relative to clock diameter</param>
        /// <param name="armSHeightMod">Second arm's Height relative to clock diameter</param>
        /// <param name="armMHeightMod">Minute arm's Height relative to clock diameter</param>
        /// <param name="armHHeightMod">Hour arm's Height relative to clock diameter</param>
        public ClockHandler(double txtDistMod = 0.88, double txtSizeMod = 0.083, double borderThicknessMod = 0.008, double armSLengthMod = 0.95, double armMLengthMod = 0.80, double armHLengthMod = 0.70, double armSHeightMod = 0.017, double armMHeightMod = 0.021, double armHHeightMod = 0.026)
        {
            Arad = ToRadians(A);
            Brad = ToRadians(B);
            Crad = ToRadians(C);
            this.txtDistMod = txtDistMod;
            this.txtSizeMod = txtSizeMod;
            this.borderThicknessMod = borderThicknessMod;
            this.armSLengthMod = armSLengthMod;
            this.armMLengthMod = armMLengthMod;
            this.armHLengthMod = armHLengthMod;
            this.armSHeightMod = armSHeightMod;
            this.armMHeightMod = armMHeightMod;
            this.armHHeightMod = armHHeightMod;
        }

        public int Diameter
        {
            get { return diameter; }
            private set {
                if(value > 0)
                {
                    diameter = value;
                } else
                {
                    throw new ArgumentOutOfRangeException("The diameter of the clock must be greater than zero.");
                }
            }
        }

        public int Radius
        {
            get { return diameter / 2; }
        }

        public double TxtCoordX
        {
            get { return (Math.Sin(ToRadians(A)) * diameter * txtDistMod) / Math.Sin(ToRadians(C)); }
        }

        public double TxtCoordY
        {
            get { return (Math.Sin(ToRadians(B)) * diameter * txtDistMod) / Math.Sin(ToRadians(C)); }
        }
        public double TxtSize
        {
            get { return diameter * txtSizeMod; }
        }
        public double TxtDist
        {
            get { return diameter * txtDistMod; }
        }
        public double BordorThickness
        {
            get { return diameter * borderThicknessMod; }
        }
        public double ArmSLength
        {
            get { return Radius * armSLengthMod; }
        }
        public double ArmMLength
        {
            get { return Radius * armMLengthMod; }
        }
        public double ArmHLength
        {
            get { return Radius * armHLengthMod; }
        }
        public double ArmSHeight
        {
            get { return diameter * armSHeightMod; }
        }
        public double ArmMHeight
        {
            get { return diameter * armMHeightMod; }
        }
        public double ArmHHeight
        {
            get { return diameter * armHHeightMod; }
        }
        /// <summary>
        /// Calculated all sized needed for the clock based on current width and height of its container.
        /// </summary>
        /// <param name="width">Container height</param>
        /// <param name="height">Container Width</param>
        public void SizeCalc(int width, int height)
        {
            if (height > width)
            {
                if (width % 2 == 0)
                {
                    diameter = width;
                }
                else
                {
                    diameter = width - 1;
                }
            }
            else
            {
                if (height % 2 == 0)
                {
                    diameter = height;
                }
                else
                {
                    diameter = height - 1;
                }
            }
        }
        /// <summary>
        /// Gives a list with rotations for all arms for a specific time. List{second, minute, hour}
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public List<double> GetAllRotations(DateTime time)
        {
            List<double> angles = new List<double>
            {
                GetRotationSecondsArm(time),
                GetRotationMinutesArm(time),
                GetRotationHoursArm(time)
            };
            return angles;
        }
        /// <summary>
        /// Calculates angle for seconds arm for a given time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public double GetRotationSecondsArm(DateTime time)
        {
            return time.Second * 6 - 90;
        }
        /// <summary>
        /// Calculates angle for Minutes arm for a given time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public double GetRotationMinutesArm(DateTime time)
        {
            return time.Minute * 6 - 90;
        }
        /// <summary>
        /// Calculates angle for Hours arm for a given time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public double GetRotationHoursArm(DateTime time)
        {
            return (time.Hour * 30) + (time.Minute * 0.5) - 90;
        }
        /// <summary>
        /// Conversion from degrees to radians for the analogue clock
        /// </summary>
        /// <param name="angle">Angle to be converted</param>
        /// <returns></returns>
        private double ToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }
    }
}
