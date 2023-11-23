﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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
    /// Interaction logic for FriendRequestCardUserControl.xaml
    /// </summary>
    public partial class FriendRequestCardUserControl : UserControl
    {
        public event EventHandler ButtonAcceptClicked;
        public event EventHandler ButtonRejectClicked;

        public FriendRequestCardUserControl()
        {
            InitializeComponent();
        }

        public void setFriend(string username)
        {
            labelFriendUsername.Content = username;
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
            Wits.WitsService.Player player = client.GetPlayerByUser(username);

            int profilePictureId = player.ProfilePictureId;
            string profilePictureFileName = profilePictureId + ".png";
            string profilePicturePath = "ProfilePictures/" + profilePictureFileName;
            Uri profilePictureUri = new Uri(profilePicturePath, UriKind.Relative);
            imageFriendProfile.Source = new BitmapImage(profilePictureUri);
        }

        private void AcceptFriendRequest(object sender, RoutedEventArgs e)
        {
            string acceptedPlayer = labelFriendUsername.Content.ToString();
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();

            try
            {
                client.AcceptRequest(UserSingleton.Instance.Username, acceptedPlayer);
                client.AcceptRequest(acceptedPlayer, UserSingleton.Instance.Username);
                AddFriendships(acceptedPlayer);
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            ButtonAcceptClicked?.Invoke(this, EventArgs.Empty);
        }

        private void RejectFriendRequest(object sender, RoutedEventArgs e)
        {
            string rejectedPlayer = labelFriendUsername.Content.ToString();
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();

            try
            {
                client.RejectRequest(UserSingleton.Instance.Username, rejectedPlayer);
                client.RejectRequest(rejectedPlayer, UserSingleton.Instance.Username);
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            ButtonRejectClicked?.Invoke(this, EventArgs.Empty);
        }

        private void AddFriendships(string acceptedPlayer)
        {
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
            try
            {
                client.AddFriendship(UserSingleton.Instance.Username, acceptedPlayer);
                client.AddFriendship(acceptedPlayer, UserSingleton.Instance.Username);
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
