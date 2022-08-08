using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEGearPreviewEntityData))]
	public class MEGearPreviewEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEGearPreviewEntityData>
	{
		public new FrostySdk.Ebx.MEGearPreviewEntityData Data => data as FrostySdk.Ebx.MEGearPreviewEntityData;
		public override string DisplayName => "MEGearPreview";

		public MEGearPreviewEntity(FrostySdk.Ebx.MEGearPreviewEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

