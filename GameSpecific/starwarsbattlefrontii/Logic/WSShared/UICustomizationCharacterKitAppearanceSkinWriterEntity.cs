using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICustomizationCharacterKitAppearanceSkinWriterEntityData))]
	public class UICustomizationCharacterKitAppearanceSkinWriterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UICustomizationCharacterKitAppearanceSkinWriterEntityData>
	{
		public new FrostySdk.Ebx.UICustomizationCharacterKitAppearanceSkinWriterEntityData Data => data as FrostySdk.Ebx.UICustomizationCharacterKitAppearanceSkinWriterEntityData;
		public override string DisplayName => "UICustomizationCharacterKitAppearanceSkinWriter";

		public UICustomizationCharacterKitAppearanceSkinWriterEntity(FrostySdk.Ebx.UICustomizationCharacterKitAppearanceSkinWriterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

