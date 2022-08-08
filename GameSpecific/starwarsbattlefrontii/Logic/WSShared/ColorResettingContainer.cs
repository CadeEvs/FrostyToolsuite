using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ColorResettingContainerData))]
	public class ColorResettingContainer : WSUIContainerEntity, IEntityData<FrostySdk.Ebx.ColorResettingContainerData>
	{
		public new FrostySdk.Ebx.ColorResettingContainerData Data => data as FrostySdk.Ebx.ColorResettingContainerData;
		public override string DisplayName => "ColorResettingContainer";

		public ColorResettingContainer(FrostySdk.Ebx.ColorResettingContainerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

