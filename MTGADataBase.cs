using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGADraftHelper
{
    public class MTGADatabase
    {
        public MTGADatabase()
        {
            string cardidsandinfo = "";// System.IO.File.ReadAllText("data_cards.mtga");
            string cardnamestext = "";// System.IO.File.ReadAllText("data_loc.mtga");
            foreach (string filePath in Directory.GetFiles(Properties.Settings.Default.mtgadir))
            {
                int DirCounts = filePath.Split('\\').Count();
                string fileName = filePath.Split('\\')[DirCounts - 1].ToString();
                if (fileName.StartsWith("data_cards") && fileName.EndsWith(".mtga"))
                    cardidsandinfo = System.IO.File.ReadAllText(filePath);
                if (fileName.StartsWith("data_loc") && fileName.EndsWith(".mtga"))
                    cardnamestext = System.IO.File.ReadAllText(filePath);
            }

            JArray x1 = JArray.Parse(cardnamestext);
            JToken english = x1[0]["keys"];
            List<IDNameObject> cardnames = Newtonsoft.Json.JsonConvert.DeserializeObject<List<IDNameObject>>(english.ToString());
            List<MTGACardInfo> cardids = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MTGACardInfo>>(cardidsandinfo);
            cards = new Dictionary<int, MTGACard>();
            foreach (MTGACardInfo card in cardids)
            {
                IDNameObject match = cardnames.FirstOrDefault(p => p.id == card.titleId);
                if (match == null)
                    continue;
                bool island = false;
                foreach(var type in card.types)
                {
                    string t = type.ToString();
                    if (t == "5")
                        island = true;
                }
                cards.Add(card.grpid, new MTGACard() { id = card.grpid, name = match.text, isLand = island });
            }

            //JObject cards2 = JObject.Parse(text);//["cards"];
            //cards = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, MTGACard>>(cards2["cards"].ToString());
        }


        public Dictionary<int, MTGACard> cards { get; set; }// = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, MTGACard>>(System.IO.File.ReadAllText("database.json"));

        public static List<string> KnownLessons()
        {
            List<string> ret = new List<string>();
            ret.Add("Academic Probation");
            ret.Add("Basic Conjuration");
            ret.Add("Confront the Past");
            ret.Add("Containment Breach");
            ret.Add("Elemental Summoning");
            ret.Add("Environmental Sciences");
            ret.Add("Expanded Anatomy");
            ret.Add("Fractal Summoning");
            ret.Add("Illuminate History");
            ret.Add("Inkling Summoning");
            ret.Add("Introduction to Annihilation");
            ret.Add("Introduction to Prophecy");
            ret.Add("Mascot Exhibition");
            ret.Add("Mercurial Transformation");
            ret.Add("Necrotic Fumes");
            ret.Add("Pest Summoning");
            ret.Add("Reduce to Memory");
            ret.Add("Spirit Summoning");
            ret.Add("Start from Scratch");
            ret.Add("Teachings of the Archaics");
            return ret;
        }

    }
    public class MTGACard
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isLand { get; set; }
        //public string type { get; set; }
    }

    public class MTGACardInfo
    {
        //this is the id in the logs
        public int grpid { get; set; }
        //this is the id match into localizations.
        public int titleId { get; set; }
        public JToken types { get; set; }
    }

    public class IDNameObject
    {
        public int id { get; set; }
        public string text { get; set; }
    }
}
