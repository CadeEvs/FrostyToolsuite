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
        /// <param name="containsUncompressedData">A boolean deciding if the bundle contains any uncompressed data.</param>
        /// <param name="bundleOffset">A long containing the offset of the bundle.</param>
        /// <returns>A DbObject containing the bundle's ebx, chunk, and res data.</returns>
        DbObject ReadDbObject(DbReader reader, bool containsUncompressedData, long bundleOffset);

        /// <summary>
        /// Parses a datablock within a bundle and adds additiona data to the referenced <see cref="DbObject"/> list.
        /// </summary>
        /// <param name="reader">A <see cref="DbReader"/> that will parse the data block.</param>
        /// <param name="list">A <see cref="DbObject"/> list of all the bundle DbObjects.</param>
        /// <param name="containsUncompressedData">A boolean deciding if the bundle contains any uncompressed data.</param>
        /// <param name="bundleOffset"> long containing the offset of the bundle.</param>
        void ReadDataBlock(DbReader reader, DbObject list, bool containsUncompressedData, long bundleOffset);
    }
}
