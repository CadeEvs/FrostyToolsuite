using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventCounterManagerEntityData))]
	public class EventCounterManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EventCounterManagerEntityData>
	{
		public new FrostySdk.Ebx.EventCounterManagerEntityData Data => data as FrostySdk.Ebx.EventCounterManagerEntityData;
		public override string DisplayName => "EventCounterManager";

		public EventCounterManagerEntity(FrostySdk.Ebx.EventCounterManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

