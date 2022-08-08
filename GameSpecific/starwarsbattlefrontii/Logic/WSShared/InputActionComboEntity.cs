using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputActionComboEntityData))]
	public class InputActionComboEntity : LogicEntity, IEntityData<FrostySdk.Ebx.InputActionComboEntityData>
	{
		public new FrostySdk.Ebx.InputActionComboEntityData Data => data as FrostySdk.Ebx.InputActionComboEntityData;
		public override string DisplayName => "InputActionCombo";

		public InputActionComboEntity(FrostySdk.Ebx.InputActionComboEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

