using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIMaskingContainerEntityData))]
	public class UIMaskingContainerEntity : UIContainerEntity, IEntityData<FrostySdk.Ebx.UIMaskingContainerEntityData>
	{
		public new FrostySdk.Ebx.UIMaskingContainerEntityData Data => data as FrostySdk.Ebx.UIMaskingContainerEntityData;
		public override string DisplayName => "UIMaskingContainer";

		public UIMaskingContainerEntity(FrostySdk.Ebx.UIMaskingContainerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

