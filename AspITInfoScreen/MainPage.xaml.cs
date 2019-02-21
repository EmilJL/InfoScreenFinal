using System;
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
        private Model model;
        private MealHandler mealHandler;
        private MessageHandler messageHandler;
        private CalendarHandler calendarHandler;
        private RSSFeedHandler rSSFeedHandler;
        private List<ViewMealsVsLunchPlansJoin> menu;
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
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Title = calendarHandler.GetStringDate("dd/MM/yyyy") + " - Uge : " + calendarHandler.GetWeekNumber();
            model = dbHandler.Model;
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
            if (counter == 1)
            {
                counter++;
            }

            TBlockTime.Text = calendarHandler.GetStringDate("hh:mm:ss");

            if( counter % 10 == 0)
            {
                WeatherAndModuleToggle();
                if (rSSFeedHandler.NewsList.Count > 0)
                {
                    SetNews();
                    NewsTextFormatting();
                } else
                {
                    rSSFeedHandler.GetFeed();
                }
            }

            if (counter >= 300)
            {
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

        private void GetMealPlan()
        {
            menu = lunchPlanHandler.GetMealsForWeek(calendarHandler.GetWeekNumber());

            foreach (var item in menu)
            {
                //Checks against system autogenerated values
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
                throw;
            }
        }
        /// <summary>
        /// Changes news containers text based on current content
        /// </summary>
        private void NewsTextFormatting()
        {
            //Set text block elements height based on parent element
            double newsPanelHeight = StackPanelNews.ActualHeight; //Parent

            TBlockNewsTitle.MaxHeight = newsPanelHeight * 0.20; //20%
            TBlockNewsContent.MaxHeight = newsPanelHeight * 0.60; // 60%

            //Inside another stackpanel (height after parent's parent)
            TBlockNewspublishDate.MaxHeight = newsPanelHeight * 0.10; //10%
            TBlockNewsAuthor.MaxHeight = TBlockNewspublishDate.MaxHeight; //10%

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
            //Incrementally shrink author, date and title until everything fitsot minimum limit reached
            while (TBlockNewsTitle.IsTextTrimmed && TBlockNewsTitle.FontSize > 15)
            {
                //Title can't be smaller than Author or Date text element
                if (TBlockNewsTitle.FontSize - TBlockNewspublishDate.FontSize > 6)
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
    }
}
