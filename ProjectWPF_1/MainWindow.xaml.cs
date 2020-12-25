using Microsoft.Win32;
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

namespace ProjectWPF_1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            VolumeSlider.Value = 0.5;
            player.Stop();
            Task.Factory.StartNew(videoDuration);
        }

        bool isPlayng = true;
        Task sliderMove;
        bool isSliderMove = false;
        string[] fileNames;

        /// <summary>
        /// старт/стоп проигрователя
        /// </summary>
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPlayng == true)
            {
                player.Pause();
                PlayButton.Content = "Play";
            }
            else
            {
                player.Play();
                sliderMove = new Task(SliderMove);
                sliderMove.Start();
                PlayButton.Content = "Pause";
            }
            isPlayng = !isPlayng;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            player.Stop();
            PlayButton.Content = "Play";
            isPlayng = false;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Volume = VolumeSlider.Value;
        }

        /// <summary>
        /// Длина видео
        void videoDuration()
        {
            Duration duration = new Duration();
            Dispatcher.Invoke(new Action(()=>{ duration = player.NaturalDuration; }));

            while (duration.HasTimeSpan == false)
            {
                Dispatcher.Invoke(new Action(() => { duration = player.NaturalDuration; }));
            }
            TimeSpan timeSpan = duration.TimeSpan;
            //MessageBox.Show(timeSpan.TotalSeconds.ToString());
            Dispatcher.Invoke(new Action(() => { PositionSlider.Maximum = timeSpan.TotalSeconds; }));
        }

        void SliderMove()
        {
            while(isPlayng)
            {
                if (isSliderMove = false)
                {
                    sliderMove.Wait(1000);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        PositionSlider.Value = player.Position.TotalSeconds;
                    }));
                }
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;             //выделение нескольких файлов

            if (openFileDialog.ShowDialog() == true)
            {
                player.Stop();
                isPlayng = false;
                PlayButton.Content = "Play";
                Uri uri = new Uri(openFileDialog.FileName);
                player.Source = uri;
                fileNames = openFileDialog.FileNames;
                Form.Title = openFileDialog.FileName;
                Task.Factory.StartNew(videoDuration);
            }
        }

        private void PositionSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TimeSpan timeSpan = new TimeSpan(0, 0, (int)PositionSlider.Value);
            player.Position = timeSpan;
            isSliderMove = false;
        }

        private void PositionSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            isSliderMove = true;
        }
    }
}
