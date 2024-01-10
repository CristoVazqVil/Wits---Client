using System;
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
    /// Interaction logic for MyFriendCardUserControl.xaml
    /// </summary>
    public partial class MyFriendCardUserControl : UserControl
    {
        private const int PENDING = 0;
        private const int ACCEPTED = 1;
        public event EventHandler ImageClicked;

        public MyFriendCardUserControl()
        {
            InitializeComponent();
        }

        public void setFriend(string username)
        {
            labelFriendUsername.Content = username;
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();

            try
            {
                Wits.WitsService.Player player = client.GetPlayerByUser(username);
                int profilePictureId = player.ProfilePictureId;
                string profilePictureFileName = profilePictureId + ".png";
                string profilePicturePath = "ProfilePictures/" + profilePictureFileName;
                Uri profilePictureUri = new Uri(profilePicturePath, UriKind.Relative);
                imageFriendProfile.Source = new BitmapImage(profilePictureUri);
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
            }
            
        }

        private void BlockPlayer(object sender, MouseButtonEventArgs e)
        {
            string enteredPlayer = labelFriendUsername.Content.ToString();
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();

            MessageBoxResult result = MessageBox.Show(Properties.Resources.BlockConfirmationMesssage, Properties.Resources.BlockPlayer, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (client.BlockPlayer(UserSingleton.Instance.Username, enteredPlayer) >= 1)
                    {
                        DeleteFriendships(enteredPlayer);
                        DeleteAllRequests(enteredPlayer);
                        MessageBox.Show(Properties.Resources.BlockPlayerMessage, Properties.Resources.BlockedPlayer, MessageBoxButton.OK, MessageBoxImage.Information);
                        ImageClicked?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (TimeoutException ex)
                {
                    Logger.LogErrorException(ex);
                    MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (CommunicationException ex)
                {
                    Logger.LogErrorException(ex);
                    MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DeleteFriend(object sender, MouseButtonEventArgs e)
        {
            string enteredPlayer = labelFriendUsername.Content.ToString();
            MessageBoxResult result = MessageBox.Show(Properties.Resources.DeleteFriendConfirmationMessage, Properties.Resources.DeleteFriend, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (DeleteFriendships(enteredPlayer) == 2)
                {
                    DeleteAcceptedRequests(enteredPlayer);
                    MessageBox.Show(Properties.Resources.FriendDeletedMessage, Properties.Resources.FriendDeleted, MessageBoxButton.OK, MessageBoxImage.Information);
                    ImageClicked?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private int DeleteFriendships(string enteredPlayer)
        {
            int result = 0;
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();

            try
            {
                result = client.DeleteFriendship(enteredPlayer, UserSingleton.Instance.Username);
                result = result + client.DeleteFriendship(UserSingleton.Instance.Username, enteredPlayer);
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }

            return result;  
        }
        private void DeleteAllRequests(string enteredPlayer)
        {
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
            try
            {
                client.DeleteRequest(enteredPlayer, UserSingleton.Instance.Username, PENDING);
                client.DeleteRequest(enteredPlayer, UserSingleton.Instance.Username, ACCEPTED);
                client.DeleteRequest(UserSingleton.Instance.Username, enteredPlayer, PENDING);
                client.DeleteRequest(UserSingleton.Instance.Username, enteredPlayer, ACCEPTED);
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteAcceptedRequests(string enteredPlayer)
        {
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
            try
            {
                client.DeleteRequest(enteredPlayer, UserSingleton.Instance.Username, ACCEPTED);
                client.DeleteRequest(UserSingleton.Instance.Username, enteredPlayer, ACCEPTED);
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
