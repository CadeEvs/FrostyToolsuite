using FrostySdk.Managers;
using System.Collections.Generic;

namespace Frosty.Core.Mod
{
    /// <summary>
    /// Provides functionality for the handling of custom data from saving and loading in the editor to loading and
    /// applying in the mod manager.
    /// </summary>
    public interface IModCustomActionHandler
    {
        /// <summary>
        /// Gets the type of action this handler performs. This has no impact on the actual functionality of the
        /// handler and is used to display information in the mod managers actions screen.
        /// </summary>
        HandlerUsage Usage { get; }

        /// <summary>
        /// Handles the loading and merging of the custom data.
        /// </summary>
        /// <param name="existing">The existing object data that has been loaded from previous mods.</param>
        /// <param name="newData">The byte array consisting of the data to be loaded.</param>
        /// <returns>The new object data.</returns>
        object Load(object existing, byte[] newData);

        /// <summary>
        /// Handles the actual modification of the base data with the custom data.
        /// </summary>
        /// <param name="origEntry">The asset currently being modified.</param>
        /// <param name="am">The asset manager.</param>
        /// <param name="runtimeResources">The container for storing any dynamically built resources.</param>
        /// <param name="data">The object data to be applied.</param>
        /// <param name="outData">The final assets data in a compressed byte array.</param>
        void Modify(AssetEntry origEntry, AssetManager am, RuntimeResources runtimeResources, object data, out byte[] outData);

        /// <summary>
        /// Returns a collection of sub resources that this handler modifies, this gives the handler an oppurtunity to list
        /// what changes it makes and might be overriden by other mods on a change by change basis.
        /// </summary>
        /// <param name="name">The name of the base resource.</param>
        /// <param name="data">The byte array consisting of the resource data.</param>
        /// <returns>A collection of sub resource strings. In the format of ResourceName;ResourceType;Action</returns>
        IEnumerable<string> GetResourceActions(string name, byte[] data);
    }
}
