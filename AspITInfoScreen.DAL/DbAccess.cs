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
        private const string connectionString = @"Data Source=cvdb3,1488;Initial Catalog=DAHO.AspITInfoScreen;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public Model GetDataAndCreateModel()
        {    
            const string GetLunchPlansQuery = "SELECT * from LunchPlans";
            const string GetMessagesQuery = "SELECT * from Messages";
            const string GetMealsVsLunchPlansQuery = "SELECT * from MealsVsLunchPlans";
            const string GetMealsQuery = "SELECT * from Meals";
            var lunchPlans = new ObservableCollection<LunchPlan>();
            var messages = new ObservableCollection<Message>();
            var mealsVsLunchPlansCollection = new ObservableCollection<MealsVsLunchPlans>();
            var meals = new ObservableCollection<Meal>();
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
                                    var mealsVsLunchPlans = new MealsVsLunchPlans();
                                    mealsVsLunchPlans.Id = reader.GetInt32(0);
                                    mealsVsLunchPlans.LunchPlanId = reader.GetInt32(1);
                                    mealsVsLunchPlans.MealId = reader.GetInt32(2);
                                    mealsVsLunchPlans.Weekday = reader.GetString(3);
                                    mealsVsLunchPlansCollection.Add(mealsVsLunchPlans);
                                }
                            }
                            cmd.CommandText = GetLunchPlansQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var lunchPlan = new LunchPlan();
                                    lunchPlan.Id = reader.GetInt32(0);
                                    lunchPlan.Week = reader.GetInt32(1);
                                    lunchPlans.Add(lunchPlan);
                                }
                            }
                            cmd.CommandText = GetMessagesQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var message = new Message();
                                    message.Id = reader.GetInt32(0);
                                    message.AdminId = reader.GetInt32(1);
                                    message.Date = reader.GetDateTime(2);
                                    message.Text = reader.GetString(3);
                                    message.Header = reader.GetString(4);
                                    messages.Add(message);
                                }
                            }
                            cmd.CommandText = GetMealsQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var meal = new Meal();
                                    meal.Id = reader.GetInt32(0);
                                    meal.Description = reader.GetString(1);
                                    meal.TimesChosen = reader.GetInt32(2);
                                    meals.Add(meal);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            Model model = new Model(lunchPlans, messages, meals, mealsVsLunchPlansCollection);
            return model;
        }
    }
}
