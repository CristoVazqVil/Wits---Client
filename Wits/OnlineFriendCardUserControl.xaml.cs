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

namespace Wits
{
    /// <summary>
    /// Interaction logic for FriendCardUserControl.xaml
    /// </summary>
    public partial class OnlineFriendCardUserControl : UserControl
    {
        public OnlineFriendCardUserControl()
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
            catch (FaultException ex)
            {
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (CommunicationException ex)
            {
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
        }
    }
}
