using FrostySdk.IO;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Frosty.Controls
{
    [TemplatePart(Name = PART_Button, Type = typeof(Button))]
    [TemplatePart(Name = PART_ClearButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_Image, Type = typeof(Image))]
    public class FrostyImageButton : Control
    {
        private const string PART_Button = "PART_Button";
        private const string PART_ClearButton = "PART_ClearButton";
        private const string PART_Image = "PART_Image";

        #region -- Properties --

        #region -- Title --
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(FrostyImageButton));
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        #endregion

        #endregion

        #region -- Events --
        public delegate bool OnValidateDelegate(object sender, FileInfo fi, BitmapImage bimage);
        private event OnValidateDelegate onValidate;
        public event OnValidateDelegate OnValidate { add => onValidate += value;  remove => onValidate -= value; }
        #endregion

        private Button button;
        private Button clearButton;
        private Image image;
        private byte[] imageData;

        static FrostyImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostyImageButton), new FrameworkPropertyMetadata(typeof(FrostyImageButton)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            button = GetTemplateChild(PART_Button) as Button;
            image = GetTemplateChild(PART_Image) as Image;
            clearButton = GetTemplateChild(PART_ClearButton) as Button;

            button.Click += Button_Click;
            clearButton.Click += ClearButton_Click;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            imageData = null;
            image.Source = null;
            button.Opacity = 1.0;
            clearButton.Visibility = Visibility.Collapsed;
        }

        public void SetImage(byte[] inData)
        {
            imageData = inData;
            if (imageData != null)
            {
                BitmapImage bimage = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    bimage.BeginInit();
                    bimage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    bimage.CacheOption = BitmapCacheOption.OnLoad;
                    bimage.UriSource = null;
                    bimage.StreamSource = ms;
                    bimage.EndInit();
                }
                image.Source = bimage;
                button.Opacity = 0.0;
                clearButton.Visibility = Visibility.Visible;
            }
        }

        public byte[] GetImage() { return imageData; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            OpenFileDialog ofd = new OpenFileDialog {Filter = "(All supported formats)|*.png;*.jpg;*.gif|*.png (PNG File)|*.png|*.jpg (JPEG File)|*.jpg|*.gif (GIF File)|*.gif"};

            if (ofd.ShowDialog() == true)
            {
                byte[] buffer = null;
                using (NativeReader reader = new NativeReader(new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read)))
                    buffer = reader.ReadToEnd();

                BitmapImage bimage = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    bimage.BeginInit();
                    bimage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    bimage.CacheOption = BitmapCacheOption.OnLoad;
                    bimage.UriSource = null;
                    bimage.StreamSource = ms;
                    bimage.EndInit();
                }

                if (onValidate?.Invoke(this, new FileInfo(ofd.FileName), bimage) == true)
                {
                    imageData = buffer;
                    image.Source = bimage;
                    button.Opacity = 0.0;
                    clearButton.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
