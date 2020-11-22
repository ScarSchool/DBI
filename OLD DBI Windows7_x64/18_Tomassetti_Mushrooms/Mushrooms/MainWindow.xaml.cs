﻿using Microsoft.Win32;
using pkgDatabase;
using WpfDiscovery;
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


namespace Mushrooms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Database db = null;

        public MainWindow()
        {
            InitializeComponent();
            cbUseful.ItemsSource = Enum.GetValues(typeof(MushroomUseful));
        }

        public void LoadDB()
        {
            if (db == null)
            {
                try
                {
                    db = Database.Instance;
                    db.Connect();
                    RefreshMushrooms();
                    db.RefreshDiscoveries();
                    Log("Database loaded!");
                    return;
                }
                catch (Exception ex)
                {
                    Log(ex);
                }
            }
            else
            {
                Log("Database already loaded!");
            }
        }

        public void Add()
        {
            try
            {
                Mushroom mushroom = new Mushroom();
                db.Add(mushroom);
                mushroom.PropertyChanged += MushroomPropertyChanged;

                Log("Mushroom with default values added!");
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        public void Update()
        {
            try
            {
                int nrOfUpdatedMushrooms = 0;

                foreach (Mushroom mushroom in db.Mushrooms)
                {
                    if (mushroom.Updated)
                    {
                        db.Update(mushroom);
                        nrOfUpdatedMushrooms++;
                    }
                }

                RefreshMushrooms();
                Log($"{nrOfUpdatedMushrooms} mushrooms updated!");
                return;
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        public void Remove(Mushroom mushroom)
        {
            try
            {
                db.Remove(mushroom);
                Log($"Mushroom with id = {mushroom.Id} deleted!");
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        public void RefreshMushrooms()
        {
            db.RefreshMushrooms();

            foreach (Mushroom mushroom in db.Mushrooms)
            {
                mushroom.PropertyChanged += MushroomPropertyChanged;
            }

            Log("Mushrooms refreshed!");
        }

        public void Commit()
        {
            db.Commit();
            Log("Transaction commited!");
        }

        public void Rollback()
        {
            db.Rollback();
            Log("Transaction Rolbacked!");
        }

        public void StoreAsXml()
        {
            db.StoreAsXml();
            Log("Database stored as XML!");
        }



        private void OnMenuItemClicked(Object sender, EventArgs e)
        {
            if (sender.Equals(miStoreAsXml))
            {
                StoreAsXml();
            }

            else if (sender.Equals(miLoadDB))
            {
                LoadDB();
                dgMushrooms.ItemsSource = db.Mushrooms;
                miMushroom.IsEnabled = true;
                miCommit.IsEnabled = true;
                miRollback.IsEnabled = true;
                miStoreAsXml.IsEnabled = true;
            }
            else if (sender.Equals(miCommit))
            {
                Commit();
            }
            else if (sender.Equals(miRollback))
            {
                Rollback();
            }

            else if (sender.Equals(miAdd))
            {
                Add();
            }
            else if (sender.Equals(miUpdate))
            {
                Update();
            }
            else if (sender.Equals(miRemove))
            {
                if (dgMushrooms.SelectedItem != null)
                {
                    Remove(dgMushrooms.SelectedItem as Mushroom);
                }
            }
            else if (sender.Equals(miDiscoveries))
            {
                DiscoveryWindow dw = new DiscoveryWindow();
                dw.Show();
            }
        }

        private void OnDgRowSelected(Object sender, EventArgs e)
        {
            if (dgMushrooms.SelectedItem != null)
            {
                Mushroom selectedMushroom = dgMushrooms.SelectedItem as Mushroom;
                cbUseful.SelectedItem = selectedMushroom.Useful;
                tbPathPic.Text = selectedMushroom.ImagePath;
                tbDescription.Text = selectedMushroom.Description;
            }
        }

        private void MushroomPropertyChanged(Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Mushroom mushroom = sender as Mushroom;
            mushroom.Updated = true;
            Log($"Mushroom {e.PropertyName} locally modified for the mushroom with id = {mushroom.Id}!");
        }

        private void UsefulChanged(Object sender, EventArgs e)
        {
            Mushroom mushroom = dgMushrooms.SelectedItem as Mushroom;

            if (mushroom.Useful != (MushroomUseful)cbUseful.SelectedItem)
            {
                mushroom.Useful = (MushroomUseful)cbUseful.SelectedItem;
                mushroom.Updated = true;
            }
        }

        private void PathPicChanged(Object sender, EventArgs e)
        {
            Mushroom mushroom = dgMushrooms.SelectedItem as Mushroom;

            if (mushroom.ImagePath != tbPathPic.Text)
            {
                mushroom.ImagePath = tbPathPic.Text;
                mushroom.Updated = true;
            }
        }

        private void DescriptionChanged(Object sender, EventArgs e)
        {
            Mushroom mushroom = dgMushrooms.SelectedItem as Mushroom;

            if (mushroom.Description != tbDescription.Text)
            {
                mushroom.Description = tbDescription.Text;
                mushroom.Updated = true;
            }
        }

        private void OpenFileDialog(Object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "png files|*.png|jpg files|*.jp*g"
            };

            if (fileDialog.ShowDialog() == true)
            {
                tbPathPic.Text = fileDialog.FileName;
            }
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
    }
}
