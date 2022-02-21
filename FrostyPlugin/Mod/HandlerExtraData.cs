using FrostySdk.Managers;

namespace Frosty.Core.Mod
{
    /// <summary>
    /// Holds the custom handler and the final data block for a given handler. Used by the mod manager to store
    /// the handlers per asset.
    /// </summary>
    public sealed class HandlerExtraData : AssetExtraData
    {
        /// <summary>
        /// Gets or sets the action handler.
        /// </summary>
        /// <returns>The action handler.</returns>
        public IModCustomActionHandler Handler { get; set; }

        /// <summary>
        /// Gets or sets the final data block.
        /// </summary>
        /// <returns>The final data block.</returns>
        public object Data { get; set; }
    }
}
