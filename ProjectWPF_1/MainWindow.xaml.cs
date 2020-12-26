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
using System.IO;

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
            try
            {
                int i = 0;
                while(!reader.EndOfStream)
                {
                    fileNames[i] = reader.ReadLine();
                    PlayList.Items.Add(fileNames[i])
                }
            }
            catch
            {

            }

        }

        bool isPlayng = true;
        Task sliderMove;
        bool isSliderMove = false;
        string[] fileNames;
        int index = 0;


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
            Dispatcher.Invoke(new Action(() => { duration = player.NaturalDuration; }));

            while (duration.HasTimeSpan == false)
            {
                Dispatcher.Invoke(new Action(() => { duration = player.NaturalDuration; }));
            }
            TimeSpan timeSpan = duration.TimeSpan;
            Dispatcher.Invoke(new Action(() => { PositionSlider.Maximum = timeSpan.TotalSeconds; }));
            Dispatcher.Invoke(new Action(() =>
            {
                TimeLabelEnd.Content = (PositionSlider.Value / 60).ToString() + ":"
                            + Convert.ToInt32(PositionSlider.Value % 60).ToString();
            }));

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
                        TimeLabelBegin.Content = (PositionSlider.Value / 60).ToString() + ":"
                        + Convert.ToInt32(PositionSlider.Value%60).ToString();
                        
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
                index = 0;
                player.Stop();
                isPlayng = false;
                PlayButton.Content = "Play";
                Uri uri = new Uri(openFileDialog.FileName);
                player.Source = uri;
                fileNames = openFileDialog.FileNames;
                Form.Title = openFileDialog.FileName;
                Task.Factory.StartNew(videoDuration);
                PlayList.Items.Clear();
                foreach(string name in fileNames)
                {
                    PlayList.Items.Add(name.Remove(0,name.LastIndexOf("\\")+1));
                }
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

        void SetSourse(int index)
        {
            player.Stop();
            isPlayng = false;
            PlayButton.Content = "Play";
            try
            {
                Form.Title = fileNames[index];
                player.Source = new Uri(fileNames[PlayList.SelectedIndex]);
            }
            catch
            {
            }
        }

        private void PlayList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlayList.SelectedIndex >= 0)
            {
                SetSourse(PlayList.SelectedIndex);
            }
        }

        private void PlayList_MouseEnter(object sender, MouseEventArgs e)
        {
            PlayListColumn.Width = new GridLength(200);
        }

        private void PlayList_MouseLeave(object sender, MouseEventArgs e)
        {
            PlayListColumn.Width = new GridLength(15);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            index++;
            if(index == PlayList.Items.Count)
            {
                index = 0;
            }
            PlayList.SelectedIndex = index;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            index--;
            if (index == -1)
            {
                index = PlayList.Items.Count - 1;
            }
            PlayList.SelectedIndex = index;
        }

        private void Form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StreamWriter streamWriter = new StreamWriter("playlist");

            foreach(string name in fileNames)
            {
                streamWriter.WriteLine(name);
            }
            streamWriter.Close();
        }
    }
}
