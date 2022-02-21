using System;

namespace Frosty.Core.Attributes
{
    /// <summary>
    /// This attribute registers a new asset definition to the plugin system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class RegisterAssetDefinitionAttribute : Attribute
    {
        /// <summary>
        /// Gets the name to use as the lookup type for this asset definition.
        /// </summary>
        /// <returns>The name to use as the lookup type.</returns>
        public string LookupType { get; set; }

        /// <summary>
        /// Gets the type to use to construct the asset definition class. Must inherit from <see cref="AssetDefinition"/>.
        /// </summary>
        /// <returns>The type to use to construct the asset definition class.</returns>
        public Type AssetDefinitionType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterAssetDefinitionAttribute"/> class with the lookup name and derived asset definition class type.
        /// </summary>
        /// <param name="lookupType">A string containing the name to use as the lookup type.</param>
        /// <param name="type">A <see cref="Type"/> representing the type to use to construct the asset definition class. Must inherit from <see cref="AssetDefinition"/>.</param>
        public RegisterAssetDefinitionAttribute(string lookupType, Type type)
        {
            LookupType = lookupType;
            AssetDefinitionType = type;
        }
    }
}
