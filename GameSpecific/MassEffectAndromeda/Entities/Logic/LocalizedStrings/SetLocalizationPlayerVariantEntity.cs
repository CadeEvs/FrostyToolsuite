using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetLocalizationPlayerVariantEntityData))]
	public class SetLocalizationPlayerVariantEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetLocalizationPlayerVariantEntityData>
	{
		public new FrostySdk.Ebx.SetLocalizationPlayerVariantEntityData Data => data as FrostySdk.Ebx.SetLocalizationPlayerVariantEntityData;
		public override string DisplayName => "SetLocalizationPlayerVariant";

		public SetLocalizationPlayerVariantEntity(FrostySdk.Ebx.SetLocalizationPlayerVariantEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

