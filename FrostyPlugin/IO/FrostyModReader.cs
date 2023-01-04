using Frosty.Core.Mod;
using FrostySdk;
using FrostySdk.IO;
using System.IO;

namespace Frosty.Core.IO
{
    public sealed class FrostyModReader : NativeReader
    {
        public bool IsValid { get; } = false;
        public int GameVersion { get; }
        public uint Version { get; }

        private readonly long dataOffset;
        private readonly int dataCount;

        public FrostyModReader(Stream inStream)
            : base(inStream)
        {
            ulong magic = ReadULong();
            if (magic != FrostyMod.Magic)
                return;

            Version = ReadUInt();
            if (Version > FrostyMod.Version)
                return;

            dataOffset = ReadLong();
            dataCount = ReadInt();

            string profileName = ReadSizedString(ReadByte());
            if (profileName.ToLowerInvariant() != ProfilesLibrary.ProfileName.ToLowerInvariant())
                return;

            GameVersion = ReadInt();
            IsValid = true;
        }

        public FrostyModDetails ReadModDetails()
        {
            return new FrostyModDetails(
                ReadNullTerminatedString(),
                ReadNullTerminatedString(),
                ReadNullTerminatedString(),
                ReadNullTerminatedString(),
                ReadNullTerminatedString(),
                Version >= 5 ? ReadNullTerminatedString() : ""
                );
        }

        public BaseModResource[] ReadResources()
        {
            int count = ReadInt();
            BaseModResource[] resources = new BaseModResource[count];

            for (int i = 0; i < count; i++)
            {
                ModResourceType type = (ModResourceType)ReadByte();
                switch (type)
                {
                    case ModResourceType.Embedded: resources[i] = new EmbeddedResource(); break;
                    case ModResourceType.Ebx: resources[i] = new EbxResource(); break;
                    case ModResourceType.Res: resources[i] = new ResResource(); break;
                    case ModResourceType.Chunk: resources[i] = new ChunkResource(); break;
                    case ModResourceType.Bundle: resources[i] = new BundleResource(); break;
                    case ModResourceType.FsFile: resources[i] = new FsFileResource(); break;
                }

                resources[i].Read(this);
            }

            return resources;
        }

        public byte[] GetResourceData(BaseModResource resource)
        {
            if (resource.ResourceIndex == -1)
                return null;

            Position = dataOffset + (resource.ResourceIndex * 16);

            long offset = ReadLong();
            long size = ReadLong();

            Position = dataOffset + (dataCount * 16) + offset;
            return ReadBytes((int)size);
        }
    }
}
