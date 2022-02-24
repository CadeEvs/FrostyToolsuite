using FrostySdk;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Frosty.Core.Converters
{
    public class StringToBitmapSourceConverter : IValueConverter
    {
        public static readonly ImageSource CopySource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Copy.png") as ImageSource;
        public static readonly ImageSource PasteSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Paste.png") as ImageSource;

        private static readonly ImageSource BlankSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/BlankFileType.png") as ImageSource;
        private static readonly ImageSource ImageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/ImageFileType.png") as ImageSource;
        private static readonly ImageSource SoundWaveSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/SoundFileType.png") as ImageSource;
        private static readonly ImageSource BlueprintSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/BlueprintFileType.png") as ImageSource;
        private static readonly ImageSource SubWorldSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/SubWorldFileType.png") as ImageSource;
        private static readonly ImageSource ShaderSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/ShaderFileType.png") as ImageSource;
        private static readonly ImageSource ShaderPresetSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/ShaderPresetFileType.png") as ImageSource;
        private static readonly ImageSource SpreadsheetSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/SpreadsheetFileType.png") as ImageSource;
        private static readonly ImageSource StatSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/StatFileType.png") as ImageSource;
        private static readonly ImageSource SkeletonSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/SkeletonFileType.png") as ImageSource;
        private static readonly ImageSource EncryptedSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/EncryptedFileType.png") as ImageSource;
        private static readonly ImageSource MovieTextureSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/MovieTextureFileType.png") as ImageSource;
        private static readonly ImageSource ArchiveSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/ArchiveFileType.png") as ImageSource;
        private static readonly ImageSource BlueprintBundleSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/BlueprintBundleFileType.png") as ImageSource;
        private static readonly ImageSource EmitterSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/EmitterFileType.png") as ImageSource;
        private static readonly ImageSource HavokSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/HavokFileType.png") as ImageSource;
        private static readonly ImageSource LogicPrefabSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/LogicPrefabFileType.png") as ImageSource;
        private static readonly ImageSource InternalSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/InternalFileType.png") as ImageSource;
        private static readonly ImageSource ObjectVariationSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/ObjectVariationFileType.png") as ImageSource;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;

            var definition = App.PluginManager.GetAssetDefinition(str);
            if (definition != null)
            {
                return definition.GetIcon();
            }

            if (str == null)
                return BlankSource;

            if (str == "EncryptedAsset")
                return EncryptedSource;

            if (str == "ShaderGraph")
                return ShaderSource;

            if (str == "SurfaceShaderPreset")
                return ShaderPresetSource;

            if (str == "DifficultyWeaponTableData" || str == "DifficultyNPCTableData")
                return SpreadsheetSource;

            if (TypeLibrary.IsSubClassOf(str, "HavokAsset") || TypeLibrary.IsSubClassOf(str, "PhysicsAsset"))
                return HavokSource;

            if (TypeLibrary.IsSubClassOf(str, "EmitterGraphBaseAsset") || TypeLibrary.IsSubClassOf(str, "EmitterBaseAsset") || TypeLibrary.IsSubClassOf(str, "EmitterAsset")) 
                return EmitterSource;

            if (TypeLibrary.IsSubClassOf(str, "ZeroLatencyImpulseResponseAsset"))
                return SoundWaveSource;

            if (TypeLibrary.IsSubClassOf(str, "BWBaseStat") || TypeLibrary.IsSubClassOf(str, "BWAggregatedStat"))
                return StatSource;

            if (TypeLibrary.IsSubClassOf(str, "SubWorldData"))
                return SubWorldSource;

            if (TypeLibrary.IsSubClassOf(str, "LogicPrefabBlueprint"))
                return LogicPrefabSource;

            if (TypeLibrary.IsSubClassOf(str, "Blueprint"))
                return BlueprintSource; 

            if (TypeLibrary.IsSubClassOf(str, "SkeletonAsset"))
                return SkeletonSource;

            if (TypeLibrary.IsSubClassOf(str, "BlueprintBundle"))
                return BlueprintBundleSource;

            if (TypeLibrary.IsSubClassOf(str, "ObjectVariation"))
                return ObjectVariationSource;

            if (TypeLibrary.IsSubClassOf(str, "DataContainer") && !TypeLibrary.IsSubClassOf(str, "Asset"))
                return InternalSource;

            if (TypeLibrary.IsSubClassOf(str, "MovieTextureAsset") || TypeLibrary.IsSubClassOf(str, "MovieTexture2Asset"))
                return MovieTextureSource;

            return BlankSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
