using Frosty.Core.Converters;
using FrostySdk.Managers;
using System.Windows.Media;
using FrostySdk.Managers.Entries;

namespace Frosty.Core.Bookmarks
{
    /// <summary>
    /// A <see cref="BookmarkTarget"/> that points to a specific EBX asset by filename.
    /// </summary>
    public class AssetBookmarkTarget : BookmarkTarget
    {
        /// <summary>
        /// The path and type name of the referenced asset.
        /// </summary>
        public override string Text => Asset == null ? path : Asset.Name;

        /// <summary>
        /// The path of the referenced asset.
        /// </summary>
        public AssetEntry Asset { get; private set; }
        private string path;

        /// <summary>
        /// The type name of the referenced asset.
        /// </summary>
        private string TypeName => Asset == null ? "[Nonexistent Asset]" : Asset.Type;

        private static readonly AssetEntryToBitmapSourceConverter IconFinder = new AssetEntryToBitmapSourceConverter();
        /// <summary>
        /// Retrieves the icon that corresponds to <see cref="Asset"/>, as determined by <see cref="AssetEntryToBitmapSourceConverter"/>.
        /// </summary>
        public override ImageSource Icon => IconFinder.Convert(Asset, null, null, System.Globalization.CultureInfo.CurrentCulture) as ImageSource;

        /// <summary>
        /// Creates a blank AssetBookmarkTarget.
        /// </summary>
        public AssetBookmarkTarget()
        {
            // Called by System.Activator when loading from file
        }

        /// <summary>
        /// Creates an AssetBookmarkTarget referencing the asset at the given path.
        /// </summary>
        /// <param name="entry"></param>
        public AssetBookmarkTarget(AssetEntry entry)
        {
            Asset = entry;
            path = Asset.Name;
        }

        /// <summary>
        /// Directs the Data Explorer of the current Frosty Editor instance to select and open the referenced asset.
        /// </summary>
        public override void NavigateTo(bool bOpen)
        {
            Controls.FrostyDataExplorer explorer = App.EditorWindow.VisibleExplorer;
            explorer.SelectAsset(Asset);
            if (bOpen)
                explorer.DoubleClickSelectedAsset();
        }

        /// <summary>
        /// Sets this AssetBookmarkTarget to reference the asset at the given path.
        /// </summary>
        /// <param name="serializedData">The path of the asset to reference.</param>
        public override void LoadData(string serializedData)
        {
            string[] str = serializedData.Split(';');
            if (str.Length == 1)
            {
                Asset = App.AssetManager.GetEbxEntry(serializedData);
                path = serializedData;
            }
            else
            {
                Asset = str[0] == "ebx" ? App.AssetManager.GetEbxEntry(str[1]) : App.AssetManager.GetCustomAssetEntry(str[0], str[1]);
                path = str[1];
            }
        }

        /// <summary>
        /// Returns the path to the referenced asset.
        /// </summary>
        /// <returns></returns>
        protected override string SerializeData() => Asset == null ? path : Asset.AssetType + ";" + Asset.Name;
    }
}
