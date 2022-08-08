using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugModeHandlerControllerData))]
	public class DebugModeHandlerController : LogicController, IEntityData<FrostySdk.Ebx.DebugModeHandlerControllerData>
	{
		public new FrostySdk.Ebx.DebugModeHandlerControllerData Data => data as FrostySdk.Ebx.DebugModeHandlerControllerData;
		public override string DisplayName => "DebugModeHandlerController";

		public DebugModeHandlerController(FrostySdk.Ebx.DebugModeHandlerControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

