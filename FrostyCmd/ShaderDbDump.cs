using Frosty.Hash;
using FrostySdk;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using FrostySdk.Managers.Entries;

namespace FrostyCmd
{
    public class ShaderDbDump
    {
        class ShaderConstants
        {
            public class TextureConstant
            {
                public byte index;
                public TextureType type;
                public ushort pad;
                public uint pad2;
                public string name;
                public long texture;
                public uint nameHash;
                public uint pad3;
            }
            public class ExternalValueConstant
            {
                public string name;
                public uint nameHash;
                public ushort index;
                public ushort arraySize;
                public byte size;
                public byte required;
                public ushort pad;
                public float[] defaultValue = new float[4];
            }
            public class ExternalTextureConstant
            {
                public string name;
                public uint nameHash;
                public ushort index;
                public TextureType type;
                public byte required;
            }
            public class UnknownConstant
            {
                public string name;
                public uint nameHash;
                public ushort value1;
                public ushort value2;
            }
            public class SamplerState
            {
                public uint index;
                public uint unknown;
                // D3D11_SAMPLER_DESC
                public uint filter;
                public uint addressU;
                public uint addressV;
                public uint addressW;
                public float mipLODBias;
                public uint maxAnisotropy;
                public uint comparisonFunc;
                public float[] borderColor = new float[4];
                public float minLOD;
                public float maxLOD;
                // D3D11_SAMPLER_DESC
                public long state;
            }
            public ushort constantCount;
            public ushort valueConstantStart;
            public List<TextureConstant> textureConstants = new List<TextureConstant>();
            public List<ExternalValueConstant> externalValueConstants = new List<ExternalValueConstant>();
            public List<ExternalTextureConstant> externalTextureConstants = new List<ExternalTextureConstant>();
            public List<UnknownConstant> unknownConstants = new List<UnknownConstant>();
            public List<SamplerState> samplerStates = new List<SamplerState>();

            public ShaderConstants(NativeReader reader)
            {
                long size = reader.ReadLong();
                long startPos = reader.Position;

                long[] offsets = new long[6]
                {
                    (reader.ReadLong() - 8) + startPos,
                    (reader.ReadLong() - 8) + startPos,
                    (reader.ReadLong() - 8) + startPos,
                    (reader.ReadLong() - 8) + startPos,
                    (reader.ReadLong() - 8) + startPos,
                    (reader.ReadLong() - 8) + startPos
                };

                constantCount = reader.ReadUShort();
                valueConstantStart = reader.ReadUShort();

                byte[] counts = new byte[6]
                {
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadByte(),
                        reader.ReadByte()
                };

                // unknown
                reader.Position = offsets[0];
                for (int j = 0; j < counts[0]; j++)
                {
                }
                // textures
                reader.Position = offsets[1];
                for (int j = 0; j < counts[1]; j++)
                {
                    TextureConstant tc = new TextureConstant();
                    if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII)
                    {
                        tc.nameHash = reader.ReadUInt();
                        tc.index = reader.ReadByte();
                        tc.type = (TextureType)reader.ReadByte();
                        tc.pad = reader.ReadUShort();
                        tc.pad2 = reader.ReadUInt();
                        tc.pad3 = reader.ReadUInt();
                    }
                    else
                    {
                        tc.index = reader.ReadByte();
                        tc.type = (TextureType)reader.ReadByte();
                        tc.pad = reader.ReadUShort();
                        tc.pad2 = reader.ReadUInt();
                        tc.name = reader.ReadSizedString(0x80);
                        tc.texture = reader.ReadLong();
                        tc.nameHash = reader.ReadUInt();
                        tc.pad3 = reader.ReadUInt();
                    }
                    textureConstants.Add(tc);
                }
                // parameters
                reader.Position = offsets[2];
                for (int j = 0; j < counts[2]; j++)
                {
                    ExternalValueConstant evc = new ExternalValueConstant();
                    evc.name = reader.ReadSizedString(32);
                    evc.nameHash = reader.ReadUInt();
                    evc.index = reader.ReadUShort();
                    evc.arraySize = reader.ReadUShort();
                    evc.size = reader.ReadByte();
                    evc.required = reader.ReadByte();
                    evc.pad = reader.ReadUShort();
                    evc.defaultValue[0] = reader.ReadFloat();
                    evc.defaultValue[1] = reader.ReadFloat();
                    evc.defaultValue[2] = reader.ReadFloat();
                    evc.defaultValue[3] = reader.ReadFloat();
                    externalValueConstants.Add(evc);
                }
                // texture parameters
                reader.Position = offsets[3];
                for (int j = 0; j < counts[3]; j++)
                {
                    ExternalTextureConstant etc = new ExternalTextureConstant();
                    etc.name = reader.ReadSizedString(32);
                    etc.nameHash = reader.ReadUInt();
                    etc.index = reader.ReadUShort();
                    etc.type = (TextureType)reader.ReadByte();
                    etc.required = reader.ReadByte();
                    externalTextureConstants.Add(etc);
                }
                // unknown
                reader.Position = offsets[4];
                for (int j = 0; j < counts[4]; j++)
                {
                    UnknownConstant uc = new UnknownConstant();
                    uc.name = reader.ReadSizedString(32);
                    uc.nameHash = reader.ReadUInt();
                    uc.value1 = reader.ReadUShort();
                    uc.value2 = reader.ReadUShort();
                    unknownConstants.Add(uc);
                }
                // unknown
                reader.Position = offsets[5];
                for (int j = 0; j < counts[5]; j++)
                {
                    SamplerState ss = new SamplerState();
                    ss.index = reader.ReadUInt();
                    if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MirrorsEdgeCatalyst)
                        ss.unknown = reader.ReadUInt();
                    ss.filter = reader.ReadUInt();
                    ss.addressU = reader.ReadUInt();
                    ss.addressV = reader.ReadUInt();
                    ss.addressW = reader.ReadUInt();
                    ss.mipLODBias = reader.ReadFloat();
                    ss.maxAnisotropy = reader.ReadUInt();
                    ss.comparisonFunc = reader.ReadUInt();
                    ss.borderColor[0] = reader.ReadFloat();
                    ss.borderColor[1] = reader.ReadFloat();
                    ss.borderColor[2] = reader.ReadFloat();
                    ss.borderColor[3] = reader.ReadFloat();
                    ss.minLOD = reader.ReadFloat();
                    ss.maxLOD = reader.ReadFloat();
                    ss.state = reader.ReadLong();
                    if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MirrorsEdgeCatalyst)
                        reader.ReadUInt();
                    samplerStates.Add(ss);
                }

                reader.Position = startPos + size - 8;
            }
        }

        class GeometryDeclaration
        {
            public uint hash;
            public GeometryDeclarationDesc desc = new GeometryDeclarationDesc();

            public GeometryDeclaration(NativeReader reader)
            {
                int maxStreamCount = (ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield4 || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeed) ? 4 : (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefront) ? 6 : 8;
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII)
                    maxStreamCount = 16;
                int maxElementCount = 16;

                desc.Elements = new GeometryDeclarationDesc.Element[maxElementCount];
                desc.Streams = new GeometryDeclarationDesc.Stream[maxStreamCount];

                hash = reader.ReadUInt();
                for (int j = 0; j < desc.Elements.Length; j++)
                {
                    GeometryDeclarationDesc.Element elem = new GeometryDeclarationDesc.Element();
                    elem.Usage = (VertexElementUsage)reader.ReadByte();
                    elem.Format = (VertexElementFormat)reader.ReadByte();
                    elem.Offset = reader.ReadByte();
                    elem.StreamIndex = reader.ReadByte();
                    desc.Elements[j] = elem;
                }
                for (int j = 0; j < desc.Streams.Length; j++)
                {
                    GeometryDeclarationDesc.Stream stream = new GeometryDeclarationDesc.Stream();
                    stream.VertexStride = reader.ReadByte();
                    stream.Classification = (VertexElementClassification)reader.ReadByte();
                    desc.Streams[j] = stream;
                }
                desc.ElementCount = reader.ReadByte();
                desc.StreamCount = reader.ReadByte();
                reader.ReadBytes(2); // padding
            }
        }

        class ShaderPermutation
        {
            public Guid guid;
            public byte[] data;
            public uint unknown;
            public uint[] unknownValues = new uint[4];

            public ShaderPermutation(NativeReader reader)
            {
                guid = reader.ReadGuid();

                uint size = reader.ReadUInt();
                data = reader.ReadBytes((int)size);

                unknownValues[0] = reader.ReadUInt();
                unknownValues[1] = reader.ReadUInt();
                unknownValues[2] = reader.ReadUInt();
                if (ProfilesLibrary.DataVersion != (int)ProfileVersion.DragonAgeInquisition && ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield4)
                    unknownValues[3] = reader.ReadUInt();
            }
        }

        class ShaderSolution
        {
            public ulong stateHash;
            public byte flags;
            public SurfaceShaderType surfaceType;
            public ShaderBlendMode blendMode;
            public ulong state;
            public long vertexShaderIndex;
            public long pixelShaderIndex;
            public long geometryShaderIndex;
            public long hullShaderIndex;
            public long domainShaderIndex;
            public long vertexConstantsIndex;
            public long pixelConstantsIndex;
            public long hullConstantsIndex;
            public long domainConstantsIndex;
            public long[] unknowns = new long[5];
            public long[] unknownHash = new long[2];

            public ShaderSolution(NativeReader reader)
            {
                if (ProfilesLibrary.DataVersion != (int)ProfileVersion.MirrorsEdgeCatalyst)
                    stateHash = reader.ReadULong();

                flags = reader.ReadByte();
                surfaceType = (SurfaceShaderType)reader.ReadByte();
                blendMode = (ShaderBlendMode)reader.ReadByte();
                reader.ReadBytes(5);
                state = reader.ReadULong();

                vertexShaderIndex = reader.ReadLong();
                pixelShaderIndex = reader.ReadLong();
                geometryShaderIndex = reader.ReadLong();
                hullShaderIndex = reader.ReadLong();
                domainShaderIndex = reader.ReadLong();

                vertexConstantsIndex = reader.ReadLong();
                pixelConstantsIndex = reader.ReadLong();
                hullConstantsIndex = reader.ReadLong();
                domainConstantsIndex = reader.ReadLong();

                if (ProfilesLibrary.DataVersion >= (int)ProfileVersion.MirrorsEdgeCatalyst)
                {
                    unknowns[0] = reader.ReadLong();
                    unknowns[1] = reader.ReadLong();
                    unknowns[2] = reader.ReadLong();
                    unknowns[3] = reader.ReadLong();
                    if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda)
                        unknowns[4] = reader.ReadLong();
                    else if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII)
                    {
                        unknowns[4] = reader.ReadLong();
                        unknowns[0] = reader.ReadLong();
                    }

                    unknownHash[0] = reader.ReadLong();
                    unknownHash[1] = reader.ReadLong();
                }
            }
        }

        class ShaderSolutionState
        {
            public uint surfaceShaderNameHash;
            public uint vertexShaderFragmentNameHash;
            public uint unknownHash;
            public uint geometryDeclarationHash;
            public byte[] stateInfo1;
            public byte[] stateInfo2;

            public ShaderSkinningMethod skinningMethod;
            public ShaderRenderMode renderMode;
            public byte unknown;
            public ShaderInstancingMethod instancingMethod;

            public bool tessellationEnable;

            public ShaderSolutionState(NativeReader reader)
            {
                surfaceShaderNameHash = reader.ReadUInt();
                vertexShaderFragmentNameHash = reader.ReadUInt();
                unknownHash = reader.ReadUInt();
                geometryDeclarationHash = reader.ReadUInt();
                stateInfo1 = reader.ReadBytes(8);
                stateInfo2 = reader.ReadBytes(8);

                skinningMethod = (ShaderSkinningMethod)(stateInfo1[1] >> 4);
                renderMode = (ShaderRenderMode)(stateInfo1[1] & 0x0F);
                unknown = (byte)(stateInfo1[2] >> 4);
                instancingMethod = (ShaderInstancingMethod)(stateInfo1[2] & 0x0F);

                tessellationEnable = stateInfo2[0] == 1;
            }
        }

        class VertexShaderPermutation : ShaderPermutation
        {
            public class InputElementDesc
            {
                public string SemanticName;
                public uint SemanticIndex;
                public uint Format;
                public uint InputSlot;
                public uint AlignedByteOffset;
                public uint InputSlotClass;
                public uint InstanceDataStepRate;
            }

            public byte[] inputLayout;
            public List<InputElementDesc> inputElements = new List<InputElementDesc>();

            public VertexShaderPermutation(NativeReader reader)
                : base(reader)
            {
                uint size = reader.ReadUInt();
                inputLayout = reader.ReadBytes((int)size);

                int numElements = reader.ReadInt();
                for (int i = 0; i < numElements; i++)
                {
                    InputElementDesc desc = new InputElementDesc();
                    desc.SemanticIndex = reader.ReadUInt();
                    desc.Format = reader.ReadUInt();
                    desc.InputSlot = reader.ReadUInt();
                    desc.AlignedByteOffset = reader.ReadUInt();
                    desc.InputSlotClass = reader.ReadUInt();
                    desc.InstanceDataStepRate = reader.ReadUInt();
                    inputElements.Add(desc);
                }
                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition || ProfilesLibrary.DataVersion == (int)ProfileVersion.DragonAgeInquisition)
                {
                    numElements = reader.ReadInt();
                    for (int i = 0; i < numElements; i++)
                        inputElements[i].SemanticName = reader.ReadNullTerminatedString();
                }
                numElements = reader.ReadInt();
                for (int i = 0; i < numElements; i++)
                    reader.ReadUInt();
                unknown = reader.ReadUInt();
            }
        }
        class PixelShaderPermutation : ShaderPermutation
        {
            public PixelShaderPermutation(NativeReader reader)
                : base(reader)
            {
                unknown = reader.ReadUInt();
            }
        }
        class GeometryShaderPermutation : ShaderPermutation
        {
            public GeometryShaderPermutation(NativeReader reader)
                : base(reader)
            {
                unknown = reader.ReadUInt();
            }
        }
        class HullShaderPermutation : ShaderPermutation
        {
            public HullShaderPermutation(NativeReader reader)
                : base(reader)
            {
                unknown = reader.ReadUInt();
            }
        }
        class DomainShaderPermutation : ShaderPermutation
        {
            public DomainShaderPermutation(NativeReader reader)
                : base(reader)
            {
                unknown = reader.ReadUInt();
            }
        }

        class Matrix
        {
            public float[,] m = new float[4, 4];
        }

        enum ShaderInstancingMethod
        {
            ShaderInstancingMethod_None = 0,
            ShaderInstancingMethod_ObjectTransform4x3Half = 1,
            ShaderInstancingMethod_ObjectTransform4x3InstanceData4x1Half = 2,
            ShaderInstancingMethod_ObjectTransform4x3InstanceData4x2Half = 3,
            ShaderInstancingMethod_WorldTransform4x3Float = 4,
            ShaderInstancingMethod_WorldTransform4x3FloatInstanceData4x2Half = 5,
            ShaderInstancingMethod_ObjectTranslationScaleHalf = 6,
            ShaderInstancingMethod_ObjectTranslationScaleHalfInstanceData4x1H = 7,
            ShaderInstancingMethod_ObjectTranslationScaleHalfInstanceData4x2H = 8,
            ShaderInstancingMethod_PositionStream = 9,
            ShaderInstancingMethod_DxBuffer = 10,
            ShaderInstancingMethod_DxBufferInstanceData4x1Float = 11,
            ShaderInstancingMethod_Manual = 12,
            ShaderInstancingMethodCount = 13
        }

        enum ShaderSkinningMethod
        {
            ShaderSkinningMethod_None = 0,
            ShaderSkinningMethod_Linear1Bone = 1,
            ShaderSkinningMethod_Linear2Bone = 2,
            ShaderSkinningMethod_Linear4Bone = 4,
            ShaderSkinningMethod_Linear8Bone = 8,
            ShaderSkinningMethod_Null = 9,
            ShaderSkinningMethod_DualQuaternion4Bone = 10,
            ShaderSkinningMethodCount = 11,
        }

        enum ShaderRenderPath
        {
            ShaderRenderPath_Dx10 = 0x0,
            ShaderRenderPath_Dx10Plus = 0x1,
            ShaderRenderPath_Dx10_1 = 0x2,
            ShaderRenderPath_Dx11 = 0x3,
            ShaderRenderPath_Xenon = 0x4,
            ShaderRenderPath_Ps3 = 0x5,
            ShaderRenderPath_Gl = 0x6,
            ShaderRenderPathCount = 0x7,
        }

        enum ShaderRenderMode
        {
            ShaderRenderMode_Default = 0x0,
            ShaderRenderMode_DynamicEnvmap = 0x1,
            ShaderRenderMode_ZOnly = 0x2,
            ShaderRenderMode_DeferredShadingGBufferLayout0 = 0x3,
            ShaderRenderMode_DeferredShadingGBufferLayout1 = 0x4,
            ShaderRenderMode_DeferredShadingGBufferLayout2 = 0x5,
            ShaderRenderMode_DeferredShadingGBufferLayout3 = 0x6,
            ShaderRenderMode_DeferredShadingGBufferLayout4 = 0x7,
            ShaderRenderMode_DeferredShadingGBufferLayout5 = 0x8,
            ShaderRenderMode_DeferredShadingGBufferLayout6 = 0x9,
            ShaderRenderMode_DeferredShadingGBufferLayout7 = 0xA,
            ShaderRenderMode_DeferredShadingEmissive = 0xB,
            ShaderRenderMode_DeferredShadingUnlit = 0xC,
            ShaderRenderMode_VelocityVector = 0xD,
            ShaderRenderMode_DistortionVector = 0xE,
            ShaderRenderMode_DebugMulti = 0xF,
            ShaderRenderModeCount = 0x10,
        }

        enum SurfaceShaderType
        {
            SurfaceShaderType_Opaque = 0,
            SurfaceShaderType_OpaqueAlphaTest = 1,
            SurfaceShaderType_OpaqueAlphaTestSimple = 2,
            SurfaceShaderType_Transparent = 3,
            SurfaceShaderType_TransparentDecal = 4,
            SurfaceShaderType_TransparentDepth = 5
        }

        enum ShaderBlendMode
        {
            ShaderBlendMode_Lerp = 0,
            ShaderBlendMode_Additive = 1,
            ShaderBlendMode_Multiply = 2,
            ShaderBlendMode_LerpPremultiplied = 3,
            ShaderBlendMode_DecalLerpNoSpec = 4,
            ShaderBlendMode_DecalLerpNormal = 5,
            ShaderBlendMode_DecalLerpNormalMultiply = 6,
            ShaderBlendMode_DecalLerpDiffuse = 7,
            ShaderBlendMode_DecalLerpDiffuseNoSpec = 8
        }

        public ShaderDbDump()
        {
        }

        int ShaderHashString(string strToHash)
        {
            int hash = 5381;
            for (int i = 0; i < strToHash.Length; i++)
            {
                byte B = (byte)strToHash[i];
                int c = B + 32 * ((uint)(B - 65) <= 0x19 ? 1 : 0);
                hash = 33 * hash ^ c;
            }

            return hash;
        }

        public void Execute(string basePath, string filename, ILogger logger)
        {
            FileInfo baseFile = new FileInfo(basePath);

            ProfilesLibrary.Initialize(baseFile.Name.Replace(baseFile.Extension, ""));
            TypeLibrary.Initialize(false);

            FileSystemManager fs = new FileSystemManager(baseFile.DirectoryName);
            foreach (FileSystemSource source in ProfilesLibrary.Sources)
                fs.AddSource(source.Path, source.SubDirs);
            fs.Initialize();

            ResourceManager rm = new ResourceManager(fs);
            rm.SetLogger(logger);
            rm.Initialize();

            AssetManager am = new AssetManager(fs, rm);
            am.SetLogger(logger);
            am.Initialize();

            Dictionary<int, string> shaderMap = new Dictionary<int, string>();
            foreach (EbxAssetEntry entry in am.EnumerateEbx("ShaderGraph"))
                shaderMap.Add(Fnv1.HashString(entry.Name.ToLower()), entry.Name);

            Dictionary<int, string> textureMap = new Dictionary<int, string>();
            foreach (EbxAssetEntry entry in am.EnumerateEbx("TextureAsset"))
                textureMap.Add(Fnv1.HashString(entry.Name.ToLower()), entry.Name);

            List<GeometryDeclaration> geometryDeclarations = new List<GeometryDeclaration>(); 
            List<ShaderConstants> constants = new List<ShaderConstants>();
            List<VertexShaderPermutation> vertexShaderPermutations = new List<VertexShaderPermutation>();
            List<PixelShaderPermutation> pixelShaderPermutations = new List<PixelShaderPermutation>();
            List<GeometryShaderPermutation> geometryShaderPermutations = new List<GeometryShaderPermutation>();
            List<HullShaderPermutation> hullShaderPermutations = new List<HullShaderPermutation>();
            List<DomainShaderPermutation> domainShaderPermutations = new List<DomainShaderPermutation>();
            List<ShaderSolution> shaderSolutions = new List<ShaderSolution>();
            List<ShaderSolutionState> shaderSolutionStates = new List<ShaderSolutionState>();

            Dictionary<uint, GeometryDeclaration> geometryDeclarationMap = new Dictionary<uint, GeometryDeclaration>();

            using (NativeReader reader = new NativeReader(new FileStream(filename, FileMode.Open, FileAccess.Read)))
            {
                uint _0x00 = reader.ReadUInt();
                while (reader.Position < reader.Length)
                {
                    uint _0x04 = reader.ReadUInt();

                    uint blockSize = reader.ReadUInt();
                    long blockStartPos = reader.Position;

                    uint _0x0C = reader.ReadUInt();
                    ShaderRenderPath renderPath = (ShaderRenderPath)reader.ReadUInt();
                    uint _0x14 = reader.ReadUInt();

                    if (renderPath != ShaderRenderPath.ShaderRenderPath_Dx11)
                    {
                        reader.Position = blockStartPos + blockSize;
                        continue;
                    }

                    // 1. Shader Constants
                    uint numEntries = reader.ReadUInt();
                    for (int i = 0; i < numEntries; i++)
                        constants.Add(new ShaderConstants(reader));

                    // 2. Unknown
                    int tmp = 0;
                    while (tmp != 0x43425844)
                    {
                        tmp = reader.ReadInt();
                        if (tmp == 0x43425844)
                        {
                            reader.Position -= 0x1c;
                            break;
                        }
                        else
                            reader.Position -= 3;
                    }

                    // 3. Vertex Shader Permutations
                    uint numVertexShaders = reader.ReadUInt();
                    for (int i = 0; i < numVertexShaders; i++)
                        vertexShaderPermutations.Add(new VertexShaderPermutation(reader));

                    // 4. Pixel Shader Permutations
                    uint numPixelShaders = reader.ReadUInt();
                    for (int i = 0; i < numPixelShaders; i++)
                        pixelShaderPermutations.Add(new PixelShaderPermutation(reader));

                    // 5. Geometry Shader Permutations
                    uint numGeomShaders = reader.ReadUInt();
                    for (int i = 0; i < numGeomShaders; i++)
                        geometryShaderPermutations.Add(new GeometryShaderPermutation(reader));

                    // 6. Hull Shader Permutations
                    uint numHullShaders = reader.ReadUInt();
                    for (int i = 0; i < numHullShaders; i++)
                        hullShaderPermutations.Add(new HullShaderPermutation(reader));

                    // 7. Domain Shader Permutations
                    uint numDomainShaders = reader.ReadUInt();
                    for (int i = 0; i < numDomainShaders; i++)
                        domainShaderPermutations.Add(new DomainShaderPermutation(reader));

                    // 8. Shader Solutions
                    uint numSolutions = reader.ReadUInt();
                    for (int i = 0; i < numSolutions; i++)
                        shaderSolutions.Add(new ShaderSolution(reader));

                    // 9. Shader Solution States
                    uint numStates = reader.ReadUInt();
                    for (int i = 0; i < numStates; i++)
                        shaderSolutionStates.Add(new ShaderSolutionState(reader));

                    // 10. Geometry Declarations
                    uint numDecls = reader.ReadUInt();
                    for (int i = 0; i < numDecls; i++)
                    {
                        GeometryDeclaration decl = new GeometryDeclaration(reader);
                        geometryDeclarationMap.Add(decl.hash, decl);
                        geometryDeclarations.Add(decl);
                    }
                    reader.Position = blockStartPos + blockSize;
                }
            }

            int hash = Fnv1.HashString("Objects/Nature/Kashyyyk/_KashyyykBase/KashyyykBase_TreeGiant_01/SS_KashyyykBase_TreeGiant_02_Branch_01_LOD".ToLower());
            int idx = 0;
            foreach(ShaderSolutionState state in shaderSolutionStates)
            {
                if (state.surfaceShaderNameHash == hash)
                {
                    ShaderSolution solution = shaderSolutions[idx];
                    ShaderConstants vConstants = constants[(int)solution.vertexConstantsIndex];
                    ShaderConstants pConstants = constants[(int)solution.pixelConstantsIndex];
                    GeometryDeclaration decl = geometryDeclarationMap[state.geometryDeclarationHash];

                    VertexShaderPermutation vShader = vertexShaderPermutations[(int)solution.vertexShaderIndex];
                    using (NativeWriter writer = new NativeWriter(new FileStream(@"input.bin", FileMode.Create)))
                        writer.Write(vShader.inputLayout);
                }
                idx++;
            }
        }
    }
}
