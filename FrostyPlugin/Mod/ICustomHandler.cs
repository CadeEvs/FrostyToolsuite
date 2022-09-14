using Frosty.Core.IO;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;

namespace Frosty.Core.Mod
{
    /// <summary>
    /// Provides functionality for the handling of custom data from saving and loading in the editor to loading and
    /// applying in the mod manager.
    /// </summary>
    public interface ICustomActionHandler : IModCustomActionHandler
    {
        /// <summary>
        /// Handles the saving of the custom data to a mod.
        /// </summary>
        /// <param name="writer">The writer that is writing the mod.</param>
        /// <param name="entry">The asset being written.</param>
        void SaveToMod(FrostyModWriter writer, AssetEntry entry);
    }
}
