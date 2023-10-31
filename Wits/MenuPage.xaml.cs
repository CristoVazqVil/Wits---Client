using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reflection.Emit;
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

namespace Wits
{
    /// <summary>
    /// Lógica de interacción para Menu.xaml
    /// </summary>
    public partial class Menu : Page
    {
        private MediaPlayer mediaPlayer;
        private Random random = new Random();
        private List<Uri> songs = new List<Uri>()
        {
            new Uri(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song1.wav", UriKind.Absolute),
            new Uri(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song2.wav", UriKind.Absolute),
            new Uri(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song3.wav", UriKind.Absolute),
            new Uri(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song4.wav", UriKind.Absolute),
            new Uri(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song5.wav", UriKind.Absolute),
            new Uri(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song6.wav", UriKind.Absolute),
            new Uri(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song7.wav", UriKind.Absolute),
            new Uri(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song8.wav", UriKind.Absolute)
        };

        public Menu()
        {
            InitializeComponent();
            backgroundVideo.Play();
            mediaPlayer = new MediaPlayer();
            mediaPlayer.MediaEnded += SongEnded;
            PlayRandomSong();
            LoadConnectedUsers();
        }

        private void PlayRandomSong()
        {
            int randomIndex = random.Next(songs.Count);
            int videoNum = randomIndex + 1;
            String numString = videoNum.ToString();
            Uri songUri = songs[randomIndex];
            string videoPath = @"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Video" + numString + ".mp4";
            songPlaying.Source = new Uri(videoPath);

            mediaPlayer.Open(songUri);
            mediaPlayer.Play();
            var slideAnimation = (Storyboard)this.Resources["SlideAnimation"];
            songPlaying.RenderTransform = new TranslateTransform();
            slideAnimation.Begin();
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
        private void LoadConnectedUsers()
        {
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient();
            string[] connectedUsersArray = client.GetConnectedUsers();
            List<string> connectedUsers = new List<string>(connectedUsersArray);

            string usersText = string.Join(", ", connectedUsers);

            Dispatcher.Invoke(() =>
            {
                textBlockOnlineFriends.Text = "Connected Users: " + usersText;
            });

            Console.WriteLine(connectedUsersArray + "usersText " + usersText + "ConnectedUser" + connectedUsers);

            Task.Delay(5000).ContinueWith(t => LoadConnectedUsers());
        }

    }
}
