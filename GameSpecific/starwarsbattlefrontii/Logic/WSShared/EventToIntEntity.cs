using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventToIntEntityData))]
	public class EventToIntEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EventToIntEntityData>
	{
		public new FrostySdk.Ebx.EventToIntEntityData Data => data as FrostySdk.Ebx.EventToIntEntityData;
		public override string DisplayName => "EventToInt";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EventToIntEntity(FrostySdk.Ebx.EventToIntEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

