using LevelEditorPlugin.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
#if MASS_EFFECT
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.SimpleHeadItemData))]
    public class SimpleHeadItem : CommonAppearanceItem, IAssetData<FrostySdk.Ebx.SimpleHeadItemData>
    {
        public new FrostySdk.Ebx.SimpleHeadItemData Data => data as FrostySdk.Ebx.SimpleHeadItemData;
        public SimpleHeadMesh HeadMeshData { get; private set; }

        public SimpleHeadItem(Guid fileGuid, FrostySdk.Ebx.SimpleHeadItemData inData)
            : base(fileGuid, inData)
        {
            HeadMeshData = LoadedAssetManager.Instance.LoadAsset<SimpleHeadMesh>(this, Data.HeadMeshData);
        }

        public override FrostySdk.Ebx.GameObjectData GenerateEntityData()
        {
            MeshAsset meshToShow = (HeadMeshData.DynamicMorph != null) ? HeadMeshData.DynamicMorph.Mesh : null;
            if (meshToShow == null || HeadMeshData.Mesh != null)
                meshToShow = HeadMeshData.Mesh;

            return new FrostySdk.Ebx.MeshProxyEntityData() { Mesh = meshToShow.ToPointerRef() };
        }

        public override void Dispose()
        {
            LoadedAssetManager.Instance.UnloadAsset(HeadMeshData);
        }
    }

    [AssetBinding(DataType = typeof(FrostySdk.Ebx.SimpleHeadMeshData))]
    public class SimpleHeadMesh : Asset, IAssetData<FrostySdk.Ebx.SimpleHeadMeshData>
    {
        public FrostySdk.Ebx.SimpleHeadMeshData Data => data as FrostySdk.Ebx.SimpleHeadMeshData;
        public DynamicMorphHead DynamicMorph { get; private set; }
        public MeshAsset Mesh { get; private set; }

        public SimpleHeadMesh(Guid fileGuid, FrostySdk.Ebx.SimpleHeadMeshData inData)
            : base(fileGuid, inData)
        {
            DynamicMorph = LoadedAssetManager.Instance.LoadAsset<DynamicMorphHead>(this, Data.DynamicMorph);
            Mesh = LoadedAssetManager.Instance.LoadAsset<MeshAsset>(this, Data.Mesh);
        }

        public override void Dispose()
        {
            LoadedAssetManager.Instance.UnloadAsset(DynamicMorph);
            LoadedAssetManager.Instance.UnloadAsset(Mesh);
        }
    }

    [AssetBinding(DataType = typeof(FrostySdk.Ebx.DynamicMorphHeadData))]
    public class DynamicMorphHead : Asset, IAssetData<FrostySdk.Ebx.DynamicMorphHeadData>
    {
        public FrostySdk.Ebx.DynamicMorphHeadData Data => data as FrostySdk.Ebx.DynamicMorphHeadData;
        public MeshAsset Mesh { get; private set; }

        public DynamicMorphHead(Guid fileGuid, FrostySdk.Ebx.DynamicMorphHeadData inData)
            : base(fileGuid, inData)
        {
            Mesh = LoadedAssetManager.Instance.LoadAsset<MeshAsset>(this, Data.Mesh);
        }

        public override void Dispose()
        {
            LoadedAssetManager.Instance.UnloadAsset(Mesh);
        }
    }
#endif
}
