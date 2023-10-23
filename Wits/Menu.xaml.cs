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

        public Menu()
        {
            InitializeComponent();
            Random randomNum = new Random();
            int songNum = randomNum.Next(1, 9);
            String numString = songNum.ToString();
            music = new SoundPlayer(@"C:\Users\dplat\OneDrive\Documentos\Codes n shit\WITS\Wits---Client\Wits\Music\Song" + numString + ".wav");
            Console.WriteLine("Song " + numString);
            music.Play();
            backgroundVideo.Play();
            LoadConnectedUsers();
        }

        private void LoadConnectedUsers()
        {
            // Llamar al servicio para obtener la lista de usuarios conectados
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient();
            string[] connectedUsersArray = client.GetConnectedUsers();
            List<string> connectedUsers = new List<string>(connectedUsersArray);

            // Crear una cadena con los usuarios conectados
            string usersText = string.Join(", ", connectedUsers);

            // Actualizar el contenido del TextBlock
            usersTextBlock.Text = "Usuarios Conectados: " + usersText;
            Console.WriteLine(connectedUsersArray + "usersText " +  usersText + "ConecteduUser" + connectedUsers);
        }





        private void RestartVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }
    }
}
