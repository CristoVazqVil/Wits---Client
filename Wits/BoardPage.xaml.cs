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
        private string userName = UserSingleton.Instance.Username;
        private int rounds = 1;
        private List<int> questionIds;
        private int gameId = GameSingleton.Instance.GameId;
        private int newQuestionId;
        private int player = GameSingleton.Instance.PlayerNumber;
        private int trueAnswer = 0;
        List<int> correctPlayers = new List<int>();
        Dictionary<int, PlayerSelectedAnswer> playerSelectedAnswers;





        public BoardPage()
        {
            InitializeComponent();
            backgroundVideo.Play();
            SetProfilePicture();
            InstanceContext context = new InstanceContext(this);
            WitsService.ActiveGameClient client = new WitsService.ActiveGameClient(context);
            client.RegisterUserInGameContext(UserSingleton.Instance.Username);
            Loaded += Page_Loaded;

            bool isReady = false;
            client.ReadyToWager(gameId, player, isReady);

            client.ReadyToShowAnswer(gameId, player, isReady);

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



            await Task.Delay(1000);
            PayCorrectAnswer(playerSelectedAnswers);

            ShowRoundWinners();

            Console.WriteLine("SHOW ANSWER MUESTRA SHOW ROUNDWINNERS");



            await Task.Delay(5000);

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


        private void SelectedAnswer(object sender, MouseButtonEventArgs e)
        {
            Image selectedImage = sender as Image;

            int selectedAnswer = GetImageAnswerPlayerNumber(selectedImage);
            WitsService.PlayerManagerClient playerManagerClient = new WitsService.PlayerManagerClient();
            Player playerData = playerManagerClient.GetPlayerByUser(UserSingleton.Instance.Username);
            int profilePictureId = playerData.ProfilePictureId;

            try
            {
                InstanceContext context = new InstanceContext(this);
                WitsService.ActiveGameClient client = new WitsService.ActiveGameClient(context);
                int playerNumber = player;
                client.ReceivePlayerSelectedAnswer(playerNumber, selectedAnswer, profilePictureId, gameId);

                bool isReady = true;
                client.ReadyToWager(gameId, player, isReady);
            }
            catch (FaultException ex)
            {
                Console.WriteLine(ex.ToString());
            }





            //lo siguiente seguramente se va a borrar jijij

            string profilePictureFileName = profilePictureId + ".png";
            string profilePicturePath = "ProfilePictures/" + profilePictureFileName;

            Uri imageSelectionPlayer1Uri = new Uri(profilePicturePath, UriKind.Relative);
            imageSelectionPlayer1.Source = new BitmapImage(imageSelectionPlayer1Uri);

            imageSelectionPlayer1.Visibility = Visibility.Visible;

            if (selectedImage != null)
            {
                switch (player)
                {
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
                        imageSelectionPlayer2.Visibility = Visibility.Visible;
                        ChangeProfilePicture(2, profilePictureId);

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
                        imageSelectionPlayer3.Visibility = Visibility.Visible;
                        ChangeProfilePicture(3, profilePictureId);

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
                        imageSelectionPlayer4.Visibility = Visibility.Visible;
                        ChangeProfilePicture(4, profilePictureId);
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




        private void ChangeProfilePicture(int player, int idProfilePicture)
        {
            string imageName = $"imageSelectionPlayer{player}";

            // Encuentra la imagen por su nombre
            Image selectedImage = FindName(imageName) as Image;

            if (selectedImage != null)
            {
                // Genera el nombre del archivo de imagen
                string profilePictureFileName = idProfilePicture + ".png";
                string profilePicturePath = "ProfilePictures/" + profilePictureFileName;

                // Crea la URI de la imagen
                Uri profilePictureUri = new Uri(profilePicturePath, UriKind.Relative);

                // Cambia la fuente de la imagen
                selectedImage.Source = new BitmapImage(profilePictureUri);

                // Hace visible la imagen si no está visible
                if (selectedImage.Visibility != Visibility.Visible)
                {
                    selectedImage.Visibility = Visibility.Visible;
                }
            }

            string imageWinnerName = $"ImageWinner{player}";

            // Encuentra la imagen por su nombre
            Image selectedImageWinner = FindName(imageWinnerName) as Image;

            if (selectedImageWinner != null)
            {
                // Genera el nombre del archivo de imagen
                string profilePictureFileName = idProfilePicture + ".png";
                string profilePicturePath = "ProfilePictures/" + profilePictureFileName;

                // Crea la URI de la imagen
                Uri profilePictureUri = new Uri(profilePicturePath, UriKind.Relative);

                // Cambia la fuente de la imagen
                selectedImageWinner.Source = new BitmapImage(profilePictureUri);

                // Hace visible la imagen si no está visible

            }
        }



        private int GetImageAnswerPlayerNumber(Image selectedImage)
        {
            string imageName = selectedImage?.Name;

            if (imageName != null && imageName.StartsWith("ImageAnswerPlayer"))
            {
                if (int.TryParse(imageName.Substring("ImageAnswerPlayer".Length), out int imageNumber))
                {
                    return imageNumber;
                }
            }
            return 0;
        }

        private void EnterWager()
        {
            GridAllAnswers.Margin = new Thickness(-20, 754, 20, -754);
            TextBoxPlayersAnswer.Text = "";
            ImageAcceptWager.Visibility = Visibility.Visible;
            GridEnterAnswer.Margin = new Thickness(0, 0, 0, 0);
            LabelInstrucion.Content = "How much will you wager?";



        }

        private void ShowRoundWinners()
        {
            Console.WriteLine("JUGADORES CORRECTOS " + correctPlayers);

            foreach (int playerNumber in correctPlayers)
            {
                string winnerImageName = "ImageWinner" + playerNumber;
                Image winnerImage = GridRoundWinners.FindName(winnerImageName) as Image;

                if (winnerImage != null)
                {
                    winnerImage.Visibility = Visibility.Visible;
                }
            }


            GridRoundWinners.Margin = new Thickness(0, 0, 0, 0);

        }


        private void SaveWager(object sender, MouseButtonEventArgs e)
        {

            int chipsAvailable = int.Parse(labelChips.Content.ToString());

            if (int.TryParse(TextBoxPlayersAnswer.Text, out int wagerAmount) && wagerAmount <= chipsAvailable)
            {
                GridEnterAnswer.Margin = new Thickness(1177, 0, -1177, 0);


                InstanceContext context = new InstanceContext(this);
                WitsService.ActiveGameClient client = new WitsService.ActiveGameClient(context);
                bool isReady = true;
                client.ReadyToShowAnswer(gameId, player, isReady);

                ImageAcceptWager.Visibility = Visibility.Hidden;
                ImageAcceptAnswer.Visibility = Visibility.Hidden;

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


            EnterWager();

        }



        public void ShowTrueAnswer()
        {
            Console.WriteLine("llamará a show answer");

            ShowAnswer();
        }




        private void PayCorrectAnswer(Dictionary<int, PlayerSelectedAnswer> playerSelectedAnswers)
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

            // Lista para almacenar los playerNumber que respondieron correctamente
            List<int> correctPlayers = new List<int>();


            foreach (var kvp in playerSelectedAnswers)
            {
                int playerNumber = kvp.Key;
                int selectedAnswer = kvp.Value.SelectedAnswer;

                // Imprimir en la consola el playerNumber si la selectedAnswer coincide con la respuesta más cercana
                if (closestAnswer.Contains(selectedAnswer))
                {
                    Console.WriteLine($"Player {playerNumber}: SelectedAnswer={selectedAnswer} RESPONDIÓ CORRECTAMENTE");

                    // Agregar el playerNumber a la lista de jugadores que respondieron correctamente
                    correctPlayers.Add(playerNumber);
                }



                if (int.TryParse(TextBoxPlayersAnswer.Text, out int wagerAmount))
                {
                    int currentChips = int.Parse(labelChips.Content.ToString());
                    int newChips = currentChips + wagerAmount;

                    // Verificar si el jugador está en la lista de jugadores correctos
                    if (correctPlayers.Contains(player))
                    {
                        // Sumar la cantidad del TextBoxPlayersAnswer al LabelChips
                        labelChips.Content = newChips.ToString();
                    }
                    else
                    {
                        // Restar la cantidad del TextBoxPlayersAnswer al LabelChips
                        labelChips.Content = (currentChips - wagerAmount).ToString();
                    }
                }



            }


        }
    }
}
