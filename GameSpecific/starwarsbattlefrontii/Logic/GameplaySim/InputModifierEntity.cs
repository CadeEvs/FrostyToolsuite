using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputModifierEntityData))]
	public class InputModifierEntity : LogicEntity, IEntityData<FrostySdk.Ebx.InputModifierEntityData>
	{
		public new FrostySdk.Ebx.InputModifierEntityData Data => data as FrostySdk.Ebx.InputModifierEntityData;
		public override string DisplayName => "InputModifier";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public InputModifierEntity(FrostySdk.Ebx.InputModifierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

