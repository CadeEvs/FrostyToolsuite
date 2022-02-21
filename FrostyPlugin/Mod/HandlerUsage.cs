namespace Frosty.Core.Mod
{
    /// <summary>
    /// Represents the usage of the handler, what types of actions it performs on its sub data.
    /// </summary>
    public enum HandlerUsage
    {
        /// <summary>
        /// Completely modifies or replaces the data per mod.
        /// </summary>
        Modify,

        /// <summary>
        /// Merges data from one mod to another.
        /// </summary>
        Merge
    }
}
