using Frosty.Core.Viewport;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using LevelEditorPlugin.Render;

namespace LevelEditorPlugin.Entities
{
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.PbrRectangularLightEntityData))]
    public class PbrRectangularLightEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.PbrRectangularLightEntityData>
    {
        public new FrostySdk.Ebx.PbrRectangularLightEntityData Data => data as FrostySdk.Ebx.PbrRectangularLightEntityData;

        public ObjRenderable SpriteData { get; private set; }

        public PbrRectangularLightEntity(FrostySdk.Ebx.PbrRectangularLightEntityData inData, Entity inParent)
            : base(inData, inParent)
        {
        }

        public override void CreateRenderProxy(List<RenderProxy> proxies, RenderCreateState state)
        {
            SpriteData = LoadedMeshManager.Instance.LoadMesh(state, "Sprite");
            proxies.Add(new SpriteRenderProxy(state, this, SpriteData, "RectLight", new Color4(Data.Color.x, Data.Color.y, Data.Color.z, 1.0f)));

            SetFlags(EntityFlags.RenderProxyGenerated);
        }

        public override void Destroy()
        {
            LoadedMeshManager.Instance.UnloadMesh(SpriteData);
        }
    }
}
