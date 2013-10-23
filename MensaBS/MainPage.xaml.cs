﻿using System;
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
using System.IO.IsolatedStorage;

namespace MensaBS
{
    public partial class MainPage : PhoneApplicationPage
    {

        Int16 load;

       
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
            mainGuiThingie.SelectedIndex = ((int)now.DayOfWeek-1)%7;

            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains("mensa"))
            {
                settings.Add("mensa",0);
            }

            switch ( (int)settings["mensa"] )
            {
                case 0:
                    load_kath(null, null);
                    break;
                case 1:
                    load_360(null, null);
                    break;
                case 2:
                    load_bh(null, null);
                    break;
                case 3:
                    load_hbk(null, null);
                    break;
                default:
                    load_kath(null, null);
                    settings["mensa"] = 0;
                    break;
            }
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
            catch (Exception )
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


        private void load_kath(object sender, EventArgs e)
        {
            mainGuiThingie.Title = "MensaBS - Katharinenstraße";
            IsolatedStorageSettings.ApplicationSettings["mensa"] = 0;
            load_data("kath");
        }

        private void load_360(object sender, EventArgs e)
        {
            mainGuiThingie.Title = "MensaBS - 360°";
            IsolatedStorageSettings.ApplicationSettings["mensa"] = 1;
            load_data("360");
        }

        private void load_bh(object sender, EventArgs e)
        {
            mainGuiThingie.Title = "MensaBS - Beethovenstraße";
            IsolatedStorageSettings.ApplicationSettings["mensa"] = 2;
            load_data("beeth");
        }

        private void load_hbk(object sender, EventArgs e)
        {
            mainGuiThingie.Title = "MensaBS - HBK";
            IsolatedStorageSettings.ApplicationSettings["mensa"] = 3;
            load_data("hbk");
        }

        private void About_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void Adds_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ZusatzPage.xaml", UriKind.Relative));
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