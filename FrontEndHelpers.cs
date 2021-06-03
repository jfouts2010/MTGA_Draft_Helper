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


namespace MTGADraftHelper
{
    public class FrontEndHelpers
    {
        public static void LoadScores(Grid MainGrid, List<DraftCard> cards, int onlyOneCard = -1)
        {
            //clear cards
            FrontEndHelpers.HideAllScores(MainGrid);
            int count = 0;
            foreach (DraftCard c in cards)
            {
                if ((onlyOneCard >= 0 && count == onlyOneCard) || onlyOneCard == -1)
                {

                    Button b1 = ((Button)LogicalTreeHelper.FindLogicalNode(MainGrid, $"Card{count}IWD"));
                    b1.Content = $"{c.drawn_improvement_win_rate}";
                    b1.ToolTip = $"IWD: {c.name}";
                    b1.Visibility = Visibility.Visible;
                    if (c.drawn_improvement_win_rate >= 3)
                        b1.Foreground = Brushes.YellowGreen;
                    else if (c.drawn_improvement_win_rate > -1)
                        b1.Foreground = Brushes.LightGoldenrodYellow;
                    else
                        b1.Foreground = Brushes.DarkRed;

                    Button b2 = ((Button)LogicalTreeHelper.FindLogicalNode(MainGrid, $"Card{count}ATTP"));
                    b2.Content = $"{c.temp_pick_relative}";
                    b2.Visibility = Visibility.Visible;
                    if (c.temp_pick_relative >= 6)
                        b2.Foreground = Brushes.DarkRed;
                    else if (c.temp_pick_relative > 2)
                        b2.Foreground = Brushes.LightGoldenrodYellow;
                    else
                        b2.Foreground = Brushes.YellowGreen;

                    /*  Button b3 = ((Button)LogicalTreeHelper.FindLogicalNode(MainGrid, $"Card{count}WR"));
                      b3.Content = $"{c.winratiostring}";
                      if (c.win_rate_temp >= 2)
                          b3.Foreground = Brushes.Green;
                      else if (c.win_rate_temp <= -2)
                          b3.Foreground = Brushes.Red;
                      else
                          b3.Foreground = Brushes.Yellow;*/
                }
                count++;
            }
        }
        public static void HideAllScores(Grid MainGrid)
        {
            for (int i = 0; i < 15; i++)
            {
                ((Button)LogicalTreeHelper.FindLogicalNode(MainGrid, $"Card{i}IWD")).Content = "";
                ((Button)LogicalTreeHelper.FindLogicalNode(MainGrid, $"Card{i}IWD")).Visibility = Visibility.Hidden;
                ((Button)LogicalTreeHelper.FindLogicalNode(MainGrid, $"Card{i}ATTP")).Content = "";
                ((Button)LogicalTreeHelper.FindLogicalNode(MainGrid, $"Card{i}ATTP")).Visibility = Visibility.Hidden;
                //((Button)LogicalTreeHelper.FindLogicalNode(MainGrid, $"Card{i}WR")).Content = "";
            }
        }
        public static void CreateGrid(int row, int column, int card, Grid MainGrid)
        {
            Grid g = new Grid();
            g.SetValue(Grid.RowProperty, row);
            g.SetValue(Grid.ColumnProperty, column);
            g.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(.109375, GridUnitType.Star) });
            g.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(.15, GridUnitType.Star) });
            g.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(.1453125, GridUnitType.Star) });
            g.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(.15, GridUnitType.Star) });
            g.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(.4453125, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.0710382513661202185792349726776, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.85792349726775956284153005464481, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.0710382513661202185792349726776, GridUnitType.Star) });

            List<Viewbox> boxes = FrontEndHelpers.ViewboxLoad(card);
            g.Children.Add(boxes[0]);
            g.Children.Add(boxes[1]);
            // g.Children.Add(boxes[2]);
            g.Name = "Card" + card.ToString();
            g.Background = Brushes.Transparent;
            MainGrid.Children.Add(g);
        }
        public static List<Viewbox> ViewboxLoad(int card)
        {
            List<Viewbox> viewboxes = new List<Viewbox>();

            Viewbox vb1 = BuildOneViewbox(1, 1);
            vb1.VerticalAlignment = VerticalAlignment.Top;
            vb1.HorizontalAlignment = HorizontalAlignment.Left;
            Button vb1Button = BuildOneButton("IWD", $"Card{card}IWD");
            vb1.Child = vb1Button;
            viewboxes.Add(vb1);

            Viewbox vb2 = BuildOneViewbox(1, 1);
            vb2.VerticalAlignment = VerticalAlignment.Top;
            vb2.HorizontalAlignment = HorizontalAlignment.Right;
            Button vb2Button = BuildOneButton("ATTP", $"Card{card}ATTP");
            vb2.Child = vb2Button;
            viewboxes.Add(vb2);

            /* Viewbox vb3 = BuildOneViewbox(3, 1);
             vb3.VerticalAlignment = VerticalAlignment.Bottom;
             vb3.HorizontalAlignment = HorizontalAlignment.Left;
             Button vb3Button = BuildOneButton("Win Rate", $"Card{card}WR");
             vb3.Child = vb3Button;
             viewboxes.Add(vb3);*/

            return viewboxes;
        }
        public static Viewbox BuildOneViewbox(int row, int column)
        {
            Viewbox vb1 = new Viewbox();
            vb1.SetValue(Grid.RowProperty, row);
            vb1.SetValue(Grid.ColumnProperty, column);
            vb1.StretchDirection = StretchDirection.Both;
            vb1.Stretch = Stretch.Uniform;
            return vb1;
        }
        public static Button BuildOneButton(string tt, string name)
        {
            Button vb1Button = new Button()
            {
                Name = name,
                Content = "",
                Foreground = Brushes.Red,
                FontWeight = FontWeights.Bold,
                BorderThickness = new Thickness(0),
                ToolTip = tt,
                Background = new SolidColorBrush(Color.FromArgb(180, 30, 30, 30))
            };
            return vb1Button;
        }
    }
}
