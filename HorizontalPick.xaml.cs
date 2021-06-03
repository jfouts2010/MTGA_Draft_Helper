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
using System.Windows.Threading;

namespace MTGADraftHelper
{
    /// <summary>
    /// Interaction logic for HorizontalPick.xaml
    /// </summary>
    public partial class HorizontalPick : Page
    {
        public List<DraftCard> outcards = new List<DraftCard>();
        public bool showingAll = true;
        DispatcherTimer gameTimer = new DispatcherTimer();
        public HorizontalPick()
        {
            InitializeComponent();
            LoadCardGrids();
            gameTimer.Tick += HoverCheck;
            gameTimer.Interval = TimeSpan.FromMilliseconds(200);
            gameTimer.Start();
        }
        public void HoverCheck(object sender, EventArgs e)
        {
            var point = WindowsServices.GetMousePosition();
            int row = 0;
            double rowStart = 0;
            foreach (RowDefinition rd in MainGrid.RowDefinitions)
            {
                rowStart += rd.ActualHeight;
                if (point.Y < rowStart)
                    break;
                else
                    row++;
            }
            int column = 0;
            double columnStart = 0;
            foreach (ColumnDefinition cd in MainGrid.ColumnDefinitions)
            {
                columnStart += cd.ActualWidth;
                if (point.X < columnStart)
                    break;
                else
                    column++;
            }
            if (point.X > System.Windows.SystemParameters.PrimaryScreenWidth || point.Y > System.Windows.SystemParameters.PrimaryScreenHeight)
                return;
            if ((decimal)point.Y / 1080M > 650M / 1080M)
            {
                MainGrid.Visibility = Visibility.Hidden;
            }
            else if (row % 2 == 1 && column % 2 == 1)
            {
                MainGrid.Visibility = Visibility.Visible;
                int card = 0;
                card = ((row - 1) / 2 * 8) + ((column - 1) / 2);

                if (card < outcards.Count())
                {
                    //in a card row
                    FrontEndHelpers.LoadScores(MainGrid, outcards, card);
                    showingAll = false;
                }
                else
                {
                    FrontEndHelpers.LoadScores(MainGrid, outcards);
                    showingAll = true;
                }
            }
            else if (!showingAll)
            {
                MainGrid.Visibility = Visibility.Visible;
                FrontEndHelpers.LoadScores(MainGrid, outcards);
                showingAll = true;
            }
            else
                MainGrid.Visibility = Visibility.Visible;
        }
        public void LoadCardGrids()
        {
            FrontEndHelpers.CreateGrid(1, 1, 0, MainGrid);
            FrontEndHelpers.CreateGrid(1, 3, 1, MainGrid);
            FrontEndHelpers.CreateGrid(1, 5, 2, MainGrid);
            FrontEndHelpers.CreateGrid(1, 7, 3, MainGrid);
            FrontEndHelpers.CreateGrid(1, 9, 4, MainGrid);
            FrontEndHelpers.CreateGrid(1, 11, 5, MainGrid);
            FrontEndHelpers.CreateGrid(1, 13, 6, MainGrid);
            FrontEndHelpers.CreateGrid(1, 15, 7, MainGrid);

            FrontEndHelpers.CreateGrid(3, 1, 8, MainGrid);
            FrontEndHelpers.CreateGrid(3, 3, 9, MainGrid);
            FrontEndHelpers.CreateGrid(3, 5, 10, MainGrid);
            FrontEndHelpers.CreateGrid(3, 7, 11, MainGrid);
            FrontEndHelpers.CreateGrid(3, 9, 12, MainGrid);
            FrontEndHelpers.CreateGrid(3, 11, 13, MainGrid);
            FrontEndHelpers.CreateGrid(3, 13, 14, MainGrid);
        }
    }
}
