using Microsoft.Win32;
using PkgData;
using pkgMiscellaneous;
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

namespace Car03
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeMyComponents();
        }

        private void InitializeMyComponents()
        {
            try
            {
                listCars.ItemsSource = Database.Instance.GetAllCars();
                dgCars.ItemsSource = Database.Instance.GetAllCars();
                dgOwners.ItemsSource = Database.Instance.collOwners;
                cbCarType.ItemsSource = Enum.GetValues(typeof(CarType));
                
                Car.CollListenerMethods += OnDBChanged;
            }
            catch(Exception ex)
            {
                lblMessage.Content = "ERROR: " + ex.Message;
            }
        }

        private void OnCarSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender.Equals(listCars))
                {
                    if (listCars.SelectedItem != null)
                    {
                        dgCars.SelectedItem = listCars.SelectedItem;
                        Car car = listCars.SelectedItem as Car;
                        txtCarId.Text = car.CarId.ToString();
                        txtCarName.Text = car.CarName;
                        cbCarType.SelectedItem = car.Type;
                        lblMessage.Content = $"selected car {car}";
                    }
                }
                else if (sender.Equals(dgCars))
                {
                    if (dgCars.SelectedItem != null)
                    {
                        listCars.SelectedItem = dgCars.SelectedItem;
                        Car car = dgCars.SelectedItem as Car;
                        txtCarId.Text = car.CarId.ToString();
                        txtCarName.Text = car.CarName;
                        cbCarType.SelectedItem = car.Type;
                        lblMessage.Content = $"selected car {car}";
                    }
                }

                Database.Instance.GetCarOwners(dgCars.SelectedItem as Car);
            }
            catch(Exception ex)
            {
                lblMessage.Content = "ERROR: " + ex.Message;
            }
        }

        private void OnButtonCarClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if(sender.Equals(btnAddCar))
                {
                    Car car = new Car(int.Parse(txtCarId.Text), txtCarName.Text, (CarType)cbCarType.SelectedItem, double.Parse(txtCarPrice.Text));
                    Database.Instance.AddCar(car);
                    lblMessage.Content = $"car {car} added";
                }
                else if (sender.Equals(btnUpdateCar))
                {
                    Car car = dgCars.SelectedItem as Car;
                    Database.Instance.UpdateCar(car);
                    lblMessage.Content = $"car {car} updated";
                }
                else if(sender.Equals(btnDelCar))
                {
                    Car car = dgCars.SelectedItem as Car;
                    Database.Instance.DeleteCar(car);
                    lblMessage.Content = $"car with id {car.CarId} removed";
                }
                else if (sender.Equals(btnAddOwner))
                {
                    Owner owner = new Owner();
                    Car car = dgCars.SelectedItem as Car;
                    Database.Instance.AddCarOwner(car, owner);
                    lblMessage.Content = $"owner {owner} added";
                }
                else if (sender.Equals(btnDelOwner))
                {
                    Owner owner = dgOwners.SelectedItem as Owner;
                    Car car = dgCars.SelectedItem as Car;
                    Database.Instance.DeleteCarOwner(car, owner);
                    lblMessage.Content = $"owner with id {owner.OwnerId} removed";
                }
                else if (sender.Equals(btnUpdateOwners))
                {
                    Car car = dgCars.SelectedItem as Car;
                    int i = 0;

                    for (int idx = 0; idx < dgOwners.Items.Count; idx++)
                    {
                        Owner owner = dgOwners.Items[idx] as Owner;

                        if(owner.OwnerUpdated)
                        {
                            Database.Instance.UpdateCarOwner(car, owner);
                            owner.OwnerUpdated = false;
                            i++;
                        }
                    }
                    
                    lblMessage.Content = $"updated {i} owners";
                }
                else if(sender.Equals(btnSort))
                {
                    Database.Instance.Sort();
                    lblMessage.Content = "cars sorted";
                }
                else if(sender.Equals(btnLoadImage))
                {
                    if (txtImage.Text.Contains(".jp"))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(txtImage.Text, UriKind.Absolute);
                        bitmap.EndInit();
                        image.Source = bitmap;
                    }
                }
            }
            catch(Exception ex)
            {
                lblMessage.Content = $"Error: {ex.Message}";
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void OnSelectFileMenu(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender.Equals(miStoreBin))
                {
                    Database.Instance.Store();
                    lblMessage.Content = "cars stored";
                }
                else if (sender.Equals(miLoadBin))
                {
                    Database.Instance.Load();
                    lblMessage.Content = "cars loaded";
                }
                else if (sender.Equals(miStoreXml))
                {
                    Database.Instance.StoreXml();
                    lblMessage.Content = "cars stored (XML)";
                }
                else if (sender.Equals(miLoadXml))
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "txt files (*.txt)|*.txt|xml files|*.xml|jpg files|*.jp*g";

                    if (ofd.ShowDialog() == true)
                    {
                        if (ofd.FileName.Contains(".jp"))

                            Database.Instance.LoadXml(ofd.FileName);
                        lblMessage.Content = "cars loaded (XML)";
                    }
                }
                else if (sender.Equals(miStoreOwnersXML))
                {
                    Database.Instance.StoreAllXml();
                    lblMessage.Content = "cars inlc. owners stored (XML)";
                }
            }
            catch(Exception ex)
            {
                lblMessage.Content = "ERROR: " + ex.Message;
            } 
        }

        private void OnSelectDBMenu(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender.Equals(miBeginTrx))
                {
                    Database.Instance.Db_Host = txtHost.Text;
                    Database.Instance.Db_User = txtUser.Text;
                    Database.Instance.Db_Password = pwdPassword.Password;

                    Database.Instance.BeginTrx();
                    lblMessage.Content = "Transaction started";
                }
                else if (sender.Equals(miLoadDB))
                {
                    Database.Instance.FillAllCars();
                    lblMessage.Content = "cars loaded (DB)";
                }
                else if (sender.Equals(miCommit))
                {
                    Database.Instance.CommitTrx();
                    lblMessage.Content = "Transaction committed";
                }
                else if (sender.Equals(miRoleback))
                {
                    Database.Instance.RollbackTrx();
                    lblMessage.Content = "Transaction rolebacked";
                }
            }
            catch(Exception ex)
            {
                lblMessage.Content = "ERROR: " + ex.Message;
            }
        }

        public void OnDBChanged(object sender, EventArgsCar e)
        {
            try
            {
                dgCars.CancelEdit();
                listCars.Items.Refresh();

                if (dgCars.SelectedIndex == dgCars.Items.Count - 1)
                {
                    txtCarName.Text = ((Car)e.Car).CarName;
                }

                lblMessage.Content = e.Car.ToString() + " " + e.Message;
            }
            catch(Exception ex)
            {
                lblMessage.Content = "ERROR: " + ex.Message;
            }
        }
    }
}
