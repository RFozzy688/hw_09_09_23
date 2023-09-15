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
        TimeSpan _currentTime;
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
            openFileDialog.Filter = "Video files (*.mp4)|*.mp4";
            openFileDialog.Multiselect = true;

            int count = 1;

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    ListBoxItem lbi = new ListBoxItem();

                    if (count % 2 != 0)
                    {
                        lbi.Background = Brushes.LightGray;
                    }

                    lbi.Content = file.Substring(file.LastIndexOf('\\') + 1);
                    ListBox.Items.Add(lbi);

                    _playList.Add((string)lbi.Content, file);

                    count++;
                }
            }
        }
        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsSelectedItem())
            {
                Slider.Value = 0;
                Play(_path);

                PlayImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/cyan_play.png"));
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
                }
            }
            else
            {
                MediaElement.Play();
                _timer.Start();
                _isPause = false;

                PlayImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/cyan_play.png"));
                PauseImg.Source = BitmapFrame.Create(new Uri(@"pack://application:,,,/Resources/pause.png"));
            }

            _isPlay = true;

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
        }
    }
}
