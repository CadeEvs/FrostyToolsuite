using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoftEdgeContainerData))]
	public class SoftEdgeContainer : WSUIContainerEntity, IEntityData<FrostySdk.Ebx.SoftEdgeContainerData>
	{
		public new FrostySdk.Ebx.SoftEdgeContainerData Data => data as FrostySdk.Ebx.SoftEdgeContainerData;
		public override string DisplayName => "SoftEdgeContainer";

		public SoftEdgeContainer(FrostySdk.Ebx.SoftEdgeContainerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

