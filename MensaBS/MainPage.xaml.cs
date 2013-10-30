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

        private bool DLerror;

        private dataDay monday=new dataDay();
        private dataDay tuesday = new dataDay();
        private dataDay wednesday = new dataDay();
        private dataDay thursday = new dataDay();
        private dataDay friday = new dataDay();
        private dataDay saturday = new dataDay();

       
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("No network connection!");
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
            int view = ((int)(now.DayOfWeek)-1);
            if (view == -1)
                view = 0; //On Sunday show monday
            mainGuiThingie.SelectedIndex = view;

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
            DLerror = false;
            SystemTray.IsVisible = true;
            SystemTray.ProgressIndicator = new ProgressIndicator();
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = "Searching for food...";
            WebClient downloader = new WebClient();
            Uri uri = new Uri("http://trivalg.de/mensa/"+mensa+"/mo.xml", UriKind.Absolute);
            downloader.DownloadStringCompleted += (sender, eventArgs) =>
            {
                downloadFinished(sender, eventArgs, monday);
            };
            downloader.DownloadStringAsync(uri);

            uri = new Uri("http://trivalg.de/mensa/" + mensa + "/di.xml", UriKind.Absolute);
            downloader = new WebClient();
            downloader.DownloadStringCompleted += (sender, eventArgs) =>
            {
                downloadFinished(sender, eventArgs, tuesday);
            };
            downloader.DownloadStringAsync(uri);


            uri = new Uri("http://trivalg.de/mensa/" + mensa + "/mi.xml", UriKind.Absolute);
            downloader = new WebClient();
            downloader.DownloadStringCompleted += (sender, eventArgs) =>
            {
                downloadFinished(sender, eventArgs, wednesday);
            };
            downloader.DownloadStringAsync(uri);



            uri = new Uri("http://trivalg.de/mensa/" + mensa + "/do.xml", UriKind.Absolute);
            downloader = new WebClient();
            downloader.DownloadStringCompleted += (sender, eventArgs) =>
            {
                downloadFinished(sender, eventArgs, thursday);
            };
            downloader.DownloadStringAsync(uri);



            uri = new Uri("http://trivalg.de/mensa/" + mensa + "/fr.xml", UriKind.Absolute);
            downloader = new WebClient();
            downloader.DownloadStringCompleted += (sender, eventArgs) =>
            {
                downloadFinished(sender, eventArgs, friday);
            };
            downloader.DownloadStringAsync(uri);


            uri = new Uri("http://trivalg.de/mensa/" + mensa + "/sa.xml", UriKind.Absolute);
            downloader = new WebClient();
            downloader.DownloadStringCompleted += (sender, eventArgs) =>
            {
                downloadFinished(sender, eventArgs, saturday);
            };
            downloader.DownloadStringAsync(uri);
        }


        private void downloadFinished(object sender, DownloadStringCompletedEventArgs e, dataDay day)
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
                    if (!DLerror)
                    {
                        MessageBox.Show("There was an error downloading the XML-file!");
                        DLerror = true;
                    }
                    return;
                }
            }
            catch (Exception )
            {
                if (!DLerror)
                {
                    MessageBox.Show("There was an error downloading food!");
                    DLerror = true;
                }
                return;
            }

            
            StackPanel sp=SatStack;
            if (day == monday)
                sp = MoStack;
            else if (day == tuesday)
                sp = DiStack;
            else if (day == wednesday)
                sp = MiStack;
            else if (day == thursday)
                sp = DoStack;
            else if (day == friday)
                sp = FriStack;

                      
            sp.Children.Clear();


            try
            {
                XDocument document = XDocument.Parse(e.Result);
                day.parse(document);
            }
            catch (Exception parserEx)
            {
                TextBlock err = new TextBlock();
                err.Text = "Yikes! I just heard this creaking noise and then a loud bang. I think something just exploded :-( Smells funny too....";
                err.TextWrapping = System.Windows.TextWrapping.Wrap;
                err.FontSize = (Double)Application.Current.Resources["PhoneFontSizeLarge"];
                err.FontWeight = System.Windows.FontWeights.Bold;

                TextBlock descr = new TextBlock();
                descr.Text = parserEx.Message+"\nThis may be a problem with the datasource. You can try the oldschool way using your browser:" ;
                descr.TextWrapping = System.Windows.TextWrapping.Wrap;

                HyperlinkButton mensaLink = new HyperlinkButton();
                mensaLink.TargetName = "_blank";
                mensaLink.NavigateUri = (new Uri("http://www.stw-on.de/braunschweig/essen"));
                mensaLink.Content = "Mensaplan online";
                mensaLink.Margin = new Thickness(0,10,0,10);

                TextBlock toGithub = new TextBlock();
                toGithub.Text = "If you think this is a bug in MensaBS you can report the issue here:";
                toGithub.TextWrapping = System.Windows.TextWrapping.Wrap;

                HyperlinkButton githubLink = new HyperlinkButton();
                githubLink.TargetName = "_blank";
                githubLink.NavigateUri = (new Uri("https://github.com/SebastianSchildt/MensaBS/issues"));
                githubLink.Content = "Issue Tracker";
                githubLink.Margin = new Thickness(0, 10, 0, 10);
                
                sp.Children.Add(err);
                sp.Children.Add(descr);
                sp.Children.Add(mensaLink);
                sp.Children.Add(toGithub); sp.Children.Add(githubLink);
                return;
            }


            //Luch header
            Border headerPanel = new Border();
            headerPanel.Background = new System.Windows.Media.SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]);
            TextBlock header = new TextBlock();
            header.FontSize = (Double)Application.Current.Resources["PhoneFontSizeLarge"];
            header.FontWeight = System.Windows.FontWeights.Bold;
            header.Margin = new Thickness(5);
            header.Text = "Mittag";
            headerPanel.Child=header;
            sp.Children.Add(headerPanel);


            addMeals(day.lunch, sp);

            //Dinner header
            headerPanel = new Border();
            headerPanel.Background = new System.Windows.Media.SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]);
            header = new TextBlock();
            header.FontSize = (Double)Application.Current.Resources["PhoneFontSizeLarge"];
            header.FontWeight = System.Windows.FontWeights.Bold;
            header.Margin = new Thickness(5);
            header.Text = "Abendessen";
            headerPanel.Child = header;
            sp.Children.Add(headerPanel);

            addMeals(day.dinner, sp);
        }

        private void addMeals(List<dataMeal> meals, StackPanel target)
        {

            foreach (dataMeal m in meals)
            {
                TextBlock title = new TextBlock();
                title.Text = m.name;
                title.TextWrapping = System.Windows.TextWrapping.Wrap;
                title.FontSize = (Double)Application.Current.Resources["PhoneFontSizeLarge"];
                title.FontWeight = System.Windows.FontWeights.Bold;

                TextBlock descr = new TextBlock();
                descr.Text = "Zusatz: " + m.poison;

                TextBlock price = new TextBlock();
                price.Text = "Preis: " + m.priceStudent + " / " + m.priceEmployee + " / " + m.priceGuest;

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

    }
}