using Frosty.Core.Viewport;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render;
using LevelEditorPlugin.Render.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.PbrSpotLightEntityData))]
    public class PbrSpotLightEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.PbrSpotLightEntityData>
    {
        public new FrostySdk.Ebx.PbrSpotLightEntityData Data => data as FrostySdk.Ebx.PbrSpotLightEntityData;
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Color", Direction.In),
                new ConnectionDesc("Intensity", Direction.In)
            };
        }

        public ObjRenderable SpriteData { get; private set; }

        public PbrSpotLightEntity(FrostySdk.Ebx.PbrSpotLightEntityData inData, Entity inParent)
            : base(inData, inParent)
        {
        }

        public override void CreateRenderProxy(List<RenderProxy> proxies, RenderCreateState state)
        {
            SpriteData = LoadedMeshManager.Instance.LoadMesh(state, "Sprite");
            proxies.Add(new SpriteRenderProxy(state, this, SpriteData, "SpotLight", new Color4(Data.Color.x, Data.Color.y, Data.Color.z, 1.0f)));

            SetFlags(EntityFlags.RenderProxyGenerated);
        }

        public override void Destroy()
        {
            LoadedMeshManager.Instance.UnloadMesh(SpriteData);
        }
    }
}
