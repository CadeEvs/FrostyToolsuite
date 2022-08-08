using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICustomizationCharacterKitAppearanceWriterEntityData))]
	public class UICustomizationCharacterKitAppearanceWriterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UICustomizationCharacterKitAppearanceWriterEntityData>
	{
		public new FrostySdk.Ebx.UICustomizationCharacterKitAppearanceWriterEntityData Data => data as FrostySdk.Ebx.UICustomizationCharacterKitAppearanceWriterEntityData;
		public override string DisplayName => "UICustomizationCharacterKitAppearanceWriter";

		public UICustomizationCharacterKitAppearanceWriterEntity(FrostySdk.Ebx.UICustomizationCharacterKitAppearanceWriterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

