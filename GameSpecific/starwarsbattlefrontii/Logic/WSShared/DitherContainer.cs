using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DitherContainerData))]
	public class DitherContainer : WSUIContainerEntity, IEntityData<FrostySdk.Ebx.DitherContainerData>
	{
		public new FrostySdk.Ebx.DitherContainerData Data => data as FrostySdk.Ebx.DitherContainerData;
		public override string DisplayName => "DitherContainer";

		public DitherContainer(FrostySdk.Ebx.DitherContainerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

