using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Entities
{
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.LogicPrefabReferenceObjectData))]
    public class LogicPrefabReferenceObject : LogicReferenceObject, IEntityData<FrostySdk.Ebx.LogicPrefabReferenceObjectData>
    {
        public new FrostySdk.Ebx.LogicPrefabReferenceObjectData Data => data as FrostySdk.Ebx.LogicPrefabReferenceObjectData;
        public new Assets.LogicPrefabBlueprint Blueprint => blueprint as Assets.LogicPrefabBlueprint;

        public LogicPrefabReferenceObject(FrostySdk.Ebx.LogicPrefabReferenceObjectData inData, Entity inParent, EntityWorld inWorld)
            : base(inData, inParent, inWorld)
        {
        }

        public LogicPrefabReferenceObject(FrostySdk.Ebx.LogicPrefabReferenceObjectData inData, Entity inParent)
            : this(inData, inParent, null)
        {
        }

        public override void AddEntity(Entity inEntity)
        {
            inEntity.SetParent(this);
            Blueprint.Data.Objects.Add(new FrostySdk.Ebx.PointerRef(inEntity.GetRawData()));
        }

        public override void RemoveEntity(Entity inEntity)
        {
            Guid instanceGuid = inEntity.InstanceGuid;
            Blueprint.Data.Objects.Remove(Blueprint.Data.Objects.Find(p => p.GetInstanceGuid() == instanceGuid));
        }

        protected override void Initialize()
        {
            blueprint = LoadedAssetManager.Instance.LoadAsset<Assets.LogicPrefabBlueprint>(this, Data.Blueprint);
        }

        public override void Destroy()
        {
            LoadedAssetManager.Instance.UnloadAsset(blueprint);
            base.Destroy();
        }
    }
}
