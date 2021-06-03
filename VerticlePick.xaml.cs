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
    /// Interaction logic for VerticlePick.xaml
    /// </summary>
    public partial class VerticlePick : Page
    {
        public bool showingAll = true;
        public List<DraftCard> outcards = new List<DraftCard>();
        DispatcherTimer gameTimer = new DispatcherTimer();
        public VerticlePick()
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
            if ((decimal)point.X / 1920M > 1505M / 1920M)
            {
                FrontEndHelpers.HideAllScores(MainGrid);
                showingAll = false;
            }
            else if (row % 2 == 1 && column % 2 == 1)
            {
                int card = 0;
                card = ((row - 1) / 2 * 5) + ((column - 1) / 2);

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
                FrontEndHelpers.LoadScores(MainGrid, outcards);
                showingAll = true;
            }
        }
        public void LoadCardGrids()
        {
            FrontEndHelpers.CreateGrid(1, 1, 0, MainGrid);
            FrontEndHelpers.CreateGrid(1, 3, 1, MainGrid);
            FrontEndHelpers.CreateGrid(1, 5, 2, MainGrid);
            FrontEndHelpers.CreateGrid(1, 7, 3, MainGrid);
            FrontEndHelpers.CreateGrid(1, 9, 4, MainGrid);

            FrontEndHelpers.CreateGrid(3, 1, 5, MainGrid);
            FrontEndHelpers.CreateGrid(3, 3, 6, MainGrid);
            FrontEndHelpers.CreateGrid(3, 5, 7, MainGrid);
            FrontEndHelpers.CreateGrid(3, 7, 8, MainGrid);
            FrontEndHelpers.CreateGrid(3, 9, 9, MainGrid);

            FrontEndHelpers.CreateGrid(5, 1, 10, MainGrid);
            FrontEndHelpers.CreateGrid(5, 3, 11, MainGrid);
            FrontEndHelpers.CreateGrid(5, 5, 12, MainGrid);
            FrontEndHelpers.CreateGrid(5, 7, 13, MainGrid);
            FrontEndHelpers.CreateGrid(5, 9, 14, MainGrid);
        }
    }
}
