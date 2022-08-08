using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ValueModifierControllerData))]
	public class ValueModifierController : ModifierController, IEntityData<FrostySdk.Ebx.ValueModifierControllerData>
	{
		public new FrostySdk.Ebx.ValueModifierControllerData Data => data as FrostySdk.Ebx.ValueModifierControllerData;
		public override string DisplayName => "ValueModifierController";

		public ValueModifierController(FrostySdk.Ebx.ValueModifierControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

