using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ModifierControllerData))]
	public class ModifierController : VisualController, IEntityData<FrostySdk.Ebx.ModifierControllerData>
	{
		public new FrostySdk.Ebx.ModifierControllerData Data => data as FrostySdk.Ebx.ModifierControllerData;
		public override string DisplayName => "ModifierController";

		public ModifierController(FrostySdk.Ebx.ModifierControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

