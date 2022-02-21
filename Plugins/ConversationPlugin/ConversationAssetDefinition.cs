using Frosty.Core;
using Frosty.Core.Controls;
using FrostySdk.Interfaces;
using System.Windows.Media;

namespace ConversationPlugin
{
    public class ConversationAssetDefinition : AssetDefinition
    {
        protected static ImageSource imageSource = new ImageSourceConverter().ConvertFromString("pack://application:,,,/ConversationPlugin;component/Images/ConversationFileType.png") as ImageSource;
        public override ImageSource GetIcon()
        {
            return imageSource;
        }

        public override FrostyAssetEditor GetEditor(ILogger logger)
        {
            return new ConversationEditor(logger);
        }
    }
}
