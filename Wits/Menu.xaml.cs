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
            backgroundVideo.Play();
            LoadConnectedUsers();
            Closing += OnWindowClosing;
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient();
            loggedInUser = client.GetCurrentlyLoggedInUser();
            Console.WriteLine("HOLA main" + loggedInUser);
        }

        private void LoadConnectedUsers()
        {
            // Llamar al servicio para obtener la lista de usuarios conectados
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient();
            string[] connectedUsersArray = client.GetConnectedUsers();
            List<string> connectedUsers = new List<string>(connectedUsersArray);

            // Crear una cadena con los usuarios conectados
            string usersText = string.Join(", ", connectedUsers);

            // Actualizar el contenido del TextBlock en el hilo de la interfaz de usuario
            Dispatcher.Invoke(() =>
            {
                usersTextBlock.Text = "Usuarios Conectados: " + usersText;
            });

            Console.WriteLine(connectedUsersArray + "usersText " + usersText + "ConecteduUser" + connectedUsers);

            // Llamar a este método nuevamente después de un cierto tiempo (por ejemplo, 5 segundos)
            Task.Delay(5000).ContinueWith(t => LoadConnectedUsers());
        }



        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Llamar al servicio para notificar al servidor que el usuario se está desconectando
            WitsService.ConnectedUsersClient client = new WitsService.ConnectedUsersClient();
            client.RemoveConnectedUser(loggedInUser);
            Console.WriteLine("HOLA desde el cierre " + loggedInUser);

        }


        private void RestartVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }
    }
}
