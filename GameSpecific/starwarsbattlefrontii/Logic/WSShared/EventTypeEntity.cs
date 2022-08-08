using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventTypeEntityData))]
	public class EventTypeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EventTypeEntityData>
	{
		public new FrostySdk.Ebx.EventTypeEntityData Data => data as FrostySdk.Ebx.EventTypeEntityData;
		public override string DisplayName => "EventType";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EventTypeEntity(FrostySdk.Ebx.EventTypeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

