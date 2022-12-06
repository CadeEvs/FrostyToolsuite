namespace FrostySdk.Managers.Entries
{
    public enum ResourceType : uint
    {
        Texture = 0x6BDE20BA,
        PcaComponentWeightsResource = 0x8D9E6F01,
        AtlasTexture = 0x957C32B1,
        MeshSet = 0x49B156D4,
        IShaderDatabase = 0x36F3F2C0,
        EAClothAssetData = 0x85AC783D,
        MeshAdjancencyResource = 0xBA02FEE0,
        MorphTargetsResource = 0x1091C8C5,
        EAClothEntityData = 0x85EA8656,
        FaceFxResource = 0x59C79990,
        HavokPhysicsData = 0x91043F65,
        OccluderMesh = 0x30B4A553,
        EnlightenDatabase = 0x70C5CB3E,
        EnlightenStaticDatabase = 0xC6CD3286,
        LargeParticleCloud = 0xAD1AC4FD,
        Dx11ShaderProgramDatabase = 0xF04F0C81,
        RenderTexture = 0x41D57E10,
        UITtfFontFile = 0x9D00966A,
        AssetBank = 0x51A3C853,
        IesResource = 0x0DEAFE10,
        MorphResource = 0xEB228507,
        PlayerPresetResource = 0x52EE0D39,
        TerrainDecals = 0x15E1F32E,
        TerrainLayerCombinations = 0xA23E75DB,
        VisualTerrain = 0x1CA38E06,
        TerrainStreamingTree = 0x22FE8AC8,
        ZoneStreamerGrid = 0xEFC70728,
        CompiledLuaResource = 0xAFECB022,
        LinearMediaAsset = 0x86521D6C,
        EnlightenShaderDatabase = 0x59CEEB57,
        AnimTrackData = 0xD070EED1,
        LocalizedStringResource = 0x5E862E05,
        GtsoLut = 0xCB8BCD07,
        MeshEmitterResource = 0xC611F34A,
        EAClothData = 0x387CA0AD,
        PamReplayResource = 0xC664A660,
        HavokClothPhysicsData = 0xE36F0D59,
        HeightfieldDecal = 0x9C4FAA17,
        DxTexture = 0x5C4954A6,
        DelayLoadBundleResource = 0x76742DC8,
        MorphMaterialResource = 0x24A019CC,
        RagdollResource = 0x319D8CD0,
        SwfMovie = 0x2D47A5FF,
        DxShaderProgramDatabase = 0x10F0E5A1,
        BundleRefTableResource = 0x428EC9D4,
        FifaPhysicsResourceData = 0xEF23407C,
        NewWaveResource = 0xB2C465F6,
        EnlightenShaderDatabaseResource = 0xB15AD3FD,
        SvgImage = 0x89983F10,
        RawFileData = 0x3568E2B7,
        PhysicsResource = 0x41759364,
        PersistenceDefRuntimeResource = 0x0EE85483,
        ScenarioDefRuntimeResource = 0x2EEC1D7A,
        SerializedExpressionNodeGraph = 0x7DD4CC89,
        MeshComputeFaceAdjacencyResource = 0x0EDE7594,
        MeshComputeIndexBufferResource = 0x2C3E1E37,
        MeshComputeMeshDefinitionResource = 0x81F0E34F,
        AntResource = 0xEC1B7BF4,
        Dx12PcRvmDatabase = 0x6B4B6E85,
        Dx12NvRvmDatabase = 0x50E8E7EE,
        Dx11NvRvmDatabase = 0xF7CC814D,
        Dx11RvmDatabase = 0x8DA16895,
        PathfindingRuntimeResource = 0x4B803D3B,
        AtlasGroupResource = 0x1445F2DB,
        EmitterGraphResource = 0x78791C75,
        RaceGroundTextureResource = 0xD41D60,
        MetaMorphTargetMeshResourceAsset = 0x39173AB8,
        FootballMetaMorphVertexRegionWeightsResource = 0x59BBF1E8,
        FootballMetaMorphMeshDeltaPositionsResource = 0x4C4D624A,
        PSDResource = 0x3B9D1688,
        CompressedClipData = 0x85548684,
        ShaderBlockDepotAsset = 0xDDB3E17F,
        ShaderBlockDepotResource = 0x73312045,
        CompiledBytecode = 0xE2B02F7,
        ShaderBlockDepot = 0xD8F5DAAF,
        Invalid = 0xFFFFFFFF
    }
    
    public class ResAssetEntry : AssetEntry
    {
        public override string Type => ((ResourceType)ResType).ToString();
        public override string AssetType => "res";
        public override string Name
        {
            get
            {
                // TODO: @techdebt find better method to move blueprint bundles to sub-folder, this will most likely break writing.
                if (ProfilesLibrary.IsLoaded(ProfileVersion.Battlefield2042) &&
                    (base.Name.StartsWith("cd_") || base.Name.StartsWith("md_") &! base.Name.Contains("win32/")))
                {
                    return $"win32/{base.Name}";
                }

                return base.Name;
            }
        }

        public ulong ResRid;
        public uint ResType;
        public byte[] ResMeta;
    }
}