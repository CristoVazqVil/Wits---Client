using System;
using System.Collections.Generic;
using System.IO.Packaging;
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
using Wits.WitsService;

namespace Wits
{
    /// <summary>
    /// Interaction logic for MyFriendsPage.xaml
    /// </summary>
    public partial class MyFriendsPage : Page
    {
        private const int PENDING = 0;
        private const int ACCEPTED = 1;
        private string username = UserSingleton.Instance.Username;

        public MyFriendsPage()
        {
            InitializeComponent();
            backgroundVideo.Play();
            SetFriends();
            SetProfile();
        }

        private void RestartVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }

        private void SetFriends()
        {
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
            string[] myFriendArray = client.GetPlayerFriends(UserSingleton.Instance.Username);
            List<string> friendList = new List<string>(myFriendArray);
            MyFriendsUserControl myFriends = new MyFriendsUserControl();
            myFriends.SetFriends(friendList);
            gridMyFriends.Children.Add(myFriends);
        }

        private void SetProfile()
        {
            WitsService.PlayerManagerClient playerManagerClient = new WitsService.PlayerManagerClient();
            Player playerData = playerManagerClient.GetPlayerByUser(username);
            int profilePictureId = playerData.ProfilePictureId;
            string profilePictureFileName = profilePictureId + ".png";
            string profilePicturePath = "ProfilePictures/" + profilePictureFileName;
            Uri profilePictureUri = new Uri(profilePicturePath, UriKind.Relative);
            imageProfile.Source = new BitmapImage(profilePictureUri);
            labelUsername.Content = username;
        }

        private void OpemMainMenu(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void SetFriendRequests(object sender, MouseButtonEventArgs e)
        {
            gridMyFriends.Children.Clear();
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
            string[] requestsArray = client.GetAllPlayerRequests(UserSingleton.Instance.Username);
            List<string> requestsList = new List<string>(requestsArray);
            FriendRequestMenuUserControl requests = new FriendRequestMenuUserControl();
            requests.SetFriendRequests(requestsList);
            gridMyFriends.Children.Add(requests);
        }

        private void SetFriendsMenu(object sender, MouseButtonEventArgs e)
        {
            gridMyFriends.Children.Clear();
            SetFriends();
        }

        private void SendFriendRequest(object sender, RoutedEventArgs e)
        {
            string playerTo = textBoxPlayerUser.Text;
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();

            if (IsAValidPlayer(playerTo))
            {
                try
                {
                    if (client.AddRequest(UserSingleton.Instance.Username, playerTo) == 1)
                    {
                        MessageBox.Show(Properties.Resources.RequestSentMessage, Properties.Resources.RequestSent, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.ExistingRequestMessage, Properties.Resources.ExistingRequest, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (FaultException ex)
                {
                    Console.WriteLine(ex.ToString());
                    MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BlockPlayer(object sender, RoutedEventArgs e)
        {
            string enteredPlayer = textBoxPlayerUser.Text;
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();

            if (IsAValidPlayer(enteredPlayer))
            {
                MessageBoxResult result = MessageBox.Show(Properties.Resources.BlockConfirmationMesssage, Properties.Resources.BlockPlayer, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (client.BlockPlayer(UserSingleton.Instance.Username, enteredPlayer) >= 1)
                    {
                        DeleteFriendshipsAndRequests(enteredPlayer);
                        MessageBox.Show(Properties.Resources.BlockPlayerMessage, Properties.Resources.BlockedPlayer, MessageBoxButton.OK, MessageBoxImage.Information);
                        SetFriends();
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.AlreadyBlockedPlayerMessage, Properties.Resources.AlreadyBlockedPlayer, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        private void DeleteFriendshipsAndRequests(string enteredPlayer)
        {
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
            client.DeleteRequest(enteredPlayer, UserSingleton.Instance.Username, PENDING);
            client.DeleteRequest(enteredPlayer, UserSingleton.Instance.Username, ACCEPTED);
            client.DeleteRequest(UserSingleton.Instance.Username, enteredPlayer, PENDING);
            client.DeleteRequest(UserSingleton.Instance.Username, enteredPlayer, ACCEPTED);
            client.DeleteFriendship(enteredPlayer, UserSingleton.Instance.Username);
            client.DeleteFriendship(UserSingleton.Instance.Username, enteredPlayer);
        }

        private bool IsAExistingPlayer(string enteredUsername)
        {
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
            WitsService.Player enteredPlayer = client.GetPlayerByUser(enteredUsername);

            if (enteredPlayer != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsAValidPlayer(string enteredUsername)
        {
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
            bool validator = false;
            if (!enteredUsername.Equals(UserSingleton.Instance.Username))
            {
                if (IsAExistingPlayer(enteredUsername))
                {
                    if (!client.IsPlayerBlocked(enteredUsername, UserSingleton.Instance.Username))
                    {
                        if (!client.IsPlayerBlocked(UserSingleton.Instance.Username, enteredUsername))
                        {
                            validator = true;
                            return validator;
                        }
                        else
                        {
                            MessageBox.Show(Properties.Resources.BlockedBeforeMessage, Properties.Resources.BlockedPlayer, MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.BlockedByMessage, Properties.Resources.BlockedBy, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.NotExistingPlayerMessage, Properties.Resources.NotExistingPlayer, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.ThatIsYouMessage, Properties.Resources.ThatIsYou, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            return validator;
        }
    }
}
