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
    /// Interaction logic for lobbyPage.xaml
    /// </summary>
    public partial class LobbyPage : Page
    {
        public LobbyPage()
        {
            InitializeComponent();
            backgroundVideo.Play();
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
    }
}
