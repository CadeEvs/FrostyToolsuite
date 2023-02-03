using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Diagnostics;
using System.IO;
using FrostySdk.Managers.Entries;

namespace FrostySdk.Resources
{
    public enum TextureType
    {
        TT_2d = 0x0,
        TT_Cube = 0x1,
        TT_3d = 0x2,
        TT_2dArray = 0x3,
        TT_1dArray = 0x4,
        TT_1d = 0x5,
        TT_CubeArray = 0x6,
    };

    [Flags]
    public enum TextureFlags
    {
        Streaming = 1 << 0,
        SrgbGamma = 1 << 1,
        CpuResource = 1 << 2,
        OnDemandLoaded = 1 << 3,
        Mutable = 1 << 4,
        NoSkipmip = 1 << 5,
        XenonPackedMipmaps = 1 << 8,
        Ps3MemoryCell = 1 << 8,
        Ps3MemoryRsx = 1 << 9,
        StreamingAlways = 1 << 10,
        SwizzledData = 1 << 11
    }

    public class Texture : Resource, IDisposable
    {
        public uint FirstMipOffset
        {
            get => m_compressedMipOffsets[0];
            set => m_compressedMipOffsets[0] = value;
        }
        public uint SecondMipOffset
        {
            get => m_compressedMipOffsets[1];
            set => m_compressedMipOffsets[1] = value;
        }
        public string PixelFormat
        {
            get
            {
                string enumType = "RenderFormat";
                string retVal = Enum.Parse(TypeLibrary.GetType(enumType), m_pixelFormat.ToString()).ToString();
                return retVal.Replace(enumType + "_", "");
            }
        }
        public TextureType Type { get; private set; }
        public TextureFlags Flags { get; set; }
        public ushort Width { get; private set; }
        public ushort Height { get; private set; }
        public ushort SliceCount
        {
            get => m_sliceCount;
            set
            {
                m_sliceCount = value;
                if (Type == TextureType.TT_2dArray || Type == TextureType.TT_3d)
                {
                    Depth = m_sliceCount;
                }
            }
        }
        public ushort Depth { get; private set; }
        public byte MipCount { get; private set; }
        public byte FirstMip { get; set; }
        public uint[] MipSizes { get; } = new uint[15];
        public string TextureGroup { get; set; }
        public uint AssetNameHash { get; set; }
        public Stream Data { get; private set; }
        public uint LogicalOffset { get; set; }
        public uint LogicalSize { get; set; }
        public uint RangeStart { get; set; }
        public uint RangeEnd { get; set; }
        public uint[] Unknown3 { get; } = new uint[4];
        public Guid ChunkId
        {
            get => m_chunkId;
            set => m_chunkId = value;
        }
        public uint ChunkSize { get; private set; }
        public uint Version => m_version;

        private uint m_version;
        private uint[] m_compressedMipOffsets = new uint[2];
        private int m_pixelFormat;
        private uint m_customPoolId;
        private ushort m_sliceCount;
        private Guid m_chunkId;

        public Texture()
        {
        }

        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {
            base.Read(reader, am, entry, modifiedData);

            /* Texture Versions:
             * 10 - Unknown
             * 11 - Version and 32 bit flags now stored in resMeta; storing of mipOffsets
             * 12 - Storing of customPoolId
             * 13 - Unknown block before the nameHash (only used in MEA)
             */
            m_version = BitConverter.ToUInt32(resMeta, 0);

            if (m_version == 0)
            {
                m_version = reader.ReadUInt();
            }

#if FROSTY_DEVELOPER
            Debug.Assert(10 <= m_version && m_version <= 13);
#endif

            if (m_version >= 11)
            {
                // offsets in compressed chunk of 2nd and 3rd mip, 1st mip is at 0
                m_compressedMipOffsets[0] = reader.ReadUInt();
                m_compressedMipOffsets[1] = reader.ReadUInt();
            }

            Type = (TextureType)reader.ReadUInt();
            m_pixelFormat = reader.ReadInt();

            if (m_version >= 12)
            {
                m_customPoolId = reader.ReadUInt();
            }

            Flags = (TextureFlags)(m_version >= 11 ? reader.ReadUShort() : reader.ReadUInt());

            Width = reader.ReadUShort();
            Height = reader.ReadUShort();
            Depth = reader.ReadUShort();
            if (ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedUnbound, ProfileVersion.DeadSpace))
            {
                m_sliceCount = reader.ReadByte();
            }
            else
            {
                m_sliceCount = reader.ReadUShort();
            }


            if (m_version <= 10)
            {
                Unknown3[0] = reader.ReadUShort();
            }

            MipCount = reader.ReadByte();
            FirstMip = reader.ReadByte();
            if (ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedUnbound, ProfileVersion.DeadSpace))
            {
                reader.ReadByte();
            }

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield2042, ProfileVersion.NeedForSpeedUnbound, ProfileVersion.DeadSpace))
            {
                Unknown3[0] = reader.ReadUInt();
            }

            m_chunkId = reader.ReadGuid();

            for (int i = 0; i < 15; i++)
            {
                MipSizes[i] = reader.ReadUInt();
            }

            ChunkSize = reader.ReadUInt();

            if (m_version >= 13)
            {
                for (int i = 0; i < 4; i++)
                {
                    Unknown3[i] = reader.ReadUInt();
                }
            }

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa18, ProfileVersion.Madden19))
            {
                Unknown3[0] = reader.ReadUInt();
            }

            AssetNameHash = reader.ReadUInt();

            if (ProfilesLibrary.IsLoaded(ProfileVersion.PlantsVsZombiesGardenWarfare2, ProfileVersion.Madden22, ProfileVersion.Madden23))
            {
                Unknown3[0] = reader.ReadUInt();
            }

            TextureGroup = reader.ReadSizedString(16);

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
            {
                Unknown3[0] = reader.ReadUInt();
            }

            if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield2042, ProfileVersion.NeedForSpeedUnbound, ProfileVersion.DeadSpace))
            {
                reader.ReadLong();
            }

#if FROSTY_DEVELOPER
            Debug.Assert(reader.Position == reader.Length);
#endif

            Data = am.GetChunk(am.GetChunkEntry(m_chunkId));
        }

        public override byte[] SaveBytes()
        {
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                if (m_version <= 10)
                {
                    writer.Write(m_version);
                }
                else
                {
                    writer.Write(m_compressedMipOffsets[0]);
                    writer.Write(m_compressedMipOffsets[1]);
                }

                writer.Write((uint)Type);
                writer.Write(m_pixelFormat);

                if (m_version >= 12)
                {
                    writer.Write(m_customPoolId);
                }

                if (m_version <= 10)
                {
                    writer.Write((uint)Flags);
                }
                else
                {
                    writer.Write((ushort)Flags);
                }

                writer.Write(Width);
                writer.Write(Height);
                writer.Write(Depth);

                if (ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedUnbound, ProfileVersion.DeadSpace))
                {
                    writer.Write((byte)0);
                }
                else
                {
                    writer.Write(m_sliceCount);
                }

                if (m_version <= 10)
                {
                    writer.Write((ushort)Unknown3[0]);
                }

                writer.Write(MipCount);
                writer.Write(FirstMip);

                if (ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedUnbound, ProfileVersion.DeadSpace))
                {
                    writer.Write(MipCount);
                }

                if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield2042, ProfileVersion.NeedForSpeedUnbound, ProfileVersion.DeadSpace))
                {
                    writer.Write(Unknown3[0]);
                }

                writer.Write(m_chunkId);

                for (int i = 0; i < 15; i++)
                {
                    writer.Write(MipSizes[i]);
                }

                writer.Write(ChunkSize);

                if (m_version >= 13)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        writer.Write(Unknown3[i]);
                    }
                }

                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa18, ProfileVersion.Madden19))
                {
                    writer.Write(Unknown3[0]);
                }

                writer.Write(AssetNameHash);

                if (ProfilesLibrary.IsLoaded(ProfileVersion.PlantsVsZombiesGardenWarfare2, ProfileVersion.Madden22, ProfileVersion.Madden23))
                {
                    writer.Write(Unknown3[0]);
                }

                writer.WriteFixedSizedString(TextureGroup, 16);

                if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons))
                {
                    writer.Write(Unknown3[0]);
                }

                if (ProfilesLibrary.IsLoaded(ProfileVersion.NeedForSpeedUnbound, ProfileVersion.DeadSpace))
                {
                    writer.Write(0);
                }

                if (m_version >= 11)
                {
                    unsafe
                    {
                        // update the res meta
                        fixed (byte* ptr = &resMeta[0])
                        {
                            *(uint*)(ptr + 0) = m_version;
                            *(uint*)(ptr + 4) = (uint)Flags;
                        }
                    }
                }

                return writer.ToByteArray();
            }
        }

        public Texture(TextureType inType, string inFormat, ushort inWidth, ushort inHeight, ushort inDepth = 1, uint inVersion = 12, byte[] inResMeta = null)
        {
            Type = inType;
            m_pixelFormat = getTextureFormat(inFormat);
            Width = inWidth;
            Height = inHeight;
            Depth = inDepth;
            m_sliceCount = inDepth;
            m_customPoolId = 0;
            Flags = 0;
            Unknown3[0] = 0xFFFFFFFF;
            Unknown3[1] = 0xFFFFFFFF;
            Unknown3[2] = 0xFFFFFFFF;
            Unknown3[3] = 0xFFFFFFFF;
            m_version = inVersion;
            if (inResMeta != null)
            {
                resMeta = inResMeta;
            }
        }

        ~Texture() => Dispose(false);

        public void SetData(Guid newChunkId, AssetManager am)
        {
            Data = am.GetChunk(am.GetChunkEntry(newChunkId));

            m_chunkId = newChunkId;
            ChunkSize = (uint)Data.Length;
        }

        public void SetData(byte[] inData)
        {
            Data = new MemoryStream(inData);
            m_chunkId = Guid.Empty;
            ChunkSize = (uint)Data.Length;
        }

        public void CalculateMipData(byte inMipCount, int blockSize, bool isCompressed, uint dataSize)
        {
            if (isCompressed)
                blockSize /= 4;

            MipCount = inMipCount;

            int currentWidth = Width;
            int currentHeight = Height;
            int currentDepth = Depth;

            int minSize = (isCompressed) ? 4 : 1;
            for (int i = 0; i < MipCount; i++)
            {
                int pitch = (isCompressed)
                    ? (Math.Max(1, ((currentWidth + 3) / 4)) * blockSize)
                    : ((currentWidth * blockSize + 7) / 8);

                MipSizes[i] = (uint)(pitch * currentHeight);
                if (Type == TextureType.TT_3d)
                    MipSizes[i] *= (uint)currentDepth;

                currentWidth >>= 1;
                currentHeight >>= 1;

                currentHeight = (currentHeight < minSize) ? minSize : currentHeight;
                currentWidth = (currentWidth < minSize) ? minSize : currentWidth;
            }

            if (MipCount == 1)
            {
                LogicalOffset = 0;
                LogicalSize = dataSize;
                Flags = 0;
            }
            else
            {
                LogicalOffset = 0;
                for (int i = 0; i < MipCount - FirstMip; i++)
                    LogicalSize |= (uint)(0x03 << (i * 2));
                LogicalOffset = dataSize & (~LogicalSize);
                LogicalSize = dataSize & LogicalSize;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Data.Dispose();
            }
        }

        private int getTextureFormat(string format)
        {
            const string enumType = "RenderFormat";
            return (int)(Enum.Parse(TypeLibrary.GetType(enumType), enumType + "_" + format));
        }
    }
}
