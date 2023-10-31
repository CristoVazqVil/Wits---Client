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
using System.Windows.Shapes;

namespace Wits
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private string loggedInUser;
        public GameWindow()
        {
            InitializeComponent();
            framePage.Navigate(new Uri("MenuPage.xaml", UriKind.Relative));
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient();
            loggedInUser = client.GetCurrentlyLoggedInUser();
            Closing += OnWindowClosing;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient();
            client.RemoveConnectedUser(loggedInUser);
        }
    }
}
