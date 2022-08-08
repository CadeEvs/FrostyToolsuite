using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugRotationControllerData))]
	public class DebugRotationController : RotationController, IEntityData<FrostySdk.Ebx.DebugRotationControllerData>
	{
		public new FrostySdk.Ebx.DebugRotationControllerData Data => data as FrostySdk.Ebx.DebugRotationControllerData;
		public override string DisplayName => "DebugRotationController";

		public DebugRotationController(FrostySdk.Ebx.DebugRotationControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

