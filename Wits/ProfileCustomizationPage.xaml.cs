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
using Wits.WitsService;

namespace Wits
{
    /// <summary>
    /// Lógica de interacción para ProfileCustomizationPage.xaml
    /// </summary>
    public partial class ProfileCustomizationPage : Page
    {
        private string loggedInUser;
        public ProfileCustomizationPage()
        {
            InitializeComponent();
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient();
            loggedInUser = client.GetCurrentlyLoggedInUser();
            userName.Content = loggedInUser;
            SetProfilePicture(loggedInUser);
        }

        private void SetProfilePicture(string username)
        {
            WitsService.PlayerManagerClient playerManagerClient = new WitsService.PlayerManagerClient();
            Player playerData = playerManagerClient.GetPlayerByUser(username);
            int profilePictureId = playerData.ProfilePictureId;
            string profilePictureFileName = profilePictureId + ".png";
            string profilePicturePath = "ProfilePictures/" + profilePictureFileName;
            Uri profilePictureUri = new Uri(profilePicturePath, UriKind.Relative);
            currentPicture.Source = new BitmapImage(profilePictureUri);
        }

        private void profilePictureClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image clickedImage)
            {
                currentPicture.Source = clickedImage.Source;
            }
        }

        private void openMainMenu(object sender, MouseButtonEventArgs e)
        {
        
            this.NavigationService.GoBack();

        }

        private void savePicture(object sender, MouseButtonEventArgs e)
        {
            string currentPicturePath = currentPicture.Source.ToString();

            string currentPictureFileName = System.IO.Path.GetFileName(currentPicturePath);
            currentPictureFileName = currentPictureFileName.Replace(".png", "");
            int.TryParse(currentPictureFileName, out int profilePictureId);

            WitsService.PlayerManagerClient playerManagerClient = new WitsService.PlayerManagerClient();
            int newProfilePictureId = profilePictureId; 
            bool success = playerManagerClient.UpdateProfilePicture(loggedInUser, newProfilePictureId);
            if (success)
            {
                this.NavigationService.GoBack();

            }
            else
            {

            }

        }
    }
}
