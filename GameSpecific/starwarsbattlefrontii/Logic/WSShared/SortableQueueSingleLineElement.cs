using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SortableQueueSingleLineElementData))]
	public class SortableQueueSingleLineElement : GameEventsListElementBase, IEntityData<FrostySdk.Ebx.SortableQueueSingleLineElementData>
	{
		public new FrostySdk.Ebx.SortableQueueSingleLineElementData Data => data as FrostySdk.Ebx.SortableQueueSingleLineElementData;
		public override string DisplayName => "SortableQueueSingleLineElement";

		public SortableQueueSingleLineElement(FrostySdk.Ebx.SortableQueueSingleLineElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

