using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventCounterEntityData))]
	public class EventCounterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EventCounterEntityData>
	{
		public new FrostySdk.Ebx.EventCounterEntityData Data => data as FrostySdk.Ebx.EventCounterEntityData;
		public override string DisplayName => "EventCounter";

		public EventCounterEntity(FrostySdk.Ebx.EventCounterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

