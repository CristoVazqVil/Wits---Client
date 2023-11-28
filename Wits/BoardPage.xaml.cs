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
using Label = System.Windows.Controls.Label;

namespace Wits
{
    /// <summary>
    /// Interaction logic for boardPage.xaml
    /// </summary>
    public partial class BoardPage : Page, WitsService.IActiveGameCallback
    {
        private string userName = UserSingleton.Instance.Username;
        private int rounds = 1;
        private List<int> questionIds;
        private int gameId = GameSingleton.Instance.GameId;
        private int newQuestionId;
        private int player = GameSingleton.Instance.PlayerNumber;
        private int trueAnswer = 0;




        public BoardPage()
        {
            InitializeComponent();
            backgroundVideo.Play();
            SetProfilePicture();
            InstanceContext context = new InstanceContext(this);
            WitsService.ActiveGameClient client = new WitsService.ActiveGameClient(context);
            client.RegisterUserInGameContext(UserSingleton.Instance.Username);
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ShowQuestion();
            labelRound.Content = "Round " + 1;

        }

        private void GetQuestionIdsFromServer(int gameId)
        {
            try
            {
                WitsService.GameManagerClient client = new WitsService.GameManagerClient();
                int[] questionIdsArray = client.GetQuestionIds(gameId);

                questionIds = questionIdsArray.ToList();
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex.ToString());
            }
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

            PayCorrectAnswer();
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

        private void PayCorrectAnswer()
        {
            // Obtener la respuesta correcta para la pregunta actual
            int correctAnswer = trueAnswer;

            // Obtener las respuestas de los jugadores desde los labels
            Dictionary<int, int> playerGuesses = new Dictionary<int, int>();
            for (int i = 1; i <= 4; i++)
            {
                string labelName = "LabelAnswer" + i;
                var label = FindName(labelName) as System.Windows.Controls.Label;

                if (label != null && int.TryParse(label.Content.ToString(), out int guess))
                {
                    playerGuesses.Add(i, guess);
                }
            }

            // Encontrar las respuestas más cercanas sin pasarse
            List<int> closestPlayers = new List<int>();
            int closestDifference = int.MaxValue;

            foreach (var kvp in playerGuesses)
            {
                int playerNumber = kvp.Key;
                int guess = kvp.Value;

                int difference = Math.Abs(guess - correctAnswer);

                if (difference < closestDifference)
                {
                    closestDifference = difference;
                    closestPlayers.Clear();
                    closestPlayers.Add(playerNumber);
                }
                else if (difference == closestDifference)
                {
                    closestPlayers.Add(playerNumber);
                }
            }

            // Imprimir en la consola los nombres de los labels más cercanos
            Console.WriteLine("Closest Player(s):");
            foreach (int playerNumber in closestPlayers)
            {
                string labelName = "LabelAnswer" + playerNumber;
                Console.WriteLine(labelName);
            }
        }


        private void PlayNextRound()
        {
            labelRound.Content = "Round " + rounds;
            TextBoxPlayersAnswer.Text = "";
            imageSelectionPlayer1.Visibility = Visibility.Hidden;
            ImageAcceptWager.Visibility = Visibility.Hidden;
            imageQuestionFrame.Source = new BitmapImage(new Uri("Images/questionFrame.png", UriKind.RelativeOrAbsolute));
            Console.Write("YA VA A LIMPIAR");
            LabelAnswer1.Content = " ";
            LabelAnswer2.Content = " ";
            LabelAnswer3.Content = " ";
            LabelAnswer4.Content = " ";

            ShowQuestion();

       

        }

        private void RestartBackgroundVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }

        private async void SetQuestion()
        {
            if (questionIds == null || questionIds.Count == 0)
            {
                GetQuestionIdsFromServer(gameId);
            }

            newQuestionId = questionIds.First();
            questionIds.RemoveAt(0);

            try
            {
                WitsService.GameManagerClient client = new WitsService.GameManagerClient();
                WitsService.Question question = client.GetQuestionByID(newQuestionId);

                if (question != null)
                {
                    trueAnswer = question.TrueAnswer;

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


        private int GetRandomQuestionIdFromServer()
        {
            WitsService.GameManagerClient client = new WitsService.GameManagerClient();
            newQuestionId = client.GetRandomQuestionId();
            return newQuestionId;
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

            try
            {
                InstanceContext context = new InstanceContext(this);
                WitsService.ActiveGameClient client = new WitsService.ActiveGameClient(context);

                int playerNumber = player;

                client.SavePlayerAnswer(playerNumber, answerText, gameId);
                
                
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex.ToString());
            }

            string labelName = "LabelAnswer" + player;

            var label = FindName(labelName) as System.Windows.Controls.Label;

            if (label != null)
            {
                label.Content = answerText;
            }
            GridAllAnswers.Margin = new Thickness(0, 0, 0, 0);
        }




        public void UpdatePlayerAnswer(int playerNumber, string answer)
        {
            string labelName = "LabelAnswer" + playerNumber;
            var label = FindName(labelName) as System.Windows.Controls.Label;

            if (label != null)
            {
                label.Content = answer;
            }
        }



        private void SelectedAnswer(object sender, MouseButtonEventArgs e)
        {
            Image selectedImage = sender as Image;

            WitsService.PlayerManagerClient playerManagerClient = new WitsService.PlayerManagerClient();
            Player playerData = playerManagerClient.GetPlayerByUser(userName);
            int profilePictureId = playerData.ProfilePictureId;
            string profilePictureFileName = profilePictureId + ".png";
            string profilePicturePath = "ProfilePictures/" + profilePictureFileName;

            Uri imageSelectionPlayer1Uri = new Uri(profilePicturePath, UriKind.Relative);
            imageSelectionPlayer1.Source = new BitmapImage(imageSelectionPlayer1Uri);

            imageSelectionPlayer1.Visibility = Visibility.Visible;

            if (selectedImage != null)
            {
                switch (player) {
                    case 1:
                        switch (selectedImage.Name)
                        {
                            case "ImageAnswerPlayer1":
                                imageSelectionPlayer1.Margin = new Thickness(45, 510, 944, 118);
                                break;
                            case "ImageAnswerPlayer2":
                                imageSelectionPlayer1.Margin = new Thickness(270, 506, 719, 122);
                                break;
                            case "ImageAnswerPlayer3":
                                imageSelectionPlayer1.Margin = new Thickness(515, 510, 474, 118);
                                break;
                            case "ImageAnswerPlayer4":
                                imageSelectionPlayer1.Margin = new Thickness(754, 510, 235, 118);
                                break;
                            default:
                                break;
                        }
                        break;
                    case 2:
                        switch (selectedImage.Name)
                        {
                            case "ImageAnswerPlayer1":
                                imageSelectionPlayer1.Margin = new Thickness(204, 510, 784, 118);
                                break;
                            case "ImageAnswerPlayer2":
                                imageSelectionPlayer1.Margin = new Thickness(448, 505, 540, 123);
                                break;
                            case "ImageAnswerPlayer3":
                                imageSelectionPlayer1.Margin = new Thickness(693, 509, 295, 119);
                                break;
                            case "ImageAnswerPlayer4":
                                imageSelectionPlayer1.Margin = new Thickness(940, 505, 48, 123);
                                break;
                            default:
                                break;
                        }
                        break;
                    case 3:
                        switch (selectedImage.Name)
                        {
                            case "ImageAnswerPlayer1":
                                imageSelectionPlayer1.Margin = new Thickness(45, 510, 944, 118);
                                break;
                            case "ImageAnswerPlayer2":
                                imageSelectionPlayer1.Margin = new Thickness(285, 107, 703, 521);
                                break;
                            case "ImageAnswerPlayer3":
                                imageSelectionPlayer1.Margin = new Thickness(540, 107, 448, 521);
                                break;
                            case "ImageAnswerPlayer4":
                                imageSelectionPlayer1.Margin = new Thickness(785, 110, 203, 518);
                                break;
                            default:
                                break;
                        }
                        break;

                    case 4:
                        switch (selectedImage.Name)
                        {
                            case "ImageAnswerPlayer1":
                                imageSelectionPlayer1.Margin = new Thickness(218, 106, 770, 522);
                                break;
                            case "ImageAnswerPlayer2":
                                imageSelectionPlayer1.Margin = new Thickness(461, 107, 527, 521);
                                break;
                            case "ImageAnswerPlayer3":
                                imageSelectionPlayer1.Margin = new Thickness(693, 124, 295, 504);
                                break;
                            case "ImageAnswerPlayer4":
                                imageSelectionPlayer1.Margin = new Thickness(940, 107, 48, 521);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }

                // Hacer visible ImageUserSelection
                imageSelectionPlayer1.Visibility = Visibility.Visible;

                // Otras acciones que desees realizar cuando se selecciona una respuesta
                // ...

                // Aquí puedes llamar a la función EnterWager o cualquier otra acción necesaria
            }
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


        private void SaveWager(object sender, MouseButtonEventArgs e)
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

         public void UpdateAnswers(Dictionary<int, string> playerAnswers)
        {
            Console.WriteLine("YA FUE LLAMADO");
            Console.WriteLine("Player Answers CLIENT:");
            foreach (var kvp in playerAnswers)
            {
                Console.WriteLine($"Player {kvp.Key}: {kvp.Value}");
            }
            foreach (var kvp in playerAnswers)
            {
                int playerNumber = kvp.Key;
                string answer = kvp.Value;

                string labelName = "LabelAnswer" + playerNumber;

                var label = FindName(labelName) as System.Windows.Controls.Label;

                if (label != null)
                {
                    label.Content = answer;
                }
            }
        }

        
    }
}
