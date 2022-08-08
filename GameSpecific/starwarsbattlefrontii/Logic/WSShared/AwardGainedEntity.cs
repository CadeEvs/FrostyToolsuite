using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AwardGainedEntityData))]
	public class AwardGainedEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AwardGainedEntityData>
	{
		public new FrostySdk.Ebx.AwardGainedEntityData Data => data as FrostySdk.Ebx.AwardGainedEntityData;
		public override string DisplayName => "AwardGained";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public AwardGainedEntity(FrostySdk.Ebx.AwardGainedEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

