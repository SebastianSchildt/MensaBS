using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MensaBS.Resources;
using Microsoft.Xna.Framework.GamerServices;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace MensaBS
{
    public partial class MainPage : PhoneApplicationPage
    {

        Int16 load;

        Int16 row;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("No network connection available!");
                return;
            }


            // start loading XML-data
           

            this.Loaded += appReady;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void appReady(object src, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;
            mainGuiThingie.SelectedIndex = (int)now.DayOfWeek;

            load_kath(null,null);
        }

        private void load_data(string mensa)
        {
            load = 6;
            SystemTray.IsVisible = true;
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = "Searching for food...";
            WebClient downloader = new WebClient();
            Uri uri = new Uri("http://trivalg.de/mensa/"+mensa+"/mo.xml", UriKind.Absolute);
            downloader.DownloadStringCompleted += (sender, eventArgs) =>
            {
                downloadFinished(sender, eventArgs, MoStack);
            };
            downloader.DownloadStringAsync(uri);

            uri = new Uri("http://trivalg.de/mensa/" + mensa + "/di.xml", UriKind.Absolute);
            downloader = new WebClient();
            downloader.DownloadStringCompleted += (sender, eventArgs) =>
            {
                downloadFinished(sender, eventArgs, DiStack);
            };
            downloader.DownloadStringAsync(uri);


            uri = new Uri("http://trivalg.de/mensa/" + mensa + "/mi.xml", UriKind.Absolute);
            downloader = new WebClient();
            downloader.DownloadStringCompleted += (sender, eventArgs) =>
            {
                downloadFinished(sender, eventArgs, MiStack);
            };
            downloader.DownloadStringAsync(uri);



            uri = new Uri("http://trivalg.de/mensa/" + mensa + "/do.xml", UriKind.Absolute);
            downloader = new WebClient();
            downloader.DownloadStringCompleted += (sender, eventArgs) =>
            {
                downloadFinished(sender, eventArgs, DoStack);
            };
            downloader.DownloadStringAsync(uri);



            uri = new Uri("http://trivalg.de/mensa/" + mensa + "/fr.xml", UriKind.Absolute);
            downloader = new WebClient();
            downloader.DownloadStringCompleted += (sender, eventArgs) =>
            {
                downloadFinished(sender, eventArgs, FriStack);
            };
            downloader.DownloadStringAsync(uri);


            uri = new Uri("http://trivalg.de/mensa/" + mensa + "/sa.xml", UriKind.Absolute);
            downloader = new WebClient();
            downloader.DownloadStringCompleted += (sender, eventArgs) =>
            {
                downloadFinished(sender, eventArgs, SatStack);
            };
            downloader.DownloadStringAsync(uri);
        }


        private void downloadFinished(object sender, DownloadStringCompletedEventArgs e, StackPanel day)
        {
            this.load--;
            if (load == 0)
                SystemTray.ProgressIndicator.IsVisible = false;

            //http://stackoverflow.com/questions/16371884/cant-download-json-array-windows-phone
            //it seems on error e can be corrupted?
            try
            {
                if (e.Result == null || e.Error != null)
                {
                    MessageBox.Show("There was an error downloading the XML-file!");
                    return;
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show("There was an error downloading food!");
                return;
            }


            // Deserialize if download succeeds
            XmlSerializer serializer = new XmlSerializer(typeof(TrivalgFeed));
            XDocument document = XDocument.Parse(e.Result);
            // get all the employees
            TrivalgFeed feed = (TrivalgFeed)serializer.Deserialize(document.CreateReader());

            day.Children.Clear();

            //Luch header
            Border headerPanel = new Border();
            headerPanel.Background = new System.Windows.Media.SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]);
            TextBlock header = new TextBlock();
            header.FontSize = (Double)Application.Current.Resources["PhoneFontSizeLarge"];
            header.FontWeight = System.Windows.FontWeights.Bold;
            header.Margin = new Thickness(5);
            header.Text = "Mittag";
            headerPanel.Child=header;
            day.Children.Add(headerPanel);


            addMeals(feed.tag.lunch.meals, day);

            //Dinner header
            headerPanel = new Border();
            headerPanel.Background = new System.Windows.Media.SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]);
            header = new TextBlock();
            header.FontSize = (Double)Application.Current.Resources["PhoneFontSizeLarge"];
            header.FontWeight = System.Windows.FontWeights.Bold;
            header.Margin = new Thickness(5);
            header.Text = "Abendessen";
            headerPanel.Child = header;
            day.Children.Add(headerPanel);

            addMeals(feed.tag.dinner.meals, day);

          
            // bind data to ListBox
            //                employeesList.ItemsSource = employees.Collection;

        }

        private void addMeals(ObservableCollection<Meal> meals, StackPanel target)
        {

            foreach (Meal m in meals)
            {
                TextBlock title = new TextBlock();
                title.Text = m.name;
                title.TextWrapping = System.Windows.TextWrapping.Wrap;
                title.FontSize = (Double)Application.Current.Resources["PhoneFontSizeLarge"];
                title.FontWeight = System.Windows.FontWeights.Bold;

                TextBlock descr = new TextBlock();
                descr.Text = "Zusatz: " + m.poison;

                TextBlock price = new TextBlock();
                foreach (Meal.price p in m.prices)
                {
                    price.Text += p.type + ": " + p.amount + "  ";
                }

                target.Children.Add(title);
                target.Children.Add(descr);
                target.Children.Add(price);
            }

            if (meals.Count == 0)
            {
                TextBlock title = new TextBlock();
                title.Text = "Kein Angebot!";
                title.TextWrapping = System.Windows.TextWrapping.Wrap;
                title.FontSize = (Double)Application.Current.Resources["PhoneFontSizeLarge"];
                title.Foreground = new SolidColorBrush(Colors.Red);
                title.FontWeight = System.Windows.FontWeights.Bold;
                target.Children.Add(title);
            }
        }

        private void MensaButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> MBOPTIONS = new List<string>();
            MBOPTIONS.Add("OK");
            string msg = "Text that was typed on the keyboard will be displayed here.\nClick OK to continue...";
            Guide.BeginShowMessageBox(
                    "title", msg, MBOPTIONS, 0,
                    MessageBoxIcon.Alert, null, null);
        }

        private void load_hbk(object sender, EventArgs e)
        {
            mainGuiThingie.Title = "MensaBS - HBK";
            load_data("hbk");
        }

        private void load_kath(object sender, EventArgs e)
        {
            mainGuiThingie.Title = "MensaBS - Katharinenstraße";
            load_data("kath");
        }

        private void load_360(object sender, EventArgs e)
        {
            mainGuiThingie.Title = "MensaBS - 360°";
            load_data("360");
        }

        private void load_bh(object sender, EventArgs e)
        {
            mainGuiThingie.Title = "MensaBS - Beethovenstraße";
            load_data("beeth");
        }





        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}