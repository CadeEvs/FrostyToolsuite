using DifficultyWeaponTableDataPlugin.Controls;
using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DifficultyWeaponTableDataPlugin.Definitions
{
    // This asset defintion is about as simple as they come. It defines a generic icon that will be used in all cases
    // for all instances of the registered asset type. And provides its own custom editor.

    public class DifficultyWeaponTableDataAssetDefinition : AssetDefinition
    {
        protected static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/DifficultyWeaponTableDataPlugin;component/Images/SpreadsheetFileType.png") as ImageSource;

        public override ImageSource GetIcon()
        {
            return imageSource;
        }

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new DifficultyWeaponTableEditor(logger);
        }
    }
}
