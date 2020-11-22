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
using Microsoft.Win32;
using Misc;
using PkgData;

namespace Car03
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Database db;


        public MainWindow()
        {
            InitializeComponent();

            InitializeMyComponents();
        }

        private void InitializeMyComponents()
        {
            db = Database.instance;
            cbCarType.ItemsSource = Enum.GetValues(typeof(CarType));
            cbCarType.SelectedItem = CarType.Other;
            lvCars.ItemsSource = db.GetAllCars();
            dgCars.ItemsSource = db.GetAllCars();
            Car.CollListenerMethods += this.OnDBChanged;
        }

        private void OnDBChanged(object eventsource, EventArgs cea)
        {
            if(cea is CarEventArgs)
            {
                dgCars.CancelEdit();
                lvCars.Items.Refresh();
                dgCars.Items.Refresh();
                lblMessage.Content = (cea as CarEventArgs).Car.ToString() + " " + (cea as CarEventArgs).Message;
                tbCarName.Text = ((cea as CarEventArgs).Car as Car).CarName; // trully beautiful
            }
        }

        private void OnCarButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender.Equals(btnAdd))
                {
                    Car car = new Car(int.Parse(tbCarId.Text), tbCarName.Text, (CarType)cbCarType.SelectedItem);
                    db.AddCar(car);
                    lblMessage.Content = "car " + car + " added";
                } else if (sender.Equals(btnDel))
                {
                    Car car = new Car(int.Parse(tbCarId.Text), tbCarName.Text, (CarType)cbCarType.SelectedItem);
                    db.DelCar(car);
                    lblMessage.Content = "car " + car + " deleted";
                } else if (sender.Equals(btnUpdate))
                {
                    Car car = new Car(int.Parse(tbCarId.Text), tbCarName.Text, (CarType)cbCarType.SelectedItem);
                    db.UpdateCar(car);
                    lblMessage.Content = "car " + car + " updated";
                } else if (sender.Equals(btnSort))
                {
                    db.Sort();
                    lblMessage.Content = "cars sorted";
                } else if (sender.Equals(btnStore))
                {
                    db.Store();
                    lblMessage.Content = "cars stored";
                } else if (sender.Equals(btnLoad))
                {
                    db.Load();
                    lblMessage.Content = "cars loaded";
                }
            }
            catch(Exception ex)
            {
                lblMessage.Content = "error: " + ex.Message;
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void OnCarSelected(object sender, SelectionChangedEventArgs e)
        {
            Car c = null;
            if (sender.Equals(dgCars))
            {
                c = dgCars.SelectedItem as Car;
            } else if (sender.Equals(lvCars))
            {
                c = lvCars.SelectedItem as Car;
            }
            if (c != null)
            {
                tbCarId.Text = c.CarId.ToString();
                tbCarName.Text = c.CarName;
                cbCarType.SelectedItem = c.Type;
                lblMessage.Content = "car selected: " + c;
            }

        }

        private void OnSelectFileMenu(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender.Equals(miStore))
                {
                    db.Store();
                    lblMessage.Content = "cars stored";
                } else if (sender.Equals(miLoad))
                {
                    db.Load();
                    lblMessage.Content = "cars loaded";
                } else if (sender.Equals(miStoreXML))
                {
                    if (tbFileDialog.Text == "" || tbFileDialog.Text == "...")
                    {
                        db.StoreXML();
                    }
                    else
                    {
                        db.StoreXML(tbFileDialog.Text);
                    }
                    db.StoreXML();
                    lblMessage.Content = "cars stored xmly";
                } else if (sender.Equals(miLoadXML))
                {
                    if (tbFileDialog.Text == "" || tbFileDialog.Text == "...")
                    {
                        db.LoadXML();
                    }
                    else
                    {
                        if (tbFileDialog.Text.Contains(".jpg"))
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(tbFileDialog.Text, UriKind.Absolute);
                            bitmap.EndInit();
                            imgCar.Source = bitmap;
                        }
                        else
                        {
                            db.LoadXML(tbFileDialog.Text);
                        }
                    }
                    lblMessage.Content = "cars loaded xmly";
                }
            }
            catch(Exception ex)
            {
                lblMessage.Content = "error: " + ex.Message;
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void BtnFileDialog_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "txt files (*.txt)|*.txt| xml files|*.xml|jpg files|*.jpg";
            if (ofd.ShowDialog() == true)
            {
                tbFileDialog.Text = ofd.FileName;
            }
        }
    }
}
