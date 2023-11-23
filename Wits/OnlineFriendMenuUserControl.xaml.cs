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
    /// Interaction logic for OnlineFriendCardUserControl.xaml
    /// </summary>
    public partial class OnlineFriendMenuUserControl : UserControl
    {
        private int rowsAdded = 0;
        public OnlineFriendMenuUserControl()
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
            OnlineFriendCardUserControl friendCard = new OnlineFriendCardUserControl();
            Grid.SetRow(friendCard, rowsAdded);
            friendCard.setFriend(onlineFriend);
            gridFriends.Children.Add(friendCard);
            rowsAdded++;

            RowDefinition rowDefinition = new RowDefinition();
            rowDefinition.Height = GridLength.Auto;
            gridFriends.RowDefinitions.Add(rowDefinition);
        }
    }
}
