using Frosty.Hash;
using FrostySdk.Ebx;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers.Entries;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace FrostySdk
{
    #region -- Misc --
    public enum VertexElementUsage : byte
    {
        Unknown = 0x00,
        Pos = 0x01,
        BoneIndices = 0x02,
        BoneIndices2 = 0x03,
        BoneWeights = 0x04,
        BoneWeights2 = 0x05,
        Normal = 0x06,
        Tangent = 0x07,
        Binormal = 0x08,
        BinormalSign = 0x09,
        WorldTrans1 = 0x0A,
        WorldTrans2 = 0x0B,
        WorldTrans3 = 0x0C,
        InstanceId = 0x0D,
        InstanceUserData0 = 0x0E,
        PrevInstanceUserData0 = 0x0F,
        InstanceUserData1 = 0x10,
        PrevWorldTrans1 = 0x11,
        PrevWorldTrans2 = 0x12,
        PrevWorldTrans3 = 0x13,
        XenonIndex = 0x14,
        XenonBarycentric = 0x15,
        XenonQuadID = 0x16,
        Index = 0x17,
        ViewIndex = 0x18,
        Color0 = 0x1E,
        Color1 = 0x1F,
        TexCoord0 = 0x21,
        TexCoord1 = 0x22,
        TexCoord2 = 0x23,
        TexCoord3 = 0x24,
        TexCoord4 = 0x25,
        TexCoord5 = 0x26,
        TexCoord6 = 0x27,
        TexCoord7 = 0x28,
        DisplacementMapTexCoord = 0x29,
        RadiosityTexCoord = 0x2A,
        VisInfo = 0x2B,
        SpriteSize = 0x2C,
        PackedTexCoord0 = 0x2D,
        PackedTexCoord1 = 0x2E,
        PackedTexCoord2 = 0x2F,
        PackedTexCoord3 = 0x30,
        ClipDistance0 = 0x31,
        ClipDistance1 = 0x32,
        SubMaterialIndex = 0x33,
        TangentSpace = 0x34,
        BranchInfo = 0x3C,
        PosAndScale = 0x3D,
        Rotation = 0x3E,
        SpriteSizeAndUv = 0x3F,
        FadePos = 0x5A,
        SpawnTime = 0x5B,
        RegionIds = 0x64,
        BlendWeights = 0x65,
        PosAndSoftMul = 0x96,
        Alpha = 0x97,
        Misc0 = 0x98,
        Misc1 = 0x99,
        Misc2 = 0x9A,
        Misc3 = 0x9B,
        LeftAndRotation = 0x9C,
        UpAndNormalBlend = 0x9D,
        Hl2BasisL0 = 0x9E,
        Hl2BasisL1 = 0x9F,
        Hl2BasisL3 = 0xA0,
        PosAndRejectCulling = 0xA1,
        Shadow = 0xA2,
        CustomParams = 0xA3,
        PatchUv = 0xB4,
        Height = 0xB5,
        MaskUVs0 = 0xB6,
        MaskUVs1 = 0xB7,
        MaskUVs2 = 0xB8,
        MaskUVs3 = 0xB9,
        UserMasks = 0xBA,
        HeightfieldUv = 0xBB,
        MaskUv = 0xBC,
        GlobalColorUv = 0xBD,
        HeightfieldPixelSizeAndAspect = 0xBE,
        WorldPositionXz = 0xBF,
        TerrainTextureNodeUv = 0xC0,
        ParentTerrainTextureNodeUv = 0xC1,
        DeformationIndex = 0xCD,
        DeformationWeight = 0xCE,
        DeformationPosition = 0xCF,
        Delta = 0xD0,
        ElementIndex = 0xD1,
        Uv01 = 0xD2,
        WorldPos = 0xD3,
        EyeVector = 0xD4,
        LightParams1 = 0xDC,
        LightParams2 = 0xDD,
        LightSubParams = 0xDE,
        LightSideVector = 0xDF,
        LightInnerAndOuterAngle = 0xE0,
        LightDir = 0xE1,
        LightMatrix1 = 0xE2,
        LightMatrix2 = 0xE3,
        LightMatrix3 = 0xE4,
        LightMatrix4 = 0xE5,
        Custom = 0xE6,
        DestructionMaskDistance = 0xF0,
        DestructionMaskTexCoord = 0xF1,
        VertIndex = 0xFA,
    }

    public enum VertexElementFormat : byte
    {
        None = 0x00,
        Float = 0x01,
        Float2 = 0x02,
        Float3 = 0x03,
        Float4 = 0x04,
        Half = 0x05,
        Half2 = 0x06,
        Half3 = 0x07,
        Half4 = 0x08,
        UByteN = 0x32,
        Byte4 = 0x0A,
        Byte4N = 0x0B,
        UByte4 = 0x0C,
        UByte4N = 0x0D,
        Short = 0x0E,
        Short2 = 0x0F,
        Short3 = 0x10,
        Short4 = 0x11,
        ShortN = 0x12,
        Short2N = 0x13,
        Short3N = 0x14,
        Short4N = 0x15,
        UShort2 = 0x16,
        UShort4 = 0x17,
        UShort2N = 0x18,
        UShort4N = 0x19,
        Int = 0x1A,
        Int2 = 0x1B,
        Int3 = 0x33,
        Int4 = 0x1C,
        IntN = 0x1D,
        Int2N = 0x1E,
        Int4N = 0x1F,
        UInt = 0x20,
        UInt2 = 0x21,
        UInt3 = 0x34,
        UInt4 = 0x22,
        UIntN = 0x23,
        UInt2N = 0x24,
        UInt4N = 0x25,
        Comp3_10_10_10 = 0x26,
        Comp3N_10_10_10 = 0x27,
        UComp3_10_10_10 = 0x28,
        UComp3N_10_10_10 = 0x29,
        Comp3_11_11_10 = 0x2A,
        Comp3N_11_11_10 = 0x2B,
        UComp3_11_11_10 = 0x2C,
        UComp3N_11_11_10 = 0x2D,
        Comp4_10_10_10_2 = 0x2E,
        Comp4N_10_10_10_2 = 0x2F,
        UComp4_10_10_10_2 = 0x30,
        UComp4N_10_10_10_2 = 0x31,
    }

    public enum VertexElementClassification
    {
        PerVertex = 0,
        PerInstance = 1,
    }

    public enum TangentSpaceCompressionType
    {
        TangentSpaceCompression_Default,
        TangentSpaceCompression_Quaternion,
        TangentSpaceCompression_Matrix,
        TangentSpaceCompression_AxisAngle_Compact,
        TangentSpaceCompression_AxisAngle_Precise
    }

    public struct VertexElement
    {
        public VertexElementUsage Usage;
        public VertexElementFormat Format;
        public byte Offset;
        public byte StreamIndex;
        public int Size
        {
            get
            {
                switch (Format)
                {
                    case VertexElementFormat.None: return 0;
                    case VertexElementFormat.Float: return 4;
                    case VertexElementFormat.Float2: return 8;
                    case VertexElementFormat.Float3: return 12;
                    case VertexElementFormat.Float4: return 16;
                    case VertexElementFormat.Half: return 2;
                    case VertexElementFormat.Half2: return 4;
                    case VertexElementFormat.Half3: return 6;
                    case VertexElementFormat.Half4: return 8;
                    case VertexElementFormat.UByteN: return 1;
                    case VertexElementFormat.Byte4: return 4;
                    case VertexElementFormat.Byte4N: return 4;
                    case VertexElementFormat.UByte4: return 4;
                    case VertexElementFormat.UByte4N: return 4;
                    case VertexElementFormat.Short: return 2;
                    case VertexElementFormat.Short2: return 4;
                    case VertexElementFormat.Short3: return 6;
                    case VertexElementFormat.Short4: return 8;
                    case VertexElementFormat.ShortN: return 2;
                    case VertexElementFormat.Short2N: return 4;
                    case VertexElementFormat.Short3N: return 6;
                    case VertexElementFormat.Short4N: return 8;
                    case VertexElementFormat.UShort2: return 4;
                    case VertexElementFormat.UShort4: return 8;
                    case VertexElementFormat.UShort2N: return 4;
                    case VertexElementFormat.UShort4N: return 8;
                    case VertexElementFormat.Int: return 4;
                    case VertexElementFormat.Int2: return 8;
                    case VertexElementFormat.Int3: return 12;
                    case VertexElementFormat.Int4: return 16;
                    case VertexElementFormat.IntN: return 4;
                    case VertexElementFormat.Int2N: return 8;
                    case VertexElementFormat.Int4N: return 16;
                    case VertexElementFormat.UInt: return 4;
                    case VertexElementFormat.UInt2: return 8;
                    case VertexElementFormat.UInt3: return 12;
                    case VertexElementFormat.UInt4: return 16;
                    case VertexElementFormat.UIntN: return 4;
                    case VertexElementFormat.UInt2N: return 8;
                    case VertexElementFormat.UInt4N: return 16;
                    case VertexElementFormat.Comp3_10_10_10: return 4;
                    case VertexElementFormat.Comp3N_10_10_10: return 4;
                    case VertexElementFormat.UComp3_10_10_10: return 4;
                    case VertexElementFormat.UComp3N_10_10_10: return 4;
                    case VertexElementFormat.Comp3_11_11_10: return 4;
                    case VertexElementFormat.Comp3N_11_11_10: return 4;
                    case VertexElementFormat.UComp3_11_11_10: return 4;
                    case VertexElementFormat.UComp3N_11_11_10: return 4;
                    case VertexElementFormat.Comp4_10_10_10_2: return 4;
                    case VertexElementFormat.Comp4N_10_10_10_2: return 4;
                    case VertexElementFormat.UComp4_10_10_10_2: return 4;
                    case VertexElementFormat.UComp4N_10_10_10_2: return 4;
                    default: return 0;
                }
            }
        }

        public override string ToString()
        {
            return Usage.ToString();
        }
    }

    public struct GeometryDeclarationDesc
    {
        public struct Element
        {
            public VertexElementUsage Usage;
            public VertexElementFormat Format;
            public byte Offset;
            public byte StreamIndex;
            public int Size
            {
                get
                {
                    switch (Format)
                    {
                        case VertexElementFormat.None: return 0;
                        case VertexElementFormat.Float: return 4;
                        case VertexElementFormat.Float2: return 8;
                        case VertexElementFormat.Float3: return 12;
                        case VertexElementFormat.Float4: return 16;
                        case VertexElementFormat.Half: return 2;
                        case VertexElementFormat.Half2: return 4;
                        case VertexElementFormat.Half3: return 6;
                        case VertexElementFormat.Half4: return 8;
                        case VertexElementFormat.UByteN: return 1;
                        case VertexElementFormat.Byte4: return 4;
                        case VertexElementFormat.Byte4N: return 4;
                        case VertexElementFormat.UByte4: return 4;
                        case VertexElementFormat.UByte4N: return 4;
                        case VertexElementFormat.Short: return 2;
                        case VertexElementFormat.Short2: return 4;
                        case VertexElementFormat.Short3: return 6;
                        case VertexElementFormat.Short4: return 8;
                        case VertexElementFormat.ShortN: return 2;
                        case VertexElementFormat.Short2N: return 4;
                        case VertexElementFormat.Short3N: return 6;
                        case VertexElementFormat.Short4N: return 8;
                        case VertexElementFormat.UShort2: return 4;
                        case VertexElementFormat.UShort4: return 8;
                        case VertexElementFormat.UShort2N: return 4;
                        case VertexElementFormat.UShort4N: return 8;
                        case VertexElementFormat.Int: return 4;
                        case VertexElementFormat.Int2: return 8;
                        case VertexElementFormat.Int3: return 12;
                        case VertexElementFormat.Int4: return 16;
                        case VertexElementFormat.IntN: return 4;
                        case VertexElementFormat.Int2N: return 8;
                        case VertexElementFormat.Int4N: return 16;
                        case VertexElementFormat.UInt: return 4;
                        case VertexElementFormat.UInt2: return 8;
                        case VertexElementFormat.UInt3: return 12;
                        case VertexElementFormat.UInt4: return 16;
                        case VertexElementFormat.UIntN: return 4;
                        case VertexElementFormat.UInt2N: return 8;
                        case VertexElementFormat.UInt4N: return 16;
                        case VertexElementFormat.Comp3_10_10_10: return 4;
                        case VertexElementFormat.Comp3N_10_10_10: return 4;
                        case VertexElementFormat.UComp3_10_10_10: return 4;
                        case VertexElementFormat.UComp3N_10_10_10: return 4;
                        case VertexElementFormat.Comp3_11_11_10: return 4;
                        case VertexElementFormat.Comp3N_11_11_10: return 4;
                        case VertexElementFormat.UComp3_11_11_10: return 4;
                        case VertexElementFormat.UComp3N_11_11_10: return 4;
                        case VertexElementFormat.Comp4_10_10_10_2: return 4;
                        case VertexElementFormat.Comp4N_10_10_10_2: return 4;
                        case VertexElementFormat.UComp4_10_10_10_2: return 4;
                        case VertexElementFormat.UComp4N_10_10_10_2: return 4;
                        default: return 0;
                    }
                }
            }

            public override string ToString()
            {
                return Usage.ToString();
            }
        }
        public struct Stream
        {
            public byte VertexStride;
            public VertexElementClassification Classification;
        }

        public static int MaxElements => 16;
        public static int MaxStreams
        {
            get
             {
                switch (ProfilesLibrary.DataVersion)
                {
                    case (int)ProfileVersion.NeedForSpeedRivals:
                    case (int)ProfileVersion.PlantsVsZombiesGardenWarfare:
                    case (int)ProfileVersion.DragonAgeInquisition:
                    case (int)ProfileVersion.Battlefield4:
                    case (int)ProfileVersion.PlantsVsZombiesGardenWarfare2:
                    case (int)ProfileVersion.NeedForSpeed:
                    case (int)ProfileVersion.NeedForSpeedEdge:
                        return 4;
                    case (int)ProfileVersion.StarWarsBattlefront:
                        return 6;
                    case (int)ProfileVersion.Battlefield5:
                        return 8;
                    case (int)ProfileVersion.StarWarsBattlefrontII:
                    case (int)ProfileVersion.Fifa18:
                    case (int)ProfileVersion.Madden19:
                    case (int)ProfileVersion.NeedForSpeedPayback:
                    case (int)ProfileVersion.Fifa19:
                    case (int)ProfileVersion.Anthem:
                    case (int)ProfileVersion.Madden20:
                    case (int)ProfileVersion.PlantsVsZombiesBattleforNeighborville:
                    case (int)ProfileVersion.NeedForSpeedHeat:
                    case (int)ProfileVersion.Fifa20:
                    case (int)ProfileVersion.StarWarsSquadrons:
                    case (int)ProfileVersion.Fifa21:
                    case (int)ProfileVersion.Madden22:
                    case (int)ProfileVersion.Fifa22:
                    case (int)ProfileVersion.Battlefield2042:
                    case (int)ProfileVersion.Madden23:
                    case (int)ProfileVersion.Fifa23:
                    case (int)ProfileVersion.NeedForSpeedUnbound:
                    case (int)ProfileVersion.DeadSpace:
                        return 16;
                    default:
                        return 8;
                }
            }
        }

        public Element[] Elements;
        public Stream[] Streams;
        public byte ElementCount;
        public byte StreamCount;

        public uint Hash => Utils.CalcFletcher32(this);

        public static GeometryDeclarationDesc Create(Element[] elements)
        {
            GeometryDeclarationDesc geomDecl = new GeometryDeclarationDesc { Elements = new Element[MaxElements] };

            int offset = 0;
            for (int i = 0; i < MaxElements; i++)
            {
                if (i < elements.Length)
                {
                    geomDecl.Elements[i] = elements[i];
                    geomDecl.Elements[i].Offset = (byte)offset;
                    offset += geomDecl.Elements[i].Size;
                }
                else
                {
                    geomDecl.Elements[i].Offset = 0xFF;
                }
            }

            geomDecl.Streams = new Stream[MaxStreams];
            geomDecl.Streams[0].Classification = VertexElementClassification.PerVertex;
            geomDecl.Streams[0].VertexStride = (byte)offset;

            geomDecl.ElementCount = (byte)elements.Length;
            geomDecl.StreamCount = 1;

            return geomDecl;
        }
    }
    #endregion

    public enum CompressionType
    {
        Default,
        None,
        ZLib,
        ZStd,
        LZ4,
        Oodle
    }

    internal static class Kernel32
    {
        [DllImport("kernel32.dll", EntryPoint = "LoadLibraryEx", SetLastError = true)]
        public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hReservedNull, uint dwFlags);

        [DllImport("kernel32", EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);
    }

    internal class LoadLibraryHandle
    {
        IntPtr handle;
        public LoadLibraryHandle(string lib)
        {
            handle = Kernel32.LoadLibraryEx(lib, IntPtr.Zero, 0);
        }
        public static implicit operator IntPtr(LoadLibraryHandle value) { return value.handle; }
        ~LoadLibraryHandle()
        {
            Kernel32.FreeLibrary(handle);
        }
    }

    internal static class Oodle
    {
        internal enum OodleFormat : uint
        {
            LZH = 0,
            LZHLW = 1,
            LZNIB = 2,
            None = 3,
            LZB16 = 4,
            LZBLW = 5,
            LZA = 6,
            LZNA = 7,
            Kraken = 8,
            Mermaid = 9,
            BitKnit = 10,
            Selkie = 11,
            Hydra = 12,
            Leviathan = 13
        }

        internal enum OodleCompressionLevel : uint
        {
            None = 0,
            SuperFast = 1,
            VeryFast = 2,
            Fast = 3,
            Normal = 4,
            Optimal1 = 5,
            Optimal2 = 6,
            Optimal3 = 7
        }

        public enum OodleFuzzSafe
        {
            No = 0,
            Yes = 1
        }

        public enum OodleCheckCRC
        {
            No = 0,
            Yes = 1
        }

        public enum OodleVerbosity
        {
            None = 0,
            Minimal = 1,
            Some = 2,
            Lots = 3
        }

        public enum OodleThreadPhase
        {
            ThreadPhase1 = 1,
            ThreadPhase2 = 2,
            ThreadPhaseAll = 3,
            Unthreaded = ThreadPhaseAll
        }

        public delegate int DecompressFunc(IntPtr srcBuffer, long srcSize, IntPtr dstBuffer, long dstSize, OodleFuzzSafe fuzzSafe = OodleFuzzSafe.Yes, OodleCheckCRC checkCRC = OodleCheckCRC.No, OodleVerbosity verbosity = OodleVerbosity.None, IntPtr decBufBase = new IntPtr(), long decBufSize = 0, IntPtr fpCallback = new IntPtr(), IntPtr callbackUserData = new IntPtr(), IntPtr decoderMemory = new IntPtr(), long decoderMemorySize = 0, OodleThreadPhase threadModule = OodleThreadPhase.Unthreaded);
        public static DecompressFunc Decompress;

        public delegate long CompressFunc(OodleFormat cmpCode, IntPtr srcBuffer, long srcSize, IntPtr cmpBuffer, OodleCompressionLevel cmpLevel, long dict = 0, long dictSize = 0);
        public delegate long CompressFunc2(OodleFormat cmpCode, IntPtr srcBuffer, long srcSize, IntPtr cmpBuffer, OodleCompressionLevel cmpLevel, IntPtr options = new IntPtr(), IntPtr dictionaryBase = new IntPtr(), IntPtr lrm = new IntPtr(), IntPtr scratch = new IntPtr(), long scratchSize = 0);
        public static CompressFunc Compress;
        public static CompressFunc2 Compress2;

        public delegate IntPtr GetDefaultOptions(OodleFormat cmpCode, OodleCompressionLevel cmpLevel);
        public static GetDefaultOptions GetOptions;

        public delegate long MemorySizeNeededFunc(int a1, long a2);
        public static MemorySizeNeededFunc MemorySizeNeeded;

        internal static LoadLibraryHandle handle;
        internal static void Bind(string basePath)
        {
            ICompressionUtils utils = ProfilesLibrary.Profile.GetCompressionUtils();

            if (!utils.LoadOodle)
                return;

            string dllPath = utils.GetOodleDllName(basePath);


            handle = new LoadLibraryHandle(dllPath);
            if (handle == IntPtr.Zero)
                return;

            Decompress = Marshal.GetDelegateForFunctionPointer<DecompressFunc>(Kernel32.GetProcAddress(handle, "OodleLZ_Decompress"));
            Compress = Marshal.GetDelegateForFunctionPointer<CompressFunc>(Kernel32.GetProcAddress(handle, "OodleLZ_Compress"));
            if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa19, ProfileVersion.Anthem,
                ProfileVersion.PlantsVsZombiesBattleforNeighborville, ProfileVersion.Fifa20,
                ProfileVersion.NeedForSpeedHeat, ProfileVersion.Fifa21,
                ProfileVersion.Madden22, ProfileVersion.Fifa22,
                ProfileVersion.Battlefield2042, ProfileVersion.Madden23,
                ProfileVersion.Fifa23, ProfileVersion.NeedForSpeedUnbound))
            {
                Compress2 = Marshal.GetDelegateForFunctionPointer<CompressFunc2>(Kernel32.GetProcAddress(handle, "OodleLZ_Compress"));
            }
            GetOptions = Marshal.GetDelegateForFunctionPointer<GetDefaultOptions>(Kernel32.GetProcAddress(handle, "OodleLZ_CompressOptions_GetDefault"));
            MemorySizeNeeded = Marshal.GetDelegateForFunctionPointer<MemorySizeNeededFunc>(Kernel32.GetProcAddress(handle, "OodleLZDecoder_MemorySizeNeeded"));
        }
    }

    internal static class ZStd
    {
        internal enum ZSTD_error
        {
            ZSTD_error_no_error = 0,
            ZSTD_error_GENERIC = 1,
            ZSTD_error_prefix_unknown = 10,
            ZSTD_error_version_unsupported = 12,
            ZSTD_error_frameParameter_unsupported = 14,
            ZSTD_error_frameParameter_windowTooLarge = 16,
            ZSTD_error_corruption_detected = 20,
            ZSTD_error_checksum_wrong = 22,
            ZSTD_error_dictionary_corrupted = 30,
            ZSTD_error_dictionary_wrong = 32,
            ZSTD_error_dictionaryCreation_failed = 34,
            ZSTD_error_parameter_unsupported = 40,
            ZSTD_error_parameter_outOfBound = 42,
            ZSTD_error_tableLog_tooLarge = 44,
            ZSTD_error_maxSymbolValue_tooLarge = 46,
            ZSTD_error_maxSymbolValue_tooSmall = 48,
            ZSTD_error_stage_wrong = 60,
            ZSTD_error_init_missing = 62,
            ZSTD_error_memory_allocation = 64,
            ZSTD_error_dstSize_tooSmall = 70,
            ZSTD_error_srcSize_wrong = 72,
            ZSTD_error_frameIndex_tooLarge = 100,
            ZSTD_error_seekableIO = 102,
            ZSTD_error_maxCode = 120
        }

        public delegate ulong DecompressFunc(IntPtr outputBuffer, ulong outputSize, IntPtr inputBuffer, ulong inputSize);
        public static DecompressFunc Decompress;

        public delegate IntPtr CreateFunc();
        public static CreateFunc Create;

        public delegate ulong FreeFunc(IntPtr handle);
        public static FreeFunc Free;

        public delegate ulong DecompressUsingDictFunc(IntPtr dctx, IntPtr outBuf, ulong outSize, IntPtr inBuf, ulong inSize, IntPtr dict);
        public static DecompressUsingDictFunc DecompressUsingDict;

        public delegate ulong CompressFunc(IntPtr Dst, ulong DstCapacity, IntPtr Src, ulong SrcSize, int CompressionLevel);
        public static CompressFunc Compress;

        public delegate ulong CompressBoundFunc(ulong srcSize);
        public static CompressBoundFunc CompressBound;

        public delegate bool IsErrorFunc(ulong errorCode);
        public static IsErrorFunc IsError;

        public delegate ZSTD_error GetErrorCodeFunc(ulong errorCode);
        public static GetErrorCodeFunc GetErrorCode;

        public delegate IntPtr GetErrorNameFunc(ulong errorCode);
        public static GetErrorNameFunc GetErrorName;

        public delegate IntPtr CreateDigestedDictFunc(IntPtr dictBuffer, int dictSize);
        public static CreateDigestedDictFunc CreateDigestedDict;

        public delegate ulong FreeDigestedDictFunc(IntPtr dict);
        public static FreeDigestedDictFunc FreeDigestedDict;

        internal static LoadLibraryHandle handle;
        private static byte[] digestedDict;

        internal static void Bind()
        {
            ICompressionUtils utils = ProfilesLibrary.Profile.GetCompressionUtils();

            if (!utils.LoadZStd)
                return;

            if (Compress != null)
                return;

            string dllName = utils.GetZStdDllName();


            handle = new LoadLibraryHandle(dllName);
            if (handle == IntPtr.Zero)
                return;

            Create = Marshal.GetDelegateForFunctionPointer<CreateFunc>(Kernel32.GetProcAddress(handle, "ZSTD_createDCtx"));
            Free = Marshal.GetDelegateForFunctionPointer<FreeFunc>(Kernel32.GetProcAddress(handle, "ZSTD_freeDCtx"));
            Decompress = Marshal.GetDelegateForFunctionPointer<DecompressFunc>(Kernel32.GetProcAddress(handle, "ZSTD_decompress"));
            Compress = Marshal.GetDelegateForFunctionPointer<CompressFunc>(Kernel32.GetProcAddress(handle, "ZSTD_compress"));
            CompressBound = Marshal.GetDelegateForFunctionPointer<CompressBoundFunc>(Kernel32.GetProcAddress(handle, "ZSTD_compressBound"));
            IsError = Marshal.GetDelegateForFunctionPointer<IsErrorFunc>(Kernel32.GetProcAddress(handle, "ZSTD_isError"));

            if (!ProfilesLibrary.IsLoaded(ProfileVersion.Fifa17))
            {
                GetErrorCode = Marshal.GetDelegateForFunctionPointer<GetErrorCodeFunc>(Kernel32.GetProcAddress(handle, "ZSTD_getErrorCode"));
                GetErrorName = Marshal.GetDelegateForFunctionPointer<GetErrorNameFunc>(Kernel32.GetProcAddress(handle, "ZSTD_getErrorName"));
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa18, ProfileVersion.Fifa19, ProfileVersion.Fifa20, ProfileVersion.Fifa21, ProfileVersion.Fifa22))
                {
                    DecompressUsingDict = Marshal.GetDelegateForFunctionPointer<DecompressUsingDictFunc>(Kernel32.GetProcAddress(handle, "ZSTD_decompress_usingDDict"));
                    CreateDigestedDict = Marshal.GetDelegateForFunctionPointer<CreateDigestedDictFunc>(Kernel32.GetProcAddress(handle, "ZSTD_createDDict"));
                    FreeDigestedDict = Marshal.GetDelegateForFunctionPointer<FreeDigestedDictFunc>(Kernel32.GetProcAddress(handle, "ZSTD_freeDDict"));
                }
            }
        }

        internal static void SetDictionary(byte[] data)
        {
            digestedDict = data;
        }

        internal static byte[] GetDictionary()
        {
            return digestedDict;
        }
    }

    internal static class ZLib
    {
        public const string DllName = "thirdparty/zlibwapi.dll";
        public const CallingConvention Cdecl = CallingConvention.Cdecl;

        [DllImport(DllName, EntryPoint = "inflate", CallingConvention = Cdecl)]
        public static extern int Inflate(IntPtr strm, int flush);

        [DllImport(DllName, EntryPoint = "inflateEnd", CallingConvention = Cdecl)]
        public static extern int InflateEnd(IntPtr strm);

        [DllImport(DllName, EntryPoint = "inflateInit_", CallingConvention = Cdecl, CharSet = CharSet.Ansi)]
        public static extern int InflateInit(IntPtr strm, [MarshalAs(UnmanagedType.LPStr)] string version, int stream_size);

        [DllImport(DllName, EntryPoint = "deflateInit_", CallingConvention = Cdecl, CharSet = CharSet.Ansi)]
        public static extern int DeflateInit(IntPtr strm, int level, [MarshalAs(UnmanagedType.LPStr)] string version, int stream_size);

        [DllImport(DllName, EntryPoint = "deflate", CallingConvention = Cdecl)]
        public static extern int Deflate(IntPtr strm, int flush);

        [DllImport(DllName, EntryPoint = "deflateEnd", CallingConvention = Cdecl)]
        public static extern int DeflateEnd(IntPtr strm);

        [DllImport(DllName, EntryPoint = "deflateBound", CallingConvention = Cdecl)]
        public static extern int DeflateBound(IntPtr strm, ulong sourceLen);

        [StructLayout(LayoutKind.Sequential)]
        public struct ZStream
        {
            public IntPtr next_in;
            public uint avail_in;
            public uint total_in;
            public IntPtr next_out;
            public uint avail_out;
            public uint total_out;
            public IntPtr msg;
            public IntPtr state;
            public IntPtr zalloc;
            public IntPtr zfree;
            public IntPtr opaque;
            public int data_type;
            public uint adler;
            public uint reserved;
        }

        public const int Z_FINISH = 4;
    }

    internal static class LZ4
    {
        [DllImport("thirdparty/liblz4.so.1.8.0.dll", EntryPoint = "LZ4_decompress_fast")]
        public static extern int Decompress(IntPtr src, IntPtr dst, int outputSize);

        [DllImport("thirdparty/liblz4.so.1.8.0.dll", EntryPoint = "LZ4_compressBound")]
        public static extern int CompressBound(int inputSize);

        [DllImport("thirdparty/liblz4.so.1.8.0.dll", EntryPoint = "LZ4_compress_default")]
        public static extern int Compress(IntPtr src, IntPtr dst, int sourceSize, int maxDestSize);
    }

    public static class Utils
    {
        public static string ToHex(this Guid guid)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in guid.ToByteArray())
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        public static Guid GenerateDeterministicGuid(IEnumerable<object> objects, string type, Guid fileGuid)
        {
            return GenerateDeterministicGuid(objects, TypeLibrary.GetType(type), fileGuid);
        }

        public static Guid GenerateDeterministicGuid(IEnumerable<object> objects, Type type, Guid fileGuid)
        {
            Guid outGuid = Guid.Empty;

            int createCount = 0;
            foreach (object obj in objects)
                createCount++;

            while (true)
            {
                // generate a deterministic unique guid
                using (NativeWriter writer = new NativeWriter(new MemoryStream()))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(ms, type);

                        writer.Write(fileGuid);
                        writer.Write(++createCount);
                        writer.Write(ms.ToArray());
                    }

                    using (MD5 md5 = new MD5CryptoServiceProvider())
                    {
                        outGuid = new Guid(md5.ComputeHash(writer.ToByteArray()));

                        bool bFound = false;
                        foreach (dynamic obj in objects)
                        {
                            AssetClassGuid objGuid = obj.GetInstanceGuid();
                            if (objGuid.ExportedGuid == outGuid)
                            {
                                // try again
                                bFound = true;
                                break;
                            }
                        }

                        if (!bFound)
                            break;
                    }
                }
            }

            return outGuid;
        }

        public static Sha1 GenerateSha1(byte[] buffer)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                Sha1 newSha1 = new Sha1(sha1.ComputeHash(buffer));
                return newSha1;
            }
        }

        public static ulong GenerateResourceId()
        {
            Random random = new Random();

            const ulong min = ulong.MinValue;
            const ulong max = ulong.MaxValue;

            const ulong uRange = (ulong)(max - min);
            ulong ulongRand;

            do
            {
                byte[] buf = new byte[8];
                random.NextBytes(buf);
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0);

            } while (ulongRand > ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange);

            return ((ulongRand % uRange) + min) | 1;
        }

        public static int HashString(string strToHash, bool lowercase = false)
        {
            if (lowercase)
                strToHash = strToHash.ToLower();
            return Fnv1.HashString(strToHash);
        }

        private static Dictionary<int, string> strings = new Dictionary<int, string>();

        public static string GetString(int hash)
        {
            if (!strings.ContainsKey(hash))
            {
                return "0x" + hash.ToString("x8");
            }

            return strings[hash];
        }

        public static string ReverseString(string str)
        {
            string outStr = "";
            foreach (char c in str.Reverse())
                outStr += c;
            return outStr;
        }

        public static byte[] CompressTexture(byte[] inData, Texture texture, CompressionType compressionOverride = CompressionType.Default)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                uint first = 0;
                uint second = (uint)inData.Length;

                if (texture.MipCount > 1)
                {
                    if (inData.Length > 0x10000)
                    {
                        int index = 0;
                        while (second > 0x10000 && index < texture.FirstMip)
                        {
                            first += texture.MipSizes[index];
                            second -= texture.MipSizes[index++];
                        }
                    }
                }

                if (texture.LogicalOffset != first)
                {
                    texture.LogicalOffset = first;
                    texture.LogicalSize = second;
                }

                byte[] tmpData = null;
                if (first != 0)
                {
                    tmpData = new byte[first];
                    Array.Copy(inData, tmpData, first);

                    tmpData = CompressFile(tmpData, texture: texture, compressionOverride: compressionOverride);
                    ms.Write(tmpData, 0, tmpData.Length);
                    texture.RangeStart = (uint)ms.Length;
                }

                tmpData = new byte[second];
                Array.Copy(inData, first, tmpData, 0, second);

                tmpData = CompressFile(tmpData, texture: texture, compressionOverride: compressionOverride, offset: first);
                ms.Write(tmpData, 0, tmpData.Length);

                texture.RangeEnd = (uint)ms.Length;
                if (texture.RangeStart == 0)
                    texture.RangeEnd = 0;

                return ms.ToArray();
            }
        }

        private static int MaxBufferSize
        {
            get
            {
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa18, ProfileVersion.Fifa19, ProfileVersion.Fifa20, ProfileVersion.Fifa21, ProfileVersion.Fifa22))
                    return 0x40000;
                return 0x10000;
            }
        }

        public static byte[] CompressFile(byte[] inData, Texture texture = null, ResourceType resType = ResourceType.Invalid, CompressionType compressionOverride = CompressionType.Default, uint offset = 0)
        {
            CompressionType compressionType = compressionOverride;

            if (compressionOverride == CompressionType.Default)
            {
                compressionType = CompressionType.LZ4;
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Anthem, ProfileVersion.PlantsVsZombiesBattleforNeighborville,
                    ProfileVersion.NeedForSpeedHeat, ProfileVersion.NeedForSpeedUnbound) ||
                    (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa19) && texture != null))
                {
                    compressionType = CompressionType.Oodle;
                }
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.MassEffectAndromeda, ProfileVersion.Fifa17,
                         ProfileVersion.StarWarsBattlefrontII, ProfileVersion.Madden19,
                         ProfileVersion.Fifa19, ProfileVersion.Fifa20,
                         ProfileVersion.Battlefield5, ProfileVersion.StarWarsSquadrons,
                         ProfileVersion.Fifa21, ProfileVersion.Madden22,
                         ProfileVersion.Fifa22, ProfileVersion.Madden23, ProfileVersion.DeadSpace))
                {
                    compressionType = CompressionType.ZStd;
                }
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.DragonAgeInquisition, ProfileVersion.Battlefield4))
                {
                    compressionType = CompressionType.ZLib;
                }
                else if (ProfilesLibrary.IsLoaded(ProfileVersion.PlantsVsZombiesGardenWarfare2) && texture == null)
                {
                    compressionType = CompressionType.None;
                }
            }

            if (resType == ResourceType.SwfMovie)
            {
                compressionType = CompressionType.None;
            }

            MemoryStream outMs = new MemoryStream();
            NativeWriter writer = new NativeWriter(outMs);

            using (NativeReader reader = new NativeReader(new MemoryStream(inData)))
            {
                reader.Position = 0x00;
                long length = reader.Length - reader.Position;
                long total = 0;
                long compSize = 0;
                bool uncompressed = false;

                while (length > 0)
                {
                    int bufferSize = (length > MaxBufferSize) ? MaxBufferSize : (int)length;
                    byte[] buffer = reader.ReadBytes(bufferSize);

                    ushort compressCode = 0;
                    ulong compressSize = 0;
                    byte[] compBuffer = null;

                    if (compressionType == CompressionType.ZStd)
                    {
                        compressSize = CompressZStd(buffer, out compBuffer, out compressCode, ref uncompressed);
                    }
                    else if (compressionType == CompressionType.ZLib)
                    {
                        compressSize = CompressZlib(buffer, out compBuffer, out compressCode, ref uncompressed);
                    }
                    else if (compressionType == CompressionType.LZ4)
                    {
                        compressSize = CompressLZ4(buffer, out compBuffer, out compressCode, ref uncompressed);
                    }
                    else if (compressionType == CompressionType.None)
                    {
                        compressSize = CompressNone(buffer, out compBuffer, out compressCode);
                    }
                    else if (compressionType == CompressionType.Oodle)
                    {
                        compressSize = CompressOodle(buffer, out compBuffer, out compressCode, ref uncompressed);
                    }

                    if (uncompressed)
                    {
                        uncompressed = false;
                        compressionType = CompressionType.None;
                        reader.Position = 0;
                        writer.Position = 0;
                        length = reader.Length - reader.Position;
                        total = 0;
                        compSize = 0;
                        continue;
                    }

                    compressCode |= (ushort)((compressSize & 0xF0000) >> 16);

                    writer.Write(bufferSize, Endian.Big);
                    writer.Write((ushort)compressCode, Endian.Big);
                    writer.Write((ushort)compressSize, Endian.Big);
                    writer.Write(compBuffer, 0, (int)compressSize);

                    length -= bufferSize;
                    total += bufferSize;
                    compSize += (long)compressSize + 8;

                    if (texture != null)
                    {
                        if (texture.MipCount > 1)
                        {
                            if (total + offset == texture.MipSizes[0])
                            {
                                // offset of second mip in compressed chunk
                                texture.FirstMipOffset = texture.SecondMipOffset = (uint)compSize;
                            }
                            else if (total + offset == (texture.MipSizes[0] + texture.MipSizes[1]))
                            {
                                // offset of third mip in compressed chunk
                                texture.SecondMipOffset = (uint)compSize;
                            }
                        }
                    }
                }
            }

            return outMs.ToArray();
        }

        private static ulong CompressLZ4(byte[] buffer, out byte[] compBuffer, out ushort compressCode, ref bool uncompressed)
        {
            compBuffer = new byte[LZ4.CompressBound(buffer.Length)];
            compressCode = 0x0970;

            GCHandle ptr1 = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            GCHandle ptr2 = GCHandle.Alloc(compBuffer, GCHandleType.Pinned);

            ulong size = (ulong)LZ4.Compress(ptr1.AddrOfPinnedObject(), ptr2.AddrOfPinnedObject(), buffer.Length, compBuffer.Length);
            if (size > (ulong)MaxBufferSize || (uint)size > buffer.Length)
            {
                uncompressed = true;
                size = 0;
            }

            ptr1.Free();
            ptr2.Free();

            return size;
        }

        private static ulong CompressZStd(byte[] buffer, out byte[] compBuffer, out ushort compressCode, ref bool uncompressed)
        {
            ICompressionUtils utils = ProfilesLibrary.Profile.GetCompressionUtils();

            int compressionLevel = utils.OodleCompressionLevel;

            compBuffer = new byte[ZStd.CompressBound((ulong)buffer.Length)];
            compressCode = 0x0F70;

            GCHandle ptr1 = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            GCHandle ptr2 = GCHandle.Alloc(compBuffer, GCHandleType.Pinned);

            ulong size = ZStd.Compress(ptr2.AddrOfPinnedObject(), (ulong)compBuffer.Length, ptr1.AddrOfPinnedObject(), (ulong)buffer.Length, compressionLevel);
            if (size > (ulong)buffer.Length)
            {
                uncompressed = true;
                size = 0;
            }

            ptr1.Free();
            ptr2.Free();

            return size;
        }

        private static ulong CompressZlib(byte[] buffer, out byte[] compBuffer, out ushort compressCode, ref bool uncompressed)
        {
            ulong size = 0;
            compBuffer = new byte[buffer.Length * 2];
            compressCode = 0x0270;

            GCHandle ptr1 = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            GCHandle ptr2 = GCHandle.Alloc(compBuffer, GCHandleType.Pinned);
            ZLib.ZStream stream = new ZLib.ZStream();

            // calculate size
            IntPtr streamPtr = Marshal.AllocHGlobal(Marshal.SizeOf(stream));
            stream.avail_in = (uint)buffer.Length;
            stream.next_in = ptr1.AddrOfPinnedObject();
            stream.avail_out = stream.avail_in * 2;
            stream.next_out = ptr2.AddrOfPinnedObject();
            Marshal.StructureToPtr(stream, streamPtr, true);

            int retCode = ZLib.DeflateInit(streamPtr, 9, "1.2.11", Marshal.SizeOf<ZLib.ZStream>());
            retCode = ZLib.Deflate(streamPtr, ZLib.Z_FINISH);

            // copy out size
            stream = Marshal.PtrToStructure<ZLib.ZStream>(streamPtr);
            size = stream.total_out;

            // free
            retCode = ZLib.DeflateEnd(streamPtr);
            Marshal.FreeHGlobal(streamPtr);

            // if its too big
            if (size > (ulong)MaxBufferSize)
            {
                uncompressed = true;
                size = 0;
            }

            ptr1.Free();
            ptr2.Free();

            return size;
        }

        private static ulong CompressNone(byte[] buffer, out byte[] compBuffer, out ushort compressCode)
        {
            compressCode = 0x70;
            compBuffer = buffer;

            return (ulong)buffer.Length;
        }

        private static ulong CompressOodle(byte[] buffer, out byte[] compBuffer, out ushort compressCode, ref bool uncompressed)
        {
            compBuffer = new byte[0x80000];

            GCHandle ptr1 = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            GCHandle ptr2 = GCHandle.Alloc(compBuffer, GCHandleType.Pinned);

            ulong size = 0;
            if (ProfilesLibrary.IsLoaded(ProfileVersion.Anthem, ProfileVersion.Fifa19,
                ProfileVersion.Fifa20, ProfileVersion.NeedForSpeedHeat,
                ProfileVersion.PlantsVsZombiesBattleforNeighborville, ProfileVersion.Madden22,
                ProfileVersion.Madden23))
            {
                // Kraken
                compressCode = 0x1170;
                size = (ulong)Oodle.Compress2(Oodle.OodleFormat.Kraken, ptr1.AddrOfPinnedObject(), buffer.Length, ptr2.AddrOfPinnedObject(), Oodle.OodleCompressionLevel.Optimal3, Oodle.GetOptions(Oodle.OodleFormat.Kraken, Oodle.OodleCompressionLevel.Optimal3));
            }
            else if (ProfilesLibrary.IsLoaded(ProfileVersion.Fifa21, ProfileVersion.Fifa22,
                ProfileVersion.NeedForSpeedUnbound))
            {
                // Leviathan
                compressCode = 0x1970;
                size = (ulong)Oodle.Compress2(Oodle.OodleFormat.Leviathan, ptr1.AddrOfPinnedObject(), buffer.Length, ptr2.AddrOfPinnedObject(), Oodle.OodleCompressionLevel.Optimal3, Oodle.GetOptions(Oodle.OodleFormat.Leviathan, Oodle.OodleCompressionLevel.Optimal3));
            }
            else
            {
                // Selkie
                compressCode = 0x1570;
                size = (ulong)Oodle.Compress(Oodle.OodleFormat.Selkie, ptr1.AddrOfPinnedObject(), buffer.Length, ptr2.AddrOfPinnedObject(), Oodle.OodleCompressionLevel.Optimal3);
            }

            // if its too big
            if (size > (ulong)buffer.Length)
            {
                uncompressed = true;
                size = 0;
            }

            ptr1.Free();
            ptr2.Free();

            return size;
        }

        public static byte[] DecompressZLib(byte[] tmpBuffer, int decompressedSize)
        {
            GCHandle ptr1 = GCHandle.Alloc(tmpBuffer, GCHandleType.Pinned);

            byte[] outBuffer = new byte[decompressedSize];
            GCHandle ptr2 = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);

            ZLib.ZStream stream = new ZLib.ZStream
            {
                avail_in = (uint)tmpBuffer.Length,
                avail_out = (uint)outBuffer.Length,
                next_in = ptr1.AddrOfPinnedObject(),
                next_out = ptr2.AddrOfPinnedObject()
            };

            IntPtr streamPtr = Marshal.AllocHGlobal(Marshal.SizeOf(stream));
            Marshal.StructureToPtr(stream, streamPtr, true);

            int retCode = 0;
            retCode = ZLib.InflateInit(streamPtr, "1.2.11", Marshal.SizeOf<ZLib.ZStream>());
            retCode = ZLib.Inflate(streamPtr, 0);
            retCode = ZLib.InflateEnd(streamPtr);

            ptr1.Free();
            ptr2.Free();

            Marshal.FreeHGlobal(streamPtr);
            return outBuffer;
        }

        public static uint CalcFletcher32(GeometryDeclarationDesc geomDecl)
        {
            byte[] array = null;
            using (NativeWriter nativeWriter = new NativeWriter(new MemoryStream()))
            {
                foreach (GeometryDeclarationDesc.Element element in geomDecl.Elements)
                {
                    nativeWriter.Write((byte)element.Usage);
                    nativeWriter.Write((byte)element.Format);
                    nativeWriter.Write((byte)element.Offset);
                    nativeWriter.Write((byte)element.StreamIndex);
                }
                foreach (GeometryDeclarationDesc.Stream stream in geomDecl.Streams)
                {
                    nativeWriter.Write((byte)stream.VertexStride);
                    nativeWriter.Write((byte)stream.Classification);
                }
                nativeWriter.Write((byte)geomDecl.ElementCount);
                nativeWriter.Write((byte)geomDecl.StreamCount);
                nativeWriter.Write((ushort)0);

                array = nativeWriter.ToByteArray();
            }

            return CalcFletcher32Internal(array);
        }

        private static unsafe uint CalcFletcher32Internal(byte[] array)
        {
            int byteCount = array.Length;
            int a = byteCount >> 1;
            int b = 0; ;
            int twoByteCount = 0;
            uint part1 = 0;
            uint part2 = 0;
            int idx = 0;

            if (a > 0)
            {
                do
                {
                    b = ((a - 0x168) & ((int)(a - 0x168) >> 0x1F)) + 0x168;
                    twoByteCount = a - b;

                    do
                    {
                        ushort val = (ushort)((array[idx] << 8) | (array[idx + 1]));
                        idx += 2;

                        part1 += val;
                        part2 += part1;

                        b--;
                    }
                    while (b > 0);

                    part1 = (ushort)((part1 >> 16) + part1);
                    part2 = (ushort)(part2 + (part2 >> 16));
                    a = twoByteCount;
                }
                while (twoByteCount > 0);
            }

            if ((byteCount & 1) != 0)
            {
                part1 += (ushort)(array[idx] << 8);
                part2 += part1;
            }

            return (uint)((int)((part1 & 0xFFFF0000) + (part1 << 16)) | ((ushort)part2 + (part2 >> 16)));
        }

        /// <summary>
        /// Loads all resolved hashes that are found within the specified file of <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The file to be read from for hashes.</param>
        public static void LoadStringList(string path = "strings.txt", ILogger logger = null)
        {
            if (!File.Exists(path))
            {
                return;
            }

            strings.Clear();

            using (NativeReader reader = new NativeReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                while (reader.Position < reader.Length)
                {
                    string currentString = reader.ReadLine();
                    int hash = Fnv1.HashString(currentString);
                    if (!strings.ContainsKey(hash))
                    {
                        strings.Add(hash, currentString);
                    }

                    logger?.Log("progress:" + (double)reader.Position / (double)reader.Length * 100.0);
                }
            }
        }
    }
}
