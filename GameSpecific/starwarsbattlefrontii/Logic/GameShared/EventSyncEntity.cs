using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventSyncEntityData))]
	public class EventSyncEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EventSyncEntityData>
	{
		public new FrostySdk.Ebx.EventSyncEntityData Data => data as FrostySdk.Ebx.EventSyncEntityData;
		public override string DisplayName => "EventSync";

		public EventSyncEntity(FrostySdk.Ebx.EventSyncEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

