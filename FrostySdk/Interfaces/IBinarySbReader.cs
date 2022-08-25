using FrostySdk.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrostySdk.Interfaces
{
    /// <summary>
    /// Describes an interface to create a new game BinarySbReader.
    /// </summary>
    public interface IBinarySbReader
    {
        /// <summary>
        /// Gets the all together amount of ebx, chunks, and res within a bundle.
        /// </summary>
        /// <returns>A uint with the count for all the ebx, chunks, and res within a bundle.</returns>
        uint TotalCount { get; }

        /// <summary>
        /// Parses a bundle and creates a DbObject containing the bundle's ebx, chunk, and res data.
        /// </summary>
        /// <param name="reader">A <see cref="DbReader"/> that will parse the bundle.</param>
        /// <returns>A DbObject containing the bundle's ebx, chunk, and res data.</returns>
        DbObject ReadDbObject(DbReader reader);
    }
}
