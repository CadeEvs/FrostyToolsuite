using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TriggerFireEntityData))]
	public class TriggerFireEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TriggerFireEntityData>
	{
		public new FrostySdk.Ebx.TriggerFireEntityData Data => data as FrostySdk.Ebx.TriggerFireEntityData;
		public override string DisplayName => "TriggerFire";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TriggerFireEntity(FrostySdk.Ebx.TriggerFireEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

