using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MTGADraftHelper
{
    public partial class ApplicationWindow
    {
        public string Expansion = "STX";
        public string format = "PremierDraft";
        MTGADatabase db = new MTGADatabase();
        public string lastLine { get; set; }
        public List<DraftCard> currentsrc = new List<DraftCard>();
        public List<DraftCard> cards = new List<DraftCard>();
        public Dictionary<int, List<DraftCard>> knownPacks = new Dictionary<int, List<DraftCard>>();
        public List<int> currentdraftcards = new List<int>();
        public VerticlePick vp = new VerticlePick();
        public HorizontalPick hp = new HorizontalPick();
        decimal averagewinratio = .50m;
        DispatcherTimer gameTimer = new DispatcherTimer();
        public bool Vert = true;
        //MTGADatabase db { get; set; }
        public List<DraftCard> pickedCards = new List<DraftCard>();
        public ApplicationWindow()
        {

            InitializeComponent();
            FatherFrame.Content = vp;
            Vert = true;
            SetupFileWatch2();
            gameTimer.Tick += CheckAlignment;
            gameTimer.Interval = TimeSpan.FromMilliseconds(200);
            gameTimer.Start();
        }

        public void CheckAlignment(object sender, EventArgs e)
        {
            Bitmap screenPixel = new Bitmap(1, 1);
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = WindowsServices.BitBlt(hDC, 0, 0, 1, 1, hSrcDC, 1790, 30, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }
            var c = screenPixel.GetPixel(0, 0);
            screenPixel = new Bitmap(1, 1);
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = WindowsServices.BitBlt(hDC, 0, 0, 1, 1, hSrcDC, 1500, 1079, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }
            var c2 = screenPixel.GetPixel(0, 0);
            bool Vertc1Check = false;
            bool Vertc2Check = false;
            if (c.R > 100 && c.G > 100 && c.B > 100)
            {
                Vertc1Check = true;
            }
            if (c2.R > 50 && c2.G > 50)
            {
                Vertc2Check = true;
            }
            if (Vertc1Check && Vertc2Check)
            {
                if (!Vert)
                    FatherFrame.Content = vp;
                Vert = true;
            }
            else if (!Vertc1Check && !Vertc2Check)
            {
                if (Vert)
                    FatherFrame.Content = hp;
                Vert = false;
            }
        }
        public void PullCardData()
        {
            try
            {
                vp.MainGrid.Visibility = Visibility.Visible;
                hp.MainGrid.Visibility = Visibility.Visible;
                cards = new List<DraftCard>();
                knownPacks = new Dictionary<int, List<DraftCard>>();
                currentdraftcards = new List<int>();
                pickedCards = new List<DraftCard>();
                currentsrc = new List<DraftCard>();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                List<string> lessons = MTGADatabase.KnownLessons();
                string url = $"https://www.17lands.com/card_ratings/data?expansion={Expansion}&format={format}&start_date={DateTime.Now.Date.AddMonths(-3).ToString("yyyy-MM-dd")}&end_date={DateTime.Now.Date.ToString("yyyy-MM-dd")}";

                string pagetext = new WebClient().DownloadString(url);
                DraftCard[] tcards = Newtonsoft.Json.JsonConvert.DeserializeObject<DraftCard[]>(pagetext);
                BindingList<DraftCard> outcards = new BindingList<DraftCard>();

                foreach (DraftCard c in tcards)
                {
                    c.avg_pick = ((int)(100 * c.avg_pick)) / 100m;
                    c.avg_seen = ((int)(100 * c.avg_seen)) / 100m;
                    c.drawn_improvement_win_rate = ((int)(1000 * c.drawn_improvement_win_rate)) / 10m;
                    c.is_lesson = lessons.Contains(c.name);
                    if (c.is_lesson)
                        c.win_rate = c.sideboard_win_rate;
                    List<MTGACard> match = db.cards.Values.Where(p => p.name.ToLower().Trim() == c.name.ToLower().Trim()).ToList();
                    c.MTGA_ID = match.Select(p => p.id).ToList();
                    c.isLand = match.Any(p => p.isLand);
                    cards.Add(c);
                    outcards.Add(c);
                }
                averagewinratio = cards.Average(p => p.win_rate);
                foreach (DraftCard c in tcards)
                {
                    c.win_rate = (int)(1000 * (c.win_rate - averagewinratio + .5m)) / 1000m;
                }
            }
            catch (Exception ex)
            {
                //HandleError(ex, "BtnUpdateClick");
            }
        }
        public int CalculateRarityOrder(string rarity)
        {
            if (rarity.ToLower() == "mythic")
                return 1;
            if (rarity.ToLower() == "rare")
                return 2;
            if (rarity.ToLower() == "uncommon")
                return 3;
            return 4;
        }
        public int CalculateColorOrder(string colors)
        {
            bool W = colors.Contains("W");
            bool U = colors.Contains("U");
            bool B = colors.Contains("B");
            bool R = colors.Contains("R");
            bool G = colors.Contains("G");
            if (W && U && B && R && G)
                return 31;
            //4C
            if (W && U && B && R)
                return 26;
            if (U && B && R && G)
                return 27;
            if (B && R && G && W)
                return 28;
            if (R && G && W && U)
                return 29;
            if (G && W && U && B)
                return 30;
            //Wedges/Shards
            if (W && U && B)
                return 16;
            if (W && B && G)
                return 17;
            if (U && B && R)
                return 18;
            if (U && R && W)
                return 19;
            if (B && R && G)
                return 20;
            if (B && G && U)
                return 21;
            if (R && W && B)
                return 22;
            if (R && G && W)
                return 23;
            if (G && W && U)
                return 24;
            if (G && U && R)
                return 25;

            //Guilds
            if (W && U)
                return 6;
            if (W && B)
                return 7;
            if (U && B)
                return 8;
            if (U && R)
                return 9;
            if (B && R)
                return 10;
            if (B && G)
                return 11;
            if (R && G)
                return 12;
            if (R && W)
                return 13;
            if (G && W)
                return 14;
            if (G && U)
                return 15;

            //mono
            if (W)
                return 1;
            if (U)
                return 2;
            if (B)
                return 3;
            if (R)
                return 4;
            if (G)
                return 5;

            return 32;
        }
        public void ProcessLine(string s)
        {
            if (s == lastLine)
                return;
            lastLine = s;
            try
            {
                //see if its any of the lines we care about.
                string lower = s.ToLower();
                //probably we care. check for the 2 lines?
                if (lower.Contains("==> Draft.MakeHumanDraftPick".ToLower()))
                {
                    //we made a pick!
                    //see if we are pack 1 pick 1?
                    if (lower.Contains("\\\"packnumber\\\": \\\"1\\\",\\r\\n    \\\"picknumber\\\": \\\"1\\\"".ToLower()))
                    {
                        //clear picked cards.
                        pickedCards = new List<DraftCard>();
                        knownPacks = new Dictionary<int, List<DraftCard>>();
                    }
                    vp.MainGrid.Visibility = Visibility.Hidden;
                    hp.MainGrid.Visibility = Visibility.Hidden;
                    string json = lower.Substring(lower.IndexOf("{")).Replace("\\", "").Replace("\r\n", "").Replace("rn", "").Replace(":\"{", ":{").Replace("\"}\"", "\"}");
                    JObject pjson = JObject.Parse(json);
                    int cardid = Convert.ToInt32(pjson["request"]["params"]["cardid"]);
                    DraftCard card = cards.FirstOrDefault(p => p.MTGA_ID.Contains(cardid));
                    UpdatePackCards(new List<int>());
                    pickedCards.Add(card);
                    //see if we are pack 3 pick 15?
                    if (lower.Contains("\\\"packnumber\\\": \\\"3\\\",\\r\\n    \\\"picknumber\\\": \\\"15\\\"".ToLower()))
                    {
                        vp.MainGrid.Visibility = Visibility.Hidden;
                        hp.MainGrid.Visibility = Visibility.Hidden;
                    }
                }
                else if (lower.Contains("Draft.Notify".ToLower()))
                {
                    string json = lower.Substring(lower.IndexOf("{"));
                    JObject pjson = JObject.Parse(json);
                    var x = pjson["packcards"];
                    List<int> cardids = new List<int>();
                    foreach (string id in x.ToString().Split(','))
                    {
                        cardids.Add(Convert.ToInt32(id));
                    }
                    vp.MainGrid.Visibility = Visibility.Visible;
                    hp.MainGrid.Visibility = Visibility.Visible;
                    UpdatePackCards(cardids);
                }
                else if (lower.Contains("Event.Join".ToLower()))
                {
                    string json = lower.Substring(lower.IndexOf("{"));
                    JObject pjson = JObject.Parse(json);
                    if (pjson.ContainsKey("payload"))
                    {
                        string eventType = pjson["payload"]["internaleventname"].ToString();
                        if (eventType.Contains("draft"))
                        {
                            //they joined draft! find which type
                            format = eventType.Split('_')[0];
                            Expansion = eventType.Split('_')[1];
                            PullCardData();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //HandleError(e, s);
            }
        }
        public void UpdatePackCards(List<int> cardids)
        {
            try
            {
                if (cardids.Count == 0)
                    return;
                currentdraftcards = cardids;
                int pick = 16 - cardids.Count;
                if (pick == 1)
                    knownPacks = new Dictionary<int, List<DraftCard>>();
                List<DraftCard> outcards = new List<DraftCard>();
                List<DraftCard> tempcards = cards.Where(p => cardids.Any(x => p.MTGA_ID.Contains(x))).ToList();
                foreach (DraftCard c in tempcards)
                {
                    c.temp_pick_relative = Math.Round(c.avg_pick - pick);
                    outcards.Add(c);

                }
                if (pick > 8 && knownPacks.ContainsKey(pick - 8))
                {
                    //we know the pack?
                    /*  List<DraftCard> takencards = knownPacks[pick - 8].Where(x => !outcards.Any(p => p.name == x.name)).ToList();
                      DraftCard filler = new DraftCard() { name = "Taken Cards", drawn_improvement_win_rate = -100, win_rate = 0, temp_pick_relative = 100 };
                      outcards.Add(filler);
                      foreach (DraftCard c in takencards)
                      {
                          c.temp_pick_relative = Math.Round(c.avg_pick - pick);
                          outcards.Add(c);
                      }*/
                }
                outcards = outcards.OrderBy(p => CalculateRarityOrder(p.rarity)).ThenBy(p => p.isLand).ThenBy(p => CalculateColorOrder(p.color)).ThenBy(p => p.name).ToList();
                knownPacks[pick] = outcards;
                vp.outcards = outcards;
                hp.outcards = outcards;
                FrontEndHelpers.LoadScores(vp.MainGrid, outcards);
                FrontEndHelpers.LoadScores(hp.MainGrid, outcards);
            }
            catch (Exception e)
            {
            }
        }
        public static IEnumerable<FrameworkElement> FindVisualChildren(FrameworkElement obj, Func<FrameworkElement, bool> predicate)
        {
            if (obj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    var objChild = VisualTreeHelper.GetChild(obj, i);
                    if (objChild != null && predicate(objChild as FrameworkElement))
                    {
                        yield return objChild as FrameworkElement;
                    }

                    foreach (FrameworkElement childOfChild in FindVisualChildren(objChild as FrameworkElement, predicate))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
        public void SetupFileWatch2()
        {
            try
            {
                string appdatalocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).Replace("Local", "LocalLow");
                string finalfile = System.IO.Path.Combine(appdatalocation, "Wizards Of The Coast\\MTGA\\Player.log");
                var fs = new FileStream(finalfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                int lines = System.IO.File.ReadAllLines(finalfile).Count();
                Tail tail = new Tail(finalfile, lines);
                tail.Changed += tail_Changed;
                tail.Run();
            }
            catch (Exception e)
            {
                //HandleError(e, "FATAL. MUST RESTART. Log Appears to be locked by MTGA Client, Try restarting it");
            }
        }
        public void tail_Changed(object sender, Tail.TailEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                ProcessLine(e.Line);
            });

        }
    }
}
