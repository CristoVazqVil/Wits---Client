﻿using System;
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

namespace Wits
{
    /// <summary>
    /// Interaction logic for EnterPlayerUserWindow.xaml
    /// </summary>
    public partial class EnterPlayerUserWindow : Window
    {
        public string playerUser;
        public EnterPlayerUserWindow()
        {
            InitializeComponent();
            backgroundVideo.Play();
        }

        private void RestartBackgroundVideo(object sender, RoutedEventArgs e)
        {
            backgroundVideo.Position = TimeSpan.Zero;
            backgroundVideo.Play();
        }

        private void buttonSendUser_Click(object sender, RoutedEventArgs e)
        {
            playerUser = textBoxPlayerUser.Text;
            if (!string.IsNullOrEmpty(playerUser))
            {
                this.DialogResult = true;
                this.Close();
            }
        }
    }
}
