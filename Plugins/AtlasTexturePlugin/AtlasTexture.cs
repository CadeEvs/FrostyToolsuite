using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System;
using System.IO;
using FrostySdk.Managers.Entries;

namespace AtlasTexturePlugin
{
    public class AtlasTexture : Resource
    {
        public ushort Width => width;
        public ushort Height => height;
        public Stream Data => data;
        public Guid ChunkId => chunkId;
        public int MipCount
        {
            get
            {
                if (ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsBattlefrontII)
                    return 1;

                int tmpCount = 0;
                for (int i = 0; i < mipSizes.Length; i++)
                {
                    if (mipSizes[i] > 0)
                        tmpCount++;
                }
                return tmpCount;
            }
        }

        private ushort atlasType;
        private ushort width;
        private ushort height;
        private ushort unknown2;
        private float unknown3;
        private float unknown4;
        private Guid chunkId;
        private Stream data;
        private uint[] mipSizes = new uint[15];

        public AtlasTexture()
        {
        }

        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {
            base.Read(reader, am, entry, modifiedData);
            atlasType = reader.ReadUShort();
            width = reader.ReadUShort();
            height = reader.ReadUShort();
            unknown2 = reader.ReadUShort();
            unknown3 = reader.ReadFloat();
            unknown4 = reader.ReadFloat();
            chunkId = reader.ReadGuid();

            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII)
            {
                for (int i = 0; i < 15; i++)
                    mipSizes[i] = reader.ReadUInt();
            }

            data = am.GetChunk(am.GetChunkEntry(chunkId));
        }

        public AtlasTexture(AtlasTexture other)
        {
            atlasType = other.atlasType;
            unknown2 = other.unknown2;
            unknown3 = other.unknown3;
            unknown4 = other.unknown4;
            chunkId = other.chunkId;
            data = other.data;
        }

        public void SetData(int w, int h, Guid newChunkId, AssetManager am)
        {
            width = (ushort)w;
            height = (ushort)h;
            chunkId = newChunkId;
            data = am.GetChunk(am.GetChunkEntry(chunkId));

            if (ProfilesLibrary.DataVersion != (int)ProfileVersion.StarWarsBattlefrontII)
                return;

            uint totalSize = (uint)data.Length;
            int stride = 4;

            for (int i = 0; i < 15; i++)
            {
                if (totalSize > 0)
                {
                    w = Math.Max(1, w);
                    h = Math.Max(1, h);

                    uint mipSize = (uint)(Math.Max(1, ((w + 3) / 4)) * stride * h);
                    mipSizes[i] = mipSize;

                    totalSize -= mipSize;
                    w >>= 1;
                    h >>= 1;
                }
            }
        }

        public override byte[] SaveBytes()
        {
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.Write(atlasType);
                writer.Write(width);
                writer.Write(height);
                writer.Write(unknown2);
                writer.Write(unknown3);
                writer.Write(unknown4);
                writer.Write(chunkId);

                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII)
                {
                    for (int i = 0; i < 15; i++)
                        writer.Write(mipSizes[i]);
                }

                return writer.ToByteArray();
            }
        }
    }
}
