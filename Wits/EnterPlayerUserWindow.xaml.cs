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
    /// Interaction logic for EnterPlayerUserWindow.xaml
    /// </summary>
    public partial class EnterPlayerUserWindow : Window
    {
        private string playerUser;

        public string PlayerUser
        {
            get { return playerUser; }
            private set { playerUser = value; }
        }

        public EnterPlayerUserWindow()
        {
            InitializeComponent();
            backgroundVideo.Play();
        }

        private void RestartBackgroundVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }

        private void buttonSendUser_Click(object sender, RoutedEventArgs e)
        {
            PlayerUser = textBoxPlayerUser.Text;

            if (!string.IsNullOrEmpty(PlayerUser))
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void DeleteSpaces(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }
}
