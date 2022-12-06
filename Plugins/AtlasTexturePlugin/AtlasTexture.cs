using FrostySdk;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;
using FrostySdk.Resources;
using System;
using System.IO;

namespace AtlasTexturePlugin
{
    [Flags]
    public enum AtlasFlags : ushort
    {
        NormalMap = 1,
        PerFrameBorder = 2,
        LightCookie = 4,
        LightPrefilteredCookie = 8
    }

    public class AtlasTexture : Resource
    {
        public ushort Width => m_width;
        public ushort Height => m_height;
        public Stream Data => m_data;
        public Guid ChunkId => m_chunkId;
        public int MipCount
        {
            get
            {
                if (m_version < 3)
                    return 1;

                for (int i = 0; i < m_mipSizes.Length; i++)
                {
                    if (m_mipSizes[i] == 0)
                        return i;
                }
                return m_mipSizes.Length;
            }
        }
        public int Version => m_version;
        public AtlasFlags AtlasType => m_atlasFlags;
        public uint NameHash => m_nameHash;
        public ushort Unknown => m_unknown;
        public float BorderWidth => m_borderWidth;
        public float BorderHeight => m_borderHeight;


        private int m_version;
        private uint m_nameHash;
        private AtlasFlags m_atlasFlags;
        private ushort m_width;
        private ushort m_height;
        private ushort m_unknown;
        private float m_borderWidth;
        private float m_borderHeight;
        private Guid m_chunkId;
        private Stream m_data;
        private uint[] m_mipSizes = new uint[15];

        public AtlasTexture()
        {
        }

        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {
            base.Read(reader, am, entry, modifiedData);
            
            m_version = BitConverter.ToInt32(resMeta, 0);
            m_nameHash = BitConverter.ToUInt32(resMeta, 4);

            m_atlasFlags = (AtlasFlags)reader.ReadUShort();
            m_width = reader.ReadUShort();
            m_height = reader.ReadUShort();
            m_unknown = reader.ReadUShort();
            m_borderWidth = reader.ReadFloat();
            m_borderHeight = reader.ReadFloat();
            m_chunkId = reader.ReadGuid();

            if (m_version >= 3)
            {
                for (int i = 0; i < 15; i++)
                    m_mipSizes[i] = reader.ReadUInt();
            }

            m_data = am.GetChunk(am.GetChunkEntry(m_chunkId));
        }

        public AtlasTexture(AtlasTexture other)
        {
            m_version = other.m_version;
            m_nameHash = other.m_nameHash;
            m_atlasFlags = other.m_atlasFlags;
            m_unknown = other.m_unknown;
            m_borderWidth = other.m_borderWidth;
            m_borderHeight = other.m_borderHeight;
            m_chunkId = other.m_chunkId;
            m_data = other.m_data;
            resMeta = other.resMeta;
        }

        public void SetData(int w, int h, Guid newChunkId, AssetManager am)
        {
            m_width = (ushort)w;
            m_height = (ushort)h;
            m_chunkId = newChunkId;
            m_data = am.GetChunk(am.GetChunkEntry(m_chunkId));

            uint totalSize = (uint)m_data.Length;
            int stride = 4;

            if (m_version >= 3)
            {
                for (int i = 0; i < 15; i++)
                {
                    if (totalSize > 0)
                    {
                        w = Math.Max(1, w);
                        h = Math.Max(1, h);

                        uint mipSize = (uint)(Math.Max(1, ((w + 3) / 4)) * stride * h);
                        m_mipSizes[i] = mipSize;

                        totalSize -= mipSize;
                        w >>= 1;
                        h >>= 1;
                    }
                }
            }
        }

        public void SetNameHash(uint nameHash)
        {
            m_nameHash = nameHash;
        }

        public override byte[] SaveBytes()
        {
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.Write((ushort)m_atlasFlags);
                writer.Write(m_width);
                writer.Write(m_height);
                writer.Write(m_unknown);
                writer.Write(m_borderWidth);
                writer.Write(m_borderHeight);
                writer.Write(m_chunkId);

                if (m_version >= 3)
                {
                    for (int i = 0; i < 15; i++)
                        writer.Write(m_mipSizes[i]);
                }

                unsafe
                {
                    // update the res meta
                    fixed (byte* ptr = &resMeta[0])
                    {
                        *(int*)(ptr + 0) = m_version;
                        *(uint*)(ptr + 4) = m_nameHash;
                    }
                }

                return writer.ToByteArray();
            }
        }
    }
}
