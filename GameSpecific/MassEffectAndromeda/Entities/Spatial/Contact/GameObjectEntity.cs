using Frosty.Core.Viewport;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render;
using LevelEditorPlugin.Render.Proxies;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Entities
{
#if MASS_EFFECT
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.GameObjectEntityData))]
    public class GameObjectEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.GameObjectEntityData>
    {
        public new FrostySdk.Ebx.GameObjectEntityData Data => data as FrostySdk.Ebx.GameObjectEntityData;

        public ObjRenderable SpriteData { get; private set; }

        public GameObjectEntity(FrostySdk.Ebx.GameObjectEntityData inData, Entity inParent)
            : base(inData, inParent)
        {
        }

        public override void CreateRenderProxy(List<RenderProxy> proxies, RenderCreateState state)
        {
            SpriteData = LoadedMeshManager.Instance.LoadMesh(state, "Sprite");
            proxies.Add(new SpriteRenderProxy(state, this, SpriteData, "GameObject"));

            SetFlags(EntityFlags.RenderProxyGenerated);
        }

        public override void Destroy()
        {
            LoadedMeshManager.Instance.UnloadMesh(SpriteData);
        }
    }
#endif
}
