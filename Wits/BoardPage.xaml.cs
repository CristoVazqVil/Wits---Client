using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Wits.Classes;
using Wits.WitsService;

namespace Wits
{
    /// <summary>
    /// Interaction logic for boardPage.xaml
    /// </summary>
    public partial class BoardPage : Page
    {
        private Random random = new Random();
        private int randomQuestion = 0;
        private string userName = UserSingleton.Instance.Username;

        public BoardPage()
        {
            InitializeComponent();
            backgroundVideo.Play();
            SetProfilePicture();
            Loaded += Page_Loaded;
            celebration.Play();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ShowQuestion();

        }

        private void ShowVictoryScreen()
        {
            WitsService.PlayerManagerClient playerManagerClient = new WitsService.PlayerManagerClient();
            Player playerData = playerManagerClient.GetPlayerByUser(userName);
            int profilePictureId = playerData.ProfilePictureId;
            string profilePictureFileName = profilePictureId + ".png";
            string profilePicturePath = "ProfilePictures/" + profilePictureFileName;
            
            Uri profilePictureUri = new Uri(profilePicturePath, UriKind.Relative);
            profilePicture.Source = new BitmapImage(profilePictureUri);

            int celebrationId = playerData.CelebrationId;
            string celebrationFileName = celebrationId + ".mp4";
            string celebrationPath = "Celebrations/" + celebrationFileName;

            Uri celebrationUri = new Uri(celebrationPath, UriKind.Relative);
            celebration.Source = celebrationUri;  


            Winner.Content = userName + " Wins!";


            celebration.Play();
            victoryScreen.Margin = new Thickness(0);

            DoubleAnimation showAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(2));
            victoryScreen.BeginAnimation(OpacityProperty, showAnimation);
        }


        private async Task ShowQuestion()
        {
            SetQuestion();
            await Task.Delay(2000);

            DoubleAnimation showAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
            imageQuestionFrame.BeginAnimation(OpacityProperty, showAnimation);
            textBoxQuestion.BeginAnimation(OpacityProperty, showAnimation);

            await Task.Delay(15000);

            DoubleAnimation hideAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
            imageQuestionFrame.BeginAnimation(OpacityProperty, hideAnimation);
            textBoxQuestion.BeginAnimation(OpacityProperty, hideAnimation);

            await Task.Delay(1000);
            ShowAnswer();
        }

        private async Task ShowAnswer()
        {
            SetAnswer();
            await Task.Delay(1000);

            DoubleAnimation showAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
            imageQuestionFrame.BeginAnimation(OpacityProperty, showAnimation);
            textBoxQuestion.BeginAnimation(OpacityProperty, showAnimation);

            await Task.Delay(15000);

            DoubleAnimation hideAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
            imageQuestionFrame.BeginAnimation(OpacityProperty, hideAnimation);
            textBoxQuestion.BeginAnimation(OpacityProperty, hideAnimation);

            await Task.Delay(1000);
            ShowVictoryScreen();
        }

        private void RestartBackgroundVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }

        private void SetQuestion()
        {
            randomQuestion = random.Next(1, 6);
            try
            {
                WitsService.GameManagerClient client = new WitsService.GameManagerClient();
                WitsService.Question question = client.GetQuestionByID(randomQuestion);

                if(question != null)
                {
                    string validateEN = labelRound.Content.ToString();
                    if (validateEN.Substring(0, 5).Equals("Round"))
                    {
                        textBoxQuestion.Text = question.QuestionEN;
                    }
                    else
                    {
                        textBoxQuestion.Text = question.QuestionES;
                    }
                }
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SetAnswer()
        {
            imageQuestionFrame.Source = new BitmapImage(new Uri("Images/answerFrame.png", UriKind.RelativeOrAbsolute));
            try
            {
                WitsService.GameManagerClient client = new WitsService.GameManagerClient();
                WitsService.Question answer = client.GetQuestionByID(randomQuestion);

                if (answer != null)
                {
                    string validateEN = labelRound.Content.ToString();
                    if (validateEN.Substring(0, 5).Equals("Round"))
                    {
                        textBoxQuestion.Text = answer.AnswerEN;
                    }
                    else
                    {
                        textBoxQuestion.Text = answer.AnswerES;
                    }
                }
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CelebrationLoop(object sender, RoutedEventArgs e)
        {
            celebration.Position = TimeSpan.Zero;
            celebration.Play();
        }

        private void GoToLobby(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void SetProfilePicture()
        {
            WitsService.PlayerManagerClient playerManagerClient = new WitsService.PlayerManagerClient();
            Player playerData = playerManagerClient.GetPlayerByUser(UserSingleton.Instance.Username);
            int profilePictureId = playerData.ProfilePictureId;
            string profilePictureFileName = profilePictureId + ".png";
            string profilePicturePath = "ProfilePictures/" + profilePictureFileName;
            Uri profilePictureUri = new Uri(profilePicturePath, UriKind.Relative);
            imageUserProfilePic.Source = new BitmapImage(profilePictureUri);
        }
    }
}
