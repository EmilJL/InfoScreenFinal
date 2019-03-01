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
        private CalendarHandler calendarHandler;
        private RSSFeedHandler rSSFeedHandler;
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
            calendarHandler = new CalendarHandler();
            rSSFeedHandler = new RSSFeedHandler("http://feeds.tv2.dk/nyhederne_seneste/rss");
            clockHandler = new ClockHandler();
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Title = calendarHandler.GetStringDate("dd/MM/yyyy") + " - Uge : " + calendarHandler.GetWeekNumber();
            counter = 1;
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
            //What is this?
            if (counter == 1)
            {
                counter++;
            }
            //Update clock
            UpdateAnalogueClock();
            TBlockTime.Text = calendarHandler.GetStringDate("hh:mm:ss");

            if( counter % 10 == 0)
            {
                WeatherAndModuleToggle();
                
                //News
                if (rSSFeedHandler.NewsList.Count > 0)
                {
                    SetNews();
                    NewsTextFormatting();
                } else
                {
                    rSSFeedHandler.GetFeed();
                }
            }
            //Time between global update : 5 min.
            if (counter >= 300)
            {
                //Update static model for all handlers
                dbHandler.UpdateModel();

                //Update User Interface
                UpdateUiContent();
                counter = 1;
            } else
            {
                counter++;
            }
        }
        private void UpdateUiContent()
        {
            SetWeatherImage();
            SetComicStripImage(ImageComic);
            SetComicStripImage(ImageComic2, 1);
            SetAdminMessage();
            OpenRemoteModule();
            GetMealPlan();
            SetMealPlanWidth();
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
                Uri address = new Uri("http://servlet.dmi.dk/byvejr/servlet/byvejr_dag1?by=2630&mode=long");

                weather.DecodePixelType = DecodePixelType.Logical;
                int cWidth = (int)MyGrid.ColumnDefinitions.Select(c => c.ActualWidth).FirstOrDefault() * 2;
                int rHeight = (int)MyGrid.RowDefinitions.Select(c => c.ActualHeight).FirstOrDefault() * 2;
                weather.DecodePixelWidth = cWidth;
                weather.DecodePixelHeight = rHeight;
                ImageWeather.MaxWidth = cWidth * 3;
                ImageWeather.MaxHeight = rHeight * 2;
                weather.UriSource = address;

                ImageWeather.Source = weather;

            }
            catch (Exception error)
            {   
                Debug.WriteLine(error.GetType() + ": " + error.Message);
            }
            
        }
        /// <summary>
        /// Retrieves Garfield comic strips from Cloudfront.net.
        /// </summary>
        /// <param name="container">GUI element</param>
        /// <param name="deductDays">Used to retrieve old comics</param>
        private void SetComicStripImage(Image container, int deductDays = 0)
        {
            try
            {
                BitmapImage comic = new BitmapImage();
                string url = "https://" + "d1ejxu6vysztl5.cloudfront.net/comics/garfield/" + calendarHandler.GetStringDate("yyyy") + "/" + calendarHandler.GetDate().AddDays(-deductDays).ToString("yyyy-MM-dd") + ".gif";
                Uri address = new Uri(url);

                comic.DecodePixelType = DecodePixelType.Logical;
                comic.DecodePixelWidth = (int)MyGrid.ColumnDefinitions.Select(c => c.ActualWidth).FirstOrDefault();
                comic.DecodePixelHeight = (int)Math.Round(MyGrid.RowDefinitions.Select(c => c.ActualHeight).FirstOrDefault() / 2);
                comic.UriSource = address;

                container.Source = comic;

            }
            catch (Exception error)
            {
                Debug.WriteLine(error.GetType() + ": " + error.Message);
            }
        }
        /// <summary>
        /// Retrieves the meals for the week and set GUI elements.
        /// </summary>
        private void GetMealPlan()
        {
            menu = lunchPlanHandler.GetMealsForWeek(calendarHandler.GetWeekNumber());

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
        /// <summary>
        /// Sets StackPanel and its elements width based on grid size.
        /// </summary>
        public void SetMealPlanWidth()
        {
            //Simplify?
            Grid stackParent = (Grid)StackPanelMealPlan.Parent;
            StackPanelMealPlan.Width = stackParent.ColumnDefinitions.FirstOrDefault().ActualWidth * 2;
            double tBlockWidth = StackPanelMealPlan.Width / 4;
            //Monday
            TBlockMonday.Width = tBlockWidth;
            TBlockMondayMeal.MaxWidth = tBlockWidth * 3;
            TBlockMondayMeal.TextWrapping = TextWrapping.Wrap;
            //Tuesday
            TBlockTuesday.Width = tBlockWidth;
            TBlockTuesdayMeal.MaxWidth = tBlockWidth * 3;
            TBlockTuesdayMeal.TextWrapping = TextWrapping.Wrap;
            //Wednesday
            TBlockWednesday.Width = tBlockWidth;
            TBlockWednesdayMeal.MaxWidth = tBlockWidth * 3;
            TBlockWednesdayMeal.TextWrapping = TextWrapping.Wrap;
            //Thursday
            TBlockThursday.Width = tBlockWidth;
            TBlockThursdayMeal.MaxWidth = tBlockWidth * 3;
            TBlockThursdayMeal.TextWrapping = TextWrapping.Wrap;
            //Friday
            TBlockFriday.Width = tBlockWidth;
            TBlockFridayMeal.MaxWidth = tBlockWidth * 3;
            TBlockFridayMeal.TextWrapping = TextWrapping.Wrap;
        }
        /// <summary>
        /// Retrieves the module schedule from AspIT.dk and converts it into a bitmap to display in the GUI.
        /// </summary>
        private async void OpenRemoteModule()
        {
            HttpClient client = new HttpClient();
            Uri url = new Uri("http://www.aspit.dk/fileadmin/filbibliotek/KALENDER/2018-Fremad/Efteraar-18-v2.pdf");
            var stream = await client.GetStreamAsync(url);
            var memStream = new MemoryStream();
            await stream.CopyToAsync(memStream);
            memStream.Position = 0;
            PdfDocument doc = await PdfDocument.LoadFromStreamAsync(memStream.AsRandomAccessStream());

            BitmapImage bitmap = new BitmapImage();
            var page = doc.GetPage(0);

            using (Windows.Storage.Streams.InMemoryRandomAccessStream mStream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
            {
                await page.RenderToStreamAsync(mStream);
                await bitmap.SetSourceAsync(mStream);
            }

            ImageModulePlan.Source = bitmap;
        }
        /// <summary>
        /// Retrives the custom admin message from the database and sets the relevant AdminMessage element in the GUI.
        /// </summary>
        private void SetAdminMessage()
        {
            ViewAdminMessageJoin msg = messageHandler.GetNewestViewMessage();

            TBlockAdminMessageTitle.Text = msg.Header;
            TBlockAdminMessage.Text = msg.Text;
            TBlockAdminMessageDate.Text = msg.Date.ToString("dd/MM/yyyy");
            TBlockAdminMessageAuthor.Text = msg.Username;
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateUiContent();
        }
        /// <summary>
        /// Toggles visibility of GUI elements weatherforecast and module plan.
        /// </summary>
        private void WeatherAndModuleToggle()
        {
            if (ImageWeather.Visibility == Visibility.Collapsed)
            {
                ImageWeather.Visibility = Visibility.Visible;
                ImageModulePlan.Visibility = Visibility.Collapsed;
            } else
            {
                ImageWeather.Visibility = Visibility.Collapsed;
                ImageModulePlan.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// Display/cycle news from the RSS feed retrieved with [Handler].GetFeed()
        /// </summary>
        private void SetNews()
        {
            TV2NewsItem item = rSSFeedHandler.NewsList.FirstOrDefault();
            try
            {
                TBlockNewsTitle.Text = item.Title;
                TBlockNewspublishDate.Text = item.PubDate.ToString("dd/MM/yyyy");
                TBlockNewsContent.Text = item.Description;
                TBlockNewsAuthor.Text = "Af: " + item.Author;

                rSSFeedHandler.NewsList.Remove(item);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message,err.GetType().ToString());
            }
        }
        /// <summary>
        /// Changes news containers text based on current content. Does nothing with small window size
        /// </summary>
        private void NewsTextFormatting()
        {
            //Set text block elements height based on parent element
            StackPanel parent = (StackPanel)TBlockNewsContent.Parent;
            double parentHeight = parent.ActualHeight;

            if(parentHeight > 100)
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

            //Incrementally shrink main content until everything fits - no matter the size
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
        /// <summary>
        /// Resize the analogue clock based on the smallest of its width and height
        /// </summary>
        private void AnalogueClockSize()
        {
            ParentGrid.Height = MyGrid.RowDefinitions.FirstOrDefault().ActualHeight * 0.8;
            ParentGrid.Width = MyGrid.ColumnDefinitions.FirstOrDefault().ActualWidth;
            TBlockTime.Height = MyGrid.RowDefinitions.FirstOrDefault().ActualHeight * 0.20;
            TBlockTime.FontSize = MyGrid.RowDefinitions.FirstOrDefault().ActualHeight * 0.15;

            ParentGrid.UpdateLayout();
            TBlockTime.UpdateLayout();

            //Parent meassurements
            int clockParentWidth = (int)ParentGrid.ActualWidth;
            int clockParentHeight = (int)ParentGrid.ActualHeight;

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
            List<double> angles = clockHandler.GetAllRotations(calendarHandler.GetDate());
            secondHand.Angle = angles[0];
            minuteHand.Angle = angles[1];
            hourHand.Angle = angles[2];
        }
    }
}
