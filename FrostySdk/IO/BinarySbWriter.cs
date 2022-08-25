using Frosty.Hash;
using FrostySdk.BaseProfile;
using FrostySdk.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrostySdk.IO
{
    public class BinarySbWriter : DbWriter
    {
        private Endian endian;
        private IBinarySbWriter binarySbWriter;

        public BinarySbWriter(Stream inStream, bool leaveOpen = false, Endian inEndian = Endian.Big)
            : base(inStream, leaveOpen: leaveOpen)
        {
            endian = inEndian;
            binarySbWriter = ProfilesLibrary.Profile.GetBinarySbWriter();
        }

        public override void Write(DbObject inObj)
        {
            binarySbWriter.Write(this, inObj, endian);
        }
    }
}
