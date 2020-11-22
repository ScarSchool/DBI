using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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

namespace Car01_Tomassetti
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataBase db;
        public MainWindow()
        {
            InitializeComponent();
            InitializeMyComponents();
        }

        private void InitializeMyComponents()
        {
            db = new DataBase();
            listCars.ItemsSource = db.GetAllCars();
        }

        private void btnAddCar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Car car = new Car(int.Parse(tbCarId.Text), tbCarName.Text);
                db.AddCar(car);
                lblMessage.Content = "car " + car + " added";
                int newId = int.Parse(tbCarId.Text) + 1;
                tbCarId.Text = newId.ToString();
            }
            catch (Exception ex)
            {
                lblMessage.Content = "Error: " + ex.Message;
            }
        }

        private void btnRmCar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Car car = new Car(int.Parse(tbCarId.Text), tbCarName.Text);
                db.RemoveCar(car);
                lblMessage.Content = "car removed";
            }
            catch (Exception ex)
            {
                lblMessage.Content = "error: " + ex.Message;
            }
        }

        private void btnUpdateCar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Car car = new Car(int.Parse(tbCarId.Text), tbCarName.Text);
                db.UpdateCar(car);
                lblMessage.Content = "car updated";
            }
            catch (Exception ex)
            {
                lblMessage.Content = "error: " + ex.Message;
            }
        }

        private void onCarSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                object o = e.AddedItems[0];
                lblMessage.Content = o;
                tbCarId.Text = ((Car)o).CarId.ToString();
                tbCarName.Text = ((Car)o).CarName;
            }
        }

        public void btnSort_Click(object sender, RoutedEventArgs e)
        {
            db.Sort();
            lblMessage.Content = "cars sorted";
        }

        private void miStoreBin_Click(object sender, RoutedEventArgs e)
        {
            db.Store();
            lblMessage.Content = "cars stored";
        }

        private void miLoadBin_Click(object sender, RoutedEventArgs e)
        {
            db.Load();
            lblMessage.Content = "cars loaded";
        }
    }
}
