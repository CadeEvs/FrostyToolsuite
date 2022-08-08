using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventMemoryEntityData))]
	public class EventMemoryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EventMemoryEntityData>
	{
		public new FrostySdk.Ebx.EventMemoryEntityData Data => data as FrostySdk.Ebx.EventMemoryEntityData;
		public override string DisplayName => "EventMemory";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EventMemoryEntity(FrostySdk.Ebx.EventMemoryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

