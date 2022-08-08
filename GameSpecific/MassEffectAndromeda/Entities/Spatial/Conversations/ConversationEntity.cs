using Frosty.Core.Viewport;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render;
using LevelEditorPlugin.Render.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Entities
{
#if MASS_EFFECT
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.ConversationEntityData))]
    public class ConversationEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.ConversationEntityData>
    {
        public new FrostySdk.Ebx.ConversationEntityData Data => data as FrostySdk.Ebx.ConversationEntityData;
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Start", Direction.In),
                new ConnectionDesc("Stop", Direction.In),
                new ConnectionDesc("OnStarted", Direction.Out)
            };
        }
        public ObjRenderable MeshData { get; private set; }

        public ConversationEntity(FrostySdk.Ebx.ConversationEntityData inData, Entity inParent)
            : base(inData, inParent)
        {
        }

        public override void CreateRenderProxy(List<RenderProxy> proxies, RenderCreateState state)
        {
            MeshData = LoadedMeshManager.Instance.LoadMesh(state, "Sprite");
            proxies.Add(new SpriteRenderProxy(state, this, MeshData, "Conversation"));

            SetFlags(EntityFlags.RenderProxyGenerated);
        }

        public override void Destroy()
        {
            LoadedMeshManager.Instance.UnloadMesh(MeshData);
        }
    }
#endif
}
