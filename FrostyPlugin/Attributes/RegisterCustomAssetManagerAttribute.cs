using System;
using FrostySdk.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frosty.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterCustomAssetManagerAttribute : Attribute
    {
        /// <summary>
        /// Gets the clas type to use to define a custom asset manager.
        /// </summary>
        /// <returns>The clas type to use to define a custom asset manager</returns>
        public Type CustomAssetManagerClassType { get; set; }

        /// <summary>
        /// Gets the type to use to define a custom asset manager.
        /// </summary>
        /// <returns>The type to use to define a custom asset manager</returns>
        public string CustomAssetManagerType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterCustomAssetManagerAttribute"/> class.
        /// </summary>
        /// <param name="type">The type of the custom asset manager/></param>
        /// <param name="classType">The class type of the custom asset manager. This type must derive from <see cref="ICustomAssetManager"/></param>
        public RegisterCustomAssetManagerAttribute(string type, Type classType)
        {
            CustomAssetManagerClassType = classType;
            CustomAssetManagerType = type;
        }
    }
}
