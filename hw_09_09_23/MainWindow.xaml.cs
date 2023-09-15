using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

namespace hw_09_09_23
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, string> _playList;
        DispatcherTimer _timer;
        string _path;
        bool _isPlay;
        bool _isPause;
        public MainWindow()
        {
            InitializeComponent();

            _playList = new Dictionary<string, string>();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += new EventHandler(CurrentTime);

            _isPlay = false;
            _isPause = false;
        }
        private void MenuItem_Add(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Video files (*.mp4;*.wmv;*.mp3)|*.mp4;*.wmv;*.mp3";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    ListBoxItem lbi = new ListBoxItem();

                    if (ListBox.Items.Count % 2 != 0)
                    {
                        lbi.Background = Brushes.LightGray;
                    }

                    lbi.Content = file.Substring(file.LastIndexOf('\\') + 1);
                    ListBox.Items.Add(lbi);

                    _playList.Add((string)lbi.Content, file);
                }
            }
        }
        private void MenuItem_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsSelectedItem())
            {
                Slider.Value = 0;
                Play(_path);

                _isPlay = true;

                PauseImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/pause.png"));
                StopImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/stop.png"));
            }
        }
        private void Play(string path)
        {
            MediaElement.LoadedBehavior = MediaState.Manual;
            MediaElement.Source = new Uri(path);

            MediaElement.Play();
            PlayImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/cyan_play.png"));

            _timer.Start();
        }
        private void Element_MediaOpened(object sender, RoutedEventArgs e)
        {
            Slider.Maximum = MediaElement.NaturalDuration.TimeSpan.TotalSeconds;

            TextBlockRight.Text = String.Format(MediaElement.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss"));
        }
        private void CurrentTime(object? sender, EventArgs e)
        {
            TextBlockLeft.Text = String.Format(MediaElement.Position.ToString(@"hh\:mm\:ss"));
            Slider.Value += 1;
        }
        private void TimerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaElement.Position = new TimeSpan(0, 0, (int)Slider.Value);
        }
        private void PlayImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isPause)
            {
                if (IsSelectedItem() && !_isPlay)
                {
                    Slider.Value = 0;
                    Play(_path);
                    _isPlay = true;
                }
            }
            else
            {
                MediaElement.Play();
                _timer.Start();
                _isPause = false;
                _isPlay = true;

                PlayImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/cyan_play.png"));
                PauseImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/pause.png"));
            }

            StopImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/stop.png"));
        }
        private bool IsSelectedItem()
        {
            int index = ListBox.SelectedIndex;

            if (index != -1)
            {
                ListBoxItem lbi = (ListBoxItem)(ListBox.ItemContainerGenerator.ContainerFromIndex(index));

                _path = _playList[lbi.Content.ToString()];

                return true;
            }
            return false;
        }
        private void PauseImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isPlay)
            {
                _isPause = true;
                _timer.Stop();
                MediaElement.Pause();
                PlayImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/play.png"));
                PauseImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/cyan_pause.png"));
            }
        }
        private void StopImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isPlay) return;

            MediaElement.Stop();

            StopImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/cyan_stop.png"));

            PlayImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/play.png"));
            PauseImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/pause.png"));

            _timer.Stop();
            Slider.Value = 0;
            TextBlockLeft.Text = "0:00:00";

            _isPlay = false;
        }
        private void Element_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaElement.Stop();

            _timer.Stop();
        }
        private void BackImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isPlay) return;

            TimeSpan currentPos = MediaElement.Position;

            Slider.Value = currentPos.TotalSeconds - 10;
            MediaElement.Position = new TimeSpan(0, 0, (int)Slider.Value);

            BackImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/cyan_back.png"));
        }
        private void ForwardImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isPlay) return;

            TimeSpan currentPos = MediaElement.Position;

            Slider.Value = currentPos.TotalSeconds + 10;
            MediaElement.Position = new TimeSpan(0, 0, (int)Slider.Value);

            ForwardImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/cyan_forward.png"));
        }
        private void ForwardImg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ForwardImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/forward.png"));
        }
        private void BackImg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BackImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/back.png"));
        }
        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaElement.Volume = SliderVolume.Value;
        }
    }
}
