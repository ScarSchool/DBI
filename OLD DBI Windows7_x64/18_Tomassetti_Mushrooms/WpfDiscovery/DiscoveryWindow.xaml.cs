using pkgDatabase;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfDiscovery
{
    /// <summary>
    /// Interaction logic for DiscoveryWindow.xaml
    /// </summary>
    public partial class DiscoveryWindow : Window
    {
        CultureInfo cultureInfo = new CultureInfo("en-US");

        private static readonly int WIDTH = 1167;
        private static readonly int HEIGHT = 543;
        private static readonly double MIN_LONGITUDE = 13.5404;
        private static readonly double MAX_LONGITUDE = 13.6773;
        private static readonly double MIN_LATITUDE = 46.7113;
        private static readonly double MAX_LATITUDE = 46.7534;

        private Database db = Database.Instance;
        private bool discoveriesVisible = true;
        private bool distancesVisible = false;
        private bool showNeighbours = false;
        private bool addNote = false;

        private Rectangle rectSelectArea = null;
        private Point startPoint;
        private List<Rectangle> rectSelectAreas = new List<Rectangle>();

        public DiscoveryWindow()
        {
            InitializeComponent();
            imgMap.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/../../../../stockenboi.png", UriKind.Absolute));
            cbMushrooms.ItemsSource = db.Mushrooms;
            GenerateMarkers(true);
        }


        private void OnLeftMouseButton(object sender, MouseButtonEventArgs e)
        {
            if (addNote)
            {
                OnAddNote(sender, e);
            }
            else
            {
                OnAddDiscovery(sender, e);
            }
        }

        private void OnAddDiscovery(Object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (cbMushrooms.SelectedItem == null || dpDateDiscovery.SelectedDate == null)
                    throw new Exception("No mushroom or date selected.");

                Point position = e.GetPosition(imgMap);
                double longitude = GetLongitude(position.X);
                double latitude = GetLatitude(position.Y);

                Discovery discovery = new Discovery
                {
                    Mushroom = cbMushrooms.SelectedItem as Mushroom,
                    Date = dpDateDiscovery.SelectedDate,
                    X = (int)position.X,
                    Y = (int)position.Y,
                    Longitude = longitude,
                    Latitude = latitude
                };

                db.Add(discovery);
                GenerateMarker(discovery);
                Log($"Discovery of {discovery.Mushroom} at {discovery.Latitude}, {discovery.Longitude} on {discovery.Date?.ToShortDateString()} added!");
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void OnAddNote(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(canvasMap);

            Random r = new Random(DateTime.Now.Millisecond);
            byte[] random = new byte[3];
            r.NextBytes(random);
            SolidColorBrush Background = new SolidColorBrush(Color.FromRgb(random[0], random[1], random[2]));
            Background.Opacity = 0.1;

            rectSelectArea = new Rectangle
            {
                Stroke = Brushes.LightBlue,
                StrokeThickness = 2,
                Fill = Background
            };
            

            Canvas.SetLeft(rectSelectArea, startPoint.X);
            Canvas.SetTop(rectSelectArea, startPoint.X);
            Canvas.SetZIndex(rectSelectArea, 500);
            canvasMap.Children.Add(rectSelectArea);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Released && rectSelectArea != null)
                {
                    rectSelectAreas.Add(rectSelectArea);
                    if (rectSelectAreas.Count > 1)
                    {
                        string r1 = getPolygonString(rectSelectArea);
                        int i = 0;
                        bool collided = false;
                        while (i < rectSelectAreas.Count && !collided)
                        {
                            string r2 = getPolygonString(rectSelectAreas[i]);
                            collided = db.IfSelectRectCollide(r1, r2);
                            i++;
                        }
                        if (collided)
                        {
                            canvasMap.Children.Remove(rectSelectArea);
                            rectSelectAreas.Remove(rectSelectArea);
                        }
                        else
                        {
                            drawTextBox();
                        }
                    }
                    else
                    {
                        drawTextBox();
                    }
                    rectSelectArea = null;
                }
                else if (rectSelectArea != null)
                {
                    drawRect(e);
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }
            
        }

        private void drawTextBox()
        {
            TextBox textBox = new TextBox();
            textBox.Width = rectSelectArea.ActualWidth;
            textBox.Height = 25;
            Canvas.SetLeft(textBox, Canvas.GetLeft(rectSelectArea) + (rectSelectArea.ActualWidth / 2 - textBox.Width / 2));
            Canvas.SetTop(textBox, Canvas.GetTop(rectSelectArea) - (textBox.ActualHeight + 20));
            Canvas.SetZIndex(textBox, 500);
            canvasMap.Children.Add(textBox);
            textBox.Focus();
        }

        private void drawRect(MouseEventArgs e)
        {
            var pos = e.GetPosition(canvasMap);

            // Set the position of rectangle
            var x = Math.Min(pos.X, startPoint.X);
            var y = Math.Min(pos.Y, startPoint.Y);

            // Set the dimenssion of the rectangle
            var w = Math.Max(pos.X, startPoint.X) - x;
            var h = Math.Max(pos.Y, startPoint.Y) - y;

            rectSelectArea.Width = w;
            rectSelectArea.Height = h;

            Canvas.SetLeft(rectSelectArea, x);
            Canvas.SetTop(rectSelectArea, y);
        }

        private string getPolygonString(Rectangle r)
        {
            string left, right, top, bottom;
            left = GetLongitude(Canvas.GetLeft(r)).ToString(cultureInfo);
            right = GetLongitude(Canvas.GetLeft(r) + r.ActualWidth).ToString(cultureInfo);
            top = GetLatitude(Canvas.GetTop(r)).ToString(cultureInfo);
            bottom = GetLatitude(Canvas.GetTop(r) - r.ActualHeight).ToString(cultureInfo);
            return $"POLYGON (({left} {top}, {right} {top}, {left} {bottom}, {right} {bottom}, {left} {top}))";
        }
        

        public void OnMoveHunter(Object sender, MouseButtonEventArgs e)
        {
            try
            {
                Point position = e.GetPosition(imgMap);
                double longitude = GetLongitude(position.X);
                double latitude = GetLatitude(position.Y);

                Canvas.SetTop(markerHunter, (int)position.Y - (markerHunter.Height / 2));
                Canvas.SetLeft(markerHunter, (int)position.X - (markerHunter.Width / 2));
                markerHunter.ToolTip = $"{latitude}, {longitude}";
                Canvas.SetZIndex(markerHunter, 10);

                if (showNeighbours)
                {
                    HideDiscoveries();
                    db.GetNeighbours(longitude, latitude, (int)AmountSlider.Value);
                    GenerateMarkers(false);
                    ShowDiscoveries();
                }
                else
                {
                    db.CalculateDistances(latitude, longitude);
                }

                ShowDistances();


                Log($"Hunter at {latitude}, {longitude}!");
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        public void OnMenuItemClicked(Object sender, EventArgs e)
        {
            try
            {
                if (sender.Equals(miShowNeighbours))
                {
                    if (!showNeighbours)
                    {
                        AmountLabel.Visibility = Visibility.Visible;
                        AmountSlider.Visibility = Visibility.Visible;
                        AmountLabelCount.Visibility = Visibility.Visible;

                        showNeighbours = true;
                    }

                    AmountLabelCount.Content = AmountSlider.Value;
                    ShowDistances();
                }
                else if (sender.Equals(miAddNote))
                {
                    addNote = true;
                }
                else
                {
                    if (showNeighbours)
                    {
                        showNeighbours = false;

                        AmountLabel.Visibility = Visibility.Hidden;
                        AmountSlider.Visibility = Visibility.Hidden;
                        AmountLabelCount.Visibility = Visibility.Hidden;
                    }

                    if (addNote)
                    {
                        addNote = false;
                    }

                    if (sender.Equals(miShowDiscoveries))
                    {
                        ShowDistances();
                    }
                    else if (sender.Equals(miHideDiscoveries))
                    {
                        HideDiscoveries();
                    }
                    else if (sender.Equals(miShowDistance))
                    {
                        distancesVisible = true;
                    }
                    
                }

            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void HideDiscoveries()
        {
            discoveriesVisible = false;
            foreach (UIElement child in canvasMap.Children)
            {
                if (child is Ellipse)
                {
                    Canvas.SetZIndex(child, -10);
                }
            }
        }

        private void ShowDiscoveries()
        {
            discoveriesVisible = true;
            foreach (UIElement child in canvasMap.Children)
            {
                if (child is Ellipse)
                {
                    Canvas.SetZIndex(child, 10);
                }
            }
        }

        private void GenerateMarkers(bool refresh)
        {
            try
            {
                if (refresh)
                {
                    db.RefreshDiscoveries();
                }

                foreach (Discovery discovery in db.Discoveries)
                {
                    GenerateMarker(discovery);
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void ShowDistances()
        {
            canvasMap.Children.Clear();
            canvasMap.Children.Add(imgMap);
            canvasMap.Children.Add(markerHunter);

            foreach (Discovery discovery in db.Discoveries)
            {
                GenerateMarker(discovery);
            }
        }

        private void GenerateMarker(Discovery discovery)
        {
            try
            {
                Ellipse marker = new Ellipse
                {
                    Name = "Discovery" + discovery.Id.ToString(),
                    Height = 12,
                    Width = 12,
                    Stroke = Brushes.Purple,
                    StrokeThickness = 2,
                    ToolTip = discovery.ToString(distancesVisible),
                    Fill = Brushes.GhostWhite
                };

                canvasMap.Children.Add(marker);
                Canvas.SetTop(marker, discovery.Y - (marker.Height / 2));
                Canvas.SetLeft(marker, discovery.X - (marker.Width / 2));
                Panel.SetZIndex(marker, (discoveriesVisible) ? 10 : -10);
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private double GetLongitude(double x)
        {
            return MIN_LONGITUDE + (x / WIDTH) * (MAX_LONGITUDE - MIN_LONGITUDE);
        }

        private double GetLatitude(double y)
        {
            return MAX_LATITUDE - (y / HEIGHT) * (MAX_LATITUDE - MIN_LATITUDE);
        }

        private void Log(String msg)
        {
            lblStatus.Foreground = Brushes.White;
            lblStatus.Content = msg;
        }

        private void Log(Exception ex)
        {
            lblStatus.Foreground = Brushes.PaleVioletRed;
            lblStatus.Content = $"ERROR {ex.Message}";

            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }

        private void AmountSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AmountLabelCount.Content = (int)AmountSlider.Value;
        }

    }
}
