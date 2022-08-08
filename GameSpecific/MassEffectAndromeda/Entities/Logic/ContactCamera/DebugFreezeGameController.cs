using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugFreezeGameControllerData))]
	public class DebugFreezeGameController : LogicController, IEntityData<FrostySdk.Ebx.DebugFreezeGameControllerData>
	{
		public new FrostySdk.Ebx.DebugFreezeGameControllerData Data => data as FrostySdk.Ebx.DebugFreezeGameControllerData;
		public override string DisplayName => "DebugFreezeGameController";

		public DebugFreezeGameController(FrostySdk.Ebx.DebugFreezeGameControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

