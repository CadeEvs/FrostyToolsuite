using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugScreenShotControllerData))]
	public class DebugScreenShotController : DebugConsoleCommandController, IEntityData<FrostySdk.Ebx.DebugScreenShotControllerData>
	{
		public new FrostySdk.Ebx.DebugScreenShotControllerData Data => data as FrostySdk.Ebx.DebugScreenShotControllerData;
		public override string DisplayName => "DebugScreenShotController";

		public DebugScreenShotController(FrostySdk.Ebx.DebugScreenShotControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

