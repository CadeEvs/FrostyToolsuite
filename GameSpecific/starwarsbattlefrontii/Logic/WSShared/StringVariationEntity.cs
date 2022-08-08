using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StringVariationEntityData))]
	public class StringVariationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StringVariationEntityData>
	{
		public new FrostySdk.Ebx.StringVariationEntityData Data => data as FrostySdk.Ebx.StringVariationEntityData;
		public override string DisplayName => "StringVariation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StringVariationEntity(FrostySdk.Ebx.StringVariationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

