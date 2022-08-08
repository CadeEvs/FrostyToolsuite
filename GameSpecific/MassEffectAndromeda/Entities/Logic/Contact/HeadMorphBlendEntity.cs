using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HeadMorphBlendEntityData))]
	public class HeadMorphBlendEntity : HeadMorphItemEntity, IEntityData<FrostySdk.Ebx.HeadMorphBlendEntityData>
	{
		public new FrostySdk.Ebx.HeadMorphBlendEntityData Data => data as FrostySdk.Ebx.HeadMorphBlendEntityData;
		public override string DisplayName => "HeadMorphBlend";

		public HeadMorphBlendEntity(FrostySdk.Ebx.HeadMorphBlendEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

