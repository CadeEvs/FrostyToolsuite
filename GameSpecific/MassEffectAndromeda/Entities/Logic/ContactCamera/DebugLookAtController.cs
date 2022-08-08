using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugLookAtControllerData))]
	public class DebugLookAtController : LookAtController, IEntityData<FrostySdk.Ebx.DebugLookAtControllerData>
	{
		public new FrostySdk.Ebx.DebugLookAtControllerData Data => data as FrostySdk.Ebx.DebugLookAtControllerData;
		public override string DisplayName => "DebugLookAtController";

		public DebugLookAtController(FrostySdk.Ebx.DebugLookAtControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

