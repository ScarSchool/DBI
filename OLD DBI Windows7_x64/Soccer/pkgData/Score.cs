using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pkgData
{
    public class Score
    {
        public DateTime GameDate { get; set; }
        public string GameDateFormatted
        {
            get
            {
                return GameDate.ToString("dd.MM.yyyy");
            }
            set
            {
                try
                {
                    GameDate = DateTime.ParseExact(value, "dd.MM.yyyy", null);
                }
                catch (Exception ex)
                {
                }
            }
        }

        public string Opponent { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public long SCN { get; set; }
        public bool Modified { get; set; }

        public Score()
        {
        }

        public Score(DateTime gameDate, string opponent, int goals, int assists)
        {
            GameDate = gameDate;
            Opponent = opponent;
            Goals = goals;
            Assists = assists;
        }
    }
}
