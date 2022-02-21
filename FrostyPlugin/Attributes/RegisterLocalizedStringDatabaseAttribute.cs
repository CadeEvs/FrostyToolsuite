using System;

namespace Frosty.Core.Attributes
{
    /// <summary>
    /// This attribute registers the specified type as the type to use to construct the localized string database for use within
    /// the editor. The type must implement the <see cref="ILocalizedStringDatabase"/> interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class RegisterLocalizedStringDatabaseAttribute : Attribute
    {
        /// <summary>
        /// Gets the type to use to construct the localized string database. Must implement the <see cref="ILocalizedStringDatabase"/> interface.
        /// </summary>
        /// <returns>The type to use to construct the localized string database.</returns>
        public Type LocalizedStringDatabaseType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterLocalizedStringDatabaseAttribute"/> class with the localized string database type.
        /// </summary>
        /// <param name="type">A type containing the type to use to construct the localized string database. Must implement the <see cref="ILocalizedStringDatabase"/> interface.</param>
        public RegisterLocalizedStringDatabaseAttribute(Type type)
        {
            LocalizedStringDatabaseType = type;
        }
    }
}
