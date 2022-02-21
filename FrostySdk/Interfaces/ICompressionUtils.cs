using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrostySdk.Interfaces
{
    /// <summary>
    /// Describes an interface that defines compression information.
    /// </summary>
    public interface ICompressionUtils
    {
        /// <summary>
        /// Gets the the name of the dll file for the oodle compression version.
        /// </summary>
        /// <returns>A string containing the name of the dll file for the oodle compression version.</returns>
        string GetOodleDllName(string basePath);

        /// <summary>
        /// Gets the the name of the dll file for the ZStd compression version.
        /// </summary>
        /// <returns>A string containing the name of the dll file for the ZStd compression version.</returns>
        string GetZStdDllName();

        /// <summary>
        /// Gets a boolean deciding if the oodle binding process should be executed.
        /// </summary>
        bool LoadOodle { get; }

        /// <summary>
        /// Gets a boolean deciding if the ZStd binding process should be executed.
        /// </summary>
        bool LoadZStd { get; }

        /// <summary>
        /// <para>Gets an integer defining the compression level Oodle should utilize when compressing data.</para>
        /// <br><see cref="ProfileVersion.StarWarsBattlefrontII"/> and <see cref="ProfileVersion.Battlefield5"/> are set to 18, while the other profiles are set to 16 by default.</br>
        /// </summary>
        int OodleCompressionLevel { get; }
    }
}
