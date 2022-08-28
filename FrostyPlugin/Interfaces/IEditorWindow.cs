using Frosty.Core.Controls;
using FrostySdk.Managers;
using System.Windows.Controls;

namespace Frosty.Core.Interfaces
{
    public interface IEditorWindow
    {
        FrostyDataExplorer DataExplorer { get; }
        FrostyDataExplorer LegacyExplorer { get; }
        FrostyDataExplorer VisibleExplorer { get; }
        TabControl MiscTabControl { get; }

        void OpenAsset(AssetEntry asset, bool createDefaultEditor = true);
        void OpenEditor(string title, FrostyBaseEditor editor);
        AssetEntry GetOpenedAssetEntry();
    }
}
