using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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

namespace Wits
{
    /// <summary>
    /// Lógica de interacción para Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        private SoundPlayer music;

        public Menu()
        {
            InitializeComponent();
            Random randomNum = new Random();
            int songNum = randomNum.Next(1, 9);
            String numString = songNum.ToString();
            music = new SoundPlayer(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song" + numString + ".wav");
            Console.WriteLine("Song " + numString);
            music.Play();
            backgroundVideo.Play();
        }

        private void RestartVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }
    }
}
