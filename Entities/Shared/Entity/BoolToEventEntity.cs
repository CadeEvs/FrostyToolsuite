using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BoolToEventEntityData))]
	public class BoolToEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BoolToEventEntityData>
	{
		public new FrostySdk.Ebx.BoolToEventEntityData Data => data as FrostySdk.Ebx.BoolToEventEntityData;
		public override string DisplayName => "BoolToEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BoolToEventEntity(FrostySdk.Ebx.BoolToEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

