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
using Wits.Classes;

namespace Wits
{
    /// <summary>
    /// Interaction logic for MyFriendsUserControl.xaml
    /// </summary>
    public partial class MyFriendsUserControl : UserControl
    {
        private int rowsAdded = 0;

        public MyFriendsUserControl()
        {
            InitializeComponent();
        }

        public void SetFriends(List<string> friendList)
        {
            rowsAdded = 0;
            gridFriends.Children.Clear();
            gridFriends.RowDefinitions.Clear();

            foreach (string friendUsername in friendList)
            {
                AddFriendRow(friendUsername);
            }

            RowDefinition lastRowDefinition = new RowDefinition();
            lastRowDefinition.Height = new GridLength(1, GridUnitType.Star);
            gridFriends.RowDefinitions.Add(lastRowDefinition);
        }

        private void AddFriendRow(string onlineFriend)
        {
            MyFriendCardUserControl friendCard = new MyFriendCardUserControl();
            friendCard.ImageBlockClicked += MyFriendCard_ImageBlockClicked;
            friendCard.ImageDeleteClicked += MyFriendCard_ImageDeleteClicked;
            Grid.SetRow(friendCard, rowsAdded);
            friendCard.setFriend(onlineFriend);
            gridFriends.Children.Add(friendCard);
            rowsAdded++;

            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = GridLength.Auto;
            gridFriends.RowDefinitions.Add(rowDefinition);
        }

        private void RemoveFriendCard(MyFriendCardUserControl card)
        {
            gridFriends.Children.Remove(card);
            rowsAdded--;
        }

        private void MyFriendCard_ImageBlockClicked(object sender, EventArgs e)
        {
            MyFriendCardUserControl card = (MyFriendCardUserControl)sender;
            RemoveFriendCard(card);
        }

        private void MyFriendCard_ImageDeleteClicked(object sender, EventArgs e)
        {
            MyFriendCardUserControl card = (MyFriendCardUserControl)sender;
            RemoveFriendCard(card);
        }
    }
}
