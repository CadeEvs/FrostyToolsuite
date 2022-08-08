using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HeadMorphItemEntityData))]
	public class HeadMorphItemEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HeadMorphItemEntityData>
	{
		public new FrostySdk.Ebx.HeadMorphItemEntityData Data => data as FrostySdk.Ebx.HeadMorphItemEntityData;
		public override string DisplayName => "HeadMorphItem";

		public HeadMorphItemEntity(FrostySdk.Ebx.HeadMorphItemEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

