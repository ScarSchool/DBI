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
using PkgData;
using PkgMISC;

namespace Car04
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Database db;
        public MainWindow()
        {
            InitializeComponent(); //do not (re)move
            InitializeMyComponents();
        }

        private void InitializeMyComponents()
        {
            db = Database.Instance;
            Car.CollListenerMethods += OnDbChanged;
            cmbCarType.ItemsSource = Enum.GetValues(typeof(Car.CarType));
            listCars.ItemsSource = db.GetAllCars();
            dgCars.ItemsSource = db.GetAllCars();
            dgOwners.ItemsSource = db.GetAllOwners();
        }
  

        public void OnDbChanged(object item, EventArgs ea)
        {
            if (ea is EventArgsCar)
            {
                lblMessage.Content = ((EventArgsCar)ea).Car.ToString() + " " + ((EventArgsCar)ea).Message;
                txtCarName.Text = ((Car)((EventArgsCar)ea).Car).CarName;
                dgCars.CancelEdit();
                listCars.Items.Refresh();
            }
        }
        private void OnButtonCarClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender.Equals(btnAddCar))
                {
                    Car car = new Car(int.Parse(txtCarId.Text), txtCarName.Text, (Car.CarType)cmbCarType.SelectedItem, int.Parse(txtCarPrice.Text));
                    db.AddCar(car);
                    lblMessage.Content = "Added Car: " + car;
                }
                else if (sender.Equals(btnDeleteCar))
                {
                    Car car = new Car(int.Parse(txtCarId.Text), txtCarName.Text, (Car.CarType)cmbCarType.SelectedItem, int.Parse(txtCarPrice.Text));
                    db.DeleteCar(car);
                    lblMessage.Content = "Deleted Car: " + car;
                }
                else if (sender.Equals(btnUpdateCar))
                {
                    Car car = new Car(int.Parse(txtCarId.Text), txtCarName.Text, (Car.CarType)cmbCarType.SelectedItem, int.Parse(txtCarPrice.Text));
                    db.UpdateCar(car);
                    listCars.Items.Refresh();
                    lblMessage.Content = "Updated Car: " + car;
                }
                else if (sender.Equals(btnSortCars))
                {
                    db.SortCars();
                    lblMessage.Content = "Sorted Cars";
                }
                //else if (sender.Equals(btnStoreCars))
                //{
                //    db.StoreCars();
                //    lblMessage.Content = "Stored Cars";
                //}
                //else if (sender.Equals(btnLoadCars))
                //{
                //    db.SortCars();
                //    lblMessage.Content = "Loaded Cars";
                //}
                else if (sender.Equals(btnAddOwner))
                {
                    Owner owner = new Owner(0, "", DateTime.Now, DateTime.Now, int.Parse(txtCarId.Text));
                    db.AddOwner(owner);
                    lblMessage.Content = "Added Owner: " + owner;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Content = "error: " + ex.Message;
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void OnCarSelected(object sender, SelectionChangedEventArgs e)
        {
            Car c = sender.Equals(listCars)
                        ? (Car)listCars.SelectedItem
                        : (Car)dgCars.SelectedItem;

            if (c != null)
            {
                txtCarId.Text = c.CarId.ToString();
                txtCarName.Text = c.CarName;
                cmbCarType.SelectedItem = c.Type;

                lblMessage.Content = "Selected Car: " + c;
            }
            db.FillAllOwnersOfCar(c.CarId);
        }

        private void OnMenuFileSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender.Equals(mitLoadCars))
                {
                    db.LoadCars();
                    lblMessage.Content = "Loaded Cars: ";
                }
                else if (sender.Equals(mitStoreCars))
                {
                    db.StoreCars();
                    lblMessage.Content = "Stored Cars: ";
                }
                else if (sender.Equals(mitLoadCarsXml))
                {
                    db.LoadXmlCars();
                    lblMessage.Content = "Loaded Xml Cars: ";
                }
                else if (sender.Equals(mitStoreCarsXml))
                {
                    db.StoreXmlCars();
                    lblMessage.Content = "Stored Xml Cars: ";
                }
                else if (sender.Equals(mitBeginnTrx))
                {
                    db.DB_Host = txtDBHost.Text;
                    db.DB_Password = pwDBPassword.Password;
                    db.DB_Username = txtDBUserName.Text;

                    db.BeginnTrx();
                    lblMessage.Content = "Began Trx: ";
                }
                else if (sender.Equals(mitCommit))
                {
                    db.CommitTrx();
                    lblMessage.Content = "Commited: ";
                }
                else if (sender.Equals(mitLoad))
                {
                    db.FillAllCars();
                    lblMessage.Content = "Loaded cars: ";
                }
                else if (sender.Equals(mitRollback))
                {
                    db.Rollback();
                    lblMessage.Content = "Rolledback: ";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Content = "error: " + ex.Message;
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|xml files (*.xml)|*.xml|jpg files (*.jpg)|*.jpg";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                //Get the path of specified file
                txtFilePath.Text = openFileDialog.FileName;

                if (txtFilePath.Text.EndsWith(".jpg"))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(txtFilePath.Text, UriKind.Absolute);
                    bitmap.EndInit();
                    img.Source = bitmap;
                }
            }

        }
       
    }
}
