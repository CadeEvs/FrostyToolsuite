using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputActionModifierEntityData))]
	public class InputActionModifierEntity : LogicEntity, IEntityData<FrostySdk.Ebx.InputActionModifierEntityData>
	{
		public new FrostySdk.Ebx.InputActionModifierEntityData Data => data as FrostySdk.Ebx.InputActionModifierEntityData;
		public override string DisplayName => "InputActionModifier";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public InputActionModifierEntity(FrostySdk.Ebx.InputActionModifierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

