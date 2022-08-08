using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HeadMorphBoneOffsetEntityData))]
	public class HeadMorphBoneOffsetEntity : HeadMorphItemEntity, IEntityData<FrostySdk.Ebx.HeadMorphBoneOffsetEntityData>
	{
		public new FrostySdk.Ebx.HeadMorphBoneOffsetEntityData Data => data as FrostySdk.Ebx.HeadMorphBoneOffsetEntityData;
		public override string DisplayName => "HeadMorphBoneOffset";

		public HeadMorphBoneOffsetEntity(FrostySdk.Ebx.HeadMorphBoneOffsetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

