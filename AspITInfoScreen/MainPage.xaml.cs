﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Net;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Documents;
using AspITInfoScreen.Business;
using AspITInfoScreen.DAL;
using Windows.Data.Pdf;
using System.Net.Http;
using System.Collections.ObjectModel;
using AspITInfoScreen.DAL.Entities;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AspITInfoScreen
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DBHandler dbHandler;
        private LunchPlanHandler lunchPlanHandler;
        private MealHandler mealHandler;
        private MessageHandler messageHandler;
        private ImageHandler imageHandler;
        private RSSFeedHandler rSSFeedHandler;
        private IPHandler iPHandler;
        private List<ViewMealsVsLunchPlansJoin> menu;
        private ClockHandler clockHandler;
        int counter;

        public ObservableCollection<BitmapImage> PdfPages
        {
            get;
            set;
        } = new ObservableCollection<BitmapImage>();
        public MainPage()
        {
            this.InitializeComponent();
            lunchPlanHandler = new LunchPlanHandler();
            mealHandler = new MealHandler();
            dbHandler = new DBHandler();
            messageHandler = new MessageHandler();
            imageHandler = new ImageHandler();
            rSSFeedHandler = new RSSFeedHandler("http://feeds.tv2.dk/nyhederne_seneste/rss");
            iPHandler = new IPHandler();
            clockHandler = new ClockHandler();
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Title = CalendarHandler.GetStringDate("dd/MM/yyyy") + " - Uge : " + CalendarHandler.GetWeekNumber();
            UpdateIp();
            counter = 0;
            SetDpTimer();
        }

        private void SetDpTimer()
        {
            DispatcherTimer timer = new DispatcherTimer() { Interval = new TimeSpan(0,0,1) };
            timer.Tick += Dispatcher_Elapsed;
            timer.Start();
        }

        private void Dispatcher_Elapsed(object sender, object e)
        {
            //Correctly update layout on start
            if (counter == 0)
            {
                UpdateUiContent();
                UpdateUiContent();
            }
            //Update clock
            UpdateAnalogueClock();
            TBlockTime.Text = CalendarHandler.GetStringDate("HH:mm:ss");

            if( counter % 20 == 0) //20 seconds
            {
                WeatherAndComicToggle();

                //News
                if (counter % 30 == 0) //30 seconds
                {
                    if (rSSFeedHandler.NewsList.Count > 0)
                    {
                        SetNews();
                        NewsTextFormatting();
                    }
                    else //If empty
                    {
                        rSSFeedHandler.GetFeed();
                    }
                }
            }
            //Time between global update : 5 min.
            if (counter >= 300)
            {
                //Update static model for all handlers
                dbHandler.UpdateModel();

                //Update User Interface
                UpdateUiContent();

                //Update IP
                //UpdateIp();

                counter = 1;
            } else
            {
                counter++;
            }
        }
        /// <summary>
        /// Update and resize all GUI elements
        /// </summary>
        private void UpdateUiContent()
        {
            SetStackPanelLeft();
            SetStackPanelMiddle();
            SetStackPanelRight();

            SetWeatherImage();
            SetComicStripImage(ImageComic, imageHandler.GetComic());
            SetComicStripImage(ImageComic2, imageHandler.GetComic(1));

            SetAdminMessage();

            GetMealPlan();
            SetMealPlanSize();
            SetMealPlanTextSize();

            UpdateTextElements();

            AnalogueClockSize();
        }
        /// <summary>
        /// Retrieves a BitmapImage of the weather chart from DMI.
        /// </summary>
        private void SetWeatherImage()
        {
            try
            {
                BitmapImage weather = new BitmapImage();

                weather.DecodePixelType = DecodePixelType.Logical;
                //Change to stackpanel
                int spWidth = (int)StackPanelLeftCol.ActualWidth;
                int spHeight = (int)(StackPanelLeftCol.ActualHeight * 0.33);
                weather.DecodePixelWidth = spWidth;
                weather.DecodePixelHeight = spHeight;
                ImageWeather.MaxWidth = spWidth;
                ImageWeather.MaxHeight = spHeight;
                weather.UriSource = imageHandler.GetWeather();

                ImageWeather.Source = weather;

            }
            catch (Exception error)
            {
                DataValidation.SaveError(error.ToString());
            }
            
        }
        /// <summary>
        /// Retrieves Garfield comic strips from Cloudfront.net.
        /// </summary>
        /// <param name="container">GUI element</param>
        /// <param name="deductDays">Used to retrieve old comics</param>
        private void SetComicStripImage(Image container, Uri uri)
        {
            try
            {
                BitmapImage comic = new BitmapImage();
                Uri address = uri;

                comic.DecodePixelType = DecodePixelType.Logical;
                comic.DecodePixelWidth = (int)MyGrid.ColumnDefinitions.Select(c => c.ActualWidth).FirstOrDefault();
                comic.DecodePixelHeight = (int)Math.Round(MyGrid.RowDefinitions.Select(c => c.ActualHeight).FirstOrDefault() / 2);
                comic.UriSource = address;

                container.Source = comic;

            }
            catch (Exception error)
            {
                DataValidation.SaveError(error.ToString());
            }
        }
        /// <summary>
        /// Set height for child element in the stackpanel
        /// </summary>
        private void SetStackPanelLeft()
        {
            StackPanelLeftCol.Height = MyGrid.ActualHeight;
            double pHeight = (StackPanelLeftCol.ActualHeight * 0.33) - StackPanelLeftCol.Spacing;

            ImageLogo.MaxHeight = pHeight;

            StackPanelNews.MinHeight = pHeight;
            StackPanelNews.MaxHeight = pHeight;

            ImageWeather.MaxHeight = pHeight;
            StackPanelComic.MaxHeight = pHeight;
        }
        /// <summary>
        /// Set height for child element in the stackpanel
        /// </summary>
        private void SetStackPanelMiddle()
        {
            StackPanelMidCol.Height = MyGrid.ActualHeight;
            double pHeight = StackPanelMidCol.ActualHeight;

            StackPanelMessage.MaxHeight = pHeight * 0.9;
            StackPanelMessage.MinHeight = pHeight * 0.9;
        }
        /// <summary>
        /// Set height for child element in the stackpanel
        /// </summary>
        private void SetStackPanelRight()
        {
            StackPanelRightcol.Height = MyGrid.ActualHeight;
            double pHeight = StackPanelRightcol.ActualHeight * 0.33;

            ParentGrid.MaxHeight = pHeight;
            StackPanelMealPlan.MaxHeight = pHeight * 2;
        }
        /// <summary>
        /// Retrieves the meals for the week and set GUI elements.
        /// </summary>
        private void GetMealPlan()
        {
            try
            {
                menu = lunchPlanHandler.GetMealsForWeek(CalendarHandler.GetWeekNumber());

                foreach (var item in menu)
                {
                    //Checks against application autogenerated values
                    switch (item.Weekday.ToLower())
                    {
                        case "monday":
                            TBlockMondayMeal.Text = item.Meal;
                            break;
                        case "tuesday":
                            TBlockTuesdayMeal.Text = item.Meal;
                            break;
                        case "wednesday":
                            TBlockWednesdayMeal.Text = item.Meal;
                            break;
                        case "thursday":
                            TBlockThursdayMeal.Text = item.Meal;
                            break;
                        case "friday":
                            TBlockFridayMeal.Text = item.Meal;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception error)
            {

                DataValidation.SaveError(error.ToString());
            }
            
        }
        /// <summary>
        /// Sets StackPanel and its elements width based on grid size.
        /// </summary>
        private void SetMealPlanSize()
        {
            try
            {
                StackPanel stackParent = (StackPanel)StackPanelMealPlan.Parent;
                StackPanelMealPlan.MaxWidth = stackParent.ActualWidth * 0.99;
                StackPanelMealPlan.MaxHeight = stackParent.ActualHeight * 0.65;
                StackPanelMealPlan.MinHeight = stackParent.ActualHeight * 0.6;
                double tBlockWidth = StackPanelMealPlan.MaxWidth / 3;

                double stackPanelHeight = (StackPanelMealPlan.MaxHeight / 5) - 12;

                foreach (var item in StackPanelMealPlan.Children)
                {
                    if(item.GetType() == typeof(StackPanel))
                    {
                        StackPanel sp = (StackPanel)item;
                        sp.Margin = new Thickness(5, 0, 5, 0);
                        sp.Height = stackPanelHeight;
                        TextBlock tb = sp.Children[0] as TextBlock;
                        tb.Width = tBlockWidth;
                        tb = sp.Children[1] as TextBlock;
                        tb.MaxWidth = tBlockWidth * 2 - 10;
                        tb.TextWrapping = TextWrapping.WrapWholeWords;
                        tb.TextTrimming = TextTrimming.CharacterEllipsis;
                    }
                }
            }
            catch (Exception error)
            {
                DataValidation.SaveError(error.ToString());
            }
        }
        /// <summary>
        /// Incrementally resizes text until it fits within the mealplan
        /// </summary>
        private void SetMealPlanTextSize()
        {
            try
            {
                List<StackPanel> days = new List<StackPanel>();
                double totalHeight = 0;
                StackPanelMealPlan.UpdateLayout();
                foreach (var item in StackPanelMealPlan.Children)
                {
                    if (item.GetType() == typeof(StackPanel))
                    {
                        StackPanel sp = (StackPanel)item;
                        totalHeight += sp.ActualHeight;
                        days.Add(sp);
                        //Day
                        TextBlock tb = sp.Children[0] as TextBlock;
                        tb.FontSize = TBlockAdminMessage.FontSize + 5;
                        tb.VerticalAlignment = VerticalAlignment.Center;
                        tb.UpdateLayout();
                        //Meal
                        tb = sp.Children[1] as TextBlock;
                        tb.FontSize = TBlockAdminMessage.FontSize;
                        tb.VerticalAlignment = VerticalAlignment.Center;
                        tb.UpdateLayout();
                        //Width
                        while (tb.IsTextTrimmed && tb.FontSize > 10)
                        {
                            tb.FontSize--;
                            tb.UpdateLayout();
                        }
                    }
                }

                while (totalHeight > StackPanelMealPlan.MaxHeight)
                {
                    totalHeight = 0;
                    //Height
                    foreach (StackPanel item in days)
                    {
                        TextBlock tbDay = (TextBlock)item.Children[0];
                        TextBlock tbMeal = (TextBlock)item.Children[1];
                        tbMeal.TextTrimming = TextTrimming.CharacterEllipsis;

                        if (tbMeal.FontSize > 10 && tbDay.FontSize > 10)
                        {
                            tbMeal.FontSize--;
                            tbDay.FontSize--;
                            tbMeal.UpdateLayout();
                            tbDay.UpdateLayout();
                            totalHeight += item.ActualHeight;
                        }
                    }
                }
            }
            catch (Exception error)
            {
                DataValidation.SaveError(error.ToString());
            }
            
        }
        /// <summary>
        /// Retrives the custom admin message from the database and sets the relevant AdminMessage element in the GUI.
        /// </summary>
        private void SetAdminMessage()
        {
            try
            {
                ViewAdminMessageJoin msg = messageHandler.GetNewestViewMessage();

                TBlockAdminMessageTitle.Text = msg.Header;
                TBlockAdminMessage.Text = msg.Text;
                TBlockAdminMessageDate.Text = msg.Date.ToString("dd/MM/yyyy");
                TBlockAdminMessageAuthor.Text = msg.Username;
                MessageTextFormatting();
            }
            catch (Exception error)
            {
                DataValidation.SaveError(error.ToString());
            }
        }
        /// <summary>
        /// Incrementally resizes text until it fits within the message area
        /// </summary>
        private void MessageTextFormatting()
        {
            try
            {
                UIElementCollection messagePanel = StackPanelMessage.Children;

                foreach (var item in messagePanel)
                {
                    if (item.GetType() == typeof(TextBlock))
                    {
                        bool overflow = false;
                        TextBlock tb = (TextBlock)item;
                        tb.TextWrapping = TextWrapping.WrapWholeWords;
                        tb.TextTrimming = TextTrimming.CharacterEllipsis;
                        tb.FontSize = 40;
                        tb.UpdateLayout();
                        if (tb.ActualHeight > StackPanelMessage.MaxHeight * 0.9 - TBlockAdminMessageTitle.ActualHeight - TBlockAdminMessageAuthor.ActualHeight)
                        {
                            overflow = true;
                        }
                        while ((tb.IsTextTrimmed || overflow) && tb.FontSize > 10)
                        {
                            tb.FontSize -= 1;
                            tb.UpdateLayout();
                            if (tb.ActualHeight < StackPanelMessage.MaxHeight * 0.9 - TBlockAdminMessageTitle.ActualHeight - TBlockAdminMessageAuthor.ActualHeight)
                            {
                                overflow = false;
                            }
                        }
                    }
                    TBlockAdminMessage.MaxHeight = StackPanelMessage.MaxHeight - TBlockAdminMessageTitle.ActualHeight - TBlockAdminMessageAuthor.ActualHeight;
                }
            }
            catch (Exception error)
            {

                DataValidation.SaveError(error.ToString());
            }
            
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateUiContent();
        }
        /// <summary>
        /// Toggles visibility of GUI elements weatherforecast and module plan.
        /// </summary>
        private void WeatherAndComicToggle()
        {
            if (ImageWeather.Visibility == Visibility.Collapsed)
            {
                ImageWeather.Visibility = Visibility.Visible;
                ImageComic.Visibility = Visibility.Collapsed;
                ImageComic2.Visibility = Visibility.Collapsed;
            } else
            {
                ImageWeather.Visibility = Visibility.Collapsed;
                ImageComic.Visibility = Visibility.Visible;
                ImageComic2.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// Display/cycle news from the RSS feed retrieved with [Handler].GetFeed()
        /// </summary>
        private void SetNews()
        {
            try
            {
                TV2NewsItem item = rSSFeedHandler.NewsList.FirstOrDefault();
                TBlockNewsTitle.Text = item.Title;
                TBlockNewspublishDate.Text = item.PubDate.ToString("dd/MM/yyyy");
                TBlockNewsContent.Text = item.Description;
                TBlockNewsAuthor.Text = "Af: " + item.Author;

                rSSFeedHandler.NewsList.Remove(item);
            }
            catch (Exception error)
            {
                DataValidation.SaveError(error.ToString());
            }
        }
        /// <summary>
        /// Changes news containers text based on current content. Does nothing with small window size
        /// </summary>
        private void NewsTextFormatting()
        {
            try
            {
                //Set text block elements height based on parent element
                StackPanel parent = (StackPanel)TBlockNewsContent.Parent;
                double parentHeight = parent.ActualHeight;

                if (parentHeight > 100)
                {
                    //StkPnl --> TxtBlock
                    TBlockNewsTitle.MaxHeight = parentHeight * 0.20; //20%
                    TBlockNewsContent.MaxHeight = parentHeight * 0.60; // 60%

                    //Inside another stackpanel - StkPnl(Parent) --> StkPnl --> TxtBlock
                    TBlockNewspublishDate.MaxHeight = parentHeight * 0.10; //10%
                    TBlockNewsAuthor.MaxHeight = TBlockNewspublishDate.MaxHeight; //10%
                }

                //Reset desired font size and proportion
                TBlockNewsAuthor.FontSize = 30;
                TBlockNewsContent.FontSize = 40;
                TBlockNewsTitle.FontSize = 48;
                TBlockNewspublishDate.FontSize = 30;

                //Updates all changes made to the text containers
                UpdateTextElements();

                //Incrementally shrink main content until everything fits - Or minimum size reached
                while (TBlockNewsContent.IsTextTrimmed && TBlockNewsContent.FontSize > 10)
                {
                    TBlockNewsContent.FontSize--;
                    TBlockNewsContent.UpdateLayout();
                }
                //Incrementally shrink author, date and title until everything fits or minimum limit reached
                while (TBlockNewsTitle.IsTextTrimmed && TBlockNewsTitle.FontSize > 15)
                {
                    //Title can't be smaller than Author or Date text element
                    if (TBlockNewsTitle.FontSize - TBlockNewspublishDate.FontSize > 5)
                    {
                        //Title can't be smaller than Author or Date text element
                        if (TBlockNewsTitle.FontSize - TBlockNewspublishDate.FontSize > 5)
                        {
                            TBlockNewsTitle.FontSize--;
                            //Author and Date can't be smaller than 10 pixels - this minimum limits other elements minimum size as well
                            if (TBlockNewspublishDate.FontSize > 10 && TBlockNewsAuthor.FontSize > 10)
                            {
                                TBlockNewspublishDate.FontSize--;

                                TBlockNewsAuthor.FontSize--;
                            }
                        }
                        UpdateTextElements();
                    }
                }
            }
            catch (Exception error)
            {
                DataValidation.SaveError(error.ToString());
            }
            
        }
        /// <summary>
        /// Resize the analogue clock based on the smallest of its width and height
        /// </summary>
        private void AnalogueClockSize()
        {
            try
            {
                ParentGrid.Height = MyGrid.RowDefinitions.FirstOrDefault().ActualHeight;
                ParentGrid.Width = MyGrid.ColumnDefinitions.FirstOrDefault().ActualWidth;
                TBlockTime.FontSize = MyGrid.RowDefinitions.FirstOrDefault().ActualHeight * 0.30;

                ParentGrid.UpdateLayout();
                TBlockTime.UpdateLayout();

                //Parent meassurements
                int clockParentWidth = (int)ParentGrid.ActualWidth;
                int clockParentHeight = (int)ParentGrid.RowDefinitions.FirstOrDefault().ActualHeight;

                clockHandler.SizeCalc(clockParentWidth, clockParentHeight);

                int clockDiameter = clockHandler.Diameter;
                int radius = clockHandler.Radius;

                //Clock meassurements
                AnalogueClockEllipsis.Height = clockDiameter;
                AnalogueClockEllipsis.Width = clockDiameter;

                //Arm Width (Length)
                rectangleSecond.Width = clockHandler.ArmSLength;
                rectangleMinute.Width = clockHandler.ArmMLength;
                rectangleHour.Width = clockHandler.ArmHLength;

                //Arm Height
                rectangleSecond.Height = clockHandler.ArmSHeight;
                rectangleMinute.Height = clockHandler.ArmMHeight;
                rectangleHour.Height = clockHandler.ArmHHeight;

                //Margin for arm offset
                rectangleSecond.Margin = new Thickness(rectangleSecond.Width, 0, 0, 0); ;
                rectangleMinute.Margin = new Thickness(rectangleMinute.Width, 0, 0, 0); ;
                rectangleHour.Margin = new Thickness(rectangleHour.Width, 0, 0, 0); ;

                //Offset for animation center
                secondHand.CenterY = rectangleSecond.Height / 2;
                minuteHand.CenterY = rectangleMinute.Height / 2;
                hourHand.CenterY = rectangleHour.Height / 2;

                //Timestamps -TextBlocks- coord and size
                double dist = clockHandler.TxtDist;
                double coordY = clockHandler.TxtCoordX;
                double coordX = clockHandler.TxtCoordY;

                //Can this be List<>() looped?
                TBlockTwelve.Margin = new Thickness(0, -dist, 0, 0);
                TBlockOne.Margin = new Thickness(coordY, -coordX, 0, 0);
                TBlockTwo.Margin = new Thickness(coordX, -coordY, 0, 0);
                TBlockThree.Margin = new Thickness(dist, 0, 0, 0);
                TBlockFour.Margin = new Thickness(coordX, coordY, 0, 0);
                TBlockFive.Margin = new Thickness(coordY, coordX, 0, 0);
                TBlockSix.Margin = new Thickness(0, dist, 0, 0);
                TBlockSeven.Margin = new Thickness(-coordY, coordX, 0, 0);
                TBlockEight.Margin = new Thickness(-coordX, coordY, 0, 0);
                TBlockNine.Margin = new Thickness(-dist, 0, 0, 0);
                TBlockTen.Margin = new Thickness(-coordX, -coordY, 0, 0);
                TBlockEleven.Margin = new Thickness(-coordY, -coordX, 0, 0);

                double txtSize = clockHandler.TxtSize;

                IEnumerable<TextBlock> txtChilden = ParentGrid.Children.OfType<TextBlock>();

                foreach (var child in txtChilden)
                {
                    child.FontSize = txtSize;
                }

                AnalogueClockEllipsis.StrokeThickness = clockHandler.BordorThickness;

                //Center dot for style
                ArmDotEllipsis.Width = rectangleHour.Height;
                ArmDotEllipsis.Height = rectangleHour.Height;
            }
            catch (Exception error)
            {
                DataValidation.SaveError(error.ToString());
            }
            
        }
        /// <summary>
        /// Calls method UpdateLayout() on all text elements
        /// </summary>
        private void UpdateTextElements()
        {
            //News
            TBlockNewsContent.UpdateLayout();
            TBlockNewsTitle.UpdateLayout();
            TBlockNewsAuthor.UpdateLayout();
            TBlockNewspublishDate.UpdateLayout();
            //Message
            TBlockAdminMessage.UpdateLayout();
            TBlockAdminMessageAuthor.UpdateLayout();
            TBlockAdminMessageDate.UpdateLayout();
            TBlockAdminMessageTitle.UpdateLayout();
        }
        /// <summary>
        /// Moves the arms on the analogue clock
        /// </summary>
        private void UpdateAnalogueClock()
        {
            try
            {
                List<double> angles = clockHandler.GetAllRotations(CalendarHandler.GetDate());
                secondHand.Angle = angles[0];
                minuteHand.Angle = angles[1];
                hourHand.Angle = angles[2];
            }
            catch (Exception error)
            {
                DataValidation.SaveError(error.ToString());
            }
            
        }
        /// <summary>
        /// Inserts the machines current IP into the database (mostly obsolete with a static IP assigned)
        /// </summary>
        private void UpdateIp()
        {
            try
            {
                if (iPHandler.HasIp())
                {
                    iPHandler.UpdateIP(IPHandler.CheckIP());
                }
                else
                {
                    iPHandler.CreateNewIp(IPHandler.CheckIP());
                }
            }
            catch (Exception error)
            {
                DataValidation.SaveError(error.ToString());
            }
        }
    }
}
