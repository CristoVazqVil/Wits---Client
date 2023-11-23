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
    /// Interaction logic for FriendRequestMenuUserControl.xaml
    /// </summary>
    public partial class FriendRequestMenuUserControl : UserControl
    {
        private int rowsAdded = 0;

        public FriendRequestMenuUserControl()
        {
            InitializeComponent();
        }
        public void SetFriendRequests(List<string> friendList)
        {
            rowsAdded = 0;
            gridFriends.Children.Clear();
            gridFriends.RowDefinitions.Clear();

            foreach (string friendUsername in friendList)
            {
                AddRequestRow(friendUsername);
            }

            RowDefinition lastRowDefinition = new RowDefinition();
            lastRowDefinition.Height = new GridLength(1, GridUnitType.Star);
            gridFriends.RowDefinitions.Add(lastRowDefinition);
        }

        private void AddRequestRow(string onlineFriend)
        {
            FriendRequestCardUserControl requestCard = new FriendRequestCardUserControl();
            requestCard.ButtonAcceptClicked += RequestCard_ButtonAcceptClicked;
            requestCard.ButtonRejectClicked += RequestCard_ButtonRejectClicked;
            Grid.SetRow(requestCard, rowsAdded);
            requestCard.setFriend(onlineFriend);
            gridFriends.Children.Add(requestCard);
            rowsAdded++;

            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = GridLength.Auto;
            gridFriends.RowDefinitions.Add(rowDefinition);
        }

        private void RemoveFriendRequestCard(FriendRequestCardUserControl card)
        {
            gridFriends.Children.Remove(card);
            rowsAdded--;
        }

        private void RequestCard_ButtonAcceptClicked(object sender, EventArgs e)
        {
            FriendRequestCardUserControl card = (FriendRequestCardUserControl)sender;
            RemoveFriendRequestCard(card);
        }

        private void RequestCard_ButtonRejectClicked(object sender, EventArgs e)
        {
            FriendRequestCardUserControl card = (FriendRequestCardUserControl)sender;
            RemoveFriendRequestCard(card);
        }
    }
}
