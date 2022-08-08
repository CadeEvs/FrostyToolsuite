using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UICustomizationAbilitySlotsReaderEntityData))]
	public class UICustomizationAbilitySlotsReaderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UICustomizationAbilitySlotsReaderEntityData>
	{
		public new FrostySdk.Ebx.UICustomizationAbilitySlotsReaderEntityData Data => data as FrostySdk.Ebx.UICustomizationAbilitySlotsReaderEntityData;
		public override string DisplayName => "UICustomizationAbilitySlotsReader";

		public UICustomizationAbilitySlotsReaderEntity(FrostySdk.Ebx.UICustomizationAbilitySlotsReaderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

