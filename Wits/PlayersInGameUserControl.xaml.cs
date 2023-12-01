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

namespace Wits
{
    /// <summary>
    /// Interaction logic for PlayersInGameUserControl.xaml
    /// </summary>
    public partial class PlayersInGameUserControl : UserControl
    {
        private int rowsAdded = 0;
        public event EventHandler ImageCloseClicked;

        public PlayersInGameUserControl()
        {
            InitializeComponent();
        }

        public void SetPlayers(List<string> playersList)
        {
            rowsAdded = 0;
            gridPlayersInGame.Children.Clear();
            gridPlayersInGame.RowDefinitions.Clear();

            foreach (string friendUsername in playersList)
            {
                AddPlayerRow(friendUsername);
            }

            RowDefinition lastRowDefinition = new RowDefinition();
            lastRowDefinition.Height = new GridLength(1, GridUnitType.Star);
            gridPlayersInGame.RowDefinitions.Add(lastRowDefinition);
        }

        private void AddPlayerRow(string playerUser)
        {
            PlayerInGameCardUserControl playerCard = new PlayerInGameCardUserControl();
            playerCard.ButtonClicked += PlayerCard_ButtonClicked;
            Grid.SetRow(playerCard, rowsAdded);
            playerCard.setFriend(playerUser);
            gridPlayersInGame.Children.Add(playerCard);
            rowsAdded++;

            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = GridLength.Auto;
            gridPlayersInGame.RowDefinitions.Add(rowDefinition);
        }

        private void PlayerCard_ButtonClicked(object sender, EventArgs e)
        {
            MyFriendCardUserControl card = (MyFriendCardUserControl)sender;
            gridPlayersInGame.Children.Remove(card);
            rowsAdded--;
        }

        private void ClosePlayersMenu(object sender, MouseButtonEventArgs e)
        {
            ImageCloseClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
