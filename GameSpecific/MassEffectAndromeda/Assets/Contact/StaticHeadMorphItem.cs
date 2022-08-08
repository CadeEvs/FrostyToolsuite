using LevelEditorPlugin.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
#if MASS_EFFECT
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.StaticHeadMorphItemData))]
    public class StaticHeadMorphItem : CommonAppearanceItem, IAssetData<FrostySdk.Ebx.StaticHeadMorphItemData>
    {
        public new FrostySdk.Ebx.StaticHeadMorphItemData Data => data as FrostySdk.Ebx.StaticHeadMorphItemData;
        public StaticHeadMorphMesh MorphMesh { get; private set; }

        public StaticHeadMorphItem(Guid fileGuid, FrostySdk.Ebx.StaticHeadMorphItemData inData)
            : base(fileGuid, inData)
        {
            MorphMesh = LoadedAssetManager.Instance.LoadAsset<StaticHeadMorphMesh>(this, Data.HeadMeshData);
        }

        public override FrostySdk.Ebx.GameObjectData GenerateEntityData()
        {
            MeshAsset meshToShow = null;
            if (MorphMesh.DynamicMorph != null)
                meshToShow = MorphMesh.DynamicMorph.Mesh;
            if (MorphMesh.Morph != null)
                meshToShow = MorphMesh.Morph.PresetMesh;
            
            return new FrostySdk.Ebx.MeshProxyEntityData() { Mesh = meshToShow.ToPointerRef() };
        }

        public override void Dispose()
        {
            LoadedAssetManager.Instance.UnloadAsset(MorphMesh);
        }
    }

    [AssetBinding(DataType = typeof(FrostySdk.Ebx.StaticHeadMorphMeshData))]
    public class StaticHeadMorphMesh : Asset, IAssetData<FrostySdk.Ebx.StaticHeadMorphMeshData>
    {
        public FrostySdk.Ebx.StaticHeadMorphMeshData Data => data as FrostySdk.Ebx.StaticHeadMorphMeshData;
        public MorphStatic Morph { get; private set; }
        public DynamicMorphHead DynamicMorph { get; private set; }

        public StaticHeadMorphMesh(Guid fileGuid, FrostySdk.Ebx.StaticHeadMorphMeshData inData)
            : base(fileGuid, inData)
        {
            Morph = LoadedAssetManager.Instance.LoadAsset<MorphStatic>(this, Data.Morph);
            DynamicMorph = LoadedAssetManager.Instance.LoadAsset<DynamicMorphHead>(this, Data.DynamicMorph);
        }

        public override void Dispose()
        {
            LoadedAssetManager.Instance.UnloadAsset(Morph);
            LoadedAssetManager.Instance.UnloadAsset(DynamicMorph);
        }
    }

    [AssetBinding(DataType = typeof(FrostySdk.Ebx.MorphStatic))]
    public class MorphStatic : Asset, IAssetData<FrostySdk.Ebx.MorphStatic>
    {
        public FrostySdk.Ebx.MorphStatic Data => data as FrostySdk.Ebx.MorphStatic;
        public MeshAsset PresetMesh { get; private set; }

        public MorphStatic(Guid fileGuid, FrostySdk.Ebx.MorphStatic inData)
            : base(fileGuid, inData)
        {
            PresetMesh = LoadedAssetManager.Instance.LoadAsset<MeshAsset>(this, Data.PresetMesh);
        }

        public override void Dispose()
        {
            LoadedAssetManager.Instance.UnloadAsset(PresetMesh);
        }
    }
#endif
}
