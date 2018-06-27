using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Threading;

namespace YouThumbnailer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            media.Volume = 0;

        }

        void timer_Tick(object sender, EventArgs e)
        {
            //if (media.Source != null)
            //{
            //    if (media.NaturalDuration.HasTimeSpan)
            //        timeChange.Text = String.Format("{0} / {1}", media.Position.ToString(@"mm\:ss"), media.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            //}
            //else timeChange.Text = "No file selected...";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap bmp =
              new RenderTargetBitmap(1280, 720,
                96, 96, PixelFormats.Default);
            Rect bounds = VisualTreeHelper.GetDescendantBounds(canvas);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(canvas);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }
            bmp.Render(dv);

            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            encoder.QualityLevel = 100;

            string filename = Guid.NewGuid().ToString() + ".jpg";
            FileStream fs = new FileStream(filename, FileMode.Create);
            encoder.Save(fs);
            fs.Close();

            Process.Start(filename);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media.Position = TimeSpan.FromSeconds(slider.Value);
        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            slider.Maximum = media.NaturalDuration.TimeSpan.TotalSeconds;
            media.ScrubbingEnabled = true;
        }

        private void btnText_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
