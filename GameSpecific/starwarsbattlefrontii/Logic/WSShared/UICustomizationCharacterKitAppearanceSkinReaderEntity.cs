using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICustomizationCharacterKitAppearanceSkinReaderEntityData))]
	public class UICustomizationCharacterKitAppearanceSkinReaderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UICustomizationCharacterKitAppearanceSkinReaderEntityData>
	{
		public new FrostySdk.Ebx.UICustomizationCharacterKitAppearanceSkinReaderEntityData Data => data as FrostySdk.Ebx.UICustomizationCharacterKitAppearanceSkinReaderEntityData;
		public override string DisplayName => "UICustomizationCharacterKitAppearanceSkinReader";

		public UICustomizationCharacterKitAppearanceSkinReaderEntity(FrostySdk.Ebx.UICustomizationCharacterKitAppearanceSkinReaderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

