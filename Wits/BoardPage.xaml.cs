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
        public Random random = new Random();
        public int randomQuestion = 0;

        public BoardPage()
        {
            InitializeComponent();
            backgroundVideo.Play();
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ShowQuestion();
        }

        private async void ShowQuestion()
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

        private async void ShowAnswer()
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
                WitsService.Question question = new WitsService.Question();
                WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
                question = client.GetQuestionByID(randomQuestion);

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
                MessageBox.Show("There´s a server problem, soory!", "Server Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SetAnswer()
        {
            imageQuestionFrame.Source = new BitmapImage(new Uri("Images/answerFrame.png", UriKind.RelativeOrAbsolute));
            try
            {
                WitsService.Question answer = new WitsService.Question();
                WitsService.PlayerManagerClient client = new WitsService.PlayerManagerClient();
                answer = client.GetQuestionByID(randomQuestion);

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
                MessageBox.Show("There´s a server problem, soory!", "Server Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
