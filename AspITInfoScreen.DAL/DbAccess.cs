﻿using System;
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
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                DataValidation.SaveError(eSql.ToString());
                //DataValidation.SaveError("Exception: " + eSql.GetType() + "\n  " + eSql.Message + "\r\n" + eSql.StackTrace + "\r\n In " + ToString());
            }
            Model model = new Model(lunchPlans, messages, meals, mealsVsLunchPlansCollection);
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
    }
}
