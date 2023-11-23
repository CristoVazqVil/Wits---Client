using System;
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
    public partial class Menu : Page
    {
        private MediaPlayer mediaPlayer;
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
            mediaPlayer = new MediaPlayer();
            mediaPlayer.MediaEnded += SongEnded;
            PlayRandomSong();
            LoadConnectedFriends();
            SetProfilePicture();
        }


        private void PlayRandomSong()
        {
            int randomIndex = random.Next(songs.Count);
            int videoNum = randomIndex + 1;
            String numString = videoNum.ToString();
            Uri songUri = songs[randomIndex];
            string videoPath = "Music/Video" + numString + ".mp4";
            songPlaying.Source = new Uri(videoPath, UriKind.Relative);

            mediaPlayer.Open(songUri);
            mediaPlayer.Play();
            var slideAnimation = (Storyboard)this.Resources["SlideAnimation"];
            songPlaying.RenderTransform = new TranslateTransform();
            slideAnimation.Begin();
        }

        private void SetProfilePicture()
        {
            WitsService.PlayerManagerClient playerManagerClient = new WitsService.PlayerManagerClient();
            Player playerData = playerManagerClient.GetPlayerByUser(username);
            int profilePictureId = playerData.ProfilePictureId;
            string profilePictureFileName = profilePictureId + ".png";
            string profilePicturePath = "ProfilePictures/" + profilePictureFileName;
            Uri profilePictureUri = new Uri(profilePicturePath, UriKind.Relative);
            profilePicture.Source = new BitmapImage(profilePictureUri);
            userName.Content = username;
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

        private void OpenGameWindow(object sender, MouseButtonEventArgs e)
        {
            mediaPlayer.Stop();
            LobbyPage lobbyPage = new LobbyPage();
            this.NavigationService.Navigate(lobbyPage);
        }
        private void LoadConnectedFriends()
        {
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient();
            string[] connectedFriendsArray = client.GetConnectedFriends(UserSingleton.Instance.Username, client.GetConnectedUsers());
            List<string> connectedFriends = new List<string>(connectedFriendsArray);
            string usersText = string.Join(", ", connectedFriends);

            Dispatcher.Invoke(() =>
            {
                gridOnlineFriends.Children.Clear();
                setFriendsMenu(connectedFriends);
            });

            Console.WriteLine(connectedFriendsArray + "usersText " + usersText + "ConnectedUser" + connectedFriends);
            Task.Delay(5000).ContinueWith(t => LoadConnectedFriends());
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
            try
            {
                WitsService.GameManagerClient client = new WitsService.GameManagerClient();
                client.CreateGame(newGameId, UserSingleton.Instance.Username, 6);
                mediaPlayer.Stop();
                GameSingleton.Instance.SetGameId(newGameId);
                LobbyPage lobbyPage = new LobbyPage();
                this.NavigationService.Navigate(lobbyPage);
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void JoinExistingGame(object sender, MouseButtonEventArgs e)
        {
            var window = new InsertGameIdWindow();
            var result = window.ShowDialog();

            if (result == true) 
            {
                int existingGameId = window.gameId;
                try
                {
                    WitsService.GameManagerClient client = new WitsService.GameManagerClient();
                    if (client.JoinGame(existingGameId, UserSingleton.Instance.Username) == 1)
                    {
                        mediaPlayer.Stop();
                        GameSingleton.Instance.SetGameId(existingGameId);
                        LobbyPage lobbyPage = new LobbyPage();
                        this.NavigationService.Navigate(lobbyPage);
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.NotExistingGameMessage, Properties.Resources.NotExistingGame, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (FaultException ex)
                {
                    Console.WriteLine(ex.ToString());
                    MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private void openCustomization(object sender, MouseButtonEventArgs e)
        {
            ProfileCustomizationPage profileCustomizationPage = new ProfileCustomizationPage();
            this.NavigationService.Navigate(profileCustomizationPage);
        }

        private void OpenMyFriendsPage(object sender, MouseButtonEventArgs e)
        {
            MyFriendsPage myFriendsPage = new MyFriendsPage();
            this.NavigationService.Navigate(myFriendsPage);
        }
    }
}
