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
using Wits.WitsService;

namespace Wits
{
    /// <summary>
    /// Interaction logic for PlayerInGameCardUserControl.xaml
    /// </summary>
    public partial class PlayerInGameCardUserControl : UserControl, WitsService.IActiveGameCallback
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

            try
            {
                Wits.WitsService.Player player = client.GetPlayerByUser(username);
                int profilePictureId = player.ProfilePictureId;
                string profilePictureFileName = profilePictureId + ".png";
                string profilePicturePath = "ProfilePictures/" + profilePictureFileName;
                Uri profilePictureUri = new Uri(profilePicturePath, UriKind.Relative);
                imagePlayerProfile.Source = new BitmapImage(profilePictureUri);
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

        private void ExpelPlayer(object sender, RoutedEventArgs e)
        {
            string enteredPlayer = labelPlayerUsername.Content.ToString();
            InstanceContext context = new InstanceContext(this);
            WitsService.GameManagerClient client = new WitsService.GameManagerClient();
            WitsService.ActiveGameClient gameClient = new WitsService.ActiveGameClient(context);

            MessageBoxResult result = MessageBox.Show(Properties.Resources.ExpelConfirmation, Properties.Resources.Expel, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    gameClient.ExpelPlayer(enteredPlayer);

                    if (client.RemovePlayerInGame(GameSingleton.Instance.GameId, enteredPlayer) >= 1)
                    {
                        MessageBox.Show(Properties.Resources.PlayerExpelled, Properties.Resources.Expel, MessageBoxButton.OK, MessageBoxImage.Information);
                        ButtonClicked?.Invoke(this, EventArgs.Empty);
                    }
                }
                catch (FaultException ex)
                {
                    Console.WriteLine(ex.ToString());
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

        public void UpdateAnswers(Dictionary<int, string> playerAnswers)
        {
            Console.WriteLine(":)");
        }

        public void UpdateSelection(Dictionary<int, PlayerSelectedAnswer> playerSelectedAnswers)
        {
            Console.WriteLine(":)");
        }

        public void ShowEnterWager()
        {
            Console.WriteLine(":)");
        }

        public void ShowTrueAnswer()
        {
            Console.WriteLine(":)");
        }

        public void BeExpelled()
        {
            Console.WriteLine(":)");
        }
    }
}
