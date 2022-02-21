namespace Frosty.Core
{
    /// <summary>
    /// Describes an extension to the options system. Classes that derive from this class can
    /// specify how to load, validate and save new options.
    /// </summary>
    public abstract class OptionsExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsExtension"/> class.
        /// </summary>
        public OptionsExtension()
        {
        }

        /// <summary>
        /// When implemented in a derived class, allows the user to specify how to load the data for this options extension.
        /// </summary>
        public virtual void Load() { }

        /// <summary>
        /// When implemented in a derived class, allows the user to specify how to validate this options extension.
        /// </summary>
        /// <returns>True if the options set are valid, false otherwise.</returns>
        public virtual bool Validate() => true;

        /// <summary>
        /// When implemented in a derived class, allows the user to specify how to save the data for this options extension.
        /// </summary>
        public virtual void Save() { }
    }
}
