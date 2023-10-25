using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
using System.Windows.Shapes;

namespace Wits
{
    /// <summary>
    /// Lógica de interacción para Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        private SoundPlayer music;
        private string loggedInUser;

        public Menu()
        {
            InitializeComponent();
            Random randomNum = new Random();
            int songNum = randomNum.Next(1, 9);
            String numString = songNum.ToString();
            music = new SoundPlayer(@"C:\Users\dplat\OneDrive\Documentos\Codes n shit\WITS\Wits---Client\Wits\Music\Song" + numString + ".wav");
            Console.WriteLine("Song " + numString);
            music.Play();

            string videoPath = @"C:\Users\dplat\OneDrive\Documentos\Codes n shit\WITS\Wits---Client\Wits\Music\Video" + numString + ".mp4";
            songPlaying.Source = new Uri(videoPath);
            backgroundVideo.Play();
            LoadConnectedUsers();
            Closing += OnWindowClosing;
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient();
            loggedInUser = client.GetCurrentlyLoggedInUser();
            var slideAnimation = (Storyboard)this.Resources["SlideAnimation"];
            songPlaying.RenderTransform = new TranslateTransform();
            slideAnimation.Begin();
        }

        private void LoadConnectedUsers()
        {
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient();
            string[] connectedUsersArray = client.GetConnectedUsers();
            List<string> connectedUsers = new List<string>(connectedUsersArray);

            string usersText = string.Join(", ", connectedUsers);

            Dispatcher.Invoke(() =>
            {
                usersTextBlock.Text = "Usuarios Conectados: " + usersText;
            });

            Console.WriteLine(connectedUsersArray + "usersText " + usersText + "ConecteduUser" + connectedUsers);

            Task.Delay(5000).ContinueWith(t => LoadConnectedUsers());
        }



        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient();
            client.RemoveConnectedUser(loggedInUser);

        }


        private void RestartVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }
    }
}
