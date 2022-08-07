using Frosty.Core;
using Frosty.Core.Viewport;
using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.MeshAsset))]
    public class MeshAsset : Asset, IAssetData<FrostySdk.Ebx.MeshAsset>
    {
        public FrostySdk.Ebx.MeshAsset Data => data as FrostySdk.Ebx.MeshAsset;
        public Render.MeshRenderable MeshData { get; private set; }

        private GenericAsset<FrostySdk.Ebx.MeshLodGroup> lodGroup;

        public MeshAsset(Guid fileGuid, FrostySdk.Ebx.MeshAsset inData)
            : base(fileGuid, inData)
        {
            lodGroup = LoadedAssetManager.Instance.LoadAsset<GenericAsset<FrostySdk.Ebx.MeshLodGroup>>(this, Data.LodGroup);
        }

        public void LoadResource(RenderCreateState state)
        {
            if (MeshData == null)
            {
                Resources.MeshSet meshSet = App.AssetManager.GetResAs<Resources.MeshSet>(App.AssetManager.GetResEntry(Data.MeshSetResource));
                MeshData = new Render.MeshRenderable(state, meshSet, lodGroup.Data);
            }
        }

        public override void Dispose()
        {
            if (MeshData != null)
                MeshData.Dispose();

            LoadedAssetManager.Instance.UnloadAsset(lodGroup);
        }
    }

    [AssetBinding(DataType = typeof(FrostySdk.Ebx.SkinnedMeshAsset))]
    public class SkinnedMeshAsset : MeshAsset
    {
        public SkinnedMeshAsset(Guid fileGuid, FrostySdk.Ebx.MeshAsset inData)
            : base(fileGuid, inData)
        {
        }
    }

    [AssetBinding(DataType = typeof(FrostySdk.Ebx.RigidMeshAsset))]
    public class RigidMeshAsset : MeshAsset
    {
        public RigidMeshAsset(Guid fileGuid, FrostySdk.Ebx.MeshAsset inData)
            : base(fileGuid, inData)
        {
        }
    }

    [AssetBinding(DataType = typeof(FrostySdk.Ebx.CompositeMeshAsset))]
    public class CompositeMeshAsset : MeshAsset
    {
        public CompositeMeshAsset(Guid fileGuid, FrostySdk.Ebx.MeshAsset inData)
            : base(fileGuid, inData)
        {
        }
    }
}
