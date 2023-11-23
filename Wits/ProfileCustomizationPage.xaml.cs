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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wits.Classes;
using Wits.WitsService;

namespace Wits
{
    /// <summary>
    /// Lógica de interacción para ProfileCustomizationPage.xaml
    /// </summary>
    public partial class ProfileCustomizationPage : Page
    {
        private Rectangle currentVisibleRectangle = null;
        private int selectedCelebrationId = -1;


        public ProfileCustomizationPage()
        {
            InitializeComponent();
            userName.Content = UserSingleton.Instance.Username;
            SetProfilePicture(UserSingleton.Instance.Username);
            currentVisibleRectangle = null;
            background.Play();
            CelebrationsVideo.Play();

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

            int celebrationId = playerData.CelebrationId;
            selectedCelebrationId = celebrationId;
            string rectangleName = "_" + celebrationId;
            Rectangle selectedRectangle = FindName(rectangleName) as Rectangle;

            if (selectedRectangle != null)
            {
                if (currentVisibleRectangle != null)
                {
                    currentVisibleRectangle.Visibility = Visibility.Hidden;
                }

                selectedRectangle.Visibility = Visibility.Visible;
                currentVisibleRectangle = selectedRectangle;
                selectedRectangle.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF00D233"));
            }
        }



        private void ProfilePictureClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image clickedImage)
            {
                currentPicture.Source = clickedImage.Source;
            }

        }

        private void OpenMainMenu(object sender, MouseButtonEventArgs e)
        {

            this.NavigationService.GoBack();

        }


        private void ShowCelebrations(object sender, MouseButtonEventArgs e)
        {
            TranslateTransform moveTransform = new TranslateTransform(1200, 0);
            imageContainer.RenderTransform = moveTransform;
            Celebrations.RenderTransform = moveTransform;


        }

        private void Rectangle_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle clickedRectangle)
            {
                string rectangleName = clickedRectangle.Name;
                string celebrationId = rectangleName.Replace("_", "");

                if (int.TryParse(celebrationId, out int parsedCelebrationId))
                {
                    foreach (var rectangle in Celebrations.Children.OfType<Rectangle>())
                    {
                        rectangle.Stroke = new SolidColorBrush(Colors.Transparent);
                    }

                    selectedCelebrationId = parsedCelebrationId;

                    clickedRectangle.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF00D233"));
                    currentVisibleRectangle = clickedRectangle;
                }
            }
        }





        private void ShowPictures(object sender, MouseButtonEventArgs e)
        {
            TranslateTransform moveBackTransform = new TranslateTransform(0, 0);
            imageContainer.RenderTransform = moveBackTransform;

            TranslateTransform moveBackTransformCelebrations = new TranslateTransform(0, 0);
            Celebrations.RenderTransform = moveBackTransformCelebrations;
        }

        private void SaveChanges(object sender, MouseButtonEventArgs e)
        {
            string currentPicturePath = currentPicture.Source.ToString();
            string currentPictureFileName = System.IO.Path.GetFileName(currentPicturePath);
            currentPictureFileName = currentPictureFileName.Replace(".png", "");
            int.TryParse(currentPictureFileName, out int profilePictureId);

                WitsService.PlayerManagerClient playerManagerClient = new WitsService.PlayerManagerClient();
                int newProfilePictureId = profilePictureId;
                bool success = playerManagerClient.UpdateProfilePicture(UserSingleton.Instance.Username, newProfilePictureId);
                if (success)
                {
                    bool celebrationUpdateSuccess = playerManagerClient.UpdateCelebration(UserSingleton.Instance.Username, selectedCelebrationId);

                    if (celebrationUpdateSuccess)
                    {
                        this.NavigationService.GoBack();
                    }
                    else
                    {
                    MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.Failed, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                }
                else
                {
                 MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.Failed, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RestartBackground(object sender, RoutedEventArgs e)
        {
            background.Position = TimeSpan.Zero;
            background.Play();
        }

        private void RestartCelebrations(object sender, RoutedEventArgs e)
        {
            CelebrationsVideo.Position = TimeSpan.Zero;
            CelebrationsVideo.Play();
        }
    }
    }
