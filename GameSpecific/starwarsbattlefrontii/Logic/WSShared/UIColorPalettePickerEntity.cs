using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIColorPalettePickerEntityData))]
	public class UIColorPalettePickerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIColorPalettePickerEntityData>
	{
		public new FrostySdk.Ebx.UIColorPalettePickerEntityData Data => data as FrostySdk.Ebx.UIColorPalettePickerEntityData;
		public override string DisplayName => "UIColorPalettePicker";

		public UIColorPalettePickerEntity(FrostySdk.Ebx.UIColorPalettePickerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

