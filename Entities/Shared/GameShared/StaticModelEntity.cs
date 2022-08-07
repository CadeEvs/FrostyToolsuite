using Frosty.Core.Screens;
using Frosty.Core.Viewport;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using D3D11 = SharpDX.Direct3D11;
using FrostySdk;
using LevelEditorPlugin.Render.Proxies;

namespace LevelEditorPlugin.Entities
{
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticModelEntityData))]
    public class StaticModelEntity : GamePhysicsEntity, IEntityData<FrostySdk.Ebx.StaticModelEntityData>
    {
        public new FrostySdk.Ebx.StaticModelEntityData Data => data as FrostySdk.Ebx.StaticModelEntityData;
        public Assets.MeshAsset Mesh { get; private set; }
        public override IEnumerable<ConnectionDesc> Links
        {
            get
            {
                List<ConnectionDesc> outLinks = new List<ConnectionDesc>();
                outLinks.AddRange(base.Links);
                outLinks.Add(new ConnectionDesc("ShaderParameters", Direction.In));
                return outLinks;
            }
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get
            {
                List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
                outProperties.AddRange(base.Properties);
                outProperties.Add(new ConnectionDesc("Visible", Direction.In));
                outProperties.Add(new ConnectionDesc("AnimatePhysics", Direction.In));
                outProperties.Add(new ConnectionDesc("Space", Direction.In));
                return outProperties;
            }
        }

        public StaticModelEntity(FrostySdk.Ebx.StaticModelEntityData inData, Entity inParent)
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
