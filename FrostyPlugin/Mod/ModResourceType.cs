
namespace Frosty.Core.Mod
{
    /// <summary>
    /// Represents the type of the data the resource represents.
    /// </summary>
    public enum ModResourceType
    {
        /// <summary>
        /// Invalid resource type.
        /// </summary>
        Invalid = -1,

        /// <summary>
        /// Embedded data such as icons or images.
        /// </summary>
        Embedded,

        /// <summary>
        /// Data relating to ebx assets.
        /// </summary>
        Ebx,

        /// <summary>
        /// Data relating to resources.
        /// </summary>
        Res,

        /// <summary>
        /// Data relating to chunks.
        /// </summary>
        Chunk,

        /// <summary>
        /// Data relating to bundles.
        /// </summary>
        Bundle
    }
}
