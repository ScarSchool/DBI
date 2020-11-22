using pkgData;
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

namespace Soccer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Database db = Database.Instance;
        private bool isNewItem = false;

        public MainWindow()
        {
            InitializeComponent();
            myInitialize();
        }

        private void myInitialize()
        {
            try
            {
                dgTeams.ItemsSource = db.Teams;
                dgPlayers.ItemsSource = db.Players;
                dgScores.ItemsSource = db.Scores;
            }
            catch(Exception ex)
            {
                lblStatus.Content = "ERROR " + ex.Message;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender.Equals(btnConnect))
                {
                    db.Connect(txtHost.Text, txtUser.Text, txtPassword.Password);
                    db.BeginTransaction();
                    lblStatus.Content = "Connected and started a transaction";
                }
                else if(sender.Equals(btnLoad))
                {
                    db.LoadTeams();
                    lblStatus.Content = "Teams loaded";
                }
                else if (sender.Equals(btnUpdate))
                {
                    try
                    {
                        int counter = 0;

                        foreach(Team team in db.Teams)
                        {
                            if (team.Modified)
                            {
                                db.UpdateTeam(team);
                                counter++;
                            }
                        }

                        db.CommitTrx();

                        Team selectedTeam = dgTeams.SelectedItem as Team;
                        foreach (Player player in db.Players)
                        {
                            if (player.Modified)
                            {
                                db.UpdatePlayer(player, selectedTeam);
                                counter++;
                            }
                        }

                        db.CommitTrx();

                        Player selectedPlayer= dgPlayers.SelectedItem as Player;
                        foreach (Score score in db.Scores)
                        {
                            if (score.Modified)
                            {
                                db.UpdateScore(score, selectedPlayer);
                                counter++;
                            }
                        }

                        db.CommitTrx();
                        lblStatus.Content = counter + " entities updated";
                    }
                    catch(Exception ex)
                    {
                        lblStatus.Content = "ERROR " + ex.Message;
                        db.RollbackTrx();
                    }
                    finally
                    {
                        db.Players.Clear();
                        db.Scores.Clear();
                        db.LoadTeams();
                    }
                }
                else if(sender.Equals(btnDeleteTeam))
                {
                    try
                    {
                        db.DeleteTeam(dgTeams.SelectedItem as Team);
                        db.CommitTrx();
                        lblStatus.Content = "Team deleted";
                    }
                    catch (Exception ex)
                    {
                        lblStatus.Content = "ERROR " + ex.Message;
                        db.RollbackTrx();
                    }
                }
                else if (sender.Equals(btnDeletePlayer))
                {
                    try
                    {
                        db.DeletePlayer(dgPlayers.SelectedItem as Player);
                        db.CommitTrx();
                        lblStatus.Content = "Player deleted";
                    }
                    catch (Exception ex)
                    {
                        lblStatus.Content = "ERROR " + ex.Message;
                        db.RollbackTrx();
                    }
                }
                else if (sender.Equals(btnDeleteScore))
                {
                    try
                    {
                        db.DeleteScore(dgScores.SelectedItem as Score, dgPlayers.SelectedItem as Player);
                        db.CommitTrx();
                        lblStatus.Content = "Score deleted";
                    }
                    catch (Exception ex)
                    {
                        lblStatus.Content = "ERROR " + ex.Message;
                        db.RollbackTrx();
                    }
                }
            }
            catch (Exception ex)
            {
                lblStatus.Content = "ERROR " + ex.Message;
            }
        }

        public void OnRowChanged(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (sender.Equals(dgTeams))
            {
                try
                {
                    Team team = e.Row.Item as Team;

                    if(isNewItem)
                    {
                        isNewItem = false;
                        db.InsertTeam(team);
                        lblStatus.Content = "Team added";
                    }
                    else
                    {
                        team.Modified = true;
                    }
                }
                catch(Exception ex)
                {
                    lblStatus.Content = "ERROR " + ex.Message;
                }
            }
            else if (sender.Equals(dgPlayers))
            {
                try
                {
                    Player player = e.Row.Item as Player;

                    if (isNewItem)
                    {
                        isNewItem = false;
                        db.InsertPlayer(player, dgTeams.SelectedItem as Team);
                        lblStatus.Content = "Player added";
                    }
                    else
                    {
                        player.Modified = true;
                    }
                }
                catch (Exception ex)
                {
                    lblStatus.Content = "ERROR " + ex.Message;
                }
            }
            else if (sender.Equals(dgScores))
            {
                try
                {
                    Score score = e.Row.Item as Score;

                    if (isNewItem)
                    {
                        isNewItem = false;
                        dgScores.Columns.ElementAt(0).IsReadOnly = true;
                        db.InsertScore(score, dgPlayers.SelectedItem as Player);
                        lblStatus.Content = "Score added";
                    }
                    else
                    {
                        score.Modified = true;
                    }
                }
                catch (Exception ex)
                {
                    db.Scores.Remove(dgScores.SelectedItem as Score);
                    lblStatus.Content = "ERROR " + ex.Message;
                }
            }
        }

        private void OnNewItem(object sender, AddingNewItemEventArgs e)
        {
            isNewItem = true;

            if (sender.Equals(dgScores))
            {
                dgScores.Columns.ElementAt(0).IsReadOnly = false;
            }
        }

        private void Row_Selected(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(dgTeams))
            {
                Team team = dgTeams.SelectedItem as Team;

                if (team != null)
                {
                    db.LoadPlayersOfTeam(team);
                }
                else
                {
                    db.Players.Clear();
                }
            }
            else if (sender.Equals(dgPlayers))
            {
                Player player = dgPlayers.SelectedItem as Player;

                if (player != null)
                {
                    db.LoadScoresOfPlayer(player);
                }
                else
                {
                    db.Scores.Clear();
                }
            }
        }

        private void onWindow_Closed(object sender, EventArgs e)
        {
            db.CloseConnection();
        }
    }
}
