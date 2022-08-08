using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PaddingContainerData))]
	public class PaddingContainer : WSUIContainerEntity, IEntityData<FrostySdk.Ebx.PaddingContainerData>
	{
		public new FrostySdk.Ebx.PaddingContainerData Data => data as FrostySdk.Ebx.PaddingContainerData;
		public override string DisplayName => "PaddingContainer";

		public PaddingContainer(FrostySdk.Ebx.PaddingContainerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

