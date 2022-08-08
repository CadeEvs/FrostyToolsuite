using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProgressShapeMaskingContainerData))]
	public class ProgressShapeMaskingContainer : WSUIContainerEntity, IEntityData<FrostySdk.Ebx.ProgressShapeMaskingContainerData>
	{
		public new FrostySdk.Ebx.ProgressShapeMaskingContainerData Data => data as FrostySdk.Ebx.ProgressShapeMaskingContainerData;
		public override string DisplayName => "ProgressShapeMaskingContainer";

		public ProgressShapeMaskingContainer(FrostySdk.Ebx.ProgressShapeMaskingContainerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

