using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputConceptIconPickerEntityData))]
	public class InputConceptIconPickerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.InputConceptIconPickerEntityData>
	{
		public new FrostySdk.Ebx.InputConceptIconPickerEntityData Data => data as FrostySdk.Ebx.InputConceptIconPickerEntityData;
		public override string DisplayName => "InputConceptIconPicker";

		public InputConceptIconPickerEntity(FrostySdk.Ebx.InputConceptIconPickerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

