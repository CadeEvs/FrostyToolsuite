using FrostySdk.BaseProfile;
using FrostySdk.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrostySdk.Interfaces
{
    public interface IBinarySbWriter
    {
        /// <summary>
        /// Writes a bundle with the <see cref="BaseBinarySb.GetMagic"/> and the salt from <see cref="BaseBinarySb.GetSalt"/>.
        /// </summary>
        /// <param name="writer">A <see cref="DbWriter"/> that will write the bundle.</param>
        /// <param name="bundleObj">A <see cref="DbObject"/> containing the bundle's ebx, chunk, and res data.</param>
        /// <param name="endian">The Endianness the bundle is going to be written in.</param>
        void Write(DbWriter writer, DbObject bundleObj, Endian endian);
    }
}
