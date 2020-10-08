using pkgDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfDiscovery
{
    /// <summary>
    /// Interaction logic for DiscoveryWindow.xaml
    /// </summary>
    public partial class DiscoveryWindow : Window
    {
        private static readonly int WIDTH = 1167;
        private static readonly int HEIGHT = 543;
        private static readonly double MIN_LONGITUDE = 13.5404;
        private static readonly double MAX_LONGITUDE = 13.6773;
        private static readonly double MIN_LATITUDE = 46.7113;
        private static readonly double MAX_LATITUDE = 46.7534;

        private Database db = Database.Instance;
        private bool showDiscoveries = false;
        private bool showDistances = false;
        private bool showNeighbors = false;

        private double hunterLat;
        private double hunterLong;

        public DiscoveryWindow()
        {
            InitializeComponent();
            imgMap.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/../../../../stockenboi.png", UriKind.Absolute));
            cbMushrooms.ItemsSource = db.Mushrooms;
            generate_markers();
        }

        public void add_discovery(Object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (cbMushrooms.SelectedItem == null || dpDateDiscovery.SelectedDate == null)
                    throw new Exception("Select a mushroom and date first.");

                Point position = e.GetPosition(imgMap);
                double longitude = getLong(position.X);
                double latitude = getLat(position.Y);

                Discovery discovery = new Discovery();
                discovery.Mushroom = cbMushrooms.SelectedItem as Mushroom;
                discovery.Date = dpDateDiscovery.SelectedDate;
                discovery.X = (int)position.X;
                discovery.Y = (int)position.Y;
                discovery.Longitude = longitude;
                discovery.Latitude = latitude;

                db.Add(discovery);
                generate_marker(discovery);

                if (showNeighbors)
                {
                    show_neighbors(null, null);
                }
                else if (showDistances)
                {
                    show_distances();
                }

                log($"Discovery of {discovery.Mushroom} at {discovery.Latitude}, {discovery.Longitude} on {discovery.Date?.ToShortDateString()} added!");
            }
            catch (Exception ex)
            {
                log(ex);
            }
            
        }

        public void move_hunter(Object sender, MouseButtonEventArgs e)
        {
            try
            {
                Point position = e.GetPosition(imgMap);
                hunterLong = getLong(position.X);
                hunterLat = getLat(position.Y);


                Canvas.SetTop(markerHunter, (int)position.Y - (markerHunter.Height / 2));
                Canvas.SetLeft(markerHunter, (int)position.X - (markerHunter.Width / 2));
                markerHunter.ToolTip = $"{hunterLat}, {hunterLong}";
                Canvas.SetZIndex(markerHunter, 10);

                db.CalculateDistances(hunterLat, hunterLong);

                if (showNeighbors)
                {
                    show_neighbors(null, null);
                }
                else if (showDistances)
                {
                    show_distances();
                }

                log($"Hunter at {hunterLat}, {hunterLong}!");
            }
            catch(Exception ex)
            {
                log(ex);
            }
        }

        public void show_neighbors(Object sender, EventArgs e)
        {
            try
            {
                if (markerHunter != null && Canvas.GetZIndex(markerHunter) == 10)
                {
                    showDiscoveries = true;
                    showDistances = true;
                    showNeighbors = true;

                    int amount = (int)sliderNeighbors.Value;
                    labelNeighbors.Content = $"Neighbors: {amount}";

                    List<Discovery> neighbors = db.GetNeighbors(hunterLat, hunterLong, amount);

                    canvasMap.Children.Clear();
                    canvasMap.Children.Add(imgMap);
                    canvasMap.Children.Add(markerHunter);

                    foreach (Discovery discovery in neighbors)
                    {
                        generate_marker(discovery);
                    }
                }
            }
            catch (Exception ex)
            {
                log(ex);
            }
        }

        public void menuItem_clicked(Object sender, EventArgs e)
        {
            try
            {
                if (sender.Equals(miShowDiscoveries))
                {
                    if (showNeighbors)
                    {
                        showNeighbors = false;
                        generate_markers();
                    }
                    else if (!showDiscoveries)
                    {
                        showDiscoveries = true;
                        foreach (UIElement child in canvasMap.Children)
                        {
                            if (child is Ellipse)
                            {
                                Canvas.SetZIndex(child, 10);
                            }
                        }
                    }
                }
                else if (sender.Equals(miHideDiscoveries))
                {
                    showDiscoveries = false;
                    foreach (UIElement child in canvasMap.Children)
                    {
                        if (child is Ellipse)
                        {
                            Canvas.SetZIndex(child, -10);
                        }
                    }
                }
                else if (sender.Equals(miShowDistance))
                {
                    if (markerHunter != null && Canvas.GetZIndex(markerHunter) == 10)
                    {
                        showDistances = true;
                        show_distances();
                    }
                }
            }
            catch (Exception ex)
            {
                log(ex);
            }
        }

        private void generate_markers()
        {
            try
            {
                db.RefreshDiscoveries();

                foreach (Discovery discovery in db.Discoveries)
                {
                    generate_marker(discovery);
                }
            }
            catch (Exception ex)
            {
                log(ex);
            }
        }

        private void show_distances()
        {
            canvasMap.Children.Clear();
            canvasMap.Children.Add(imgMap);
            canvasMap.Children.Add(markerHunter);

            db.CalculateDistances(hunterLat, hunterLong);

            foreach (Discovery discovery in db.Discoveries)
            {
                generate_marker(discovery);
            }
        }

        private void generate_marker(Discovery discovery)
        {
            try
            {
                Ellipse marker = new Ellipse();
                marker.Height = 12;
                marker.Width = 12;
                marker.Stroke = Brushes.Purple;
                marker.StrokeThickness = 3;
                marker.ToolTip = discovery.ToString(showDistances);

                canvasMap.Children.Add(marker);
                Canvas.SetTop(marker, discovery.Y - (marker.Height / 2));
                Canvas.SetLeft(marker, discovery.X - (marker.Width / 2));
                Canvas.SetZIndex(marker, (showDiscoveries) ? 10 : -10);
            }
            catch (Exception ex)
            {
                log(ex);
            }
        }

        private double getLong(double x)
        {
            return MIN_LONGITUDE + (x / WIDTH) * (MAX_LONGITUDE - MIN_LONGITUDE);
        }

        private double getLat(double y)
        {
            return MAX_LATITUDE - (y / HEIGHT) * (MAX_LATITUDE - MIN_LATITUDE);
        }

        private void log(String msg)
        {
            lblStatus.Foreground = Brushes.DarkGreen;
            lblStatus.Content = msg;
        }

        private void log(Exception ex)
        {
            lblStatus.Foreground = Brushes.DarkRed;
            lblStatus.Content = $"ERROR {ex.Message}";

            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}
