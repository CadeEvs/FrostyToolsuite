using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Frosty.Hash;
using FrostySdk.Interfaces;
using FrostySdk.Managers;

namespace FrostySdk.IO
{
    public class BinarySbReader : DbReader
    {
        public uint TotalCount { get => binarySbReader.TotalCount; }

        private IBinarySbReader binarySbReader;
        public BinarySbReader(Stream inStream, IDeobfuscator inDeobfuscator)
            : base(inStream, inDeobfuscator)
        {
            binarySbReader = ProfilesLibrary.Profile.GetBinarySbReader();
        }

        public override DbObject ReadDbObject()
        {
            return binarySbReader.ReadDbObject(this);
        }
    }
}