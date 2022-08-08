using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StackingContainerData))]
	public class StackingContainer : WSUIContainerEntity, IEntityData<FrostySdk.Ebx.StackingContainerData>
	{
		public new FrostySdk.Ebx.StackingContainerData Data => data as FrostySdk.Ebx.StackingContainerData;
		public override string DisplayName => "StackingContainer";

		public StackingContainer(FrostySdk.Ebx.StackingContainerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

