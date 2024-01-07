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
using System.Media;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Media.Media3D;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Security;
using System.ServiceModel;
using System.Globalization;
using System.Threading;
using Wits.Classes;
using System.IO;

namespace Wits
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SoundPlayer music;
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            music = new SoundPlayer("Music/Death By Glamour - Undertale.wav"); 
            music.Play();
            backgroundVideo.Play();
            logoVideo.Play();
        }

        private void OpenCreateAccountWindow(object sender, MouseButtonEventArgs e)
        {
            CreateAccountWindow createWindow = new CreateAccountWindow();
            createWindow.ShowDialog(); 
        }

        private void LoginGame(object sender, MouseButtonEventArgs e)
        {
            try
            {
                WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
                WitsService.Player player = client.GetPlayerByUserAndPassword(textBoxUser.Text, EncryptPassword(passwordBoxPassword.Password));

                if (player.Username != null && player.UserPassword != null)
                {
                    if (!client.IsPlayerLogged(textBoxUser.Text))
                    {
                        if (music != null)
                        {
                            music.Stop();
                            music.Dispose();
                        }
                        OpenGameWindow(textBoxUser.Text);
                    }
                    else
                    {
                        MessageBox.Show(Properties.Resources.AlreadyLogged, Properties.Resources.Failed, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.WrongPassword, Properties.Resources.Failed, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            } 
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OpenGameWindow(string username)
        {
            UserSingleton.Instance.SetUsername(username);
            GameWindow gameWindow = new GameWindow();
            gameWindow.Show();
            this.Close();
        }

        private void RestartBackgroundVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }

        private void LoginHighlight(object sender, MouseEventArgs e)
        {
            double scaleFactor = 1.1;
            double centerX = imageLogin.ActualWidth / 2;
            double centerY = imageLogin.ActualHeight / 2;

            scaleTransformLogin.ScaleX = scaleFactor;
            scaleTransformLogin.ScaleY = scaleFactor;

            translateTransformLogin.X = -(centerX * (scaleFactor - 1));
            translateTransformLogin.Y = -(centerY * (scaleFactor - 1));

            double centerX2 = labelGo.ActualWidth / 2;
            double centerY2 = labelGo.ActualHeight / 2;

            scaleTransformGo.ScaleX = scaleFactor;
            scaleTransformGo.ScaleY = scaleFactor;

            translateTransformGo.X = -(centerX2 * (scaleFactor - 1));
            translateTransformGo.Y = -(centerY2 * (scaleFactor - 1));
        }

        private void UndoLoginHighlight(object sender, MouseEventArgs e)
        {
            scaleTransformLogin.ScaleX = 1.0;
            scaleTransformLogin.ScaleY = 1.0;
            translateTransformLogin.X = 0;
            translateTransformLogin.Y = 0;
            scaleTransformGo.ScaleX = 1.0;
            scaleTransformGo.ScaleY = 1.0;
            translateTransformGo.X = 0;
            translateTransformGo.Y = 0;
        }

        private void GetOneHighlight(object sender, MouseEventArgs e)
        {
            double scaleFactor = 1.1;

            double centerX = textBlockGetOne.ActualWidth / 2;
            double centerY = textBlockGetOne.ActualHeight / 2;

            scaleTransformGetOne.ScaleX = scaleFactor;
            scaleTransformGetOne.ScaleY = scaleFactor;

            translateTransformGetOne.X = -(centerX * (scaleFactor - 1));
            translateTransformGetOne.Y = -(centerY * (scaleFactor - 1));
        }

        private void UndoGetOneHighlight(object sender, MouseEventArgs e)
        {
            scaleTransformGetOne.ScaleX = 1.0;
            scaleTransformGetOne.ScaleY = 1.0;
            translateTransformGetOne.X = 0;
            translateTransformGetOne.Y = 0;
        }

        private String EncryptPassword(string password)
        {
            String encryptedPassword;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                encryptedPassword = hashedPassword;
            }

            return encryptedPassword;
        }

        private void PasswordEntered(object sender, RoutedEventArgs e)
        {
            textBoxEnterPassword.Visibility = Visibility.Collapsed;
        }

        private void PasswordDeleted(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(passwordBoxPassword.Password))
            {
                textBoxEnterPassword.Visibility = Visibility.Visible;
            }
        }

        private void EnHighlight(object sender, MouseEventArgs e)
        {
            double scaleFactor = 1.1;

            double centerX = imageUK.ActualWidth / 2;
            double centerY = imageUK.ActualHeight / 2;

            scaleTransformUK.ScaleX = scaleFactor;
            scaleTransformUK.ScaleY = scaleFactor;

            translateTransformUK.X = -(centerX * (scaleFactor - 1));
            translateTransformUK.Y = -(centerY * (scaleFactor - 1));
        }

        private void UndoEnHighlight(object sender, MouseEventArgs e)
        {
            scaleTransformUK.ScaleX = 1.0;
            scaleTransformUK.ScaleY = 1.0;
            translateTransformUK.X = 0;
            translateTransformUK.Y = 0;
        }

        private void EsHighlight(object sender, MouseEventArgs e)
        {
            double scaleFactor = 1.1;

            double centerX = imageMex.ActualWidth / 2;
            double centerY = imageMex.ActualHeight / 2;

            scaleTransformMex.ScaleX = scaleFactor;
            scaleTransformMex.ScaleY = scaleFactor;

            translateTransformMex.X = -(centerX * (scaleFactor - 1));
            translateTransformMex.Y = -(centerY * (scaleFactor - 1));
        }

        private void UndoEsHighlight(object sender, MouseEventArgs e)
        {
            scaleTransformMex.ScaleX = 1.0;
            scaleTransformMex.ScaleY = 1.0;
            translateTransformMex.X = 0;
            translateTransformMex.Y = 0;
        }

        private void ChangeSpanish(object sender, MouseButtonEventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
            RestartWindow();
        }

        private void ChangeEnglish(object sender, MouseButtonEventArgs e)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            RestartWindow();
        }

        private void RestartWindow()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void RestartLogoVideo(object sender, RoutedEventArgs e)
        {
            logoVideo.Position = TimeSpan.Zero;
            logoVideo.Play();
        }

        private void EnterAsAGuest(object sender, MouseButtonEventArgs e)
        {
            WitsService.Player temporaryPlayer = CreateTemporaryPlayer();
            try
            {
                WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
                if (client.AddPlayer(temporaryPlayer) == 1)
                {
                    OpenGameWindow(temporaryPlayer.Username);
                }
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private WitsService.Player CreateTemporaryPlayer()
        {
            WitsService.Player temporaryPlayer = new WitsService.Player();

            temporaryPlayer.Username = "Guest" + random.Next(1000, 10000).ToString();
            temporaryPlayer.Email = random.Next(100000000, 1000000000).ToString();
            temporaryPlayer.UserPassword = random.Next(100000000, 1000000000).ToString();
            temporaryPlayer.HighestScore = 0;
            temporaryPlayer.ProfilePictureId = 1;
            temporaryPlayer.CelebrationId = 1;

            return temporaryPlayer;
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
