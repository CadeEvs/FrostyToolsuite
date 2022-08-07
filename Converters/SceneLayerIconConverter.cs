using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using LevelEditorPlugin.Entities;

namespace LevelEditorPlugin.Converters
{
    public class SceneLayerIconConverter : IValueConverter
    {
        private static ImageSource layerReferenceIcon = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/BlueprintFileType.png") as ImageSource;
        private static ImageSource subWorldReferenceIcon = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/SubWorldFileType.png") as ImageSource;
        private static ImageSource staticModelGroupIcon = new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Assets/blueprintbundlefiletype.png") as ImageSource;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Entity entity = value as Entities.Entity;
            if (entity is Entities.WorldReferenceObject)
            {
                return subWorldReferenceIcon;
            }
            else if (entity is Entities.SubWorldReferenceObject)
            {
                return subWorldReferenceIcon;
            }
            else if (entity is Entities.LayerReferenceObject)
            {
                return layerReferenceIcon;
            }
            else if (entity is Entities.StaticModelGroupEntity)
            {
                return staticModelGroupIcon;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
