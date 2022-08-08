using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlendModeContainerData))]
	public class BlendModeContainer : WSUIContainerEntity, IEntityData<FrostySdk.Ebx.BlendModeContainerData>
	{
		public new FrostySdk.Ebx.BlendModeContainerData Data => data as FrostySdk.Ebx.BlendModeContainerData;
		public override string DisplayName => "BlendModeContainer";

		public BlendModeContainer(FrostySdk.Ebx.BlendModeContainerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

