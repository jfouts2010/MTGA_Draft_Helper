using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGADraftHelper
{
    public class DraftCard
    {
        public override string ToString()
        {
            return name;
        }

        public int seen_count { get; set; }
        public string name { get; set; }
        public decimal avg_seen { get; set; }
        public int pick_count { get; set; }
        public decimal avg_pick { get; set; }
        public decimal temp_pick_relative { get; set; }
        public int game_count { get; set; }
        public decimal win_rate { get; set; }
        public string winratiostring
        {
            get
            {
                return string.Format("{0:0.0} %", 100 * (win_rate - .5m));
            }
        }
        public decimal win_rate_temp { get { return 100 * (win_rate - .5m); } }
        public int sideboard_game_count { get; set; }
        public decimal sideboard_win_rate { get; set; }
        public int opening_hand_game_count { get; set; }
        public decimal opening_hand_win_rate { get; set; }
        public int drawn_game_count { get; set; }
        public decimal drawn_win_rate { get; set; }
        public int ever_drawn_game_count { get; set; }
        public decimal ever_drawn_win_rate { get; set; }
        public int never_drawn_game_count { get; set; }
        public decimal never_drawn_win_rate { get; set; }
        public decimal drawn_improvement_win_rate { get; set; }

        public string color { get; set; }
        public string rarity { get; set; }
        public string url { get; set; }

        public bool is_lesson { get; set; }
        public bool isLand { get; set; }
        //public int MTGA_ID { get; set; }
        public List<int> MTGA_ID { get; set; }
    }
}
