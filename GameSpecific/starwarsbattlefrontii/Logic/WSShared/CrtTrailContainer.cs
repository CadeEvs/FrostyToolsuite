using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CrtTrailContainerData))]
	public class CrtTrailContainer : WSUIContainerEntity, IEntityData<FrostySdk.Ebx.CrtTrailContainerData>
	{
		public new FrostySdk.Ebx.CrtTrailContainerData Data => data as FrostySdk.Ebx.CrtTrailContainerData;
		public override string DisplayName => "CrtTrailContainer";

		public CrtTrailContainer(FrostySdk.Ebx.CrtTrailContainerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

