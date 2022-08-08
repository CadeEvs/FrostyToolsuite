using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICustomizationCharacterKitAppearanceReaderEntityData))]
	public class UICustomizationCharacterKitAppearanceReaderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UICustomizationCharacterKitAppearanceReaderEntityData>
	{
		public new FrostySdk.Ebx.UICustomizationCharacterKitAppearanceReaderEntityData Data => data as FrostySdk.Ebx.UICustomizationCharacterKitAppearanceReaderEntityData;
		public override string DisplayName => "UICustomizationCharacterKitAppearanceReader";

		public UICustomizationCharacterKitAppearanceReaderEntity(FrostySdk.Ebx.UICustomizationCharacterKitAppearanceReaderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

