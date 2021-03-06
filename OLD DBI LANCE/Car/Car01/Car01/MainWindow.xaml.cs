﻿using Microsoft.Win32;
using PkgData;
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

namespace Car01
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
            listCars.ItemsSource = Database.Instance.GetAllCars();
            dgCars.ItemsSource = Database.Instance.GetAllCars();
            cbCarType.ItemsSource = Enum.GetValues(typeof(CarType));
        }
        
        private void OnCarSelected(object sender, SelectionChangedEventArgs e)
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
            else if(sender.Equals(dgCars))
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
        }

        private void OnButtonCarClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if(sender.Equals(btnAddCar))
                {
                    Car car = new Car(int.Parse(txtCarId.Text), txtCarName.Text, (CarType)cbCarType.SelectedItem);
                    Database.Instance.AddCar(car);
                    lblMessage.Content = $"car {car} added";
                }
                else if(sender.Equals(btnUpdateCar))
                {
                    Car car = new Car(int.Parse(txtCarId.Text), txtCarName.Text, (CarType)cbCarType.SelectedItem);
                    Database.Instance.UpdateCar(car);
                    lblMessage.Content = $"car with id {car.CarId} updated";
                }
                else if(sender.Equals(btnDelCar))
                {
                    Car car = new Car(int.Parse(txtCarId.Text), txtCarName.Text, (CarType)cbCarType.SelectedItem);
                    Database.Instance.DeleteCar(car);
                    lblMessage.Content = $"car with id {car.CarId} removed";
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
            if (sender.Equals(miStoreBin))
            {
                Database.Instance.Store();
                lblMessage.Content = "cars stored";
            }
            else if(sender.Equals(miLoadBin))
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
                    if(ofd.FileName.Contains(".jp"))

                    Database.Instance.LoadXml(ofd.FileName);
                    lblMessage.Content = "cars loaded (XML)";
                }
            }
        }

        public void OnDBChanged(object sender, EventArgsCar e)
        {
            if(e is EventArgsCar)
            {
                lblMessage.Content = e.Car.ToString()
            }
        }
    }
}
