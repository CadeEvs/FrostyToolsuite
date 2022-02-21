using System;

namespace Frosty.Core.Attributes
{
    /// <summary>
    /// This attribute registers a referenced assembly to the plugin system, any attempts to load this assembly will be redirected to the
    /// third party directory.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterThirdPartyDllAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the referenced assembly without the extension.
        /// </summary>
        /// <returns>The name of the referenced assembly.</returns>
        public string DllName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterThirdPartyDllAttribute"/> class with the referenced assembly name.
        /// </summary>
        /// <param name="name">A string representing the name of the referenced assembly without the extension</param>
        public RegisterThirdPartyDllAttribute(string name)
        {
            DllName = name;
        }
    }
}
