using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frosty.Core.Viewport;
using FrostySdk;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render;
using LevelEditorPlugin.Render.Proxies;
using SharpDX;
using D3D11 = SharpDX.Direct3D11;

namespace LevelEditorPlugin.Entities
{
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.PbrSphereLightEntityData))]
    public class PbrSphereLightEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.PbrSphereLightEntityData>
    {
        public new FrostySdk.Ebx.PbrSphereLightEntityData Data => data as FrostySdk.Ebx.PbrSphereLightEntityData;

        public ObjRenderable SpriteData { get; private set; }

        public PbrSphereLightEntity(FrostySdk.Ebx.PbrSphereLightEntityData inData, Entity inParent)
            : base(inData, inParent)
        {
        }

        public override void CreateRenderProxy(List<RenderProxy> proxies, RenderCreateState state)
        {
            SpriteData = LoadedMeshManager.Instance.LoadMesh(state, "Sprite");
            proxies.Add(new SpriteRenderProxy(state, this, SpriteData, "PointLight", new Color4(Data.Color.x, Data.Color.y, Data.Color.z, 1.0f)));

            SetFlags(EntityFlags.RenderProxyGenerated);
        }

        public override void Destroy()
        {
            LoadedMeshManager.Instance.UnloadMesh(SpriteData);
        }
    }
}
