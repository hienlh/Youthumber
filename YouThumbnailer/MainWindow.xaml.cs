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
        private FontFamily fontFamily = new System.Windows.Media.FontFamily("Arial");
        private double fontSize = 14;
        private FontStyle fontStyle = FontStyles.Normal;
        private FontWeight fontWeight = FontWeights.Normal;
        private TextAlignment textAlignment = TextAlignment.Left;
        private Brush foreground = Brushes.Black;
        private TextDecorationCollection textDecorations = null;
        private VerticalAlignment verticalContentAlignment = VerticalAlignment.Top;

        private TextBox selectedTextbox;

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
            canvas.Cursor = Cursors.IBeam;
            canvas.EditingMode = InkCanvasEditingMode.None;
        }

        private void canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (canvas.Cursor == Cursors.IBeam && e.Source.GetType() != typeof(TextBox))  //Insert text
            {
                TextBox txt = new TextBox();
                txt.Text = "Add text here.";
                txt.TextWrapping = TextWrapping.Wrap;
                txt.AcceptsReturn = true;
                txt.FontFamily = fontFamily;
                txt.FontSize = fontSize;
                txt.FontStyle = fontStyle;
                txt.FontWeight = fontWeight;
                txt.TextAlignment = textAlignment;
                txt.Foreground = foreground;
                txt.TextDecorations = textDecorations;
                txt.VerticalContentAlignment = verticalContentAlignment;

                txt.Background = Brushes.Transparent;
                txt.BorderBrush = Brushes.Transparent;
                txt.VerticalContentAlignment = VerticalAlignment.Top;

                InkCanvas.SetLeft(txt, e.GetPosition(canvas).X);
                InkCanvas.SetTop(txt, e.GetPosition(canvas).Y);

                canvas.Children.Add(txt);
                canvas.EditingMode = InkCanvasEditingMode.Select;
                canvas.Cursor = Cursors.Arrow;

                selectedTextbox = txt;
                canvas.Select(canvas.Strokes, new UIElement[] { txt });
            }
            else if (e.Source.GetType() == typeof(TextBox))
            {
                selectedTextbox = e.Source as TextBox;
                canvas.Select(canvas.Strokes, new UIElement[] { e.Source as TextBox });
                UpdateSetUpTextBox(selectedTextbox);
            }
            else
            {
                canvas.Cursor = Cursors.Arrow;
            }
        }
        private void btnTextAlignLeft_Selected(object sender, RoutedEventArgs e)
        {
            textAlignment = TextAlignment.Left;
            if (selectedTextbox != null)
                selectedTextbox.TextAlignment = TextAlignment.Left;
        }

        private void btnTextAlignCenter_Selected(object sender, RoutedEventArgs e)
        {
            textAlignment = TextAlignment.Center;
            if (selectedTextbox != null)
                selectedTextbox.TextAlignment = TextAlignment.Center;
        }

        private void btnTextAlignRight_Selected(object sender, RoutedEventArgs e)
        {
            textAlignment = TextAlignment.Right;
            if (selectedTextbox != null)
                selectedTextbox.TextAlignment = TextAlignment.Right;
        }

        private void btnTextAlignJustify_Selected(object sender, RoutedEventArgs e)
        {
            textAlignment = TextAlignment.Justify;
            if (selectedTextbox != null)
                selectedTextbox.TextAlignment = TextAlignment.Justify;
        }

        private void btnTextAllignTop_Selected(object sender, RoutedEventArgs e)
        {
            verticalContentAlignment = VerticalAlignment.Top;
            if (selectedTextbox != null)
                selectedTextbox.VerticalContentAlignment = VerticalAlignment.Top;
        }

        private void btnTextVerticalAlignCenter_Selected(object sender, RoutedEventArgs e)
        {
            verticalContentAlignment = VerticalAlignment.Center;
            if (selectedTextbox != null)
                selectedTextbox.VerticalContentAlignment = VerticalAlignment.Center;
        }

        private void btnTextAllignBottom_Selected(object sender, RoutedEventArgs e)
        {
            verticalContentAlignment = VerticalAlignment.Bottom;
            if (selectedTextbox != null)
                selectedTextbox.VerticalContentAlignment = VerticalAlignment.Bottom;
        }

        private void UpdateSetUpTextBox(TextBox t)
        {
            fontFamily = t.FontFamily;
            fontSize = t.FontSize;
            fontStyle = t.FontStyle;
            fontWeight = t.FontWeight;
            foreground = t.Foreground;
            textAlignment = t.TextAlignment;
            verticalContentAlignment = t.VerticalContentAlignment;
            textDecorations = t.TextDecorations;
        }

        private void tbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            fontSize = Convert.ToInt32(tbFontSize.Text);
            if (selectedTextbox != null)
                selectedTextbox.FontSize = fontSize;
        }

        private void btnVideo_Click(object sender, RoutedEventArgs e)
        {
            canvas.Cursor = Cursors.Arrow;
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Videos"; // Default file name 
            dialog.DefaultExt = ".mp4"; // Default file extension 
            dialog.Filter = "MP4 Files (*.mp4)|*.mp4"; // Filter files by extension  

            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                media.Source = new Uri(dialog.FileName);
            }
        }

        private void btnFrame_Click(object sender, RoutedEventArgs e)
        {
            canvas.Cursor = Cursors.Arrow;
            canvas.EditingMode = InkCanvasEditingMode.Select;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "PNG Files (*.png)|*.png";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                frameImage.Source = new BitmapImage(new Uri(dlg.FileName));
            }
        }
    }
}
