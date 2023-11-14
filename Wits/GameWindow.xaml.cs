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
using System.Windows.Shapes;
using Wits.Classes;

namespace Wits
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
            framePage.Navigate(new Uri("MenuPage.xaml", UriKind.Relative));
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient();
            WitsService.ChatManagerClient playerManager = new WitsService.ChatManagerClient(context);
            client.RemoveConnectedUser(UserSingleton.Instance.Username);
            playerManager.UnregisterUserContext(UserSingleton.Instance.Username);
        }
    }
}
