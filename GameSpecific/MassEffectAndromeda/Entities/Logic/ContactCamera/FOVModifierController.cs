using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FOVModifierControllerData))]
	public class FOVModifierController : ValueModifierController, IEntityData<FrostySdk.Ebx.FOVModifierControllerData>
	{
		public new FrostySdk.Ebx.FOVModifierControllerData Data => data as FrostySdk.Ebx.FOVModifierControllerData;
		public override string DisplayName => "FOVModifierController";

		public FOVModifierController(FrostySdk.Ebx.FOVModifierControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

