using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugDisplayHudControllerData))]
	public class DebugDisplayHudController : LogicController, IEntityData<FrostySdk.Ebx.DebugDisplayHudControllerData>
	{
		public new FrostySdk.Ebx.DebugDisplayHudControllerData Data => data as FrostySdk.Ebx.DebugDisplayHudControllerData;
		public override string DisplayName => "DebugDisplayHudController";

		public DebugDisplayHudController(FrostySdk.Ebx.DebugDisplayHudControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

