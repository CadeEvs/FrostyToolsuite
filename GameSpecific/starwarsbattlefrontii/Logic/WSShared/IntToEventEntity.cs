using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntToEventEntityData))]
	public class IntToEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.IntToEventEntityData>
	{
		public new FrostySdk.Ebx.IntToEventEntityData Data => data as FrostySdk.Ebx.IntToEventEntityData;
		public override string DisplayName => "IntToEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public IntToEventEntity(FrostySdk.Ebx.IntToEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

