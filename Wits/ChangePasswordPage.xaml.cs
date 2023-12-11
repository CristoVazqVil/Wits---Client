using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Wits.WitsService;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace Wits
{
    /// <summary>
    /// Interaction logic for ChangePasswordPage.xaml
    /// </summary>
    public partial class ChangePasswordPage : Page
    {
        private const int SUCCESS = 1;

        public ChangePasswordPage()
        {
            InitializeComponent();
            backgroundVideo.Play();
            SetProfilePicture();
            labelNotCurrentPassword.Visibility = Visibility.Collapsed;
            labelPasswordError.Visibility = Visibility.Collapsed;
            labelPasswordUnmatch.Visibility = Visibility.Collapsed;
            labelSamePassword.Visibility = Visibility.Collapsed;
        }

        private void RestartVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }

        private void GoBack(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void ChangePassword()
        {
            string newPassword = EncryptPassword(passwordBoxConfirmNewPassword.Password);
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
            try
            {
                if (client.UpdatePassword(UserSingleton.Instance.Username, newPassword) == SUCCESS)
                {
                    MessageBox.Show(Properties.Resources.PasswordChanged, Properties.Resources.Success, MessageBoxButton.OK, MessageBoxImage.Information);
                    this.NavigationService.GoBack();
                }
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

        private void CancelChangePassword(object sender, MouseButtonEventArgs e)
        {
            ClearData();
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

        private bool IsTheCurrentPassword()
        {
            bool validator = false;
            WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
            try
            {
                WitsService.Player player = client.GetPlayerByUser(UserSingleton.Instance.Username);
                if (player.Password.Equals(EncryptPassword(passwordBoxCurrentPassword.Password)))
                {
                    validator = true;
                }

                return validator;
            }
            catch (FaultException ex)
            {
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                return validator;
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                return validator;
            }
            catch (CommunicationException ex)
            {
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                return validator;
            }
        }

        private bool IsPasswordSecure()
        {
            if (Regex.IsMatch(passwordBoxNewPassword.Password, @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{8,}$"))
            {
                return true;
            }

            return false;
        }

        private bool IsPasswordTheSame()
        {
            if (passwordBoxNewPassword.Password.Length == passwordBoxConfirmNewPassword.Password.Length)
            {
                return string.Equals(passwordBoxNewPassword.Password, passwordBoxConfirmNewPassword.Password);
            }

            return false;
        }

        private bool IsEmpty()
        {
            if (string.IsNullOrEmpty(passwordBoxNewPassword.Password) || string.IsNullOrEmpty(passwordBoxConfirmNewPassword.Password) ||
                string.IsNullOrEmpty(passwordBoxCurrentPassword.Password))
            {
                return true;
            }

            return false;
        }

        private void ValidateData(object sender, MouseButtonEventArgs e)
        {
            labelNotCurrentPassword.Visibility = Visibility.Collapsed;
            labelPasswordError.Visibility = Visibility.Collapsed;
            labelPasswordUnmatch.Visibility = Visibility.Collapsed;
            labelSamePassword.Visibility = Visibility.Collapsed;
            bool validation = true;

            if (IsEmpty())
            {
                validation = false;
                labelPasswordError.Content = Properties.Resources.EmptyFields;
                labelPasswordError.Visibility = Visibility.Visible;
            }
            else
            {
                try
                {
                    if (!IsTheCurrentPassword())
                    {
                        validation = false;
                        labelNotCurrentPassword.Visibility = Visibility.Visible;
                    }
                }
                finally
                {
                    try
                    {
                        if (passwordBoxCurrentPassword.Password.Equals(passwordBoxNewPassword.Password))
                        {
                            validation = false;
                            labelSamePassword.Visibility = Visibility.Visible;
                        }
                    }
                    finally
                    {
                        try
                        {
                            if (!IsPasswordSecure())
                            {
                                validation = false;
                                labelPasswordError.Content = Properties.Resources.WeakPassword;
                                labelPasswordError.Visibility = Visibility.Visible;
                            }
                        }
                        finally
                        {
                            if (!IsPasswordTheSame())
                            {
                                validation = false;
                                labelPasswordUnmatch.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
            }

            if (validation)
            {
                ChangePassword();
            }
        }

        private void ClearData()
        {
            labelNotCurrentPassword.Visibility = Visibility.Collapsed;
            labelPasswordError.Visibility = Visibility.Collapsed;
            labelPasswordUnmatch.Visibility = Visibility.Collapsed;
            labelSamePassword.Visibility = Visibility.Collapsed;
            passwordBoxConfirmNewPassword.Clear();
            passwordBoxCurrentPassword.Clear();
            passwordBoxNewPassword.Clear();
        }

        private void SetProfilePicture()
        {
            WitsService.PlayerManagerClient playerManagerClient = new WitsService.PlayerManagerClient();
            try
            {
                Player playerData = playerManagerClient.GetPlayerByUser(UserSingleton.Instance.Username);
                int profilePictureId = playerData.ProfilePictureId;
                string profilePictureFileName = profilePictureId + ".png";
                string profilePicturePath = "ProfilePictures/" + profilePictureFileName;
                Uri profilePictureUri = new Uri(profilePicturePath, UriKind.Relative);
                imageProfilePic.Source = new BitmapImage(profilePictureUri);
                labelUsername.Content = UserSingleton.Instance.Username;
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
    }
}
