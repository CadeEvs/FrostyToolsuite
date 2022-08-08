using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StencilContainerData))]
	public class StencilContainer : WSUIContainerEntity, IEntityData<FrostySdk.Ebx.StencilContainerData>
	{
		public new FrostySdk.Ebx.StencilContainerData Data => data as FrostySdk.Ebx.StencilContainerData;
		public override string DisplayName => "StencilContainer";

		public StencilContainer(FrostySdk.Ebx.StencilContainerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

