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
    /// Interaction logic for PlayerInGameCardUserControl.xaml
    /// </summary>
    public partial class PlayerInGameCardUserControl : UserControl
    {

        public event EventHandler ButtonClicked;
        public PlayerInGameCardUserControl()
        {
            InitializeComponent();
        }

        public void setFriend(string username)
        {
            labelPlayerUsername.Content = username;
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
            Wits.WitsService.Player player = client.GetPlayerByUser(username);

            int profilePictureId = player.ProfilePictureId;
            string profilePictureFileName = profilePictureId + ".png";
            string profilePicturePath = "ProfilePictures/" + profilePictureFileName;
            Uri profilePictureUri = new Uri(profilePicturePath, UriKind.Relative);
            imagePlayerProfile.Source = new BitmapImage(profilePictureUri);
        }

        private void ExpelPlayer(object sender, RoutedEventArgs e)
        {
            //Aquí hacer el metodo para expulsar
            ButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
