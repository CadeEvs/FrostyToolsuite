using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TriggerExplosiveEntityData))]
	public class TriggerExplosiveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TriggerExplosiveEntityData>
	{
		public new FrostySdk.Ebx.TriggerExplosiveEntityData Data => data as FrostySdk.Ebx.TriggerExplosiveEntityData;
		public override string DisplayName => "TriggerExplosive";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TriggerExplosiveEntity(FrostySdk.Ebx.TriggerExplosiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

