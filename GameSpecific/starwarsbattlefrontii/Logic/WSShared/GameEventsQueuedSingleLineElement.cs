using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameEventsQueuedSingleLineElementData))]
	public class GameEventsQueuedSingleLineElement : GameEventsListElementBase, IEntityData<FrostySdk.Ebx.GameEventsQueuedSingleLineElementData>
	{
		public new FrostySdk.Ebx.GameEventsQueuedSingleLineElementData Data => data as FrostySdk.Ebx.GameEventsQueuedSingleLineElementData;
		public override string DisplayName => "GameEventsQueuedSingleLineElement";

		public GameEventsQueuedSingleLineElement(FrostySdk.Ebx.GameEventsQueuedSingleLineElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

