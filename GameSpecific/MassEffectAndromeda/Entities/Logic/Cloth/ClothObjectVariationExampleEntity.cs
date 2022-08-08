using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClothObjectVariationExampleEntityData))]
	public class ClothObjectVariationExampleEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClothObjectVariationExampleEntityData>
	{
		public new FrostySdk.Ebx.ClothObjectVariationExampleEntityData Data => data as FrostySdk.Ebx.ClothObjectVariationExampleEntityData;
		public override string DisplayName => "ClothObjectVariationExample";

		public ClothObjectVariationExampleEntity(FrostySdk.Ebx.ClothObjectVariationExampleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

