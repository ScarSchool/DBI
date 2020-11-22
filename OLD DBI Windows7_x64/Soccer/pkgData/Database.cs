using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pkgData
{
    public class Database
    {
        public string Host { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public readonly ObservableCollection<Team> Teams = new ObservableCollection<Team>();
        public readonly ObservableCollection<Player> Players = new ObservableCollection<Player>();
        public readonly ObservableCollection<Score> Scores = new ObservableCollection<Score>();

        private static readonly String SELECT_TEAMS = "SELECT NVL(ORA_ROWSCN, -1), id, country, coach FROM teams ORDER BY id";
        private static readonly String SELECT_PLAYERS_OF_TEAM = "SELECT NVL(ORA_ROWSCN, -1), id, name, birthdate FROM players WHERE idteam = :idteam ORDER BY id";
        private static readonly String SELECT_SCORES_OF_PLAYER = "SELECT NVL(ORA_ROWSCN, -1), gamedate, country, goals, assists FROM scores INNER JOIN teams ON teams.id = idopponent WHERE idplayer = :idplayer ORDER BY gamedate";

        private static readonly String INSERT_TEAM = "INSERT INTO teams VALUES(seqTeam.NEXTVAL, :country, :coach)";
        private static readonly String INSERT_PLAYER = "INSERT INTO players VALUES(seqPlayer.NEXTVAL, :name, :birthdate, :idteam)";
        private static readonly String SELECT_OPPONENT_ID = "SELECT id FROM teams WHERE country = :opponent";
        private static readonly String INSERT_SCORE = "INSERT INTO scores VALUES(:gamedate, :idopponent, :goals, :assists, :idplayer)";

        private static readonly String LOCK_TEAM = "SELECT NVL(ORA_ROWSCN, -1) FROM teams WHERE id = :id FOR UPDATE";
        private static readonly String LOCK_PLAYER = "SELECT NVL(ORA_ROWSCN, -1) FROM players WHERE id = :id FOR UPDATE";
        private static readonly String LOCK_SCORE = "SELECT NVL(ORA_ROWSCN, -1) FROM scores WHERE gamedate = :gamedate AND idplayer = :idplayer FOR UPDATE";

        private static readonly String UPDATE_TEAM = "UPDATE teams SET country = :country, coach = :coach WHERE id = :id";
        private static readonly String UPDATE_PLAYER = "UPDATE players SET name = :name, birthdate = :birthdate, idteam = :idteam WHERE id = :id";
        private static readonly String UPDATE_SCORE = "UPDATE scores SET idopponent = :idopponent, goals = :goals, assists = :assists WHERE gamedate = :gamedate AND idplayer = :idplayer";

        private static readonly String DELETE_TEAM = "DELETE FROM teams WHERE id = :id";
        private static readonly String DELETE_PLAYER = "DELETE FROM players WHERE id = :id";
        private static readonly String DELETE_SCORE = "DELETE FROM scores WHERE gamedate = :gamedate AND idplayer = :idplayer";

        private OracleConnection conn;
        private OracleTransaction trx;
        private string CONNSTRING = @"user id={0};password={1};data source=" +
                                     "(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)" +
                                     "(HOST={2})(PORT=1521))(CONNECT_DATA=" +
                                     "(SERVICE_NAME=ora11g)))";

        public readonly static Database Instance = new Database();

        public void Connect(string host, string user, string password)
        {
            Host = host;
            User = user;
            Password = password;

            CloseConnection();
            conn = new OracleConnection(String.Format(CONNSTRING, User, Password, Host));
            conn.Open();
        }

        public void BeginTransaction()
        {
            trx = conn.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void CommitTrx()
        {
            trx.Commit();
            BeginTransaction();
        }

        public void RollbackTrx()
        {
            trx.Rollback();
            BeginTransaction();
        }

        public void CloseConnection()
        {
            if (conn != null)
            {
                conn.Close();
            }
        }

        public void LoadTeams()
        {
            OracleCommand cmd = new OracleCommand(SELECT_TEAMS, conn);
            OracleDataReader reader = cmd.ExecuteReader();
            Teams.Clear();

            while (reader.Read())
            {
                Team team = new Team(reader.GetInt32(1), reader.GetString(2), reader.GetString(3));
                team.SCN = reader.GetInt64(0);
                Teams.Add(team);
            }

            reader.Close();
        }

        public void LoadPlayersOfTeam(Team team)
        {
            OracleCommand cmd = new OracleCommand(SELECT_PLAYERS_OF_TEAM, conn);
            cmd.Parameters.Add("idteam", OracleDbType.Int32).Value = team.Id;
            OracleDataReader reader = cmd.ExecuteReader();
            Players.Clear();

            while (reader.Read())
            {
                Player player = new Player(reader.GetInt32(1), reader.GetString(2), reader.GetDateTime(3));
                player.SCN = reader.GetInt64(0);
                Players.Add(player);
            }

            reader.Close();
        }

        public void LoadScoresOfPlayer(Player player)
        {
            OracleCommand cmd = new OracleCommand(SELECT_SCORES_OF_PLAYER, conn);
            cmd.Parameters.Add("idplayer", OracleDbType.Int32).Value = player.Id;
            OracleDataReader reader = cmd.ExecuteReader();
            Scores.Clear();

            while (reader.Read())
            {
                Score score = new Score(reader.GetDateTime(1), reader.GetString(2), reader.GetInt32(3), reader.GetInt32(4));
                score.SCN = reader.GetInt64(0);
                Scores.Add(score);
            }

            reader.Close();
        }

        private int selectOpponentId(string opponent)
        {
            OracleCommand cmd = new OracleCommand(SELECT_OPPONENT_ID, conn);
            cmd.Parameters.Add("opponent", OracleDbType.Varchar2).Value = opponent;
            OracleDataReader reader = cmd.ExecuteReader();
            int opponentId = -1;

            if (reader.Read())
            {
                opponentId = reader.GetInt32(0);
                reader.Close();
            }

            return opponentId;
        }

        public void InsertTeam(Team team)
        {
            OracleCommand cmd = new OracleCommand(INSERT_TEAM, conn);
            cmd.Parameters.Add("country", OracleDbType.Varchar2).Value = team.Country;
            cmd.Parameters.Add("coach", OracleDbType.Varchar2).Value = team.Coach;
            cmd.ExecuteNonQuery();

            CommitTrx();
            LoadTeams();
        }

        public void InsertPlayer(Player player, Team team)
        {
            OracleCommand cmd = new OracleCommand(INSERT_PLAYER, conn);
            cmd.Parameters.Add("name", OracleDbType.Varchar2).Value = player.Name;
            cmd.Parameters.Add("birthdate", OracleDbType.Date).Value = player.Birthdate;
            cmd.Parameters.Add("idTeam", OracleDbType.Int32).Value = team.Id;
            cmd.ExecuteNonQuery();

            CommitTrx();
            LoadPlayersOfTeam(team);
        }

        public void InsertScore(Score score, Player player)
        {
            int opponentId = selectOpponentId(score.Opponent);

            if (opponentId != -1)
            {
                OracleCommand cmd = new OracleCommand(INSERT_SCORE, conn);
                cmd.Parameters.Add("gamedate", OracleDbType.Date).Value = score.GameDate;
                cmd.Parameters.Add("idopponent", OracleDbType.Int32).Value = opponentId;
                cmd.Parameters.Add("goals", OracleDbType.Int32).Value = score.Goals;
                cmd.Parameters.Add("assists", OracleDbType.Int32).Value = score.Assists;
                cmd.Parameters.Add("idplayer", OracleDbType.Int32).Value = player.Id;
                cmd.ExecuteNonQuery();

                CommitTrx();
                LoadScoresOfPlayer(player);
            }
            else
            {
                throw new Exception("Unkown opponent, " + score.Opponent);
            }
        }

        private bool lockTeam(Team team)
        {
            OracleCommand cmd = new OracleCommand(LOCK_TEAM, conn);
            cmd.Parameters.Add("id", OracleDbType.Int32).Value = team.Id;
            OracleDataReader reader = cmd.ExecuteReader();

            bool locked = (reader.Read() && team.SCN == reader.GetInt64(0));
            reader.Close();
            return locked;
        }

        private bool lockPlayer(Player player)
        {
            OracleCommand cmd = new OracleCommand(LOCK_PLAYER, conn);
            cmd.Parameters.Add("id", OracleDbType.Int32).Value = player.Id;
            OracleDataReader reader = cmd.ExecuteReader();

            bool locked = (reader.Read() && player.SCN == reader.GetInt64(0));
            reader.Close();
            return locked;
        }

        private bool lockScore(Score score, Player player)
        {
            OracleCommand cmd = new OracleCommand(LOCK_SCORE, conn);
            cmd.Parameters.Add("gamedate", OracleDbType.Date).Value = score.GameDate;
            cmd.Parameters.Add("idplayer", OracleDbType.Int32).Value = player.Id;
            OracleDataReader reader = cmd.ExecuteReader();

            bool locked = (reader.Read() && score.SCN == reader.GetInt64(0));
            reader.Close();
            return locked;
        }

        public void UpdateTeam(Team team)
        {
            if (lockTeam(team))
            {
                OracleCommand cmd = new OracleCommand(UPDATE_TEAM, conn);
                cmd.Parameters.Add("country", OracleDbType.Varchar2).Value = team.Country;
                cmd.Parameters.Add("coach", OracleDbType.Varchar2).Value = team.Coach;
                cmd.Parameters.Add("id", OracleDbType.Int32).Value = team.Id;
                cmd.ExecuteNonQuery();
            }
            else
            {
                throw new Exception("Team was updated by another client, therefore the update was canceled");
            }
        }

        public void UpdatePlayer(Player player, Team team)
        {
            if (lockPlayer(player))
            {
                OracleCommand cmd = new OracleCommand(UPDATE_PLAYER, conn);
                cmd.Parameters.Add("name", OracleDbType.Varchar2).Value = player.Name;
                cmd.Parameters.Add("birthdate", OracleDbType.Date).Value = player.Birthdate;
                cmd.Parameters.Add("idteam", OracleDbType.Int32).Value = team.Id;
                cmd.Parameters.Add("id", OracleDbType.Int32).Value = player.Id;
                cmd.ExecuteNonQuery();
            }
            else
            {
                throw new Exception("Player was updated by another client, therefore the update was canceled");
            }
        }

        public void UpdateScore(Score score, Player player)
        {
            if (lockScore(score, player))
            {
                int opponentId = selectOpponentId(score.Opponent);

                if (opponentId != -1)
                {
                    OracleCommand cmd = new OracleCommand(UPDATE_SCORE, conn);
                    cmd.Parameters.Add("idopponnent", OracleDbType.Varchar2).Value = opponentId;
                    cmd.Parameters.Add("goals", OracleDbType.Int32).Value = score.Goals;
                    cmd.Parameters.Add("assists", OracleDbType.Int32).Value = score.Assists;
                    cmd.Parameters.Add("gamedate", OracleDbType.Date).Value = score.GameDate;
                    cmd.Parameters.Add("idplayer", OracleDbType.Int32).Value = player.Id;
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    throw new Exception("Unkown opponent, " + score.Opponent);
                }
            }
            else
            {
                throw new Exception("Score was updated by another client, therefore the update was canceled");
            }
        }

        public void DeleteTeam(Team team)
        {
            if (lockTeam(team))
            {
                OracleCommand cmd = new OracleCommand(DELETE_TEAM, conn);
                cmd.Parameters.Add("id", OracleDbType.Int32).Value = team.Id;
                cmd.ExecuteNonQuery();
                CommitTrx();
                Teams.Remove(team);
            }
            else
            {
                throw new Exception("Team was updated by another client, therefore the delete was canceled");
            }
        }

        public void DeletePlayer(Player player)
        {
            if (lockPlayer(player))
            {
                OracleCommand cmd = new OracleCommand(DELETE_PLAYER, conn);
                cmd.Parameters.Add("id", OracleDbType.Int32).Value = player.Id;
                cmd.ExecuteNonQuery();
                CommitTrx();
                Players.Remove(player);
            }
            else
            {
                throw new Exception("Player was updated by another client, therefore the delete was canceled");
            }
        }

        public void DeleteScore(Score score, Player player)
        {
            if (lockScore(score, player))
            {
                int opponentId = selectOpponentId(score.Opponent);

                if (opponentId != -1)
                {
                    OracleCommand cmd = new OracleCommand(DELETE_SCORE, conn);
                    cmd.Parameters.Add("gamedate", OracleDbType.Date).Value = score.GameDate;
                    cmd.Parameters.Add("idplayer", OracleDbType.Int32).Value = player.Id;
                    cmd.ExecuteNonQuery();
                    CommitTrx();
                    Scores.Remove(score);
                }
                else
                {
                    throw new Exception("Unkown opponent, " + score.Opponent);
                }
            }
            else
            {
                throw new Exception("Score was updated by another client, therefore the delete was canceled");
            }
        }
    }
}
