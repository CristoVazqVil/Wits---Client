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
        private string userName = UserSingleton.Instance.Username;
        private List<int> usedQuestionIds = new List<int>();
        private int rounds = 1;
        private int newQuestionId = 0;


        public BoardPage()
        {
            InitializeComponent();
            backgroundVideo.Play();
            SetProfilePicture();
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ShowQuestion();
            labelRound.Content = "Round " + 1;

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

            GridQuestionsAndAnswers.Margin = new Thickness(1, 1, -1, -1);

            SetQuestion();
            await Task.Delay(2000);

            DoubleAnimation showAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
            imageQuestionFrame.BeginAnimation(OpacityProperty, showAnimation);
            textBoxQuestion.BeginAnimation(OpacityProperty, showAnimation);

            await Task.Delay(8000);

            DoubleAnimation hideAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
            imageQuestionFrame.BeginAnimation(OpacityProperty, hideAnimation);
            textBoxQuestion.BeginAnimation(OpacityProperty, hideAnimation);

            await Task.Delay(1000);
            LabelInstrucion.Content = "Enter Your Answer:";
            GridEnterAnswer.Margin = new Thickness(0, 0, 0, 0);
        }

        private async Task ShowAnswer()
        {
            GridAllAnswers.Margin = new Thickness(0, 754, 0, -754);
            SetAnswer();
            await Task.Delay(1000);

            DoubleAnimation showAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
            imageQuestionFrame.BeginAnimation(OpacityProperty, showAnimation);
            textBoxQuestion.BeginAnimation(OpacityProperty, showAnimation);

            await Task.Delay(8000);

            DoubleAnimation hideAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
            imageQuestionFrame.BeginAnimation(OpacityProperty, hideAnimation);
            textBoxQuestion.BeginAnimation(OpacityProperty, hideAnimation);

            if (int.TryParse(TextBoxPlayersAnswer.Text, out int wagerAmount))
            {
                int currentChips = int.Parse(labelChips.Content.ToString());
                int newChips = currentChips + wagerAmount;

                labelChips.Content = newChips.ToString();
            }

            await Task.Delay(1000);


            ShowRoundWinners();

            await Task.Delay(5000);
            GridRoundWinners.Margin = new Thickness(1177, -954, -1177, 954);

            rounds++;
            

            if (rounds < 7)
            {
                PlayNextRound();
            }
            else
            {
                labelRound.Content = "";
                ShowVictoryScreen();
            }

        }

        private void PlayNextRound()
        {
            labelRound.Content = "Round " + rounds;
            TextBoxPlayersAnswer.Text = "";
            ImageUserSelection.Visibility = Visibility.Hidden;
            ImageAcceptWager.Visibility = Visibility.Hidden;
            imageQuestionFrame.Source = new BitmapImage(new Uri("Images/questionFrame.png", UriKind.RelativeOrAbsolute));
            ShowQuestion();

        }

        private void RestartBackgroundVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }

        private void SetQuestion()
        {

          
            do
            {
                newQuestionId = random.Next(1, 16);
            } while (usedQuestionIds.Contains(newQuestionId));

            Console.WriteLine(usedQuestionIds.ToString()+ "USED QUESTION");
            Console.WriteLine(newQuestionId.ToString() + "NEW QUESTION");


            try
            {
                WitsService.GameManagerClient client = new WitsService.GameManagerClient();
                WitsService.Question question = client.GetQuestionByID(newQuestionId);

                if (question != null)
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
                    usedQuestionIds.Add(newQuestionId);
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
                WitsService.Question answer = client.GetQuestionByID(newQuestionId);

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
        private void AnswerIsNumber(object sender, TextCompositionEventArgs e)
        {
            if (!IsNumeric(e.Text))
            {
                e.Handled = true;
            }
        }

        private static bool IsNumeric(string text)
        {
            return int.TryParse(text, out _);
        }

        private void SaveAnswer(object sender, MouseButtonEventArgs e)
        {
            LabelInstrucion.Content = "Enter Your Answer:";
            GridEnterAnswer.Margin = new Thickness(1177, 0, -1177, 0);
            string answerText = TextBoxPlayersAnswer.Text;
            LabelAnswer1.Content = answerText;
            GridAllAnswers.Margin = new Thickness(0, 0, 0, 0);
        }



        private async void SelectedAnswer(object sender, MouseButtonEventArgs e)
        {
            WitsService.PlayerManagerClient playerManagerClient = new WitsService.PlayerManagerClient();
            Player playerData = playerManagerClient.GetPlayerByUser(userName);
            int profilePictureId = playerData.ProfilePictureId;
            string profilePictureFileName = profilePictureId + ".png";
            string profilePicturePath = "ProfilePictures/" + profilePictureFileName;

            Uri ImageUserSelectionUri = new Uri(profilePicturePath, UriKind.Relative);
            ImageUserSelection.Source = new BitmapImage(ImageUserSelectionUri);

            ImageUserSelection.Visibility = Visibility.Visible;

            await Task.Delay(6000);

            GridAllAnswers.Margin = new Thickness(0, 754, 0, -754);
            EnterWager();
        }



        private void EnterWager()
        {
            TextBoxPlayersAnswer.Text = "";
            ImageAcceptWager.Visibility = Visibility.Visible;
            GridEnterAnswer.Margin = new Thickness(0, 0, 0, 0);
            LabelInstrucion.Content = "How much will you wager?";
            
        }

        private void ShowRoundWinners()
        {
            WitsService.PlayerManagerClient playerManagerClient = new WitsService.PlayerManagerClient();
            Player playerData = playerManagerClient.GetPlayerByUser(userName);
            int profilePictureId = playerData.ProfilePictureId;
            string ImageProfilePicture1FileName = profilePictureId + ".png";
            string ImageProfilePicture1Path = "ProfilePictures/" + ImageProfilePicture1FileName;

            Uri ImageProfilePicture1Uri = new Uri(ImageProfilePicture1Path, UriKind.Relative);
            ImageProfilePicture1.Source = new BitmapImage(ImageProfilePicture1Uri);

            GridRoundWinners.Margin = new Thickness(0, 0, 0, 0);

        }


        private  void SaveWager(object sender, MouseButtonEventArgs e)
        {

            int chipsAvailable = int.Parse(labelChips.Content.ToString());

            if (int.TryParse(TextBoxPlayersAnswer.Text, out int wagerAmount) && wagerAmount <= chipsAvailable)
            {
                GridEnterAnswer.Margin = new Thickness(1177, 0, -1177, 0);
                ShowAnswer();
            }
            else
            {
                GridWarning.Margin = new Thickness(0, 0, 0, 0);
                TextBoxPlayersAnswer.Text = "";
            }
           
        }

        private void AcceptWarning(object sender, MouseButtonEventArgs e)
        {
            GridWarning.Margin = new Thickness(1177, 822, -1177, -822);
        }



    }
}
