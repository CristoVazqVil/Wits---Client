﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reflection.Emit;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Wits.Classes;
using Wits.WitsService;

namespace Wits
{
    /// <summary>
    /// Lógica de interacción para Menu.xaml
    /// </summary>
    /// a ver si ya 
    public partial class Menu : Page, WitsService.IConnectedUsersCallback
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private Random random = new Random();
        private string username = UserSingleton.Instance.Username;
        private List<Uri> songs = new List<Uri>()
        {
            new Uri("Music/Song1.wav", UriKind.Relative),
            new Uri("Music/Song2.wav", UriKind.Relative),
            new Uri("Music/Song3.wav", UriKind.Relative),
            new Uri("Music/Song4.wav", UriKind.Relative),
            new Uri("Music/Song5.wav", UriKind.Relative),
            new Uri("Music/Song6.wav", UriKind.Relative),
            new Uri("Music/Song7.wav", UriKind.Relative),
            new Uri("Music/Song8.wav", UriKind.Relative)
         };

        public Menu()
        {
            InitializeComponent();
            backgroundVideo.Play();
            mediaPlayer.MediaEnded += SongEnded;
            InstanceContext context = new InstanceContext(this);
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient(context);
            PlayRandomSong();
            ValidateGuest();
            SetProfilePicture();

            try
            {
                client.AddConnectedUser(username);
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
        }


        private void PlayRandomSong()
        {
            int randomIndex = random.Next(songs.Count);
            int videoNum = randomIndex + 1;
            String numString = videoNum.ToString();
            Uri songUri = songs[randomIndex];
            string videoPath = "Music/Video" + numString + ".mp4";
            songPlayingVideo.Source = new Uri(videoPath, UriKind.Relative);

            mediaPlayer.Open(songUri);
            mediaPlayer.Play();
            var slideAnimation = (Storyboard)this.Resources["SlideAnimation"];
            songPlayingVideo.RenderTransform = new TranslateTransform();
            slideAnimation.Begin();
        }

        private void SetProfilePicture()
        {
            WitsService.PlayerManagerClient playerManagerClient = new WitsService.PlayerManagerClient();

            try
            {
                Player playerData = playerManagerClient.GetPlayerByUser(username);
                int profilePictureId = playerData.ProfilePictureId;
                string profilePictureFileName = profilePictureId + ".png";
                string profilePicturePath = "ProfilePictures/" + profilePictureFileName;
                Uri profilePictureUri = new Uri(profilePicturePath, UriKind.Relative);
                imageProfilePic.Source = new BitmapImage(profilePictureUri);
                userName.Content = username;
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
                RestartGame();
            }
            
        }

        private void SongEnded(object sender, EventArgs e)
        {
            PlayRandomSong();
        }

        private void RestartVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }

        private void NewGameHighlight(object sender, MouseEventArgs e)
        {
            double scaleFactor = 1.1;
            double centerX = imageNewGame.ActualWidth / 2;
            double centerY = imageNewGame.ActualHeight / 2;

            scaleTransformNewGame.ScaleX = scaleFactor;
            scaleTransformNewGame.ScaleY = scaleFactor;

            translateTransformNewGame.X = -(centerX * (scaleFactor - 1));
            translateTransformNewGame.Y = -(centerY * (scaleFactor - 1));

            double centerX2 = labelNewGame.ActualWidth / 2;
            double centerY2 = labelNewGame.ActualHeight / 2;

            scaleTransformNewGame2.ScaleX = scaleFactor;
            scaleTransformNewGame2.ScaleY = scaleFactor;

            translateTransformNewGame2.X = -(centerX2 * (scaleFactor - 1));
            translateTransformNewGame2.Y = -(centerY2 * (scaleFactor - 1));
        }

        private void UndoNewGameHighlight(object sender, MouseEventArgs e)
        {
            scaleTransformNewGame.ScaleX = 1.0;
            scaleTransformNewGame.ScaleY = 1.0;
            translateTransformNewGame.X = 0;
            translateTransformNewGame.Y = 0;
            scaleTransformNewGame2.ScaleX = 1.0;
            scaleTransformNewGame2.ScaleY = 1.0;
            translateTransformNewGame2.X = 0;
            translateTransformNewGame2.Y = 0;
        }

        private void JoinHighlight(object sender, MouseEventArgs e)
        {
            double scaleFactor = 1.1;
            double centerX = imageJoinGame.ActualWidth / 2;
            double centerY = imageJoinGame.ActualHeight / 2;

            scaleTransformJoin.ScaleX = scaleFactor;
            scaleTransformJoin.ScaleY = scaleFactor;

            translateTransformJoin.X = -(centerX * (scaleFactor - 1));
            translateTransformJoin.Y = -(centerY * (scaleFactor - 1));

            double centerX2 = labelJoinGame.ActualWidth / 2;
            double centerY2 = labelJoinGame.ActualHeight / 2;

            scaleTransformJoin2.ScaleX = scaleFactor;
            scaleTransformJoin2.ScaleY = scaleFactor;

            translateTransformJoin2.X = -(centerX2 * (scaleFactor - 1));
            translateTransformJoin2.Y = -(centerY2 * (scaleFactor - 1));
        }

        private void UndoJoinHighlight(object sender, MouseEventArgs e)
        {
            scaleTransformJoin.ScaleX = 1.0;
            scaleTransformJoin.ScaleY = 1.0;
            translateTransformJoin.X = 0;
            translateTransformJoin.Y = 0;
            scaleTransformJoin2.ScaleX = 1.0;
            scaleTransformJoin2.ScaleY = 1.0;
            translateTransformJoin2.X = 0;
            translateTransformJoin2.Y = 0;
        }

        private void setFriendsMenu(List<string> onlineFriendsList)
        {
            OnlineFriendMenuUserControl onlineFriends = new OnlineFriendMenuUserControl();
            onlineFriends.SetFriends(onlineFriendsList);
            gridOnlineFriends.Children.Add(onlineFriends);
        }

        private void CreateNewGame(object sender, MouseButtonEventArgs e)
        {
            Random randomId = new Random();
            int newGameId = randomId.Next(10000, 100000);
            WitsService.GameManagerClient client = new WitsService.GameManagerClient();
            try
            {
                client.CreateGame(newGameId, UserSingleton.Instance.Username);
                mediaPlayer.Stop();
                GameSingleton.Instance.SetGame(newGameId, 1);
                OpenlobbyPage();
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
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
                RestartGame();
            }
        }

        private void JoinExistingGame(object sender, MouseButtonEventArgs e)
        {
            var window = new InsertGameIdWindow();
            var result = window.ShowDialog();

            if (result == true)
            {
                int existingGameId = window.GameId;
                WitsService.GameManagerClient client = new WitsService.GameManagerClient();
                try
                {
                    int playerNumber = client.JoinGame(existingGameId, UserSingleton.Instance.Username);
                    if (playerNumber > 0)
                    {
                        mediaPlayer.Stop();
                        GameSingleton.Instance.SetGame(existingGameId, playerNumber);
                        OpenlobbyPage();
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.NotExistingGameMessage, Properties.Resources.NotExistingGame, MessageBoxButton.OK, MessageBoxImage.Information);
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
                    RestartGame();
                }
            }
        }

        private void OpenCustomization(object sender, MouseButtonEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient(context);
            ProfileCustomizationPage profileCustomizationPage = new ProfileCustomizationPage();

            try
            {
                mediaPlayer.Stop();
                client.RemoveConnectedUserInMenu(username);
                this.NavigationService.Navigate(profileCustomizationPage);
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
                RestartGame();
            }
            
        }

        private void OpenMyFriendsPage(object sender, MouseButtonEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient(context);
            MyFriendsPage myFriendsPage = new MyFriendsPage();

            try
            {
                mediaPlayer.Stop();
                client.RemoveConnectedUserInMenu(username);
                this.NavigationService.Navigate(myFriendsPage);
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
                RestartGame();
            }
        }

        public void UpdateConnectedFriends()
        {
            if (!UserSingleton.Instance.Username.Substring(0, 5).Equals("Guest"))
            {
                InstanceContext context = new InstanceContext(this);
                WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient(context);

                try
                {
                    string[] connectedFriendsArray = client.GetConnectedFriends(UserSingleton.Instance.Username);
                    List<string> connectedFriends = new List<string>(connectedFriendsArray);

                    gridOnlineFriends.Children.Clear();
                    setFriendsMenu(connectedFriends);
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
                    RestartGame();
                }
            }
        }

        private void OpenlobbyPage()
        {
            InstanceContext context = new InstanceContext(this);
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient(context);
            LobbyPage lobbyPage = new LobbyPage();

            try
            {
                mediaPlayer.Stop();
                client.RemoveConnectedUserInMenu(username);
                this.NavigationService.Navigate(lobbyPage);
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
                RestartGame();
            }
            
        }

        private void ValidateGuest()
        {
            if (UserSingleton.Instance.Username.Substring(0, 5).Equals("Guest"))
            {
                labelNewGame.Visibility = Visibility.Collapsed;
                imageNewGame.Visibility = Visibility.Collapsed;
                textBlockAllMyFriends.Visibility = Visibility.Collapsed;
                gridOnlineFriends.Visibility = Visibility.Collapsed;
                imageEditProfile.Visibility = Visibility.Collapsed;
            }
        }

        public void RestartGame()
        {
            UserSingleton.Instance.ClearUsername();
            GameSingleton.Instance.ClearGame();
            var currentWindow = Window.GetWindow(this);
            var mainWindow = new MainWindow();
            mainWindow.Show();
            currentWindow.Close();
        }

        private void Logout(object sender, MouseButtonEventArgs e)
        {
            var currentWindow = Window.GetWindow(this);
            var mainWindow = new MainWindow();
            mainWindow.Show();
            currentWindow.Close();
            UserSingleton.Instance.ClearUsername();
            GameSingleton.Instance.ClearGame();
        }
    }
}
