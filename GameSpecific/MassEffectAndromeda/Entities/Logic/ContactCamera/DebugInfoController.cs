using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugInfoControllerData))]
	public class DebugInfoController : LogicController, IEntityData<FrostySdk.Ebx.DebugInfoControllerData>
	{
		public new FrostySdk.Ebx.DebugInfoControllerData Data => data as FrostySdk.Ebx.DebugInfoControllerData;
		public override string DisplayName => "DebugInfoController";

		public DebugInfoController(FrostySdk.Ebx.DebugInfoControllerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

