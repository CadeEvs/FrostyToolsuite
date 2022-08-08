using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShapeMaskingContainerData))]
	public class ShapeMaskingContainer : WSUIContainerEntity, IEntityData<FrostySdk.Ebx.ShapeMaskingContainerData>
	{
		public new FrostySdk.Ebx.ShapeMaskingContainerData Data => data as FrostySdk.Ebx.ShapeMaskingContainerData;
		public override string DisplayName => "ShapeMaskingContainer";

		public ShapeMaskingContainer(FrostySdk.Ebx.ShapeMaskingContainerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

