using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
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
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private string userName = UserSingleton.Instance.Username;
        private int rounds = 1;
        private List<int> questionIds;
        private int gameId = GameSingleton.Instance.GameId;
        private int newQuestionId;
        private int player = GameSingleton.Instance.PlayerNumber;
        private int trueAnswer = 0;
        private List<int> correctPlayers = new List<int>();
        private Dictionary<int, PlayerSelectedAnswer> playerSelectedAnswers;

        public BoardPage()
        {
            InitializeComponent();
            backgroundVideo.Play();
            mediaPlayer.MediaEnded += SongEnded;
            PlaySong();
            ValidateGameLeader();
            SetProfilePicture();
            InstanceContext context = new InstanceContext(this);
            WitsService.ActiveGameClient client = new WitsService.ActiveGameClient(context);
            labelAnswer1.Content = "";
            labelAnswer2.Content = "";
            labelAnswer3.Content = "";
            labelAnswer4.Content = "";

            bool isRegistered = false;
            client.GameEnded(gameId, player, isRegistered);

            try
            {
                client.RegisterUserInGameContext(UserSingleton.Instance.Username);
                Loaded += Page_Loaded;
                bool isReady = false;
                client.ReadyToWager(gameId, player, isReady);
                client.ReadyToShowAnswer(gameId, player, isReady);
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
        }

        private void SongEnded(object sender, EventArgs e)
        {
            PlaySong();
        }

        private void PlaySong()
        {
            mediaPlayer.Open(new Uri("Music/Elite Four Battle.wav", UriKind.Relative));
            mediaPlayer.Play();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ShowQuestion();
            labelRound.Content = Properties.Resources.Round + 1;
        }

        private void SetPlayers()
        {
            WitsService.GameManagerClient client = new WitsService.GameManagerClient();

            try
            {
                string[] playersArray = client.GetPlayersOfGameExceptLeader(gameId, userName);
                List<string> playerList = new List<string>(playersArray);
                PlayersInGameUserControl players = new PlayersInGameUserControl();
                players.ImageCloseClicked += Players_ImageCloseClicked;
                players.SetPlayers(playerList);
                gridPlayersInGame.Children.Add(players);
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
        }

        private void Players_ImageCloseClicked(object sender, EventArgs e)
        {
            gridPlayersInGame.Margin = new Thickness(-899, -594, 1211, 858);
        }

        private void GetQuestionIdsFromServer(int gameId)
        {
            try
            {
                WitsService.GameManagerClient client = new WitsService.GameManagerClient();
                int[] questionIdsArray = client.GetQuestionIds(gameId);

                questionIds = questionIdsArray.ToList();
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
        }

        private async Task ShowQuestion()
        {
            gridQuestionsAndAnswers.Margin = new Thickness(1, 1, -1, -1);

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

            labelInstrucion.Content = Properties.Resources.EnterAnswer;
            gridEnterAnswer.Margin = new Thickness(0, 0, 0, 0);
        }

        private async Task ShowAnswer()
        {
            gridAllAnswers.Margin = new Thickness(0, 754, 0, -754);
            SetAnswer();
            await Task.Delay(1000);

            DoubleAnimation showAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
            imageQuestionFrame.BeginAnimation(OpacityProperty, showAnimation);
            textBoxQuestion.BeginAnimation(OpacityProperty, showAnimation);
            await Task.Delay(8000);

            DoubleAnimation hideAnimation = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
            imageQuestionFrame.BeginAnimation(OpacityProperty, hideAnimation);
            textBoxQuestion.BeginAnimation(OpacityProperty, hideAnimation);
            await Task.Delay(1000);

            PayCorrectAnswer(playerSelectedAnswers);
            ShowRoundWinners();
            await Task.Delay(5000);

            rounds++;

            if (rounds < 7)
            {
                PlayNextRound();
            }
            else
            {
                labelRound.Content = "";

                string scoreText = labelChips.Content.ToString();

                InstanceContext context = new InstanceContext(this);
                WitsService.ActiveGameClient client = new WitsService.ActiveGameClient(context);
                WitsService.PlayerManagerClient playerManagerClient = new WitsService.PlayerManagerClient();
                Player playerData = playerManagerClient.GetPlayerByUser(UserSingleton.Instance.Username);
                int profilePictureId = playerData.ProfilePictureId;
                int celebrationId = playerData.CelebrationId;



                if (int.TryParse(scoreText, out int score))
                {

                    bool isRegistered = true;
                    client.WhoWon(gameId, player, userName, celebrationId, score, profilePictureId);
                    client.GameEnded(gameId, player, isRegistered);
                }

            }

        }

        private void SelectedAnswer(object sender, MouseButtonEventArgs e)
        {
            Image selectedImage = sender as Image;

            int selectedAnswer = GetImageAnswerPlayerNumber(selectedImage);
            WitsService.PlayerManagerClient playerManagerClient = new WitsService.PlayerManagerClient();

            try
            {
                Player playerData = playerManagerClient.GetPlayerByUser(UserSingleton.Instance.Username);
                InstanceContext context = new InstanceContext(this);
                WitsService.ActiveGameClient client = new WitsService.ActiveGameClient(context);
                int profilePictureId = playerData.ProfilePictureId;
                int playerNumber = player;
                bool isReady = true;
                client.ReceivePlayerSelectedAnswer(playerNumber, selectedAnswer, profilePictureId, gameId);
                client.ReadyToWager(gameId, player, isReady);
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
            imageSelectionPlayer1.Visibility = Visibility.Visible;
        } 

        
        private void PlayNextRound()
        {
            InstanceContext context = new InstanceContext(this);
            WitsService.ActiveGameClient client = new WitsService.ActiveGameClient(context);
            labelRound.Content = Properties.Resources.Round + rounds;
            textBoxPlayersAnswer.Text = "";
            imageSelectionPlayer1.Visibility = Visibility.Hidden;
            imageSelectionPlayer2.Visibility = Visibility.Hidden;
            imageSelectionPlayer3.Visibility = Visibility.Hidden;
            imageSelectionPlayer4.Visibility = Visibility.Hidden;
            imageAcceptWager.Visibility = Visibility.Hidden;
            imageQuestionFrame.Source = new BitmapImage(new Uri("Images/questionFrame.png", UriKind.RelativeOrAbsolute));
            labelAnswer1.Content = "";
            labelAnswer2.Content = "";
            labelAnswer3.Content = "";
            labelAnswer4.Content = "";

            try
            {
                bool isReady = false;
                client.RegisterUserInGameContext(UserSingleton.Instance.Username);
                client.ReadyToWager(gameId, player, isReady);
                client.ReadyToShowAnswer(gameId, player, isReady);
                client.ReceivePlayerSelectedAnswer(player, 0, 1, gameId);
                client.SavePlayerAnswer(player, "", gameId);

                imageWinner1.Visibility = Visibility.Hidden;
                imageWinner2.Visibility = Visibility.Hidden;
                imageWinner3.Visibility = Visibility.Hidden;
                imageWinner4.Visibility = Visibility.Hidden;
                imageAcceptAnswer.Visibility = Visibility.Visible;
                gridRoundWinners.Margin = new Thickness(1177, 0, -1177, 0);
                correctPlayers.Clear();

                ShowQuestion();

                WitsService.PlayerManagerClient playerManagerClient = new WitsService.PlayerManagerClient();
                Player playerData = playerManagerClient.GetPlayerByUser(userName);
                int profilePictureId = playerData.ProfilePictureId;
                int celebrationId = playerData.CelebrationId;
              
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
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
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
        }

        private int GetRandomQuestionIdFromServer()
        {
            WitsService.GameManagerClient client = new WitsService.GameManagerClient();

            try
            {
                newQuestionId = client.GetRandomQuestionId();
                return newQuestionId;
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
                return 0;
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
                return 0;
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
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
        }

        private void CelebrationLoop(object sender, RoutedEventArgs e)
        {
            celebrationVideo.Position = TimeSpan.Zero;
            celebrationVideo.Play();
        }

        private void GoToLobby(object sender, MouseButtonEventArgs e)
        {
            WitsService.GameManagerClient client = new WitsService.GameManagerClient();

            try
            {
                client.RemovePlayerInGame(gameId, userName);
                GameSingleton.Instance.ClearGame();
                this.NavigationService.Navigate(new Uri("MenuPage.xaml", UriKind.Relative));
                mediaPlayer.Stop();
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
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
                imageUserProfilePic.Source = new BitmapImage(profilePictureUri);
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
            }
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
            if (string.IsNullOrWhiteSpace(textBoxPlayersAnswer.Text))
            {
                return;
            }
            else
            {
                labelInstrucion.Content = Properties.Resources.EnterAnswer;
                gridEnterAnswer.Margin = new Thickness(1177, 0, -1177, 0);
                string answerText = textBoxPlayersAnswer.Text;
                try
                {
                    InstanceContext context = new InstanceContext(this);
                    WitsService.ActiveGameClient client = new WitsService.ActiveGameClient(context);

                    int playerNumber = player;

                    client.SavePlayerAnswer(playerNumber, answerText, gameId);


                }
                catch (TimeoutException ex)
                {
                    Logger.LogErrorException(ex);
                    MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                    RestartGame();
                }
                catch (CommunicationException ex)
                {
                    Logger.LogErrorException(ex);
                    MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                    RestartGame();
                }

                string labelName = "labelAnswer" + player;
                var label = FindName(labelName) as System.Windows.Controls.Label;

                if (label != null)
                {
                    label.Content = answerText;
                }
                gridAllAnswers.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        public void UpdatePlayerAnswer(int playerNumber, string answer)
        {
            string labelName = "labelAnswer" + playerNumber;
            var label = FindName(labelName) as System.Windows.Controls.Label;

            if (label != null)
            {
                label.Content = answer;
            }
        }

        private void ChangeProfilePicture(int player, int idProfilePicture)
        {
            string imageName = $"imageSelectionPlayer{player}";
            Image selectedImage = FindName(imageName) as Image;

            if (selectedImage != null)
            {
                string profilePictureFileName = idProfilePicture + ".png";
                string profilePicturePath = "ProfilePictures/" + profilePictureFileName;
                Uri profilePictureUri = new Uri(profilePicturePath, UriKind.Relative);
                selectedImage.Source = new BitmapImage(profilePictureUri);
                if (selectedImage.Visibility != Visibility.Visible)
                {
                    selectedImage.Visibility = Visibility.Visible;
                }
            }

            string imageWinnerName = $"imageWinner{player}";
            Image selectedImageWinner = FindName(imageWinnerName) as Image;

            if (selectedImageWinner != null)
            {
                string profilePictureFileName = idProfilePicture + ".png";
                string profilePicturePath = "ProfilePictures/" + profilePictureFileName;
                Uri profilePictureUri = new Uri(profilePicturePath, UriKind.Relative);
                selectedImageWinner.Source = new BitmapImage(profilePictureUri);
            }
        }

        private int GetImageAnswerPlayerNumber(Image selectedImage)
        {
            string imageName = selectedImage?.Name;

            if (imageName != null && imageName.StartsWith("imageAnswerPlayer"))
            {
                if (int.TryParse(imageName.Substring("imageAnswerPlayer".Length), out int imageNumber))
                {
                    return imageNumber;
                }
            }
            return 0;
        }

        private void ShowRoundWinners()
        {
            foreach (int playerNumber in correctPlayers)
            {
                string winnerImageName = "imageWinner" + playerNumber;
                Image winnerImage = gridRoundWinners.FindName(winnerImageName) as Image;

                if (winnerImage != null)
                {
                    winnerImage.Visibility = Visibility.Visible;
                }
            }

            gridRoundWinners.Margin = new Thickness(0, 0, 0, 0);
        }

        private void SaveWager(object sender, MouseButtonEventArgs e)
        {
            int chipsAvailable = int.Parse(labelChips.Content.ToString());

            if (int.TryParse(textBoxPlayersAnswer.Text, out int wagerAmount) && wagerAmount <= chipsAvailable)
            {
                gridEnterAnswer.Margin = new Thickness(1177, 0, -1177, 0);

                InstanceContext context = new InstanceContext(this);
                WitsService.ActiveGameClient client = new WitsService.ActiveGameClient(context);
                bool isReady = true;

                try
                {
                    client.ReadyToShowAnswer(gameId, player, isReady);
                    imageAcceptWager.Visibility = Visibility.Hidden;
                    imageAcceptAnswer.Visibility = Visibility.Hidden;
                }
                catch (TimeoutException ex)
                {
                    Logger.LogErrorException(ex);
                    MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                    RestartGame();
                }
                catch (CommunicationException ex)
                {
                    Logger.LogErrorException(ex);
                    MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                    RestartGame();
                }
            }
            else
            {
                gridWarning.Margin = new Thickness(0, 0, 0, 0);
                textBoxPlayersAnswer.Text = "";
            }

        }

        private void AcceptWarning(object sender, MouseButtonEventArgs e)
        {
            gridWarning.Margin = new Thickness(1177, 822, -1177, -822);
        }

        private void ValidateGameLeader()
        {
            WitsService.GameManagerClient client = new WitsService.GameManagerClient();
            try
            {
                string gameLeader = client.GetGameLeader(GameSingleton.Instance.GameId);
                if (gameLeader.Equals(UserSingleton.Instance.Username))
                {
                    imagePlayersSettings.Visibility = Visibility.Visible;
                    SetPlayers();
                }
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
        }

        private void OpenPlayersMenu(object sender, MouseButtonEventArgs e)
        {
            gridPlayersInGame.Margin = new Thickness(48, 205, 212, 66);
        }

        public void UpdateAnswers(Dictionary<int, string> playerAnswers)
        {
            foreach (var kvp in playerAnswers)
            {
                int playerNumber = kvp.Key;
                string answer = kvp.Value;

                string labelName = "labelAnswer" + playerNumber;

                var label = FindName(labelName) as System.Windows.Controls.Label;

                if (label != null)
                {
                    label.Content = answer;
                }
            }
        }

        public void UpdateSelection(Dictionary<int, PlayerSelectedAnswer> playerSelectedAnswers)
        {
            foreach (var kvp in playerSelectedAnswers)
            {
                int playerNumber = kvp.Key;
                PlayerSelectedAnswer selectedAnswerObject = kvp.Value;
                int selectedAnswer = selectedAnswerObject.SelectedAnswer;
                int IdProfilePicturePlayerSelection = selectedAnswerObject.IdProfilePicture;

                switch (playerNumber)
                {
                    case 1:
                        ChangeProfilePicture(1, IdProfilePicturePlayerSelection);

                        switch (selectedAnswer)
                        {
                            case 0:
                                imageSelectionPlayer1.Visibility = Visibility.Hidden;
                                break;
                            case 1:
                                imageSelectionPlayer1.Margin = new Thickness(45, 510, 944, 118);
                                break;
                            case 2:
                                imageSelectionPlayer1.Margin = new Thickness(270, 506, 719, 122);
                                break;
                            case 3:
                                imageSelectionPlayer1.Margin = new Thickness(515, 510, 474, 118);
                                break;
                            case 4:
                                imageSelectionPlayer1.Margin = new Thickness(754, 510, 235, 118);
                                break;
                            default:
                                break;
                        }
                        break;
                    case 2:
                        ChangeProfilePicture(2, IdProfilePicturePlayerSelection);
                        imageSelectionPlayer2.Visibility = Visibility.Visible;

                        switch (selectedAnswer)
                        {
                            case 0:
                                imageSelectionPlayer2.Visibility = Visibility.Hidden;
                                break;
                            case 1:
                                imageSelectionPlayer2.Margin = new Thickness(204, 510, 784, 118);
                                break;
                            case 2:
                                imageSelectionPlayer2.Margin = new Thickness(448, 505, 540, 123);
                                break;
                            case 3:
                                imageSelectionPlayer2.Margin = new Thickness(693, 509, 295, 119);
                                break;
                            case 4:
                                imageSelectionPlayer2.Margin = new Thickness(940, 505, 48, 123);
                                break;
                            default:
                                break;
                        }
                        break;
                    case 3:
                        imageSelectionPlayer3.Visibility = Visibility.Visible;
                        ChangeProfilePicture(3, IdProfilePicturePlayerSelection);

                        switch (selectedAnswer)
                        {
                            case 0:
                                imageSelectionPlayer3.Visibility = Visibility.Hidden;
                                break;
                            case 1:
                                imageSelectionPlayer3.Margin = new Thickness(45, 510, 944, 118);
                                break;
                            case 2:
                                imageSelectionPlayer3.Margin = new Thickness(285, 107, 703, 521);
                                break;
                            case 3:
                                imageSelectionPlayer3.Margin = new Thickness(540, 107, 448, 521);
                                break;
                            case 4:
                                imageSelectionPlayer3.Margin = new Thickness(785, 110, 203, 518);
                                break;
                            default:
                                break;
                        }
                        break;

                    case 4:
                        ChangeProfilePicture(4, IdProfilePicturePlayerSelection);
                        imageSelectionPlayer4.Visibility = Visibility.Visible;

                        switch (selectedAnswer)
                        {
                            case 0:
                                imageSelectionPlayer4.Visibility = Visibility.Hidden;
                                break;
                            case 1:
                                imageSelectionPlayer4.Margin = new Thickness(218, 106, 770, 522);
                                break;
                            case 2:
                                imageSelectionPlayer4.Margin = new Thickness(461, 107, 527, 521);
                                break;
                            case 3:
                                imageSelectionPlayer4.Margin = new Thickness(693, 124, 295, 504);
                                break;
                            case 4:
                                imageSelectionPlayer4.Margin = new Thickness(940, 107, 48, 521);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }

            this.playerSelectedAnswers = playerSelectedAnswers;
        }

        public void ShowEnterWager()
        {
            gridAllAnswers.Margin = new Thickness(-20, 754, 20, -754);
            textBoxPlayersAnswer.Text = "";
            imageAcceptWager.Visibility = Visibility.Visible;
            gridEnterAnswer.Margin = new Thickness(0, 0, 0, 0);
            labelInstrucion.Content = Properties.Resources.HowMuch;
        }

        public void ShowTrueAnswer()
        {
            ShowAnswer();

        }
     
       private void PayCorrectAnswer(Dictionary<int, PlayerSelectedAnswer> playerSelectedAnswers)
        {
            int correctAnswer = trueAnswer;
            Dictionary<int, int> playerGuesses = new Dictionary<int, int>();

            for (int i = 1; i <= 4; i++)
            {
                string labelName = "labelAnswer" + i;
                var label = FindName(labelName) as System.Windows.Controls.Label;

                if (label != null && int.TryParse(label.Content.ToString(), out int guess))
                {
                    playerGuesses.Add(i, guess);
                }
            }

            List<int> closestAnswer = new List<int>();
            int closestDifference = int.MaxValue;

            foreach (var kvp in playerGuesses)
            {
                int answerNumber = kvp.Key;
                int guess = kvp.Value;

                int difference = Math.Abs(guess - correctAnswer);

                if (difference < closestDifference)
                {
                    closestDifference = difference;
                    closestAnswer.Clear();
                    closestAnswer.Add(answerNumber);
                }
                else if (difference == closestDifference)
                {
                    closestAnswer.Add(answerNumber);
                }
            }

            List<int> correctPlayers = new List<int>();

            foreach (var kvp in playerSelectedAnswers)
            {
                int playerNumber = kvp.Key;
                int selectedAnswer = kvp.Value.SelectedAnswer;

                if (closestAnswer.Contains(selectedAnswer))
                {
                    correctPlayers.Add(playerNumber);
                }
            }

            if (int.TryParse(textBoxPlayersAnswer.Text, out int wagerAmount))
            {
                int currentChips = int.Parse(labelChips.Content.ToString());
                int newChips = currentChips + wagerAmount;

                if (correctPlayers.Contains(player))
                {
                    labelChips.Content = newChips.ToString();
                }
                else
                {
                    labelChips.Content = (currentChips - wagerAmount).ToString();
                }
            }

            this.correctPlayers = correctPlayers;
        }

        public void BeExpelled()
        {
            MessageBox.Show(Properties.Resources.Expelled, Properties.Resources.Expel, MessageBoxButton.OK, MessageBoxImage.Information);
            GameSingleton.Instance.ClearGame();
            this.NavigationService.Navigate(new Uri("MenuPage.xaml", UriKind.Relative));
        }

        void IActiveGameCallback.ShowVictoryScreen(string userName, int profilePictureId, int celebrationId, int score)
        {
            gridRoundWinners.Margin = new Thickness(1177, 0, -1177, 0);
            string profilePictureFileName = profilePictureId + ".png";
            string profilePicturePath = "ProfilePictures/" + profilePictureFileName;

            Uri profilePictureUri = new Uri(profilePicturePath, UriKind.Relative);
            profilePicture.Source = new BitmapImage(profilePictureUri);


            string celebrationFileName = celebrationId + ".mp4";
            string celebrationPath = "Celebrations/" + celebrationFileName;

            Uri celebrationUri = new Uri(celebrationPath, UriKind.Relative);
            celebrationVideo.Source = celebrationUri;


            labelWinner.Content = userName + Properties.Resources.Wins;


            celebrationVideo.Play();
            victoryScreen.Margin = new Thickness(0);

            DoubleAnimation showAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(2));
            victoryScreen.BeginAnimation(OpacityProperty, showAnimation);
        }

        public void TieBreaker()
        {
            rounds = 3;
            labelRound.Visibility = Visibility.Collapsed;
            labelBonusRound.Visibility = Visibility.Visible;
            PlayNextRound();
            try
            {
                InstanceContext context = new InstanceContext(this);
                WitsService.ActiveGameClient client = new WitsService.ActiveGameClient(context);
                client.CleanWinners(gameId);
            }
            catch (TimeoutException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerProblemMessage, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
            catch (CommunicationException ex)
            {
                Logger.LogErrorException(ex);
                MessageBox.Show(Properties.Resources.ServerUnavailable, Properties.Resources.ServerProblem, MessageBoxButton.OK, MessageBoxImage.Information);
                RestartGame();
            }
        }

        public void RestartGame()
        {
            UserSingleton.Instance.ClearUsername();
            GameSingleton.Instance.ClearGame();
            var currentWindow = Window.GetWindow(this);
            var mainWindow = new MainWindow();
            mainWindow.Show();
            currentWindow.Close();
        }
    }
}
