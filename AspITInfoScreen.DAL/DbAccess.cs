using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspITInfoScreen.DAL.Entities;

namespace AspITInfoScreen.DAL
{
    public class DbAccess
    {
        private string connectionString = $"Data Source=10.205.112.33;Initial Catalog=DAHO.AspITInfoScreen;User ID={PrivateInfo.DbUser};Password={PrivateInfo.DbPassword};Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public Model GetDataAndCreateModel()
        {    
            const string GetLunchPlansQuery = "SELECT * from LunchPlans";
            const string GetMessagesQuery = "SELECT * from Messages";
            const string GetMealsVsLunchPlansQuery = "SELECT * from MealsVsLunchPlans";
            const string GetMealsQuery = "SELECT * from Meals";
            const string GetIpQuery = "SELECT * FROM DeviceIP";
            var lunchPlans = new ObservableCollection<LunchPlan>();
            var messages = new ObservableCollection<Message>();
            var mealsVsLunchPlansCollection = new ObservableCollection<MealsVsLunchPlans>();
            var meals = new ObservableCollection<Meal>();
            var ip = new ObservableCollection<IpAddress>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = GetMealsVsLunchPlansQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var mealsVsLunchPlans = new MealsVsLunchPlans
                                    {
                                        Id = reader.GetInt32(0),
                                        LunchPlanId = reader.GetInt32(1),
                                        MealId = reader.GetInt32(2),
                                        Weekday = reader.GetString(3)
                                    };
                                    mealsVsLunchPlansCollection.Add(mealsVsLunchPlans);
                                }
                            }
                            cmd.CommandText = GetLunchPlansQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var lunchPlan = new LunchPlan
                                    {
                                        Id = reader.GetInt32(0),
                                        Week = reader.GetInt32(1)
                                    };
                                    lunchPlans.Add(lunchPlan);
                                }
                            }
                            cmd.CommandText = GetMessagesQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var message = new Message
                                    {
                                        Id = reader.GetInt32(0),
                                        AdminId = reader.GetInt32(1),
                                        Date = reader.GetDateTime(2),
                                        Text = reader.GetString(3),
                                        Header = reader.GetString(4)
                                    };
                                    messages.Add(message);
                                }
                            }
                            cmd.CommandText = GetMealsQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var meal = new Meal
                                    {
                                        Id = reader.GetInt32(0),
                                        Description = reader.GetString(1),
                                        TimesChosen = reader.GetInt32(2)
                                    };
                                    meals.Add(meal);
                                }
                            }
                            cmd.CommandText = GetIpQuery;
                            using(SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var address = new IpAddress
                                    {
                                        Id = reader.GetInt32(0),
                                        Ip = reader.GetString(1)
                                    };
                                    ip.Add(address);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                DataValidation.SaveError(eSql.ToString());
                //DataValidation.SaveError("Exception: " + eSql.GetType() + "\n  " + eSql.Message + "\r\n" + eSql.StackTrace + "\r\n In " + ToString());
            }
            Model model = new Model(lunchPlans, messages, meals, mealsVsLunchPlansCollection, ip);
            return model;
        }
        /// <summary>
        /// Gets the lastest message from the Database
        /// </summary>
        /// <returns>ViewAdminMessageJoin Object</returns>
        public ViewAdminMessageJoin GetMessagesView()
        {
            var message = new ViewAdminMessageJoin();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT TOP 1 * FROM ViewAdminMessageJoin ORDER BY Date DESC";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                message.Username = reader.GetString(0);
                                message.Header = reader.GetString(1);
                                message.Date = reader.GetDateTime(2);
                                message.Text = reader.GetString(3);
                            }
                        }
                    }
                }
            }
            return message;
        }

        public bool SetActiveIP(IpAddress ip)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = new SqlCommand("UPDATE DeviceIP SET Device = @Device WHERE Id = @Id", conn))
                        {
                            cmd.Parameters.AddWithValue("@Device", ip.Ip);
                            cmd.Parameters.AddWithValue("@Id", ip.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                return true;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return false;
            }
        }

        public bool CreateNewIP(string ip)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = new SqlCommand("INSERT INTO DeviceIP (Device) VALUES (@Device)", conn))
                        {
                            cmd.Parameters.AddWithValue("@Device", ip);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                return true;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return false;
            }
        }
    }
}
