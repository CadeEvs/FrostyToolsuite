using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HeadMorphCategorySelectorData))]
	public class HeadMorphCategorySelector : LogicEntity, IEntityData<FrostySdk.Ebx.HeadMorphCategorySelectorData>
	{
		public new FrostySdk.Ebx.HeadMorphCategorySelectorData Data => data as FrostySdk.Ebx.HeadMorphCategorySelectorData;
		public override string DisplayName => "HeadMorphCategorySelector";

		public HeadMorphCategorySelector(FrostySdk.Ebx.HeadMorphCategorySelectorData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

