using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardEventEntityData))]
	public class BlackboardEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BlackboardEventEntityData>
	{
		public new FrostySdk.Ebx.BlackboardEventEntityData Data => data as FrostySdk.Ebx.BlackboardEventEntityData;
		public override string DisplayName => "BlackboardEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BlackboardEventEntity(FrostySdk.Ebx.BlackboardEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

