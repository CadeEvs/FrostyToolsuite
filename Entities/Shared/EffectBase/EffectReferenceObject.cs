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
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.EffectReferenceObjectData))]
    public class EffectReferenceObject : SpatialReferenceObject, IEntityData<FrostySdk.Ebx.EffectReferenceObjectData>
    {
        public new FrostySdk.Ebx.EffectReferenceObjectData Data => data as FrostySdk.Ebx.EffectReferenceObjectData;
        public new Assets.EffectBlueprint Blueprint => blueprint as Assets.EffectBlueprint;
        public new EffectEntity RootEntity => entities[0] as EffectEntity;
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Start", Direction.In),
                new ConnectionDesc("Stop", Direction.In)
            };
        }

        public EffectReferenceObject(FrostySdk.Ebx.EffectReferenceObjectData inData, Entity inParent)
            : base(inData, inParent)
        {
        }

        protected override void Initialize()
        {
            blueprint = LoadedAssetManager.Instance.LoadAsset<Assets.EffectBlueprint>(this, Data.Blueprint);
        }

        public override void Destroy()
        {
            LoadedAssetManager.Instance.UnloadAsset(Blueprint);
        }
    }
}
