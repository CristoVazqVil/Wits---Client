using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Wits.Classes;

namespace Wits
{
    /// <summary>
    /// Lógica de interacción para CreateAccount.xaml
    /// </summary>
    public partial class CreateAccount : Window
    {
        public CreateAccount()
        {
            InitializeComponent();
            labelPasswordDontMatch.Visibility = Visibility.Collapsed;
            labelWeakPassword.Visibility = Visibility.Collapsed;
            labelInvalidEmail.Visibility = Visibility.Collapsed;
            backgroundVideo.Play();
        }

        private void ValidateData(object sender, MouseButtonEventArgs e)
        {
            labelPasswordDontMatch.Visibility = Visibility.Collapsed;
            labelWeakPassword.Visibility = Visibility.Collapsed;
            labelInvalidEmail.Visibility = Visibility.Collapsed;
            bool validation = true;

            try
            {
                if (IsEmailValid())
                {

                }
                else
                {
                    validation = false;
                    labelInvalidEmail.Visibility = Visibility.Visible;
                }
            }
            finally
            {
                try
                {
                    if (IsPasswordSecure())
                    {

                    }
                    else
                    {
                        validation = false;
                        labelWeakPassword.Visibility = Visibility.Visible;
                    }
                }
                finally
                {
                    if (IsPasswordTheSame())
                    {

                    }
                    else
                    {
                        validation = false;
                        labelPasswordDontMatch.Visibility = Visibility.Visible;
                    }
                }
            }

            if (validation == true)
            {
                CreateUser();
            }
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

        private void ConfirmDeleted(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(passwordBoxConfirmPassword.Password))
            {
                textBoxConfirmPassword.Visibility = Visibility.Visible;
            }
            
        }

        private void ConfirmEntered(object sender, RoutedEventArgs e)
        {
            textBoxConfirmPassword.Visibility = Visibility.Collapsed;
            
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

        private bool IsPasswordSecure()
        {
            if (Regex.IsMatch(passwordBoxPassword.Password, @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{8,}$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsPasswordTheSame()
        {
            if (passwordBoxPassword.Password.Length != passwordBoxConfirmPassword.Password.Length)
            {
                return false;
            }

            return string.Equals(passwordBoxPassword.Password, passwordBoxConfirmPassword.Password);
        }

        private bool IsEmailValid()
        {
            if (Regex.IsMatch(textBoxEmail.Text, @"^[a-zA-Z0-9._%+-]+@(gmail\.com|hotmail\.com|outlook\.com|estudiantes\.uv\.mx)$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DeleteSpaces(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void CreateUser()
        {
            WitsService.Player newPlayer = new WitsService.Player();
            newPlayer.User = textBoxUsername.Text;
            newPlayer.Email = textBoxEmail.Text;
            newPlayer.Password = EncryptPassword(passwordBoxPassword.Password);
            newPlayer.HighestScore = 0;
            newPlayer.ProfilePictureId = 1;
            newPlayer.CelebrationId = 1;

            try
            {
                WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
                if (client.AddPlayer(newPlayer) == 1)
                {
                    string sendedEmail = Mail.sendConfirmationMail(textBoxEmail.Text, textBoxUsername.Text);
                    MessageBox.Show("The user was successfully created\n" + sendedEmail, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("There was a problem...", "Failed", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (FaultException ex)
            {
                MessageBox.Show("The username is already used, use another one", "Failed", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteEmailCharacters(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            int maxLength = 149;

            if (textBox.Text.Length >= maxLength)
            {
                e.Handled = true;
            }
        }

        private void DeleteUsernameCharacters(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            int maxLength = 49;

            if (textBox.Text.Length >= maxLength)
            {
                e.Handled = true;
            }
        }

        private void RestartVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }

        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void CreateHighlight(object sender, MouseEventArgs e)
        {
            double scaleFactor = 1.1;
            double centerX = imageCreate.ActualWidth / 2;
            double centerY = imageCreate.ActualHeight / 2;

            scaleTransformCreateImage.ScaleX = scaleFactor;
            scaleTransformCreateImage.ScaleY = scaleFactor;

            translateTransformCreateImage.X = -(centerX * (scaleFactor - 1));
            translateTransformCreateImage.Y = -(centerY * (scaleFactor - 1));

            double centerX2 = labelCreate.ActualWidth / 2;
            double centerY2 = labelCreate.ActualHeight / 2;

            scaleTransformCreateLabel.ScaleX = scaleFactor;
            scaleTransformCreateLabel.ScaleY = scaleFactor;

            translateTransformCreateLabel.X = -(centerX2 * (scaleFactor - 1));
            translateTransformCreateLabel.Y = -(centerY2 * (scaleFactor - 1));
        }

        private void UndoCreateHighlight(object sender, MouseEventArgs e)
        {
            scaleTransformCreateImage.ScaleX = 1.0;
            scaleTransformCreateImage.ScaleY = 1.0;
            translateTransformCreateImage.X = 0;
            translateTransformCreateImage.Y = 0;
            scaleTransformCreateLabel.ScaleX = 1.0;
            scaleTransformCreateLabel.ScaleY = 1.0;
            translateTransformCreateLabel.X = 0;
            translateTransformCreateLabel.Y = 0;
        }

        private void BackHighlight(object sender, MouseEventArgs e)
        {
            double scaleFactor = 1.1;

            double centerX = imageBack.ActualWidth / 2;
            double centerY = imageBack.ActualHeight / 2;

            scaleTransformBack.ScaleX = scaleFactor;
            scaleTransformBack.ScaleY = scaleFactor;

            translateTransformBack.X = -(centerX * (scaleFactor - 1));
            translateTransformBack.Y = -(centerY * (scaleFactor - 1));
        }

        private void UndoBackHighlight(object sender, MouseEventArgs e)
        {
            scaleTransformBack.ScaleX = 1.0;
            scaleTransformBack.ScaleY = 1.0;
            translateTransformBack.X = 0;
            translateTransformBack.Y = 0;
        }
    }
}
