using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScrollableContainerData))]
	public class ScrollableContainer : WSUIContainerEntity, IEntityData<FrostySdk.Ebx.ScrollableContainerData>
	{
		public new FrostySdk.Ebx.ScrollableContainerData Data => data as FrostySdk.Ebx.ScrollableContainerData;
		public override string DisplayName => "ScrollableContainer";

		public ScrollableContainer(FrostySdk.Ebx.ScrollableContainerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

