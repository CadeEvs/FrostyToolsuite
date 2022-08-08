using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventSplitterEntityData))]
	public class EventSplitterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EventSplitterEntityData>
	{
		public new FrostySdk.Ebx.EventSplitterEntityData Data => data as FrostySdk.Ebx.EventSplitterEntityData;
		public override string DisplayName => "EventSplitter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EventSplitterEntity(FrostySdk.Ebx.EventSplitterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

