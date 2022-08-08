using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RenderToTextureContainerData))]
	public class RenderToTextureContainer : WSUIContainerEntity, IEntityData<FrostySdk.Ebx.RenderToTextureContainerData>
	{
		public new FrostySdk.Ebx.RenderToTextureContainerData Data => data as FrostySdk.Ebx.RenderToTextureContainerData;
		public override string DisplayName => "RenderToTextureContainer";

		public RenderToTextureContainer(FrostySdk.Ebx.RenderToTextureContainerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

