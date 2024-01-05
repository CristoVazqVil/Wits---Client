using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using Wits.Classes;
using Wits.WitsService;

namespace Wits
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window, WitsService.IConnectedUsersCallback
    {
        public GameWindow()
        {
            InitializeComponent();
            framePage.Navigate(new Uri("MenuPage.xaml", UriKind.Relative));
            Closing += OnWindowClosing;
        }

        public void UpdateConnectedFriends()
        {
            Console.WriteLine("Friends Updated in Menu");
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            WitsService.ConnectedUsersClient connectedClient = new WitsService.ConnectedUsersClient(context);
            WitsService.PlayerManagerClient playerClient = new WitsService.PlayerManagerClient();

            try
            {
                connectedClient.RemoveFromEverywhere(UserSingleton.Instance.Username);
                if (UserSingleton.Instance.Username.Substring(0, 5).Equals("Guest"))
                {
                    playerClient.DeletePlayer(UserSingleton.Instance.Username);
                }
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
            }
        }
    }
}
