using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Drawing;

namespace Frosty.Core
{
    public class FrostyConfiguration
    {
        public ImageSource Thumbnail { get; private set; }

        public string GamePath { get; }
        public string ProfileName { get; }
        public string GameName { get; private set; }

        public FrostyConfiguration()
        {
            Thumbnail = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Warning.png") as ImageSource;
        }

        public FrostyConfiguration(string profile) : this()
        {
            ProfileName = profile;
            GamePath = Config.Get<string>("GamePath", "", ConfigScope.Game, profile);

            FileVersionInfo vi = FileVersionInfo.GetVersionInfo(System.IO.Path.Combine(GamePath, ProfileName) + ".exe");
            GameName = vi.ProductName;

            // Try to extract the icon
            try
            {
                Icon sysicon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(GamePath, ProfileName) + ".exe");
                Thumbnail = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    sysicon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                sysicon.Dispose();
            }
            catch
            {

            }
        }
    }
}
