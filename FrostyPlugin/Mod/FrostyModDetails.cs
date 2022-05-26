using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Frosty.Core.Mod
{
    public sealed class FrostyModDetails
    {
        public string Title { get; }
        public string Author { get; }
        public string Version { get; }
        public string Description { get; }
        public string Category => (category == "") ? "Misc" : category;
        public string Link { get; }
        public ImageSource Icon { get; private set; }
        public List<ImageSource> Screenshots { get; } = new List<ImageSource>();
        public List<FrostyModRequirement> Requirements { get; } = new List<FrostyModRequirement>();

        private string category;

        public FrostyModDetails(string inTitle, string inAuthor, string inCategory, string inVersion, string inDescription, string inModPageLink)
        {
            Title = inTitle;
            Author = inAuthor;
            Version = inVersion;
            Description = inDescription;
            category = inCategory;
            Link = inModPageLink;
        }

        public void SetIcon(byte[] buffer)
        {
            Icon = LoadImage(buffer);
        }

        public void AddScreenshot(byte[] buffer)
        {
            ImageSource screenshot = LoadImage(buffer);
            Screenshots.Add(screenshot);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ Title.GetHashCode();
                hash = (hash * 16777619) ^ Author.GetHashCode();
                hash = (hash * 16777619) ^ Version.GetHashCode();
                return hash;
            }
        }

        private BitmapImage LoadImage(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return null;

            BitmapImage image = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = ms;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
    }
}