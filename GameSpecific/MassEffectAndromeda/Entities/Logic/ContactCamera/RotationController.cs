using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RotationControllerData))]
	public class RotationController : ModifierController, IEntityData<FrostySdk.Ebx.RotationControllerData>
	{
		public new FrostySdk.Ebx.RotationControllerData Data => data as FrostySdk.Ebx.RotationControllerData;
		public override string DisplayName => "RotationController";

		public RotationController(FrostySdk.Ebx.RotationControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

