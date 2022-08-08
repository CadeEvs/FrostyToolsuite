using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugConsoleCommandControllerData))]
	public class DebugConsoleCommandController : LogicController, IEntityData<FrostySdk.Ebx.DebugConsoleCommandControllerData>
	{
		public new FrostySdk.Ebx.DebugConsoleCommandControllerData Data => data as FrostySdk.Ebx.DebugConsoleCommandControllerData;
		public override string DisplayName => "DebugConsoleCommandController";

		public DebugConsoleCommandController(FrostySdk.Ebx.DebugConsoleCommandControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

