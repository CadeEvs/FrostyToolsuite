using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.IO;

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
    public enum TextureFlags : ushort
    {
        Streaming = 0x1,
        SrgbGamma = 0x2,
        CpuResource = 0x4,
        OnDemandLoaded = 0x8,
        Mutable = 0x10,
        NoSkipmip = 0x20,
        XenonPackedMipmaps = 0x100,
        Ps3MemoryCell = 0x100,
        Ps3MemoryRsx = 0x200,
    }

    public class Texture : Resource, IDisposable
    {
        public uint FirstMipOffset 
        { 
            get => mipOffsets[0];
            set => mipOffsets[0] = value;
        }
        public uint SecondMipOffset 
        { 
            get => mipOffsets[1];
            set => mipOffsets[1] = value;
        }
        public string PixelFormat
        {
            get
            {
                string enumType = "RenderFormat";
                string retVal = Enum.Parse(TypeLibrary.GetType(enumType), pixelFormat.ToString()).ToString();
                return retVal.Replace(enumType + "_", "");
            }
        }
        public TextureType Type { get; private set; }
        public TextureFlags Flags { get; set; }
        public ushort Width { get; private set; }
        public ushort Height { get; private set; }

        public ushort SliceCount
        {
            get => sliceCount;
            set
            {
                sliceCount = value;
                if (Type == TextureType.TT_2dArray || Type == TextureType.TT_3d)
                    Depth = sliceCount;
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
            get => chunkId;
            set => chunkId = value;
        }

        public uint ChunkSize { get; private set; }

        private uint[] mipOffsets = new uint[2];
        private int pixelFormat;
        private uint unknown1;
        private ushort sliceCount;
        private Guid chunkId;

        /*
           private uint[] mipOffsets = new uint[2];
           private TextureType type;
           private int pixelFormat;
           private uint unknown1;
           private TextureFlags flags;
           private ushort width;
           private ushort height;
           private ushort depth;
           private ushort sliceCount;
           private byte mipCount;
           private byte firstMip;
           private Guid chunkId;
           private uint[] mipSizes = new uint[15];
           private uint chunkSize;
           private uint[] unknown3 = new uint[4];
           private uint assetNameHash;
           private string textureGroup;
           private Stream data;
           
           private uint logicalOffset;
           private uint logicalSize;
           private uint rangeStart;
           private uint rangeEnd;
        */

        public Texture()
        {
        }

        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {
            base.Read(reader, am, entry, modifiedData);

            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedRivals)
            {
                Unknown3[0] = reader.ReadUInt();
                Type = (TextureType)reader.ReadUInt();
                pixelFormat = reader.ReadInt();
                Unknown3[1] = reader.ReadUInt();
            }
            else
            {
                mipOffsets[0] = reader.ReadUInt();
                mipOffsets[1] = reader.ReadUInt();
                Type = (TextureType)reader.ReadUInt();
                pixelFormat = reader.ReadInt();
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII
                    || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19
                    || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5
                    || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat
                    || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons
                        )
                {
                    unknown1 = reader.ReadUInt();
                }
                Flags = (TextureFlags)reader.ReadUShort();
            }

            Width = reader.ReadUShort();
            Height = reader.ReadUShort();
            Depth = reader.ReadUShort();
            sliceCount = reader.ReadUShort();
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedRivals)
                Flags = (TextureFlags)reader.ReadUShort();
            MipCount = reader.ReadByte();
            FirstMip = reader.ReadByte();
            chunkId = reader.ReadGuid();
            for (int i = 0; i < 15; i++)
                MipSizes[i] = reader.ReadUInt();
            ChunkSize = reader.ReadUInt();

            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Anthem)
            {
                for (int i = 0; i < 3; i++)
                    Unknown3[i] = reader.ReadUInt();
            }
            else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda)
            {
                for (int i = 0; i < 4; i++)
                    Unknown3[i] = reader.ReadUInt();
            }
            else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19)
                Unknown3[0] = reader.ReadUInt();
            AssetNameHash = reader.ReadUInt();
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesGardenWarfare2)
                Unknown3[0] = reader.ReadUInt();
            TextureGroup = reader.ReadSizedString(16);
            if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                Unknown3[0] = reader.ReadUInt();
            Data = am.GetChunk(am.GetChunkEntry(chunkId));
        }

        public override byte[] SaveBytes()
        {
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.Write(mipOffsets[0]);
                writer.Write(mipOffsets[1]);
                writer.Write((uint)Type);
                writer.Write(pixelFormat);
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII ||
                    ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 ||
                    ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden20 ||
                    ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa20 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedHeat || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons
#if FROSTY_ALPHA || FROSTY_DEVELOPER
                || ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville
#endif
                    )
                {
                    writer.Write(unknown1);
                }
                writer.Write((ushort)Flags);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(Depth);
                writer.Write(sliceCount);
                writer.Write(MipCount);
                writer.Write(FirstMip);
                writer.Write(chunkId);
                for (int i = 0; i < 15; i++)
                    writer.Write(MipSizes[i]);
                writer.Write(ChunkSize);
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda)
                {
                    for (int i = 0; i < 4; i++)
                        writer.Write(Unknown3[i]);
                }
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19)
                    writer.Write(Unknown3[0]);
                writer.Write(AssetNameHash);
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.PlantsVsZombiesGardenWarfare2)
                    writer.Write(Unknown3[0]);
                writer.WriteFixedSizedString(TextureGroup, 16);
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                    writer.Write(Unknown3[0]);

                return writer.ToByteArray();
            }
        }

        public Texture(TextureType inType, string inFormat, ushort inWidth, ushort inHeight, ushort inDepth = 1)
        {
            Type = inType;
            pixelFormat = getTextureFormat(inFormat);
            Width = inWidth;
            Height = inHeight;
            Depth = inDepth;
            sliceCount = inDepth;
            unknown1 = 0;
            Flags = 0;
            Unknown3[0] = 0xFFFFFFFF;
            Unknown3[1] = 0xFFFFFFFF;
            Unknown3[2] = 0xFFFFFFFF;
            Unknown3[3] = 0xFFFFFFFF;
        }

        ~Texture() => Dispose(false);

        public void SetData(Guid newChunkId, AssetManager am)
        {
            Data = am.GetChunk(am.GetChunkEntry(newChunkId));

            chunkId = newChunkId;
            ChunkSize = (uint)Data.Length;
        }

        public void SetData(byte[] inData)
        {
            Data = new MemoryStream(inData);
            chunkId = Guid.Empty;
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
