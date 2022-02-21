using System;

namespace Frosty.Core.Attributes
{
    /// <summary>
    /// This attribute registers a new property grid type editor for the specified type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class RegisterGlobalTypeEditorAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the type the type editor will be used for.
        /// </summary>
        /// <returns>The name of the type.</returns>
        public string LookupName { get; set; }

        /// <summary>
        /// Gets the type to use to construct the property grid type editor.
        /// </summary>
        /// <returns>The type of the property grid type editor.</returns>
        public Type EditorType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterGlobalTypeEditorAttribute"/> class using the lookup name and type editor type.
        /// </summary>
        /// <param name="lookupName">The name of the type the type editor will be used for</param>
        /// <param name="type">The type to use to construct the property grid type editor</param>
        public RegisterGlobalTypeEditorAttribute(string lookupName, Type type)
        {
            LookupName = lookupName;
            EditorType = type;
        }
    }
}
