using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
        private MediaPlayer mediaPlayer;
        private Random random = new Random();
        private List<Uri> songs = new List<Uri>()
        {
            new Uri(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song1.wav", UriKind.Absolute),
            new Uri(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song2.wav", UriKind.Absolute),
            new Uri(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song3.wav", UriKind.Absolute),
            new Uri(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song4.wav", UriKind.Absolute),
            new Uri(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song5.wav", UriKind.Absolute),
            new Uri(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song6.wav", UriKind.Absolute),
            new Uri(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song7.wav", UriKind.Absolute),
            new Uri(@"D:\UV\Tecnologias\Wits\Wits\Wits\Music\Song8.wav", UriKind.Absolute)
        }; 

        public Menu()
        {
            InitializeComponent();
            backgroundVideo.Play();
            mediaPlayer = new MediaPlayer();
            mediaPlayer.MediaEnded += SongEnded;
            PlayRandomSong();
        }

        private void PlayRandomSong()
        {
            int randomIndex = random.Next(songs.Count);
            Uri songUri = songs[randomIndex];

            mediaPlayer.Open(songUri);
            mediaPlayer.Play();
        }

        private void SongEnded(object sender, EventArgs e)
        {
            PlayRandomSong();
        }

        private void RestartVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }
    }
}
