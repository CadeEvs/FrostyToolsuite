using Frosty.Core.Mod;
using System.Windows.Media;

namespace FrostyModManager
{
    public class FrostyAppliedMod
    {
        public string ModName
        {
            get
            {
                if (Mod != null)
                    return Mod.ModDetails.Title;
                else
                    return BackupFileName;
            }
        }
        public ImageSource ModIcon
        {
            get
            {
                if (Mod != null)
                    return Mod.ModDetails.Icon;
                else
                    return new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyModManager;component/Images/ModImportWarning.png") as ImageSource;
            }
        }

        public bool IsEnabled { get; set; }
        public bool IsFound { get; set; }
        public FrostyMod Mod { get; }

        public string BackupFileName { get; }

        public FrostyAppliedMod(FrostyMod inMod, bool inIsEnabled = true)
        {
            Mod = inMod;
            IsEnabled = inIsEnabled;
            IsFound = true;
        }

        public FrostyAppliedMod(string inBackupFileName, bool inIsEnabled = true)
        {
            BackupFileName = inBackupFileName;
            IsEnabled = inIsEnabled;
            IsFound = false;
        }
    }
}
