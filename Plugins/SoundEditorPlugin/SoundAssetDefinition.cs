using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk;
using FrostySdk.Interfaces;
using System.Windows.Media;

namespace SoundEditorPlugin
{
    public class SoundWaveAssetOverride : BaseTypeOverride
    {
//#if FROSTY_DEVELOPER
        public BaseFieldOverride Chunks { get; set; }
        public BaseFieldOverride RuntimeVariations { get; set; }
        public BaseFieldOverride Segments { get; set; }
        public BaseFieldOverride Localization { get; set; }
        public BaseFieldOverride SubtitleStringIds { get; set; }
        public BaseFieldOverride Subtitles { get; set; }
//#else
        //[IsHidden]
        //public BaseFieldOverride Chunks { get; set; }
        //[IsHidden]
        //public BaseFieldOverride RuntimeVariations { get; set; }
        //[IsHidden]
        //public BaseFieldOverride Segments { get; set; }
        //[IsHidden]
        //public BaseFieldOverride Localization { get; set; }
        //[IsHidden]
        //public BaseFieldOverride SubtitleStringIds { get; set; }
        //[IsHidden]
        //public BaseFieldOverride Subtitles { get; set; }
//#endif
    }

    public class SoundAssetDefinition : AssetDefinition
    {
        protected static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/SoundEditorPlugin;component/Images/SoundFileType.png") as ImageSource;
        public override ImageSource GetIcon()
        {
            return imageSource;
        }
    }

    public class SoundWaveAssetDefinition : SoundAssetDefinition
    {
        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new FrostySoundWaveEditor(logger);
        }
    }
    public class NewWaveAssetDefinition : SoundAssetDefinition
    {
        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new FrostyNewWaveEditor(logger);
        }
    }
    public class HarmonySampleBankAssetDefinition : SoundAssetDefinition
    {
        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new FrostyHarmonySampleBankEditor(logger);
        }
    }
    public class OctaneAssetDefinition : SoundAssetDefinition
    {
        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new FrostyOctaneSoundEditor(logger);
        }
    }
    public class ImpulseResponseAssetDefinition : SoundAssetDefinition
    {
        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new FrostyImpulseResponseEditor(logger);
        }
    }
}
