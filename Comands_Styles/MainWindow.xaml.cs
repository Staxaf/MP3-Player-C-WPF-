using MahApps.Metro.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.IO;

//using HundredMilesSoftware.UltraID3Lib;
namespace Comands_Styles
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        
        //System.Media.SoundPlayer sound = new System.Media.SoundPlayer();
        private MediaPlayer media = new MediaPlayer();
        //DirectoryInfo Directory 
        OpenFileDialog open = new OpenFileDialog();
        Button but = new Button();
        TagLib.File audio;
        List<Songs> items = new List<Songs>();
        public bool flag = true; // для play/pause
        public bool res = false; // для sound off/ on
        public bool change = true; // для boost/unboost
        public MainWindow()
        {
            InitializeComponent();
            list_songs.ItemsSource = items;
            OpenFile();
            list_songs.ItemsSource = items;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            
        }

        private bool User_Filter(object sender)
        {
            if (String.IsNullOrEmpty(box_song.Text) || String.IsNullOrEmpty((sender as Songs).Name_song))
            {
                return true; 
            }
            else
            {
                return ((sender as Songs).Name_song.IndexOf(box_song.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }
        void timer_Tick(object sender, EventArgs e)
        {
            if (media.Source != null)
            {
                position.Text = String.Format(media.Position.ToString(@"mm\:ss"));

                //endofmusic.Text = media.NaturalDuration.TimeSpan.ToString(@"mm\:ss"); // конец песни в текстблок
                /*if (media.HasAudio)
                {
                    endofmusic.Text = String.Format(media.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                }*/

                if (!flag)
                {
                    time.Value = media.Position.TotalSeconds;
                }

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            media.Pause();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }
        public void OpenFile()
        {
            open.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (open.ShowDialog() == true)
            {

                media.Open(new Uri(open.FileName));
                audio = TagLib.File.Create(open.FileName);
                endofmusic.Text = audio.Properties.Duration.ToString("mm\\:ss");
                string[] temp = open.FileName.Split('\\');
                string[] files = Directory.GetFiles(audio.Name.Replace(temp[temp.Length - 1], ""));
                foreach (string file in files)
                {
                    try
                    {
                        audio = TagLib.File.Create(file);
                        if(!(audio.Properties.Duration.ToString("mm\\:ss") == "00:00"))
                        {
                            items.Add(new Songs
                            {
                                Name_song = audio.Tag.Title,
                                Artist = audio.Tag.FirstPerformer,
                                Time = audio.Properties.Duration.ToString("mm\\:ss"),
                                Year = audio.Tag.Year.ToString(),
                                Path = audio.Name
                            }
                        );
                            list_songs.Items.Add(new Songs
                            {
                                Name_song = audio.Tag.Title,
                                Artist = audio.Tag.FirstPerformer,
                                Time = audio.Properties.Duration.ToString("mm\\:ss"),
                                Year = audio.Tag.Year.ToString(),
                                Path = audio.Name
                            });
                        }
                        
                        
                    } 
                    catch(Exception)
                    {

                    }
                }
                ToolTip for_name = new ToolTip();
                if (audio.Tag.Title == " ")
                {
                    name_song.Text = temp[temp.Length - 1];
                }
                else
                {
                    name_song.Text = audio.Tag.Title;
                }
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(list_songs.ItemsSource);
                view.Filter = User_Filter;
                for_name.Content = temp[temp.Length - 1];
                //name_song.Text = audio.Name; // путь всей песни
                list_songs.SelectedIndex = items.Count - 1;
                count.Text = items.Count.ToString();
                list_songs.ItemsSource = items;
            }
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (flag)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("images/pause.png", UriKind.Relative));
                play.Content = img;
                flag = false;
                media.Play();
            }
            else
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("images/play.png", UriKind.Relative));
                play.Content = img;
                flag = true;
                media.Pause();
            }
        }
        private void Time_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            time.Minimum = 0;
            if(media.NaturalDuration.HasTimeSpan)
            {
                double max = Convert.ToDouble(media.NaturalDuration.TimeSpan.TotalSeconds.ToString());
                time.Maximum = max;
            }
           
            media.Pause();
            media.Position = TimeSpan.FromSeconds(time.Value);
            if (!flag)
            {
                media.Play();
            }
            else
            {
                media.Pause();
            }
        }
        private void Click_Stop(object sender, RoutedEventArgs e)
        {
            if (media.Source != null)
            {
                media.Stop();
                time.Value = 0;
                if (flag)
                {
                    media.Pause();
                }

            }
        }
        private void MouseOn(object sender, MouseEventArgs e)
        {
            if ((sender as Button).Name == "play")
            {
                Image image1 = new Image();
                if (flag)
                {
                    image1.Source = new BitmapImage(new Uri("images/play_on.png",UriKind.Relative));
                    play.Content = image1;
                }
                else
                {
                    image1.Source = new BitmapImage(new Uri("images/pause_on.png", UriKind.Relative));
                    play.Content = image1;
                }
            }
            if ((sender as Button).Name == "stop")
            {
                image2.Source = new BitmapImage(new Uri("images/stop_on.png", UriKind.Relative));

                stop.Content = image2;
            }
            if ((sender as Button).Name == "noise")
            {

                Image omg = new Image();
                omg.Source = new BitmapImage(new Uri("images/sound_on.png", UriKind.Relative));
                noise.Content = omg;
            }
            if ((sender as Button).Name == "nexter")
            {
                next.Source = new BitmapImage(new Uri("images/next_black.png", UriKind.Relative));
                nexter.Content = next;
            }
            if ((sender as Button).Name == "previous")
            {
                prev.Source = new BitmapImage(new Uri("images/prev_black.png", UriKind.Relative));
                previous.Content = prev;
            }
            /*if ((sender as Button).Name == "set_image")
            {
                set_image.Background = Brushes.Red;
                //image_set.Width = image_set.Height -= 10;
            }*/
            if ((sender as Button).Name == "set_audio")
            {
                set_aud.Source = new BitmapImage(new Uri("images/note_black.png", UriKind.Relative));
                set_audio.Content = set_aud;
            }
            if ((sender as Button).Name == "exit")
            {
                image_exit.Source = new BitmapImage(new Uri("images/exit.png",UriKind.Relative));
                exit.Content = image_exit;
            }
        }
        private void MouseOut(object sender, MouseEventArgs e)
        {
            if ((sender as Button).Name == "play")
            {
                Image image = new Image();
                if (flag)
                {
                    image.Source = new BitmapImage(new Uri("images/play.png", UriKind.Relative));
                    play.Content = image;
                }
                else
                {
                    image.Source = new BitmapImage(new Uri("images/pause.png", UriKind.Relative));
                    play.Content = image;
                }

            }
            if ((sender as Button).Name == "stop")
            {
                image2.Source = new BitmapImage(new Uri("images/stop1.png", UriKind.Relative));
                stop.Content = image2;
            }
            if ((sender as Button).Name == "noise")
            {
                if (!res)
                {
                    Image omg = new Image();
                    omg.Source = new BitmapImage(new Uri("images/sound_max.png", UriKind.Relative));
                    noise.Content = omg;
                }
                else
                {
                    Image omg1 = new Image();
                    omg1.Source = new BitmapImage(new Uri("images/sound_min.png", UriKind.Relative));
                    noise.Content = omg1;
                }
            }
            if ((sender as Button).Name == "nexter")
            {
                next.Source = new BitmapImage(new Uri("images/next.png", UriKind.Relative));
                nexter.Content = next;
            }
            if ((sender as Button).Name == "previous")
            {
                prev.Source = new BitmapImage(new Uri("images/prev.png", UriKind.Relative));
                previous.Content = prev;
            }
            /*if((sender as Button).Name == "set_image")
            {
                set_image.Background = Brushes.Transparent;
                //image_set.Width = image_set.Height += 10;
            }*/
            if ((sender as Button).Name == "set_audio")
            {
                set_aud.Source = new BitmapImage(new Uri("images/note_white.png", UriKind.Relative));
                set_audio.Content = set_aud;
            }
            if ((sender as Button).Name == "exit")
            {
                image_exit.Source = new BitmapImage(new Uri("images/exit_default.png", UriKind.Relative));
                exit.Content = image_exit;
            }
        }

        private void SetSound(object sender, RoutedEventArgs e)
        {
            if (res)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("images/sound_max.png", UriKind.Relative));
                noise.Content = img;
                res = false;
                media.Volume = 0.5;
                slider_sound.Value = media.Volume;
                media.Pause();
            }
            else
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("images/sound_min.png", UriKind.Relative));
                noise.Content = img;
                res = true;
                media.Volume = 0;
                slider_sound.Value = 0;
                media.Pause();
            }

        }
        private void Sound_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            slider_sound.Maximum = 1;
            media.Pause();
            media.Volume = slider_sound.Value;
            if (!flag)
                media.Play();// если картинка паузы, то музыка играет
        }
        private void SetImage(object sedner, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (file.ShowDialog() == true)
            {
                image_song.Source = new BitmapImage(new Uri(file.FileName));
            }
        }
        private void GetPath(object sender, MouseEventArgs e)
        {

            Songs path = list_songs.SelectedItem as Songs;
            media.Pause();
            media.Stop();
            media.Open(new Uri(path.Path));
            name_song.Text = path.Name_song;
            endofmusic.Text = path.Time;
        }
        private void Click_Next(object sender, RoutedEventArgs e)
        {
            media.Pause();
            int index = list_songs.SelectedIndex;
            if (items.Count == index + 1)
            {
                list_songs.SelectedIndex = 0;
                Songs path2 = list_songs.Items[0] as Songs;
                media.Open(new Uri(path2.Path));
                name_song.Text = path2.Name_song;
                endofmusic.Text = path2.Time;
            }
            else
            {
                list_songs.SelectedIndex = index + 1;
                Songs path1 = list_songs.Items[index + 1] as Songs;
                media.Open(new Uri(path1.Path));
                name_song.Text = path1.Name_song;
                endofmusic.Text = path1.Time;
            }

        }
        private void Click_Prev(object sender, RoutedEventArgs e)
        {
            int index = list_songs.SelectedIndex;
            if (index <=0)
            {
                list_songs.SelectedIndex = items.Count - 1;
                Songs path3 = list_songs.Items[items.Count - 1] as Songs;
                media.Pause();
                media.Open(new Uri(path3.Path));
                name_song.Text = path3.Name_song;
                endofmusic.Text = path3.Time;
            }
            else
            {
                list_songs.SelectedIndex = index - 1;
                Songs path4 = list_songs.Items[index - 1] as Songs;
                media.Pause();
                media.Open(new Uri(path4.Path));
                name_song.Text = path4.Name_song;
                endofmusic.Text = path4.Time;
            }
        }
        private void Find_Song(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(list_songs.ItemsSource).Refresh();
        }
        private void Del_Playlist(object sender, RoutedEventArgs e)
        {
            items.Clear();
            count.Text = items.Count.ToString();
            list_songs.ItemsSource = new List<Songs>();
        }
    }
}

