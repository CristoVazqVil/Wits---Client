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
using Wits.Classes;

namespace Wits
{
    /// <summary>
    /// Interaction logic for InsertGameIdWindow.xaml
    /// </summary>
    public partial class InsertGameIdWindow : Window
    {
        private int gameId;

        public int GameId
        {
            get { return gameId; }
            private set { gameId = value; }
        }

        public InsertGameIdWindow()
        {
            InitializeComponent();
            backgroundVideo.Play();
        }

        private void RestartBackgroundVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }

        private void buttonSendId_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string gameIdString = textBoxGameId.Text;
                if (!string.IsNullOrEmpty(gameIdString))
                {
                    GameId = int.Parse(gameIdString);

                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (FormatException ex)
            {
                Logger.LogErrorException(ex);
            }
        }

        private void OnlyNumbers(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
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
