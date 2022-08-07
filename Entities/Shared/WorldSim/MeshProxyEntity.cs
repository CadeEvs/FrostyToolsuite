using Frosty.Core.Viewport;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.MeshProxyEntityData))]
    public class MeshProxyEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.MeshProxyEntityData>
    {
        public new FrostySdk.Ebx.MeshProxyEntityData Data => data as FrostySdk.Ebx.MeshProxyEntityData;
        public Assets.MeshAsset Mesh { get; private set; }

        public MeshProxyEntity(FrostySdk.Ebx.MeshProxyEntityData inData, Entity inParent)
            : base(inData, inParent)
        {
            Mesh = LoadedAssetManager.Instance.LoadAsset<Assets.MeshAsset>(this, Data.Mesh);
        }

        public override void CreateRenderProxy(List<RenderProxy> proxies, RenderCreateState state)
        {
            Mesh.LoadResource(state);
            proxies.Add(new ModelRenderProxy(state, this));

            SetFlags(EntityFlags.RenderProxyGenerated);
        }

        public override void Destroy()
        {
            LoadedAssetManager.Instance.UnloadAsset(Mesh);
        }
    }
}
