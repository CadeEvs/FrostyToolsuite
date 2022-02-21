using FrostySdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrostySdk.Interfaces
{
    /// <summary>
    /// Describes an extension to create a new game profile.
    /// </summary>
    public interface IProfile
    {
        /// <summary>
        /// Gets the type to use to construct a derived BinarySbReader. Must implement the <see cref="IBinarySbReader"/> interface.
        /// </summary>
        Type BinarySbReaderType { get; }

        /// <summary>
        /// Gets the type to define compression information. Must implement the <see cref="ICompressionUtils"/> interface.
        /// </summary>
        Type CompressionUtilsType { get; }

        /// <summary>
        /// When impplemented in a derived class, returns a new instance of a derived BinarySbReader. Must inherit from <see cref="IBinarySbReader"/>.
        /// </summary>
        IBinarySbReader GetBinarySbReader();

        /// <summary>
        /// When impplemented in a derived class, returns a new instance defining compression information. Must inherit from <see cref="ICompressionUtils"/>.
        /// </summary>
        ICompressionUtils GetCompressionUtils();

        /// <summary>
        /// When implemented in a derived class, allows the user to to create a new profile structure for this profile extension.
        /// </summary>
        Profile CreateProfile();
    }
}
