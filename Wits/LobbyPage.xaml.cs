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
    /// Interaction logic for lobbyPage.xaml
    /// </summary>
    public partial class LobbyPage : Page, WitsService.IChatManagerCallback
    {
        public int gameId = GameSingleton.Instance.GameId;
        public LobbyPage()
        {
            InitializeComponent();
            backgroundVideo.Play();
            labelGameId.Content = gameId;
            imageStartGame.Visibility = Visibility.Hidden;
            labelStartGame.Visibility = Visibility.Hidden;
            InstanceContext context = new InstanceContext(this);
            WitsService.ChatManagerClient client = new WitsService.ChatManagerClient(context);
            client.RegisterUserContext(UserSingleton.Instance.Username);
            ValidateGameLeader();
        }

        private void RestartVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }

        private void OpenGameWindow(object sender, MouseButtonEventArgs e)
        {
            BoardPage boardPage = new BoardPage();
            this.NavigationService.Navigate(boardPage);
        }

        private void StartGameHighlight(object sender, MouseEventArgs e)
        {
            double scaleFactor = 1.1;
            double centerX = imageStartGame.ActualWidth / 2;
            double centerY = imageStartGame.ActualHeight / 2;

            scaleTransformStartGame.ScaleX = scaleFactor;
            scaleTransformStartGame.ScaleY = scaleFactor;

            translateTransformStartGame.X = -(centerX * (scaleFactor - 1));
            translateTransformStartGame.Y = -(centerY * (scaleFactor - 1));

            double centerX2 = labelStartGame.ActualWidth / 2;
            double centerY2 = labelStartGame.ActualHeight / 2;

            scaleTransformStartGame2.ScaleX = scaleFactor;
            scaleTransformStartGame2.ScaleY = scaleFactor;

            translateTransformStartGame2.X = -(centerX2 * (scaleFactor - 1));
            translateTransformStartGame2.Y = -(centerY2 * (scaleFactor - 1));
        }

        private void UndoStartGameHighlight(object sender, MouseEventArgs e)
        {
            scaleTransformStartGame.ScaleX = 1.0;
            scaleTransformStartGame.ScaleY = 1.0;
            translateTransformStartGame.X = 0;
            translateTransformStartGame.Y = 0;
            scaleTransformStartGame2.ScaleX = 1.0;
            scaleTransformStartGame2.ScaleY = 1.0;
            translateTransformStartGame2.X = 0;
            translateTransformStartGame2.Y = 0;
        }

        private void InviteHighlight(object sender, MouseEventArgs e)
        {
            double scaleFactor = 1.1;
            double centerX = imageInviteFriend.ActualWidth / 2;
            double centerY = imageInviteFriend.ActualHeight / 2;

            scaleTransformInvite.ScaleX = scaleFactor;
            scaleTransformInvite.ScaleY = scaleFactor;

            translateTransformInvite.X = -(centerX * (scaleFactor - 1));
            translateTransformInvite.Y = -(centerY * (scaleFactor - 1));

            double centerX2 = labelInviteFriend.ActualWidth / 2;
            double centerY2 = labelInviteFriend.ActualHeight / 2;

            scaleTransformInvite2.ScaleX = scaleFactor;
            scaleTransformInvite2.ScaleY = scaleFactor;

            translateTransformInvite2.X = -(centerX2 * (scaleFactor - 1));
            translateTransformInvite2.Y = -(centerY2 * (scaleFactor - 1));
        }

        private void UndoInviteHighlight(object sender, MouseEventArgs e)
        {
            scaleTransformInvite.ScaleX = 1.0;
            scaleTransformInvite.ScaleY = 1.0;
            translateTransformInvite.X = 0;
            translateTransformInvite.Y = 0;
            scaleTransformInvite2.ScaleX = 1.0;
            scaleTransformInvite2.ScaleY = 1.0;
            translateTransformInvite2.X = 0;
            translateTransformInvite2.Y = 0;
        }

        private void SendMessage(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string message = textBoxMessage.Text;
                if (!string.IsNullOrWhiteSpace(message))
                {
                    try
                    {
                        InstanceContext context = new InstanceContext(this);
                        WitsService.ChatManagerClient client = new WitsService.ChatManagerClient(context);
                        client.SendNewMessage(UserSingleton.Instance.Username + ": " + message, gameId);
                        textBoxMessage.Clear();
                    }
                    catch (FaultException ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    
                }
            }
        }

        private void ValidateGameLeader()
        {
            WitsService.GameServiceClient client = new WitsService.GameServiceClient();
            try
            {
                string gameLeader = client.GetGameLeader(gameId);
                if (gameLeader.Equals(UserSingleton.Instance.Username))
                {
                    imageStartGame.Visibility = Visibility.Visible;
                    labelStartGame.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void UpdateChat(string message)
        {
            listBoxChat.Items.Add(message);
        }

        private void InviteUser(object sender, MouseButtonEventArgs e)
        {
            var window = new EnterPlayerUserWindow();
            var result = window.ShowDialog();

            if (result == true)
            {
                string invitedUser = window.playerUser;
                try
                {
                    WitsService.Player invitedPlayer = new WitsService.Player();
                    WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
                    invitedPlayer = client.GetPlayerByUser(invitedUser);

                    if (invitedPlayer != null)
                    {
                        string sendedEmail = Mail.sendInvitationMail(invitedPlayer.Email, gameId);
                        MessageBox.Show("The player has been invited\n" + sendedEmail, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("That player does not exist...", "Not Existing Player", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (FaultException ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
        private void GoBack(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
